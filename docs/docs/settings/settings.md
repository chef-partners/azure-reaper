---
title: Settings
layout: default
has_children: true
nav_order: 20
---

This section contains all the settings that are stored in the Azure CosmosDB. Some of the settings have corresponding tags that will affect the value when the Reaper encounters them on a resource.

## Default Settings

The default settings need to be applied to the application after deployment. Once the function has been deployed an HTTP endpoint will be generated. The following code snippets show how the settings can be uploaded into the database.

{% include note.html content="It is assumed that the Reaper repository has been cloned, however if not then please do so or look at the section on downloading the files so they can be applied" %}

The `settings.json` file needs to be updated with information. This information is mainly for the Slack API connection which is unique to each installation. The following need to be updated in the file:

 - [`web_hook_url`](/settings/web_hook_url.html)
 - [`token`](/settings/token.html)

Once the settings have been updated, they can be added to the database.

{% include note.html content="For the following example a dummy HTTP endpoint has been used of https://reaper.example.com, this needs to be modified to suit your environment" %}

### Bash

```bash
curl -XPOST https://reaper.example/com/api/setting -d settings.json
curl -XPOST https://reaper.example/com/api/location -d timezones.json
```

### PowerShell

```powershell
$data = Get-Content -Path settings.json -Raw
Invoke-RestMethod -Method POST -Uri https://reaper.example/com/api/setting -Body $data -Content "application/json"
$data = Get-Content -Path timezones.json -Raw
Invoke-RestMethod -Method POST -Uri https://reaper.example/com/api/location -Body $data -Content "application/json"
```

## Download settings files

The following commands show how the two files that are used for settings can be downloaded if the repo has not been cloned.

### Bash

```bash
curl https://raw.githubusercontent.com/chef-partners/azure-reaper/master/data/settings.json --output settings.json
curl https://raw.githubusercontent.com/chef-partners/azure-reaper/master/data/timezones.json --output timezones.json
```

### PowerShell

```powershell
Invoke-RestMethod -uri https://raw.githubusercontent.com/chef-partners/azure-reaper/master/data/settings.json -outfile settings.json
Invoke-RestMethod -uri https://raw.githubusercontent.com/chef-partners/azure-reaper/master/data/timezones.json -outfile timezones.json
```