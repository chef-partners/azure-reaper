---
title: TIMEZONE
parent: Tags
layout: default
---

| Type | Resource Group | Virtual Machine |
|---|---|---|
| User | Y | Y |

# Description

The timezone when assessing when a resource group should be deleted or a virtual machine should be shutdown, is based on the Azure data centre that the machine has been deployed into.

However in some cases it maybe necessary to change this timezone to another region, for example building a demo in one region and then showing that demo in another.

When this is required the Windows timezone id can be set as the value for this tag.

A full list of these ids can be found from the [Microsoft Time Zone Index Values](https://support.microsoft.com/en-gb/help/973627/microsoft-time-zone-index-values) page.

# Example

![TIMEZONE Tag](/images/tags/timezone.png)