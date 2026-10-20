# Los Santos Underground

A single-player GTA V street-racing mod built with C#, ScriptHookVDotNet 3, and LemonUI.

## Current repair focus

This source revision fixes the main race-start and race-driving failures:
- Loads race/location JSON beside the installed script DLL, with a `scripts/Data/Json` fallback.
- Shows the number of races and locations loaded at startup to make missing data obvious.
- Teleports the player's current vehicle to the selected race's start grid. If the player is on foot, it creates a Sultan and seats the player before preparing the race.
- Starts route progression at checkpoint 1 because checkpoint 0 is the start grid.
- Adds a GPS route blip and an in-world marker for the current checkpoint.
- Stops reissuing NPC driving tasks every frame, which was continually resetting the AI task.
- Corrects the `DriveTo` argument order in the standalone AI helper.
- Updates racer positions and shows checkpoint progress.
- Uses the Story Mode wallet for entry fees and prizes, charges the entry fee before the race, and refunds it if race setup fails.

The race window remains 5:00 PM–5:00 AM in GTA V game time.

## Build

Requirements:
- Windows
- .NET SDK
- ScriptHookV and ScriptHookVDotNet 3 for GTA V
- LemonUI SHVDN3

From this directory, run PowerShell:

```powershell
.\build.ps1
```

Or build directly:

```powershell
dotnet build .\src\LosSantosUnderground.csproj -c Release
```

The expected output is `build\output\net48\LosSantosUnderground.dll`. The build script also copies `LemonUI.SHVDN3.dll` into the output folder.

## Install

1. Build the project.
2. Copy the contents of `build\output\net48\` into your GTA V `scripts` directory. Keep the `Data\Json\` and `Data\Config\` subfolders beside `LosSantosUnderground.dll`.
3. Ensure ScriptHookV and ScriptHookVDotNet 3 are installed correctly.
4. Launch GTA V Story Mode and set the in-game time to 5:00 PM or later (or before 5:00 AM).
5. Press **F5** to open the race list, select a race, and wait for the countdown. The mod should move the player to the race start grid, spawn opponents, and route the player to the next checkpoint.

## Notes

This archive contains repaired source code, not a newly compiled DLL. Build it locally before testing in GTA V. The source has not been runtime-tested in GTA V in this environment, so please report the first ScriptHookVDotNet log error if one occurs.
