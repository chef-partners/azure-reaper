---
title: daysOfWeek
parent: Tags
layout: default
---

| Type |
|---|
| User |

# Description

This tag can be applied to a resource group or a virtual machine to set the days that the virtual machine(s) is permitted to run on.

As with the default setting this is a comma delimited list of the days as shown in the following table:

| # | Day |
|---|---|
| 0 | Sunday |
| 1 | Monday |
| 2 | Tuesday |
| 3 | Wednesday |
| 4 | Thursday |
| 5 | Friday |
| 6 | Saturday |

The order of precedence of the tags and settings is as follows:

| Priority | Type | Description |
|---|---|---|
| 1 | Virtual Machine | Take the permitted days from this tag if it exists. Only applies to this machine |
| 2 | Resource Group | Use the permitted days from this tag if it exists. Applies to all machine in th Resource Group unless a machine has its own tag |
| 3 | Setting | Takes the permitted days from the settings |

# Example

The following screen shot shows this tag being used on resource group to state that virtual machines are only permitted to run on Tuesdays, Wednesdays and Thursdays.

![DAYSOFWEEK Tag](/images/tags/daysofweek.png)