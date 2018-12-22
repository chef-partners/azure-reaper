# Azure Reaper

The Azure Reaper is designed to assist in the management of Resource Groups and Virtual Machines in Azure.

Whilst Resource Groups do not cost money themselves, they invariably contain resources that do. The Azure Reaper checks when a Resource Group was created to determine if its has expired (a default of 7 days old). This duration can be overridden using tags. When a Resource Group is deemed to be expired it is deleted. There are additional tags that can be used to denote that the group should not be managed by the Reaper.

By far the most expensive components in Azure are running virtual machines. The Azure Reaper will ensure that machines are only running during permitted times on permitted days. There are default settings for this, but they can be overridden using tags on the virtual machine. The power management of machines is aware of local timezones which can also be overridden using tags.

So that people are aware of what is happening with their resources, the owner is notified on Slack. Resource Groups do not hold the creation date of owner information so they system uses the Azure Activity Monitor to understand when a new Resource Group is created. The information received contains the name of the group, the subscription it was created in and who created it. This information is added to the tags of the Resource Group so that the Azure Reaper is able to perform the necessary operations.

## Process

