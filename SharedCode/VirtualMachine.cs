using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Azure.Reaper
{
  public class VirtualMachine
  {
    private IVirtualMachine virtualMachine;
    private ILogger logger;
    private IEnumerable<Setting> settings;
    private DateTime timeNowUtc;
    private ResourceGroup resourceGroup;
    private IEnumerable<LocationTZ> timezones;
    private string startTime;
    private string stopTime;
    private Regex pattern = new Regex("[0-2]([0-9]{3}) *- *[0-2]([0-9]{3})");
    private TimeZoneInfo zoneInfo = TimeZoneInfo.Utc;
    private bool permittedToRunOnDay;

    public enum Status { Started, Stopped, NoChange };
    private NotificationDelay notificationDelay;
    private string subscriptionId;

    public VirtualMachine(
      string subscriptionId,
      IVirtualMachine virtualMachine,
      ILogger logger,
      IEnumerable<Setting> settings,
      NotificationDelay notificationDelay,
      ResourceGroup resourceGroup,
      LocationTZ timezone,
      DateTime timeNowUtc
    )
    {
      this.subscriptionId = subscriptionId;
      this.virtualMachine = virtualMachine;
      this.logger = logger;
      this.settings = settings;
      this.notificationDelay = notificationDelay;
      this.resourceGroup = resourceGroup;
      this.timezones = timezone.GetAll();
      this.timeNowUtc = timeNowUtc;

      // Using the settings, set the start and stop time for the vm
      startTime = settings.First(s => s.name == "vm_start").value;
      stopTime = settings.First(s => s.name == "vm_stop").value;

      SetPowerState();
    }

    public bool ShouldBeRunning()
    {
      bool result = false;

      // Get schedule from the resource group, if one exists
      ScheduleFromTags(true);

      // Get the schedule from the virtual machine, if one exists
      ScheduleFromTags(false);

      // Set the timezone to work with
      SetTimeZone();

      // Using the strings for start and stop time, turn them into timespans so that
      // easy comparisons can be made
      TimeSpan tsStart = DateTime.ParseExact(startTime, "HHmm", CultureInfo.InvariantCulture).TimeOfDay;
      TimeSpan tsStop = DateTime.ParseExact(stopTime, "HHmm", CultureInfo.InvariantCulture).TimeOfDay;

      // Convert the timenow UTC time to local time as specified by the zoneInfo
      DateTime timeNowZone = TimeZoneInfo.ConvertTimeFromUtc(timeNowUtc, zoneInfo);

      // Perform test to determine if the machine should be running or not
      if ((timeNowZone.TimeOfDay > tsStart) && (timeNowZone.TimeOfDay < tsStop) && permittedToRunOnDay)
      {
        result = true;
      }

      return result;
    }

    public Status SetPowerState()
    {
      Status status = Status.NoChange;
      bool isRunning = false;
      string powerState = virtualMachine.PowerState.ToString();

      // Determine if VMs are being managed
      bool manage_vm = Convert.ToBoolean(settings.First(s => s.name == "manage_vms").value);

      // Only operate on the machine if it is running or deallocated
      // There are many states that the vm could be in so this needs to be guarded
      logger.LogDebug("{0} - {1}: Power state - {2}", resourceGroup.GetName(), GetName(), powerState);

      if (powerState.ToLower() == "powerstate/running" ||
          powerState.ToLower() == "powerstate/deallocated")
      {
        // Set the isRunning flag
        if (powerState.ToLower() == "powerstate/running") {
          isRunning = true;
        }

        // Perform checks to see if this machine should be running
        bool permitted = ShouldBeRunning();

        if (isRunning && !permitted && manage_vm)
        {
          logger.LogInformation("action=stop, resource_group={resourceGroup}, vm_name={vmName}, message=Stop machine", resourceGroup.GetName(), GetName());
          virtualMachine.Deallocate();
          status = Status.Stopped;
        }

        if (!isRunning && permitted && manage_vm)
        {
          logger.LogInformation("action=start, resource_group={resourceGroup}, vm_name={vmName}, message=Start machine", resourceGroup.GetName(), GetName());
          virtualMachine.Start();
          status = Status.Started;
        }
      }

      return status;
    }

    public string GetName()
    {
      return virtualMachine.Name;
    }

    /// <summary>
    /// Determine if the group contains the specifed tag
    /// If it does then return the value
    /// If not return false
    /// </summary>
    /// <param name="name"></param>
    /// <returns>dynamic</returns>
    public dynamic HasTag(string name)
    {
      dynamic result = false;
      string tagName =  settings.First(s => s.name == name).value;

      if (virtualMachine.Tags.ContainsKey(tagName))
      {
        result = tagName;
      }

      return result;
    }    

    /// <summary>
    /// Determine if the resource group tags contain schedule times for the virtual machine
    /// If it does set the startTime and stopTime accordingly
    /// 
    /// The format is HHmm-HHmm in the tag
    /// </summary>
    private void ScheduleFromTags(bool fromResourceGroup)
    {
      bool hasTag;
      string tagSchedule = String.Empty;
      string message = String.Empty;
      
      if (fromResourceGroup)
      {
        hasTag = resourceGroup.HasTag("tag_vm_start_stop_time");
        if (hasTag)
        {
          tagSchedule = resourceGroup.GetTag("tag_vm_start_stop_time");
          message = String.Format("{0}: VM power state schedule found on resource group: {1}", resourceGroup.GetName(), (string) tagSchedule);
        }
      }
      else
      {
        hasTag = HasTag("tag_vm_start_stop_time");
        if (hasTag)
        {
          tagSchedule = GetTag("tag_vm_start_stop_time");
          message = String.Format("{0} - {1}: VM power state schedule found on virtual machine: {2}", resourceGroup.GetName(), GetName(), (string) tagSchedule);
        }
      }

      if (hasTag)
      {
        // Ensure that the schedule matches the regex pattern
        if (pattern.IsMatch(tagSchedule))
        {
          logger.LogInformation(message);
          string[] parts = tagSchedule.Split('-').ToArray();

          // use the parts from the string to set the times
          startTime = parts[0].Trim();
          stopTime = parts[1].Trim();
        }
        else
        {
          logger.LogError("{0}: VM Schedule tag has an invalid format - '{1}'", resourceGroup.GetName(), (string) tagSchedule);
        }
      }
    }

    public string GetTag(string name)
    {
      string result = String.Empty;

      // Determine that the tag exists
      if (HasTag(name))
      {
        string tagName =  settings.First(s => s.name == name).value;
        result = virtualMachine.Tags[tagName];
      }

      return result;
    }

    /// <summary>
    /// Set the timezone that is to be used for time comparison
    /// The precendence of this is:
    ///    1. Tags
    ///    2. Location of virtual machine
    ///    3. Utc
    /// </summary>
    private void SetTimeZone()
    {
      // Find the TimeZoneInfo based on the location of the VM
      LocationTZ vmZone = timezones.First(t => t.name == virtualMachine.RegionName);
      if (vmZone != null)
      {
        zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(vmZone.tzId);
      }

      // Set the zone if the vm has a tag that states what zone it is in
      dynamic tagZone = HasTag("tag_timezone");
      if (tagZone)
      {
        zoneInfo = TimeZoneInfo.FindSystemTimeZoneById((string) tagZone);
      }
    }

    /// <summary>
    /// Determines if the machine is permitted to run on the current day or not
    /// </summary>
    private void PermittedToRunDay(DateTime timeNow)
    {
      dynamic tagDaysPermitted;

      // Determine the days of the week that the machine is permitted to run
      string days = settings.First(s => s.name == "permitted_days").value;

      // Determine if the resource group has the tag
      tagDaysPermitted = resourceGroup.HasTag("permitted_days");
      if (tagDaysPermitted)
      {
        logger.LogInformation("{0}: Resource group contains permitted days for running machines - {1}", resourceGroup.GetName(), (string) tagDaysPermitted);
        days = tagDaysPermitted;
      }

      // Determine if the vm has the tag
      tagDaysPermitted = HasTag("permitted_days");
      if (tagDaysPermitted)
      {
        logger.LogInformation("{0} - {1}: Virtual machine contains permitted days for running machines - {2}", resourceGroup.GetName(), GetName(), (string) tagDaysPermitted);
        days = tagDaysPermitted;
      }

      // Turn the days permitted into an array that can be searched for the current day
      string[] daysPermitted = days.Split(",").ToArray();

      // get the current day in the timezone
      int currentDay = (int) timeNow.DayOfWeek;

      // if the current day exists in the daysPermitted then the machine can run
      if (daysPermitted.Contains(currentDay.ToString()))
      {
        permittedToRunOnDay = true;
      }
      else
      {
        logger.LogInformation("{0}: Virtual machine is not permitted to run on a {1}: {2}", resourceGroup.GetName(), timeNow.DayOfWeek, GetName());
        permittedToRunOnDay = false;
      }

    }

    /// <summary>
    /// Determine if the virtual machine has previously been notified
    /// The last_notified value is used against the current time to check that it has not
    /// had notifications within the duration in settings
    /// </summary>
    /// <returns>bool</returns>
    public bool NotifiedWithinTimePeriod()
    {
      bool result = false;

      // Use a notification object to determine if the group already exists in the table
      // for this time period
      NotificationDelay previousNotification = (NotificationDelay) notificationDelay.Get(
        new string[] { subscriptionId, virtualMachine.ResourceGroupName, "virtualMachine", virtualMachine.Name },
        new string[] { "subscription", "group_name", "type", "name" }
      );

      if (previousNotification != null)
      {
        int elapsed = (DateTime.UtcNow - previousNotification.last_notified).Seconds;
        result = elapsed > (int) settings.First(s => s.name == "notify_delay_vm").value;

        if (result)
        {
          logger.LogDebug("{0} - {1}: last notified {2} seconds ago", virtualMachine.ResourceGroupName, virtualMachine.Name, elapsed.ToString());
        }
      }

      return result;
    }
  }
}