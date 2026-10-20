# Los Santos Underground

**An underground street racing experience for Grand Theft Auto V Story Mode.**

Los Santos Underground is a C#-based GTA V single-player mod focused on illegal street racing, rival NPC drivers, custom race routes, police detection, betting, and progression. It aims to turn Los Santos into a street racing scene where players can compete in races, earn rewards, and build their reputation.

## Features

- **Custom Street Races** — Race definitions and locations configured through JSON data files.
- **Checkpoint Navigation** — Ordered checkpoints, race progression, and GPS route guidance.
- **NPC Rival Racers** — Spawned opponents with AI-controlled vehicles and checkpoint-based driving.
- **Race Management** — Race selection, countdowns, race states, finishing positions, and results.
- **Police Detection** — Police awareness and race-abort mechanics intended to add risk to illegal street racing.
- **Economy System** — Entry fees, prize pools, money handling, and betting-related components.
- **Reputation and Progression** — Reputation tracking and race-unlock systems.
- **In-Game UI** — Race menus, HUD elements, notifications, map blips, and checkpoint markers.
- **Configurable Data** — JSON-based race, location, racer, and configuration data.

## Requirements

Before installing, make sure you have:

- Grand Theft Auto V Legacy (Story Mode)
- ScriptHookV
- ScriptHookVDotNet 3
- LemonUI for ScriptHookVDotNet 3
- .NET Framework 4.8 targeting support if building from source

This mod is intended for single-player Story Mode, not GTA Online.

## Installation

1. Install ScriptHookV and ScriptHookVDotNet 3 according to their respective installation instructions.
2. Obtain the compatible LemonUI ScriptHookVDotNet 3 dependency.
3. Build the project or obtain a compiled release.
4. Copy `LosSantosUnderground.dll` into your GTA V `scripts` directory.
5. Copy the required `LemonUI.SHVDN3.dll` dependency into the same directory if it is not already installed.
6. Preserve the mod's `Data` directory structure when distributing or installing the JSON data files.
7. Launch GTA V and enter Story Mode.

Example installation path:

```text
Grand Theft Auto V/
├── scripts/
│   ├── LosSantosUnderground.dll
│   ├── LemonUI.SHVDN3.dll
│   └── Data/
│       └── Json/
│           ├── races.json
│           ├── locations.json
│           └── racers.json
```

The exact data files included in a release may vary by version. Follow the structure provided by that release.

## Controls

| Key | Action |
|---|---|
| `F5` | Open the street racing menu |

Additional interactions may be available through the in-game menus.

## Building from Source

### 1. Clone the repository

```powershell
git clone https://github.com/YOUR_USERNAME/LosSantosUnderground.git
cd LosSantosUnderground
```

Replace `YOUR_USERNAME` with the GitHub account that hosts the repository.

### 2. Build the project

Open PowerShell in the project root and execute:

```powershell
.\build.ps1
```

The build script restores the required project dependencies and builds the mod.

If the build succeeds, locate the generated `LosSantosUnderground.dll` in the build output directory reported by the script. Install the DLL and required dependencies into your GTA V `scripts` directory.

### 3. Troubleshooting build errors

- Verify that the .NET Framework 4.8 targeting components are available.
- Check that the required ScriptHookVDotNet and LemonUI references resolve correctly.
- Review the compiler output for missing references or namespace conflicts.
- Ensure the build script is executed from the project root.

## Configuration and Data

The mod uses JSON files to define gameplay data.

| File | Purpose |
|---|---|
| `races.json` | Race definitions and checkpoint data |
| `locations.json` | Race locations and associated world coordinates |
| `racers.json` | NPC racer profiles, where supported by the active implementation |
| `config.json` | Configurable gameplay settings, where supported by the active implementation |

Keep JSON formatting valid when editing these files. Incorrect coordinates or invalid race definitions can cause unexpected navigation or gameplay behavior.

## Troubleshooting

### The menu says "No races available"

- Confirm that the mod DLL is installed in the correct `scripts` directory.
- Check that the race JSON data is present or embedded in the installed build.
- Review `ScriptHookVDotNet.log` for loading errors.

### Race blips do not appear

- Verify that the location data loads correctly.
- Check the coordinates in `locations.json`.
- Review the ScriptHookVDotNet log for location-loading errors.

### NPC racers drive incorrectly

- Check the race checkpoint coordinates and their ordering.
- Verify that checkpoints are positioned on accessible roads.
- Check for errors related to vehicle spawning or driving tasks.

### The mod does not load

- Verify that ScriptHookV and ScriptHookVDotNet are installed correctly.
- Confirm that the installed DLL was built for the intended runtime and API.
- Inspect `ScriptHookVDotNet.log` for script initialization errors.

## Development Status

Los Santos Underground is an actively developed mod project.

The project contains race definitions, race management, checkpoint navigation, NPC driving logic, police-related mechanics, UI components, and progression-related systems. Feature completeness and in-game reliability may vary by version.

Testing and refinement are particularly important for route accuracy, NPC driving behavior, race cleanup, and the integration of progression and economy systems.

## Contributing

Contributions, bug reports, and suggestions are welcome.

If you find a bug:

1. Describe the issue and how to reproduce it.
2. Include your GTA V edition and ScriptHookVDotNet version.
3. Attach relevant error messages from `ScriptHookVDotNet.log`.
4. Include screenshots or video when the issue involves race routes or NPC behavior.

Please test changes in Story Mode before submitting them.

## Disclaimer

Los Santos Underground is an independent fan-made modification. It is not affiliated with, endorsed by, or sponsored by Rockstar Games.

Grand Theft Auto V and its related assets are the property of their respective owners. This project is intended for single-player use.

## License

No license has been specified for this repository yet. Until a license is added, the source code should not be assumed to be available for unrestricted redistribution or reuse.
