# Settings

There are a number of settings that are included with the Azure Reaper. These or course can be overridden using the Operations REST API. The following table shows the all the settings and there initial default values.

| Name | Description | Category | Default |
|---|---|---|---|
| `bot_username` | Name of the Azure Reaper in Slack | slack | Azure Reaper |
| `destroy` | State if the Azure Reaper is in destructive mode | lifecycle | false |
| `duration` | Number of days that a Resource Group can exist | lifecycle | 7 |
| `icon_url` | URL to the icon that will be displayed in the Slack notification | slack | https://cdn.icon-icons.com/icons2/390/PNG/512/grim-reaper_38285.png | 
| `manage_vms` | State of the Azure Reaper can stop and start virtual machines | lifecycle | false |
| `notification_emails` | List of people that will receive notifications. A empty value mens anyone can receive them | |
| `notify_delay` | How often someone will be notified about a resource. This delay is the time period between each notification for the same resource | lifecycle | 86400 |
| `permitted_days` | Days that a machine can run on | lifecycle | 1,2,3,4,5 |
| `tag_date` | Name of the creation date tag | tags | createdDate |
| `tag_days_of_week` | Name of the days of the week tag | tags | DAYSOFWEEK |
| `tag_owner` | Name of the owner tag | tags | owner |
| `tag_owner_email` | Name of the owner email address tag | tags | ownerEmail |
| `tag_timezone` | Name of the timezone tag |tags | TIMEZONE |
| `tag_vm_start_stop_time` | Name of the start / stop time tag for virtual machines | tags | STARTSTOPTIME |
| `slack_enabled` | State if notification via Slack is turned on | slack | false |
| `token` | Slack API token to be used to send messages | slack | |
| `vm_start` | Default time that a virtual machine can be started | lifecycle | 0800 |
| `vm_stop` | Default time that a virtual machine should be stopped | lifecycle | 1800 |
| `webhook_url` | Webhook URL for the Slack application | slack | |
