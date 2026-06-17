using winform2.Model;

namespace winform2.Core
{
    public class SignalController
    {
        private SignalEngine engine = new();

        public event Action<Sample> OnSample;
        public event Action<Sample[], int> OnBucket;

        public SignalController()
        {
            engine.OnSample += s => OnSample?.Invoke(s);
            engine.OnBucket += (b, c) => OnBucket?.Invoke(b, c);    
        }

        public void Start()
        {
            // 🔥 stop old engine completely
            engine.Stop();

            // 🔥 create fresh engine (NO OLD DATA)
            engine = new SignalEngine();

            // 🔥 reattach events
            engine.OnSample += s => OnSample?.Invoke(s);
            engine.OnBucket += (b, c) => OnBucket?.Invoke(b, c);

            // 🔥 start clean
            engine.IsRunning = true;
            engine.Start();
        }

        public void Pause()
        {
            engine.IsRunning = false;
        }

        public void Clear()
        {
            engine.Reset();
        }
    }
}