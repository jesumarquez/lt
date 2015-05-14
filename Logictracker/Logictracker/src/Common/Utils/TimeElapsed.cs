using System;

namespace Logictracker.Utils
{
    public class TimeElapsed : IDisposable
    {

        public TimeElapsed()
        {
            Restart();
        }

        private DateTime _startedAt;

        public DateTime startedAt
        {
            get { return _startedAt; }
        }

        public void Restart()
        {
            _startedAt = DateTime.UtcNow;
        }

        public TimeSpan getTimeElapsed()
        {
            return DateTime.UtcNow.Subtract(_startedAt);
        }

        public void Dispose()
        {
            
        }
    }
}
