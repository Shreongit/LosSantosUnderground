using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using LosSantosUnderground.Races;
using LosSantosUnderground.UI;

namespace LosSantosUnderground.Racers
{
    public class RacerManager : IDisposable
    {
        private readonly List<Racer> _racers =
            new List<Racer>();

        private RaceDefinition _race;
        private Racer _playerRacer;
        private bool _raceStarted;
        private bool _disposed;
        private DateTime _raceStartTime;
        private readonly Dictionary<string, int> _lastCommandCheckpoint =
            new Dictionary<string, int>();
        private readonly Dictionary<string, int> _lastCommandTime =
            new Dictionary<string, int>();

        private readonly RacerSpawner _spawner =
            new RacerSpawner();

        public RacerManager()
        {
        }

        public RacerManager(
            RaceDefinition race)
        {
            _race = race;
        }

        public IReadOnlyList<Racer> Racers
        {
            get { return _racers.AsReadOnly(); }
        }

        public void SetRace(
            RaceDefinition race)
        {
            if (_disposed)
                return;

            ClearRacers();

            _race = race;

            if (_race == null ||
                _race.Checkpoints == null ||
                _race.Checkpoints.Count == 0)
            {
                return;
            }

            int totalRacers =
                _race.MaxRacers > 0
                    ? _race.MaxRacers
                    : 6;

            if (totalRacers < 1)
                totalRacers = 1;

            int npcCount =
                totalRacers - 1;

            Racer[] spawned =
                _spawner.SpawnRaceRacers(
                    _race,
                    npcCount);

            if (spawned == null)
                return;

            foreach (Racer racer in spawned)
            {
                if (racer == null)
                    continue;

                AddRacer(racer);

                if (racer.Id == "player")
                    _playerRacer = racer;
            }
        }

        public RaceDefinition GetRace()
        {
            return _race;
        }

        public bool HasPlayerRacer()
        {
            return _playerRacer != null &&
                   _playerRacer.HasValidEntities();
        }

        public Racer GetPlayerRacer()
        {
            return _playerRacer;
        }

        public int GetRacerCount()
        {
            return _racers.Count;
        }

        public int GetActiveRacerCount()
        {
            int count = 0;

            foreach (Racer racer in _racers)
            {
                if (racer == null)
                    continue;

                if (racer.IsEliminated)
                    continue;

                if (racer.HasValidEntities())
                    count++;
            }

            return count;
        }

        public void AddRacer(
            Racer racer)
        {
            if (_disposed ||
                racer == null)
            {
                return;
            }

            if (_racers.Contains(racer))
                return;

            _racers.Add(racer);
        }

        public void SetPlayerRacer(
            Racer racer)
        {
            if (racer == null)
                return;

            AddRacer(racer);

            _playerRacer = racer;
        }

        public bool RemoveRacer(
            Racer racer)
        {
            if (racer == null)
                return false;

            if (racer == _playerRacer)
                _playerRacer = null;

            return _racers.Remove(racer);
        }

        public void StartRace()
        {
            if (_disposed)
                return;

            if (_racers.Count == 0)
                return;

            if (!HasPlayerRacer())
                return;

            _raceStarted = true;
            _raceStartTime = DateTime.UtcNow;

            _lastCommandCheckpoint.Clear();
            _lastCommandTime.Clear();

            int firstRaceCheckpoint =
                _race != null && _race.Checkpoints != null &&
                _race.Checkpoints.Count > 1 ? 1 : 0;

            foreach (Racer racer in _racers)
            {
                if (racer == null)
                    continue;

                // Checkpoint 0 is the starting grid; route navigation begins
                // with checkpoint 1 after the countdown.
                racer.StartRace(firstRaceCheckpoint);
            }
        }

        public void Update()
        {
            if (_disposed ||
                !_raceStarted)
            {
                return;
            }

            UpdateCheckpointProgress();
            UpdatePositions();
            UpdateNpcDrivers();
        }

        private void UpdateCheckpointProgress()
        {
            if (_race == null ||
                _race.Checkpoints == null ||
                _race.Checkpoints.Count == 0)
            {
                return;
            }

            foreach (Racer racer in _racers)
            {
                if (racer == null ||
                    !racer.IsActive ||
                    racer.IsFleeing ||
                    racer.HasFinished ||
                    racer.IsEliminated)
                {
                    continue;
                }

                if (!racer.HasValidEntities())
                {
                    racer.Eliminate();
                    continue;
                }

                int checkpointIndex =
                    racer.CurrentCheckpoint;

                if (checkpointIndex < 0 ||
                    checkpointIndex >=
                    _race.Checkpoints.Count)
                {
                    continue;
                }

                RaceCheckpoint checkpoint =
                    _race.Checkpoints[
                        checkpointIndex];

                if (checkpoint == null)
                    continue;

                Vector3 racerPosition =
                    racer.GetPosition();

                float distance =
                    racerPosition.DistanceTo(
                        checkpoint.Position);

                if (distance > checkpoint.Radius)
                    continue;

                bool isFinish =
                    checkpoint.IsFinish ||
                    checkpointIndex ==
                    _race.Checkpoints.Count - 1;

                if (isFinish)
                {
                    FinishRacer(racer);
                }
                else
                {
                    racer.AdvanceCheckpoint();

                    if (racer == _playerRacer)
                    {
                        RaceUI.OnCheckpointPassed(
                            checkpointIndex + 1,
                            _race.Checkpoints.Count);
                    }
                }
            }
        }

