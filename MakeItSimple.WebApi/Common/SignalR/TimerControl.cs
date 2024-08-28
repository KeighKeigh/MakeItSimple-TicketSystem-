﻿using System.Collections.Concurrent;
using System.Threading;

namespace MakeItSimple.WebApi.Common.SignalR
{
    public class TimerControl
    {
        private readonly ConcurrentDictionary<string, Timer> _timers = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TimerControl(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void ScheduleTimer(string key, Func<IServiceScopeFactory, Task> scopedAction, int delay, int period)
        {
            // Stop existing timer if it exists for the given key
            if (_timers.TryRemove(key, out var existingTimer))
            {
                existingTimer.Dispose();
            }

            // Create and start a new timer
            var timer = new Timer(async _ =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await scopedAction(scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>());
            }, null, delay, period);

            _timers[key] = timer;
        }

        public void StopTimer(string key)
        {
            if (_timers.TryRemove(key, out var timer))
            {
                timer.Dispose();
            }
        }

        public void StopAllTimers()
        {
            foreach (var timer in _timers.Values)
            {
                timer.Dispose();
            }
            _timers.Clear();
        }
    }
}
