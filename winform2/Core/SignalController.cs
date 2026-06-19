using winform2.Model;

namespace winform2.Core
{
    public class SignalController
    {
        private SignalEngine? engine;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public event Action<Sample> OnSample;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public event Action<Sample[], int> OnBucket;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public void Start(double[] inputData)
        {
            if (engine != null)
                engine.Stop();

            engine = new SignalEngine(inputData);

            engine.OnSample += s => OnSample?.Invoke(s);
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