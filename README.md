# PubNet

Private pub.dev alternative.

## Setup

Add a `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "Packages": "Data Source=pubnet.db",
    "Authors": "Data Source=pubnet.db"
  },
  "Authentication": {
    "SecretKey": "$om3th1ng.R4nd0m"
  }
}
```
