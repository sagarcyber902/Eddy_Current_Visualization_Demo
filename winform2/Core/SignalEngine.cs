using System.Diagnostics;
using winform2.Model;



namespace winform2.Core
{
    public class SignalEngine
    {
        public event Action<Sample[], int> OnBucket;

        private readonly SignalGenerator generator;
        private readonly BucketProcessor bucket = new();

        private const double SampleRate = 1000;

        public bool IsRunning { get; set; }

        private Thread worker;
        private bool running;

        public SignalEngine(double[] inputData)
        {
            generator = new SignalGenerator(inputData);
        }

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

                if (sw.ElapsedTicks >= nextTick)
                {
                    nextTick += (long)ticksPerSample;

                    if (!generator.HasMoreData)
                    {
                        // ✅ flush remaining partial bucket
                        if (bucket.TryGetReady(out var last))
                        {
                            OnBucket?.Invoke(last, last.Length);
                        }

                        running = false;   // 🔥 STOP THREAD
                        return;
                    }

                    var sample = generator.Generate();
                    bucket.Add(sample);

                    if (bucket.TryGetReady(out var ready))
                    {
                        OnBucket?.Invoke(ready, ready.Length);
                    }
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
            generator.Reset();
        }
    }
}