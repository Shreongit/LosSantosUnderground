using System;
using GTA;
using GTA.UI;

namespace LosSantosUnderground.UI
{
    public static class CountdownUI
    {
        private static readonly TimeSpan CountdownDuration =
            TimeSpan.FromSeconds(3.0);

        private static DateTime _countdownStart;
        private static bool _active;
        private static int _lastNumber;

        public static bool IsActive =>
            _active;

        public static void Start()
        {
            _countdownStart =
                DateTime.UtcNow;

            _active = true;
            _lastNumber = 3;
        }

        public static bool IsFinished()
        {
            if (!_active)
                return true;

            TimeSpan elapsed =
                DateTime.UtcNow -
                _countdownStart;

            return elapsed >=
                   CountdownDuration;
        }

        public static void Update()
        {
            if (!_active)
                return;

            TimeSpan elapsed =
                DateTime.UtcNow -
                _countdownStart;

            if (elapsed >= CountdownDuration)
            {
                _active = false;

                Screen.ShowSubtitle(
                    "GO!",
                    1000);

                return;
            }

            double remaining =
                Math.Ceiling(
                    (CountdownDuration -
                     elapsed).TotalSeconds);

            int number =
                Math.Max(
                    1,
                    (int)remaining);

            if (number != _lastNumber)
            {
                _lastNumber = number;
            }

            Screen.ShowSubtitle(
                number.ToString(),
                900);
        }

        public static int GetCurrentNumber()
        {
            if (!_active)
                return 0;

            TimeSpan elapsed =
                DateTime.UtcNow -
                _countdownStart;

            if (elapsed >= CountdownDuration)
                return 0;

            return Math.Max(
                1,
                (int)Math.Ceiling(
                    (CountdownDuration -
                     elapsed).TotalSeconds));
        }

        public static void Cancel()
        {
            _active = false;
            _lastNumber = 0;
        }
    }
}