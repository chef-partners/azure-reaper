---
title: permitted_days
parent: Settings
layout: default
---

| Default Value | Category | Type |
|---|---|---|
| 1,2,3,4,5 | lifecycle | `string` |

# Description

This specifies the days that a virtual machine is permitted to run on.

| # | Day |
|---|---|
| 0 | Sunday |
| 1 | Monday |
| 2 | Tuesday |
| 3 | Wednesday |
| 4 | Thursday |
| 5 | Friday |
| 6 | Saturday |

The value is a comma delimited list of the days that are permitted.

# Tags

The permitted days can be overridden on the resource using the [`DAYSOFWEEK`](/pages/tags/daysofweek.html) tag.