# Tags

Tags are at the heart of how the Azure Reaper operates. Although there are defaults that are applied, the way in which Resource Groups and Virtual Machines  are managed can be customised.

## Default Tags

When a resource group is created the following default tags are applied to it.

| Tag | Description | Example |
|---|---|---|
| `createdDate` | Time that the group was created | |
| `owner` | Full name of the person that created the group | Russell Seymour |
| `ownerEmail` | Email address of the owner | rseymour@chef.io |

## User Tags

A number of tags can be applied to the Resource Group and Virtual Machine which control how the Azure Reaper operates. The following table shows the tags that can be applied and where.

| Tag | Description | Resource Group | Virtual Machine |
|---|---|---|---|---|
| `DAYSOFWEEK` | Days of the week that machines are permitted run on | | Y |
| `InUse` | States that the resource is in use, e.g. live and should not be managed | Y | Y |
| `STARTSTOPTIME` | Start and stop time of machines on permitted days. | | Y |
| `TIMEZONE` | Timezone that the resource should be managed in. | Y | Y |


