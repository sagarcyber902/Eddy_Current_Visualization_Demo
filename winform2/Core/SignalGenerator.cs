using winform2.Model;

namespace winform2.Core
{
    public class SignalGenerator
    {
        private double[] data;
        private int index = 0;

        public SignalGenerator(double[] input)
        {
            data = input;
        }

        // ✅ Check if more data is available
        public bool HasMoreData
        {
            get
            {
                return data != null && index < data.Length - 1;
            }
        }

        public Sample Generate()
        {
            // ❌ No data
            if (data == null || data.Length < 2)
                return default;

            // ❌ Safety check (important)
            if (index >= data.Length - 1)
                return default;

            // 🔥 Read X,Y


            float x = (float)data[index];
            float y = (float)data[index + 1];

            index += 2;

            double z = Math.Sqrt(x * x + y * y);

            return new Sample
            {
                X = x,
                Y = y,
                Z = z
            };
        }

        public void Reset()
        {
            index = 0;
        }
    }
}