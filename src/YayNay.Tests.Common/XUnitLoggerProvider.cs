using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace NatMarchand.YayNay.Tests.Common
{
    public class XUnitLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ConcurrentDictionary<string, XUnitLogger> _instances;
        private IExternalScopeProvider? _externalScopeProvider;

        public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _instances = new ConcurrentDictionary<string, XUnitLogger>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            //ConsoleLoggerProvider
            return _instances.GetOrAdd(categoryName, (cat, t) => new XUnitLogger(t.TestOutputHelper, cat) { ScopeProvider = t.ExternalScopeProvider }, (TestOutputHelper: _testOutputHelper, ExternalScopeProvider: _externalScopeProvider));
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _externalScopeProvider = scopeProvider;
            foreach (var logger in _instances.Values)
            {
                logger.ScopeProvider = scopeProvider;
            }
        }

        public void Dispose()
        {
        }

        private class XUnitLogger : ILogger
        {
            private readonly ITestOutputHelper _testOutputHelper;
            private readonly string _categoryName;

            public IExternalScopeProvider? ScopeProvider { get; set; }

            public XUnitLogger(ITestOutputHelper testOutputHelper, string categoryName)
            {
                _testOutputHelper = testOutputHelper;
                _categoryName = categoryName;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var bag = new List<string>();
                ScopeProvider?.ForEachScope(AppendToBag, bag);
                if (state != null)
                {
                    AppendToBag(state, bag);
                }

                string? level = null;
                switch (logLevel)
                {
                    case LogLevel.Trace:
                        level = "TRACE";
                        break;
                    case LogLevel.Debug:
                        level = "DEBUG";
                        break;
                    case LogLevel.Information:
                        level = "INFO";
                        break;
                    case LogLevel.Warning:
                        level = "WARN";
                        break;
                    case LogLevel.Error:
                        level = "ERROR";
                        break;
                    case LogLevel.Critical:
                        level = "CRIT";
                        break;
                    case LogLevel.None:
                        level = "NONE";
                        break;
                }

                var message = formatter(state, exception)?.Replace("\r\n", "\r\n\t\t\t\t ");
                _testOutputHelper.WriteLine($"{DateTime.Now:mm:ss.ffff} {level,-6}{_categoryName}{(bag.Count > 0 ? $"\r\n\t[{string.Join("; ", bag)}]" : string.Empty)}\r\n\t\t\t>\t {message}");
            }

            private static void AppendToBag(object o, ICollection<string> bag)
            {
                if (o is IEnumerable<KeyValuePair<string, object>> kvps)
                {
                    foreach (var (key, value) in kvps)
                    {
                        if (key == "{OriginalFormat}")
                        {
                            continue;
                        }

                        bag.Add($"{key} = {value}");
                    }
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return ScopeProvider?.Push(state)!;
            }
        }
    }
}