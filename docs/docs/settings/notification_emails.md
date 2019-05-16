---
title: notification_emails
parent: Settings
layout: default
---

| Default Value | Category | Type |
|---|---|---|
| _null_ | slack | `string` |

# Description

This option allows only certain people to be notified about any changes that will occur in the subscription.

By default when Slack notifications are enabled, everyone that the Reaper can find a valid account for will be notified. This might not be ideal, especially when first implementing the Reaper.

If a (comma delimited) list of email addresses is added here, then only those people will be notified about _their_ resources. They will **not** be notified about resources they do not own.

The email address is not use directly. It is used to find the user id of the person in Slack so that the Reaper can target a notification to them.

# Example

```json
{
  "name": "notification_emails",
  "value": "fredblogg@example.com, rseymour@chef.io",
  "category": "slack",
  "display_name": "Notifications",
  "description": "List of email addresses that should receive Slack notifications from the reaper"
}
```