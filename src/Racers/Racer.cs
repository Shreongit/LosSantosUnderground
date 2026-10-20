using System;
using GTA;
using GTA.Math;

namespace LosSantosUnderground.Racers
{
    public class Racer : IDisposable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public RacerProfile Profile { get; set; }

        public Vehicle Vehicle { get; private set; }
        public Ped Ped { get; private set; }

        public int CurrentCheckpoint { get; private set; }
        public bool IsActive { get; private set; }
        public bool HasFinished { get; private set; }
        public bool IsFleeing { get; private set; }
        public bool IsEliminated { get; private set; }

        public TimeSpan FinishTime { get; private set; }
        public int Position { get; private set; }

        public Racer()
        {
            CurrentCheckpoint = 0;
            Position = 0;
            FinishTime = TimeSpan.Zero;
        }

        public void AssignEntities(
            Ped ped,
            Vehicle vehicle)
        {
            Ped = ped;
            Vehicle = vehicle;

            if (Ped != null &&
                Ped.Exists())
            {
                Ped.BlockPermanentEvents = true;
            }
        }

        public void StartRace(int initialCheckpoint = 0)
        {
            if (!HasValidEntities())
                return;

            IsActive = true;
            HasFinished = false;
            IsFleeing = false;
            IsEliminated = false;
            CurrentCheckpoint = Math.Max(0, initialCheckpoint);
            Position = 0;
            FinishTime = TimeSpan.Zero;

            Ped.Task.ClearAll();
        }

        public void SetPosition(int position)
        {
            Position = Math.Max(0, position);
        }

        public void AdvanceCheckpoint()
        {
            if (!IsActive ||
                HasFinished ||
                IsFleeing)
            {
                return;
            }

            CurrentCheckpoint++;
        }

        public void Finish(
            TimeSpan raceTime,
            int finalPosition)
        {
            if (HasFinished)
                return;

            HasFinished = true;
            IsActive = false;
            IsFleeing = false;
            FinishTime = raceTime;
            Position = finalPosition;

            if (Ped != null &&
                Ped.Exists())
            {
                Ped.Task.ClearAll();
            }
        }

        public void FleeFromPolice()
        {
            if (!HasValidEntities())
                return;

            IsActive = false;
            HasFinished = false;
            IsFleeing = true;

            Ped.BlockPermanentEvents = false;

            Ped.Task.FleeFrom(
                Game.Player.Character,
                -1);

            if (Vehicle != null &&
                Vehicle.Exists())
            {
                Vehicle.IsPersistent = false;
            }
        }

        public void AbortRace()
        {
            IsActive = false;
            HasFinished = false;
        }

        public void Eliminate()
        {
            IsActive = false;
            IsEliminated = true;
        }

        public bool HasValidEntities()
        {
            return Ped != null &&
                   Ped.Exists() &&
                   !Ped.IsDead &&
                   Vehicle != null &&
                   Vehicle.Exists() &&
                   !Vehicle.IsDead;
        }

        public Vector3 GetPosition()
        {
            if (Vehicle != null &&
                Vehicle.Exists())
            {
                return Vehicle.Position;
            }

            if (Ped != null &&
                Ped.Exists())
            {
                return Ped.Position;
            }

            return Vector3.Zero;
        }

        public float GetDistanceTo(
            Vector3 position)
        {
            return GetPosition()
                .DistanceTo(position);
        }

        public void Dispose()
        {
            IsActive = false;

            if (Ped != null &&
                Ped.Exists())
            {
                Ped.Task.ClearAll();
                Ped.MarkAsNoLongerNeeded();
            }

            if (Vehicle != null &&
                Vehicle.Exists())
            {
                Vehicle.MarkAsNoLongerNeeded();
            }

            Ped = null;
            Vehicle = null;
        }
    }
}