using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using winform2.Model;

namespace winform2.Core
{
    public class SignalEngine
    {
       
        public event Action<Sample[], int> OnBucket;

        private SignalGenerator generator;
        private readonly BucketProcessor bucket = new();

        private const double SampleRate = 1000; // Hz
        public SignalEngine(double[] inputData)
        {
            generator = new SignalGenerator(inputData);
        }

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

                    // 🔥 STOP when data finished
                    if (!generator.HasMoreData)
                    {
                        running = false;
                        IsRunning = false;
                        return;
                    }

                    var sample = generator.Generate();



                    bucket.Add(sample);

                    if (bucket.TryGetReady(out var ready))
                    {
                        OnBucket?.Invoke(ready, 20);
                    }
                }
                else
                {
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
            //bucket.Reset();
            generator.Reset();
        }
    }
}