---
title: vm_start
parent: Settings
layout: default
---

| Default Value | Category | Type |
|---|---|---|
| 0800 | lifecycle | `string` |

# Description

Time at which a virtual machine can run from.

This time is calculated in the timezone of the data centre the machine is in or from the [`TIMEZONE`](/pages/tags/timezone.html) tag applied to a resource group or virtual machine.

{% include note.html content="This time only takes effect if the machine is allowed to run on the current day" %}

# Tag

The corresponding tag for setting the start time for a machine is [`STARTSTOPTIME`](/pages/tags/startstoptime.html)

