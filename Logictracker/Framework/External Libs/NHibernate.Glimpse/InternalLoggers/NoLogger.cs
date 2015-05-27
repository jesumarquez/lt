using System;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class NoLogger : IInternalLogger
    {
        public void Error(object message)
        {
            
        }

        public void Error(object message, Exception exception)
        {
            
        }

        public void ErrorFormat(string format, params object[] args)
        {
            
        }

        public void Fatal(object message)
        {
            
        }

        public void Fatal(object message, Exception exception)
        {
            
        }

        public void Debug(object message)
        {
            
        }

        public void Debug(object message, Exception exception)
        {
            
        }

        public void DebugFormat(string format, params object[] args)
        {
            
        }

        public void Info(object message)
        {
            
        }

        public void Info(object message, Exception exception)
        {
            
        }

        public void InfoFormat(string format, params object[] args)
        {
            
        }

        public void Warn(object message)
        {
            
        }

        public void Warn(object message, Exception exception)
        {
            
        }

        public void WarnFormat(string format, params object[] args)
        {
            
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }
    }
}