        private void FinishRacer(
            Racer racer)
        {
            if (racer == null ||
                racer.HasFinished)
            {
                return;
            }

            int position = 1;

            foreach (Racer other in _racers)
            {
                if (other == null ||
                    other == racer ||
                    other.IsEliminated)
                {
                    continue;
                }

                if (other.HasFinished &&
                    other.Position > 0 &&
                    other.Position < position)
                {
                    position =
                        other.Position + 1;
                }
            }

            TimeSpan raceTime =
                DateTime.UtcNow -
                _raceStartTime;

            racer.Finish(
                raceTime,
                position);
        }

        private void UpdatePositions()
        {
            foreach (Racer racer in _racers)
            {
                if (racer == null ||
                    racer.IsEliminated ||
                    racer.HasFinished)
                {
                    continue;
                }

                int position = 1;

                foreach (Racer other in _racers)
                {
                    if (other == null ||
                        other == racer ||
                        other.IsEliminated)
                    {
                        continue;
                    }

                    if (other.HasFinished)
                    {
                        position++;
                        continue;
                    }

                    if (other.CurrentCheckpoint >
                        racer.CurrentCheckpoint)
                    {
                        position++;
                        continue;
                    }

                    if (other.CurrentCheckpoint <
                        racer.CurrentCheckpoint)
                    {
                        continue;
                    }

                    Vector3 checkpointPosition =
                        GetCheckpointPosition(
                            racer.CurrentCheckpoint);

                    float otherDistance =
                        other.GetPosition()
                            .DistanceTo(
                                checkpointPosition);

                    float racerDistance =
                        racer.GetPosition()
                            .DistanceTo(
                                checkpointPosition);

                    if (otherDistance < racerDistance)
                        position++;
                }

                racer.SetPosition(position);
            }
        }

        private Vector3 GetCheckpointPosition(
            int checkpointIndex)
        {
            if (_race == null ||
                _race.Checkpoints == null ||
                _race.Checkpoints.Count == 0)
            {
                return Vector3.Zero;
            }

            if (checkpointIndex < 0)
                checkpointIndex = 0;

            if (checkpointIndex >=
                _race.Checkpoints.Count)
            {
                checkpointIndex =
                    _race.Checkpoints.Count - 1;
            }

            RaceCheckpoint checkpoint =
                _race.Checkpoints[
                    checkpointIndex];

            if (checkpoint == null)
                return Vector3.Zero;

            return checkpoint.Position;
        }

        private void UpdateNpcDrivers()
        {
            if (_race == null ||
                _race.Checkpoints == null)
            {
                return;
            }

            foreach (Racer racer in _racers)
            {
                if (racer == null ||
                    racer == _playerRacer ||
                    !racer.IsActive ||
                    racer.IsFleeing ||
                    racer.HasFinished ||
                    racer.IsEliminated)
                {
                    continue;
                }

                UpdateNpcDriver(racer);
            }
        }

