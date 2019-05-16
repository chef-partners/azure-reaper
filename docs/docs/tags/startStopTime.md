---
title: STARTSTOPTIME
parent: Tags
layout: default
---

| Type |
|---|
| User |

# Description

The format of the value of this tag is:

`STARTTIME-STOPTIME`

Both the times must in 24 hour format, thus if the machine is permitted to run from 9am to 4pm the correct value would be:

`0900-1600`

This tag can be applied to a resource group or individual virtual machines. When it is applied to a resource group it applies to all virtual machines in the group unless that virtual machine has its own `STARTSTOPTIME` tag.

# Example

![STARTSTOPTIME Tag](/images/tags/startstoptime.png)