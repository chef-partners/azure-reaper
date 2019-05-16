---
title: vm_stop
parent: Settings
layout: default
---

| Default Value | Category | Type |
|---|---|---|
| 1800 | lifecycle | `string` |

# Description

Time from which a machine should not be running.

This time is calculated in the timezone of the data centre the machine is in or from the [`TIMEZONE`](/pages/tags/timezone.html) tag applied to a resource group or virtual machine.

# Tag

The corresponding tag for setting the start time for a machine is [`STARTSTOPTIME`](/pages/tags/startstoptime.html)

