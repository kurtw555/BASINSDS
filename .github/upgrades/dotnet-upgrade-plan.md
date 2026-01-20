# .NET 8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 8.0 upgrade.
3. Upgrade atcStatusMonitor\atcStatusMonitorDS.vbproj

## Settings

This section contains settings and data used by execution steps.

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### atcStatusMonitor\atcStatusMonitorDS.vbproj modifications

Project properties changes:
  - Project file needs to be converted to SDK-style format
  - Target framework should be changed from `.NETFramework,Version=v3.5` to `net8.0-windows`

Other changes:
  - Convert from legacy project format to modern SDK-style project format
  - Update project references and dependencies to be compatible with .NET 8.0