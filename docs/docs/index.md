---
permalink: /
layout: default
title: Overview
nav_order: 10
---

{% include note.html content="Please refer to the [Azure Reaper Deployment](https://chef-partners.github.io/azure-reaper-deploy) for information on how to deploy the Reaper. The documentation here is about how the Reaper works" %}

The Azure Reaper is an Azure Function that has been developed to assist in the management of resources in Azure.

The reaper considers to things:

 1. Resource Group age
 2. Virtual Machines within a Resource Group

By default the reaper will delete a resource group after 7 days. There are reasons that this may not be practical, such a preparing for a demonstration or it contains a live service; in this case tags can be applied to the groups to change the default behaviour.

After the Reaper has considered a resource group, it will look at the power state of any virtual machines in that group. It will ensure that machines are only running between "office hours" (whatever that has been defined as). If a machine is running outside of these hours it will be shutdown and vice versa. Again this default behaviour can be overridden using tags.

The Reaper does not do all this without letting people know what is going on. It does this using Slack. Part of the configuration defines how to communicate with Slack. Built into the Reaper is a notification delay system which prevents people from being bombarded by the Reaper each time it runs.

