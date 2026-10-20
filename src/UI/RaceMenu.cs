using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using LemonUI;
using LemonUI.Menus;
using LosSantosUnderground.Core;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.UI
{
    public class RaceMenu : IDisposable
    {
        private readonly ObjectPool _pool;
        private readonly RaceManager _raceManager;

        private readonly NativeMenu _menu;
        private readonly List<RaceDefinition> _races;

        private bool _disposed;

        public NativeMenu Menu =>
            _menu;

        public bool IsVisible =>
            _menu.Visible;

        public RaceMenu(
            ObjectPool pool,
            RaceManager raceManager)
        {
            _pool = pool
                ?? throw new ArgumentNullException(
                    nameof(pool));

            _raceManager = raceManager
                ?? throw new ArgumentNullException(
                    nameof(raceManager));

            _races =
                new List<RaceDefinition>();

            _menu =
                new NativeMenu(
                    "Los Santos Underground",
                    "NIGHT RACES");

            _pool.Add(_menu);

            BuildMenu();
        }

        private void BuildMenu()
        {
            _menu.Clear();

            _races.Clear();

            IReadOnlyList<RaceDefinition>
                availableRaces =
                    _raceManager
                        .GetAvailableRaces()
                        .ToList();

            if (availableRaces == null ||
                availableRaces.Count == 0)
            {
                NativeItem emptyItem =
                    new NativeItem(
                        "No races available");

                emptyItem.Enabled = false;

                _menu.Add(emptyItem);

                return;
            }

            foreach (RaceDefinition race
                     in availableRaces)
            {
                if (race == null)
                    continue;

                _races.Add(race);

                NativeItem raceItem =
                    new NativeItem(
                        race.Name,
                        BuildRaceDescription(race));

                RaceDefinition selectedRace =
                    race;

                raceItem.Activated +=
                    (sender, args) =>
                    {
                        SelectRace(
                            selectedRace);
                    };

                _menu.Add(raceItem);
            }
        }

        private string BuildRaceDescription(
            RaceDefinition race)
        {
            return
                $"Type: {race.Type}\n" +
                $"Entry: ${race.EntryFee:N0}\n" +
                $"Racers: {race.MaxRacers}\n" +
                $"Checkpoints: " +
                $"{race.Checkpoints?.Count ?? 0}\n\n" +
                "Race window: 5:00 PM - 5:00 AM";
        }

        private void SelectRace(
            RaceDefinition race)
        {
            if (_disposed ||
                race == null)
            {
                return;
            }

            if (!_raceManager.CanRegisterRace())
            {
                Notifications.Show(
                    "Street races are closed during the day.");

                return;
            }

            if (_raceManager.IsRaceActive)
            {
                Notifications.Show(
                    "A race is already active.");

                return;
            }

            bool started =
                _raceManager.StartRace(
                    race.Id);

            if (!started)
            {
                Notifications.Show(
                    "Unable to start this race.");

                return;
            }

            _menu.Visible = false;
        }

        public void Refresh()
        {
            if (_disposed)
                return;

            BuildMenu();
        }

        public void Show()
        {
            if (_disposed)
                return;

            Refresh();

            _menu.Visible = true;
        }

        public void Hide()
        {
            if (_disposed)
                return;

            _menu.Visible = false;
        }

        public void Toggle()
        {
            if (_disposed)
                return;

            if (_menu.Visible)
                Hide();
            else
                Show();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _menu.Visible = false;
        }
    }
}