using winform2.Model;

namespace winform2.Core
{
    public class SignalController
    {
        private SignalEngine? engine;


        public event Action<Sample[], int> OnBucket;

        public void Start(double[] inputData)
        {
            if (engine != null)
                engine.Stop();

            engine = new SignalEngine(inputData);

            engine.OnBucket += (b, c) => OnBucket?.Invoke(b, c);

            engine.IsRunning = true;
            engine.Start();
        }

        public void Pause()
        {
            if (engine != null)
                engine.IsRunning = false;
        }

        public void Clear()
        {
            if (engine != null)
                engine.Reset();
        }
    }
}