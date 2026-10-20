# Race Repair Notes

## Root causes identified in the source
1. `RaceLoader` and `StreetRacing.LoadLocations` resolved JSON from the game root, although the normal ScriptHookVDotNet installation places the DLL and its data under `scripts`. The mod could load with zero races/locations.
2. Selecting a race in the F5 menu created a session but never moved the player to the start line.
3. `SpawnPlayerRacer` returned `null` when the player was on foot, causing the session to abort in its next update.
4. NPC driving orders were reissued on every game tick, continually restarting the `DriveTo` task.
5. The player and NPCs started progress at checkpoint 0 even though that checkpoint is the starting grid.
6. The session did not create an active GPS route or draw the current race checkpoint.
7. `RacerManager.UpdatePositions` calculated positions but did not store them on racers.
8. The AI helper passed `DriveTo` speed and stopping-range arguments in the wrong order.
9. The money helper accessed the `MP0_WALLET_BALANCE` stat rather than the Story Mode player's wallet.

## What changed
- Script-relative data loading with a fallback path.
- Startup count feedback for races and locations.
- Player vehicle positioning and fallback vehicle creation.
- Route blip and active checkpoint marker.
- NPC task refresh throttling and corrected driving arguments.
- Checkpoint progression starts after the starting grid.
- Racer position storage and checkpoint notifications.
- Entry-fee handling with a setup-failure refund.

## Validation status
JSON files and archive structure were checked. This environment does not have `dotnet`, MSBuild, Mono, or a C# compiler available, so a fresh compiled DLL could not be produced here. The project must be built locally and tested in GTA V before considering the mod verified.

## Follow-up: no blips / "No races available"
The supplied ScriptHookVDotNet log confirms that `LosSantosUnderground.dll` was loaded and its script instantiated, but it does not show a race-data load message. The most likely installation failure is that `Data/Json/races.json` and `locations.json` were not copied beside the DLL under `scripts/Data/Json/`. This revision embeds both JSON files into the assembly and uses them as a fallback when loose files are absent. It also writes diagnostic count messages through `Debug.WriteLine` for ScriptHookVDotNet logging.


## Follow-up: road targets, checkpoint placement, and NPC navigation
- At startup, race checkpoints and race-location markers are projected onto a nearby GTA street node when the nearest node is within 65 metres. Far-away/off-road targets are left untouched and logged rather than redirected to an unrelated road.
- The active checkpoint marker is smaller and drawn closer to the road surface.
- NPC driving tasks are retained until the checkpoint changes. A task is only reissued for recovery after an NPC has been nearly stationary for at least four seconds. NPCs no longer use `StopForVehicles`, which could make traffic block the race line.
- This improves bad-coordinate handling and task stability, but the authored route still needs in-game validation; automatic snapping cannot replace hand-authored waypoints through every turn.


## Follow-up: aggressive NPC street-racing behavior
- Removed traffic-stop behavior from the active NPC race task; NPCs do not receive `StopForVehicles` or `StopAtLights` flags.
- Increased AI target speeds to a profile-driven 56–70 m/s (about 202–252 km/h target speed; actual speed remains limited by vehicle handling/model constraints).
- Enabled obstacle/pedestrian steering, shortcut links, and wrong-way allowance for competitive racing. Wrong-way allowance is intentionally aggressive and can be risky on complex routes; routes still need road-node validation.
- Applied GTA's rushed driving style (786468) after issuing the checkpoint drive task and set the vehicle speed limit to the chosen target.
- These are source changes; compile and in-game validation are still required before treating the build as verified.
