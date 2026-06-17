using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using winform2.Model;

namespace winform2.Core
{
    public class SignalEngine
    {
        public event Action<Sample> OnSample;
        public event Action<Sample[], int> OnBucket;

        private readonly SignalGenerator generator = new();
        private readonly BucketProcessor bucket = new();

        private const double SampleRate = 1000; // Hz
        

        public bool IsRunning { get; set; }

        private Thread worker;
        private bool running;

        public void Start()
        {
            running = true;

            worker = new Thread(Run)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            worker.Start();
        }

        private void Run()
        {
            var sw = Stopwatch.StartNew();

            double ticksPerSample = Stopwatch.Frequency / SampleRate;
            long nextTick = sw.ElapsedTicks;

            while (running)
            {
                if (!IsRunning)
                {
                    Thread.Sleep(1);
                    continue;
                }

                long now = sw.ElapsedTicks;

                if (now >= nextTick)
                {
                    nextTick += (long)ticksPerSample;

                    // 🔥 Generate sample
                    var sample = generator.Generate();

                    OnSample?.Invoke(sample);

                    // 🔥 Bucket logic
                    if (bucket.Add(sample, out var ready, out int count))
                    {
                        OnBucket?.Invoke(ready, count);
                    }
                }
                else
                {
                    // 🔥 Prevent CPU 100%
                    Thread.SpinWait(20);
                }
            }
        }

        public void Stop()
        {
            running = false;
            worker?.Join();
        }

        public void Reset()
        {
            bucket.Reset();
            generator.Reset();
        }
    }
}