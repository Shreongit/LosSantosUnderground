using System;
using GTA;
using GTA.UI;
using LosSantosUnderground.Races;

namespace LosSantosUnderground.UI
{
    public static class RaceHUD
    {
        private static DateTime _lastUpdate;
        private static string _lastText;

        public static void Update(
            RaceDefinition race,
            int currentCheckpoint,
            int totalCheckpoints,
            TimeSpan elapsed)
        {
            if (race == null)
                return;

            if (totalCheckpoints <= 0)
                return;

            if ((DateTime.UtcNow - _lastUpdate).TotalMilliseconds < 100)
                return;

            _lastUpdate = DateTime.UtcNow;

            int displayCheckpoint =
                Math.Min(
                    currentCheckpoint + 1,
                    totalCheckpoints);

            string text =
                $"{race.Name}\n" +
                $"Checkpoint {displayCheckpoint}/{totalCheckpoints}\n" +
                $"Time {FormatTime(elapsed)}";

            if (text == _lastText)
                return;

            _lastText = text;

            Screen.ShowSubtitle(
                text,
                150);
        }

        public static void ShowRaceStarted()
        {
            _lastText = null;

            Screen.ShowSubtitle(
                "GO!  STREET RACE STARTED",
                2000);
        }

        public static void ShowCheckpoint(
            int checkpoint,
            int total)
        {
            _lastText = null;

            Screen.ShowSubtitle(
                $"CHECKPOINT {checkpoint}/{total}",
                1000);
        }

        public static void ShowFinished(
            TimeSpan elapsed)
        {
            _lastText = null;

            Screen.ShowSubtitle(
                $"FINISH!\nTIME {FormatTime(elapsed)}",
                3000);
        }

        public static void ShowPoliceAbort()
        {
            _lastText = null;

            Screen.ShowSubtitle(
                "POLICE DETECTED!\n" +
                "RACE ABORTED — ESCAPE!",
                4000);
        }

        public static void Clear()
        {
            _lastText = null;
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