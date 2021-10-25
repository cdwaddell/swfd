using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace swfd.test
{
    public class TestLoggerDisposedEvent<T> : IDisposable
    {
        private readonly TestLogger<T> _testLogger;

        public TestLoggerDisposedEvent(TestLogger<T> testLogger)
        {
            _testLogger = testLogger;
        }

        public void Dispose()
        {
            _testLogger.ResetScope();
        }

        public Guid LogKey = Guid.NewGuid();
    }

    public class TestLogger<T> : ILogger<T>
    {
        public readonly Dictionary<Guid, List<string>> LogEntries = new Dictionary<Guid, List<string>>();
        public Guid scopeGuid = Guid.Empty;
        private readonly Stack<TestLoggerDisposedEvent<T>> _logEvents = new Stack<TestLoggerDisposedEvent<T>>();

        public IDisposable BeginScope<TState>(TState state)
        {
            var logEvent = new TestLoggerDisposedEvent<T>(this);
            scopeGuid = logEvent.LogKey;

            _logEvents.Push(logEvent);

            return logEvent;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            List<string> val;
            if (!LogEntries.TryGetValue(scopeGuid, out val))
            {
                val = new List<string>();
                LogEntries.Add(scopeGuid, val);
            }

            var message = formatter(state, exception);
            val.Add(message);
        }

        internal void ResetScope()
        {
            var logEvent = _logEvents.Pop();
            if(logEvent == null)
                scopeGuid = Guid.Empty;
            else
                scopeGuid = logEvent.LogKey;
        }
    }
}