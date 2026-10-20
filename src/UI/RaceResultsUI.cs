using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.UI;
using LosSantosUnderground.Core;
using LosSantosUnderground.Racers;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.UI
{
    public static class RaceResultsUI
    {
        private static DateTime _displayUntil;

        public static bool IsVisible
        {
            get { return DateTime.UtcNow < _displayUntil; }
        }

        public static void Show(
            RaceDefinition race,
            IEnumerable<RacerResult> results,
            TimeSpan playerTime)
        {
            if (race == null)
                return;

            List<RacerResult> orderedResults =
                results?
                    .Where(result => result != null)
                    .OrderBy(result =>
                        result.Position <= 0
                            ? int.MaxValue
                            : result.Position)
                    .ToList()
                ?? new List<RacerResult>();

            string positionText =
                GetPlayerPositionText(
                    orderedResults);

            string message =
                string.Format(
                    "{0}\nRACE FINISHED\n{1}\nTime: {2}",
                    race.Name,
                    positionText,
                    FormatTime(playerTime));

            Screen.ShowSubtitle(
                message,
                5000);

            _displayUntil =
                DateTime.UtcNow.AddSeconds(5);
        }

        public static void ShowWinner(
            RaceDefinition race,
            int prize,
            TimeSpan playerTime)
        {
            if (race == null)
                return;

            string message =
                string.Format(
                    "~g~WINNER!~s~\n{0}\nTime: {1}\nPrize: ${2:N0}",
                    race.Name,
                    FormatTime(playerTime),
                    prize);

            Screen.ShowSubtitle(
                message,
                6000);

            _displayUntil =
                DateTime.UtcNow.AddSeconds(6);
        }

        public static void ShowAborted(
            RaceAbortReason reason)
        {
            string message;

            if (reason ==
                RaceAbortReason.PoliceDetected)
            {
                message =
                    "~r~RACE ABORTED~s~\n" +
                    "Police detected the race.\n" +
                    "Escape the police!";
            }
            else
            {
                message =
                    "~r~RACE ABORTED~s~\n" +
                    GetAbortReasonText(reason);
            }

            Screen.ShowSubtitle(
                message,
                5000);

            _displayUntil =
                DateTime.UtcNow.AddSeconds(5);
        }

        public static void Clear()
        {
            _displayUntil =
                DateTime.UtcNow;
        }

        private static string GetPlayerPositionText(
            IEnumerable<RacerResult> results)
        {
            RacerResult playerResult =
                results?.FirstOrDefault(
                    result => result.IsPlayer);

            if (playerResult == null ||
                playerResult.Position <= 0)
            {
                return "Position: —";
            }

            return string.Format(
                "Position: {0}",
                playerResult.Position);
        }

        private static string GetAbortReasonText(
            RaceAbortReason reason)
        {
            switch (reason)
            {
                case RaceAbortReason.PlayerLeft:
                    return "You left the race.";

                case RaceAbortReason.PlayerDied:
                    return "Race ended because you died.";

                case RaceAbortReason.InvalidRace:
                    return "The race became invalid.";

                case RaceAbortReason.TimeWindowClosed:
                    return "Race registration closed.";

                case RaceAbortReason.VehicleDestroyed:
                    return "Your vehicle was destroyed.";

                case RaceAbortReason.Cancelled:
                    return "Race cancelled.";

                default:
                    return "The race has ended.";
            }
        }

        private static string FormatTime(
            TimeSpan time)
        {
            return string.Format(
                "{0:00}:{1:00}.{2:000}",
                (int)time.TotalMinutes,
                time.Seconds,
                time.Milliseconds);
        }
    }
}