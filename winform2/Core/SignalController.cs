using winform2.Model;

namespace winform2.Core
{
    public class SignalController
    {
        private SignalEngine engine;

        public event Action<Sample[], int> OnBucket;

        public void Start(double[] data)
        {
            engine?.Stop();

            engine = new SignalEngine(data);
            engine.OnBucket += (b, c) => OnBucket?.Invoke(b, c);

            engine.IsRunning = true;
            engine.Start();
        }

        public void Pause() => engine.IsRunning = false;

        public void Clear() => engine.Reset();
    }
}