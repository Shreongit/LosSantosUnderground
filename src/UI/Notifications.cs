using System;
using GTA;
using GTA.UI;

namespace LosSantosUnderground.UI
{
    public static class Notifications
    {
        private static DateTime _lastNotificationTime;
        private static string _lastMessage;

        private static readonly TimeSpan Cooldown =
            TimeSpan.FromMilliseconds(500);

        public static void Show(
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            DateTime now =
                DateTime.UtcNow;

            if (message == _lastMessage &&
                now - _lastNotificationTime < Cooldown)
            {
                return;
            }

            _lastMessage = message;
            _lastNotificationTime = now;

            Screen.ShowSubtitle(
                message,
                2500);
        }

        public static void Success(
            string message)
        {
            Show(
                $"~g~{message}~s~");
        }

        public static void Warning(
            string message)
        {
            Show(
                $"~y~{message}~s~");
        }

        public static void Error(
            string message)
        {
            Show(
                $"~r~{message}~s~");
        }

        public static void RaceStarted(
            string raceName)
        {
            Show(
                $"~g~{raceName}~s~\n" +
                "Race started!");
        }

        public static void PoliceDetected()
        {
            Show(
                "~r~POLICE DETECTED!~s~\n" +
                "Race aborted — escape!");
        }

        public static void RaceFinished(
            int prize)
        {
            Show(
                $"~g~RACE FINISHED!~s~\n" +
                $"Prize: ${prize:N0}");
        }

        public static void EntryFeeRequired(
            int amount)
        {
            Show(
                $"~r~Insufficient funds.~s~\n" +
                $"Entry fee: ${amount:N0}");
        }

        public static void Clear()
        {
            _lastMessage = null;
            _lastNotificationTime =
                DateTime.MinValue;
        }
    }
}