---
title: ownerEmail
parent: Tags
layout: default
---

| Type |
|---|
| System |

# Description

This tag is automatically added to the Resource Group when it has been created by the Tagger function.

The value of this tag is the email address that is associated with the user that created the resource group. This information comes from the Azure Monitor Alert that is triggered when the group is created.

The email address is used within Slack to determine the user id that needs to be used to send the owner notification messages.

# Example

![Resource Tagging](/images/settings/resource_tagging.png)