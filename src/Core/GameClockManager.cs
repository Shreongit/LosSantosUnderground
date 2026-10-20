using GTA;
using GTA.Native;

namespace LosSantosUnderground.Core
{
    public class GameClockManager
    {
        private int _hour;
        private int _minute;

        public int Hour
        {
            get { return _hour; }
        }

        public int Minute
        {
            get { return _minute; }
        }

        public GameClockManager()
        {
            Update();
        }

        public void Update()
        {
            _hour = Function.Call<int>(
                Hash.GET_CLOCK_HOURS);

            _minute = Function.Call<int>(
                Hash.GET_CLOCK_MINUTES);
        }

        public bool IsRaceRegistrationOpen()
        {
            return _hour >= 17 || _hour < 5;
        }

        public bool IsNightTime()
        {
            return IsRaceRegistrationOpen();
        }

        public bool IsMorning()
        {
            return _hour >= 5 && _hour < 12;
        }

        public bool IsAfternoon()
        {
            return _hour >= 12 && _hour < 17;
        }

        public bool IsEvening()
        {
            return _hour >= 17;
        }

        public bool IsBeforeClosingTime()
        {
            return _hour < 5;
        }

        public string GetFormattedTime()
        {
            return string.Format(
                "{0:00}:{1:00}",
                _hour,
                _minute);
        }
    }
}