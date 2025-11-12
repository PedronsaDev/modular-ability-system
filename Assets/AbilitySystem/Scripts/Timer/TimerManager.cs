using System.Collections.Generic;

namespace YourNamespace
{
    /// <summary>
    /// Central static manager that updates all active timers each frame. Inserted into PlayerLoop via TimerBootstrapper.
    /// </summary>
    public static class TimerManager
    {
        private static readonly List<Timer> _timers = new();
        private static readonly List<Timer> _sweep = new();

        /// <summary>Registers a timer for updates.</summary>
        /// <param name="timer">The timer to register.</param>
        public static void RegisterTimer(Timer timer) => _timers.Add(timer);

        /// <summary>Deregisters a timer stopping future updates.</summary>
        /// <param name="timer">The timer to deregister.</param>
        public static void DeregisterTimer(Timer timer) => _timers.Remove(timer);

        /// <summary>
        /// Ticks all registered timers. Uses a sweep list to avoid modification during iteration.
        /// </summary>
        public static void UpdateTimers()
        {
            if (_timers.Count == 0) return;

            _sweep.RefreshWith(_timers);
            foreach (var timer in _sweep)
            {
                timer.Tick();
            }
        }

        /// <summary>Disposes and clears all timers (used when exiting play mode in editor).</summary>
        public static void Clear()
        {
            _sweep.RefreshWith(_timers);
            foreach (var timer in _sweep)
            {
                timer.Dispose();
            }

            _timers.Clear();
            _sweep.Clear();
        }
    }
}