using System;
using GTA;
using GTA.UI;
using LosSantosUnderground.Core;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.UI
{
    public static class RaceUI
    {
        private static bool _countdownActive;
        private static int _countdownStartTime;
        private static int _countdownDuration = 3000;

        public static void ShowRaceRegistration(
            RaceDefinition race)
        {
            if (race == null)
                return;

            Screen.ShowSubtitle(
                string.Format(
                    "~r~LOS SANTOS UNDERGROUND~w~\n{0}\nEntry: ~g~${1:N0}~w~\nPress ~y~E~w~ to enter",
                    race.Name,
                    race.EntryFee),
                5000);
        }

        public static void ShowPreparing(
            RaceDefinition race)
        {
            if (race == null)
                return;

            Screen.ShowSubtitle(
                string.Format(
                    "~r~{0}~w~\nGet ready...",
                    race.Name),
                3000);
        }

        public static void StartCountdown()
        {
            _countdownActive = true;
            _countdownStartTime = Environment.TickCount;
        }

        public static bool IsCountdownFinished()
        {
            if (!_countdownActive)
                return true;

            int elapsed =
                Environment.TickCount - _countdownStartTime;

            if (elapsed >= _countdownDuration)
            {
                _countdownActive = false;
                return true;
            }

            int remaining =
                _countdownDuration - elapsed;

            int seconds =
                (int)Math.Ceiling(
                    remaining / 1000.0);

            if (seconds <= 0)
                seconds = 1;

            Screen.ShowSubtitle(
                seconds.ToString(),
                900);

            return false;
        }

        public static void ShowRaceStarted()
        {
            Screen.ShowSubtitle(
                "~r~GO!~w~",
                1500);
        }

        public static void OnCheckpointPassed(
            int checkpointIndex,
            int totalCheckpoints)
        {
            Screen.ShowSubtitle(
                string.Format(
                    "Checkpoint ~y~{0}~w~/~y~{1}",
                    checkpointIndex,
                    totalCheckpoints),
                1000);
        }

        public static void UpdateRaceHUD(
            int position,
            int totalRacers,
            int checkpoint,
            int totalCheckpoints)
        {
            string positionText =
                position > 0
                    ? string.Format(
                        "Position: ~y~{0}~w~/~y~{1}",
                        position,
                        totalRacers)
                    : string.Format(
                        "Racers: ~y~{0}~w~",
                        totalRacers);

            string checkpointText =
                string.Format(
                    "Checkpoint: ~y~{0}~w~/~y~{1}",
                    checkpoint,
                    totalCheckpoints);

            Screen.ShowSubtitle(
                positionText +
                "\n" +
                checkpointText,
                100);
        }

        public static void ShowRaceResults(
            RaceDefinition race,
            int position,
            int totalRacers,
            int prize)
        {
            if (race == null)
                return;

            string result;

            if (position == 1)
            {
                result = string.Format(
                    "~g~YOU WON!~w~\nPrize: ~g~${0:N0}~w~",
                    prize);
            }
            else if (position > 0)
            {
                result = string.Format(
                    "~r~RACE FINISHED~w~\nPosition: ~y~{0}~w~/~y~{1}",
                    position,
                    totalRacers);
            }
            else
            {
                result =
                    "~r~RACE FINISHED~w~";
            }

            Screen.ShowSubtitle(
                result,
                5000);
        }

        public static void ShowRaceAborted(
            RaceAbortReason reason)
        {
            string message;

            if (reason ==
                RaceAbortReason.PoliceDetected)
            {
                message =
                    "~r~RACE BUSTED~w~\nPolice detected the race!";
            }
            else
            {
                message =
                    "~r~RACE ABORTED~w~";
            }

            Screen.ShowSubtitle(
                message,
                4000);
        }

        public static void Reset()
        {
            _countdownActive = false;
            _countdownStartTime = 0;
        }
    }
}