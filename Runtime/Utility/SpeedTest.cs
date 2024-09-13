using System;
using System.Diagnostics;

namespace Noo.Tools
{
    public readonly struct SpeedTest : IDisposable
    {
        static readonly Stopwatch sw = Stopwatch.StartNew();

        readonly double startTime;
        readonly string name;
        readonly bool log;
        public double ElapsedMs => sw.Elapsed.TotalMilliseconds - startTime;

        private SpeedTest(double startTime, string name, bool log)
        {
            this.name = name;
            this.startTime = startTime;
            this.log = log;
        }

        public static SpeedTest New(string name = "", bool log = true)
        {
            return new(sw.Elapsed.TotalMilliseconds, name, log);
        }

        public void Dispose()
        {
            if (log) UnityEngine.Debug.Log($"{name}: {sw.Elapsed.TotalMilliseconds - startTime:0.000}ms");
        }
    }
}