        private void UpdateNpcDriver(
            Racer racer)
        {
            if (racer.Ped == null ||
                racer.Vehicle == null ||
                !racer.Ped.Exists() ||
                !racer.Vehicle.Exists() ||
                !racer.Ped.IsInVehicle(racer.Vehicle))
            {
                return;
            }

            int checkpointIndex = racer.CurrentCheckpoint;
            if (checkpointIndex < 0 ||
                checkpointIndex >= _race.Checkpoints.Count)
                return;

            RaceCheckpoint checkpoint = _race.Checkpoints[checkpointIndex];
            if (checkpoint == null)
                return;

            string racerId = racer.Id ?? racer.GetHashCode().ToString();
            int now = Game.GameTime;
            int lastTime;
            int lastCheckpoint;

            bool hasLastTime = _lastCommandTime.TryGetValue(racerId, out lastTime);
            bool hasLastCheckpoint = _lastCommandCheckpoint.TryGetValue(racerId, out lastCheckpoint);

            // Keep the current navigation task alive. Reissuing DriveTo on a
            // short timer makes the AI recalculate its path repeatedly, which
            // produces steering oscillation and wall-seeking at intersections.
            // Only assign a task for a new checkpoint, or recover an NPC that
            // has remained nearly stationary for at least four seconds.
            int elapsed = hasLastTime
                ? unchecked(now - lastTime)
                : int.MaxValue;

            bool checkpointChanged = !hasLastCheckpoint ||
                lastCheckpoint != checkpointIndex;
            bool stalled = hasLastTime &&
                racer.Vehicle.Speed < 1.5f &&
                elapsed >= 4000;

            if (!checkpointChanged && !stalled)
                return;

            // Street-race driving: do not use StopForVehicles or StopAtLights.
            // Allow aggressive lane changes and shortcuts, while still steering
            // around obstacles. The native driving style makes the task rushed
            // rather than the conservative default used by ordinary traffic.
            float targetSpeed = GetAggressiveRaceSpeed(racer);
            VehicleDrivingFlags raceFlags =
                VehicleDrivingFlags.SwerveAroundAllVehicles |
                VehicleDrivingFlags.SteerAroundStationaryVehicles |
                VehicleDrivingFlags.SteerAroundPeds |
                VehicleDrivingFlags.UseShortCutLinks |
                VehicleDrivingFlags.AllowGoingWrongWay;

            Function.Call(
                Hash.SET_VEHICLE_MAX_SPEED,
                racer.Vehicle.Handle,
                targetSpeed);

            racer.Ped.Task.DriveTo(
                racer.Vehicle,
                checkpoint.Position,
                targetSpeed,
                raceFlags,
                Math.Max(2.0f, Math.Min(checkpoint.Radius, 4.0f)));

            // 786468 is the commonly used rushed/aggressive GTA driving style.
            Function.Call(
                Hash.SET_DRIVE_TASK_DRIVING_STYLE,
                racer.Ped.Handle,
                786468);

            _lastCommandTime[racerId] = now;
            _lastCommandCheckpoint[racerId] = checkpointIndex;
        }

        private static float GetAggressiveRaceSpeed(Racer racer)
        {
            // GTA driving task speed is expressed in metres/second. Keep the
            // target high enough to feel like a race, but vary it by the rival's
            // profile so the field does not drive at one identical pace.
            float aggression = 0.70f;
            float skill = 0.70f;

            if (racer != null && racer.Profile != null)
            {
                aggression = Math.Max(0.0f, Math.Min(1.0f, racer.Profile.Aggression));
                skill = Math.Max(0.0f, Math.Min(1.0f, racer.Profile.Skill));
            }

            float speed = 56.0f + (aggression * 8.0f) + (skill * 6.0f);
            return Math.Max(56.0f, Math.Min(70.0f, speed));
        }

        public bool HasPlayerFinished()
        {
            return _playerRacer != null &&
                   _playerRacer.HasFinished;
        }

        public int GetPlayerPosition()
        {
            if (_playerRacer == null)
                return 0;

            if (_playerRacer.HasFinished &&
                _playerRacer.Position > 0)
            {
                return _playerRacer.Position;
            }

            int position = 1;

            foreach (Racer racer in _racers)
            {
                if (racer == null ||
                    racer == _playerRacer ||
                    racer.IsEliminated)
                {
                    continue;
                }

                if (racer.HasFinished)
                {
                    position++;
                    continue;
                }

                if (racer.CurrentCheckpoint >
                    _playerRacer.CurrentCheckpoint)
                {
                    position++;
                    continue;
                }

                if (racer.CurrentCheckpoint <
                    _playerRacer.CurrentCheckpoint)
                {
                    continue;
                }

                Vector3 checkpointPosition =
                    GetCheckpointPosition(
                        _playerRacer.CurrentCheckpoint);

                if (racer.GetPosition()
                        .DistanceTo(
                            checkpointPosition)
                    <
                    _playerRacer.GetPosition()
                        .DistanceTo(
                            checkpointPosition))
                {
                    position++;
                }
            }

            return position;
        }

        public int GetPrizePool()
        {
            if (_race == null)
                return 0;

            return _race.EntryFee *
                   _racers.Count;
        }

        public bool AllRacersFinished()
        {
            if (_racers.Count == 0)
                return true;

            foreach (Racer racer in _racers)
            {
                if (racer == null ||
                    racer.IsEliminated)
                {
                    continue;
                }

                if (racer.IsActive)
                    return false;

                if (!racer.HasFinished &&
                    !racer.IsFleeing)
                {
                    return false;
                }
            }

            return true;
        }

        public void AbortRace()
        {
            _raceStarted = false;

            foreach (Racer racer in _racers)
            {
                if (racer == null)
                    continue;

                racer.AbortRace();
            }
        }

        public void FleeFromPolice()
        {
            _raceStarted = false;

            foreach (Racer racer in _racers)
            {
                if (racer == null)
                    continue;

                if (!racer.HasValidEntities())
                    continue;

                racer.FleeFromPolice();
            }
        }

        public void ClearRacers()
        {
            foreach (Racer racer in _racers)
            {
                if (racer == null)
                    continue;

                racer.Dispose();
            }

            _racers.Clear();
            _playerRacer = null;
            _raceStarted = false;
            _lastCommandCheckpoint.Clear();
            _lastCommandTime.Clear();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            ClearRacers();
        }
    }
}