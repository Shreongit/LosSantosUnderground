using System;
using GTA;
using LemonUI;
using LemonUI.Menus;
using LosSantosUnderground.Core;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.UI
{
    public class RaceInfoMenu : IDisposable
    {
        private readonly ObjectPool _pool;
        private readonly RaceManager _raceManager;

        private readonly NativeMenu _menu;
        private bool _disposed;

        public NativeMenu Menu =>
            _menu;

        public bool IsVisible =>
            _menu.Visible;

        public RaceInfoMenu(
            ObjectPool pool,
            RaceManager raceManager)
        {
            _pool = pool
                ?? throw new ArgumentNullException(
                    nameof(pool));

            _raceManager = raceManager
                ?? throw new ArgumentNullException(
                    nameof(raceManager));

            _menu =
                new NativeMenu(
                    "Race Information",
                    "LOS SANTOS UNDERGROUND");

            _pool.Add(_menu);

            _menu.Closed +=
                OnMenuClosed;
        }

        public void Show(
            RaceDefinition race)
        {
            if (_disposed ||
                race == null)
            {
                return;
            }

            BuildRaceInfo(race);

            _menu.Visible = true;
        }

        public void Hide()
        {
            if (_disposed)
                return;

            _menu.Visible = false;
        }

        private void BuildRaceInfo(
            RaceDefinition race)
        {
            _menu.Clear();

            NativeItem title =
                new NativeItem(
                    race.Name);

            title.Enabled = false;

            _menu.Add(title);

            NativeItem type =
                new NativeItem(
                    "Race Type",
                    race.Type.ToString());

            type.Enabled = false;

            _menu.Add(type);

            NativeItem entry =
                new NativeItem(
                    "Entry Fee",
                    $"${race.EntryFee:N0}");

            entry.Enabled = false;

            _menu.Add(entry);

            NativeItem racers =
                new NativeItem(
                    "Racers",
                    race.MaxRacers.ToString());

            racers.Enabled = false;

            _menu.Add(racers);

            NativeItem checkpoints =
                new NativeItem(
                    "Checkpoints",
                    (race.Checkpoints?.Count ?? 0)
                        .ToString());

            checkpoints.Enabled = false;

            _menu.Add(checkpoints);

            NativeItem prizePool =
                new NativeItem(
                    "Prize Pool",
                    $"${GetPrizePool(race):N0}");

            prizePool.Enabled = false;

            _menu.Add(prizePool);

            NativeItem schedule =
                new NativeItem(
                    "Race Hours",
                    "5:00 PM - 5:00 AM");

            schedule.Enabled = false;

            _menu.Add(schedule);

            NativeItem rules =
                new NativeItem(
                    "Race Rules",
                    BuildRulesText());

            rules.Enabled = false;

            _menu.Add(rules);

            NativeItem start =
                new NativeItem(
                    "Join Race",
                    "Pay entry fee and start");

            start.Activated +=
                (sender, args) =>
                {
                    JoinRace(race);
                };

            _menu.Add(start);

            NativeItem close =
                new NativeItem(
                    "Close");

            close.Activated +=
                (sender, args) =>
                {
                    Hide();
                };

            _menu.Add(close);
        }

        private int GetPrizePool(
            RaceDefinition race)
        {
            if (race == null)
                return 0;

            return race.EntryFee *
                   race.MaxRacers;
        }

        private string BuildRulesText()
        {
            return
                "6 racers maximum.\n" +
                "Everyone pays the entry fee.\n" +
                "Winner receives the full prize pool.\n" +
                "Checkpoints must be crossed in order.\n" +
                "Police detection immediately aborts the race.\n" +
                "Police incident gives 3 stars.\n" +
                "No refund after police detection.";
        }

        private void JoinRace(
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
                    "Racing is closed right now.");

                return;
            }

            if (_raceManager.IsRaceActive)
            {
                Notifications.Show(
                    "A race is already active.");

                return;
            }

            if (!_raceManager.StartRace(
                    race.Id))
            {
                Notifications.Show(
                    "Unable to join this race.");

                return;
            }

            Hide();
        }

        private void OnMenuClosed(
            object sender,
            EventArgs e)
        {
            if (_disposed)
                return;
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