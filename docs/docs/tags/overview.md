---
title: Tags
has_children: true
nav_order: 30
layout: default
---

The settings that are held within the database are the default values that are applied when considering resource groups and virtual machines.

As the Azure Reaper is designed to be run across multiple teams and subscriptions it needs to be able to accomodate different scenarios. To this end tags can be applied to resource groups and virtual machines which will override the default values.

The tags that are supported by the Reaper are listed in the left hand navigation.

# Automatic Tags

Some of the tags that are listed are automatically added to the resource group, these are not intended to be manually added to the group.

When the Reaper is deployed into Azure a monitor rule, alert and action group are configured. The rule will detect when a new resource group has been created which will then fire an alert to the action group. It is at this point that the tagger receives this information.

The tagger will then attempt to retrieve the date and time, the person that created the group and their email address. This information is then set on the resource group in the following tags:

 - [`createdDate`](/pages/tags/createddate.html)
 - [`owner`](/pages/tags/owner.html)
 - [`ownerEmail`](/pages/tags/owneremail.html)