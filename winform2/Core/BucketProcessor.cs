using winform2.Model;

namespace winform2.Core
{
    public class BucketProcessor
    {
        private const int Size = 40; // 🔥 your fixed bucket size

        private readonly Sample[] bucket1 = new Sample[Size];
        private readonly Sample[] bucket2 = new Sample[Size];

        private int index = 0;
        private bool fillingB1 = true;

        /// <summary>
        /// Adds sample to current bucket.
        /// When bucket is full → returns ready buffer.
        /// </summary>
        public bool Add(Sample s, out Sample[] ready, out int count)
        {
            // 🔥 Select active buffer
            var current = fillingB1 ? bucket1 : bucket2;

            // 🔥 Fill sample
            current[index++] = s;

            // 🔥 Check if bucket full
            if (index >= Size)
            {
                // ✅ Current bucket ready
                ready = current;
                count = Size;

                // 🔥 IMMEDIATE SWITCH (core of ping-pong)
                fillingB1 = !fillingB1;

                // 🔥 Reset index for next buffer
                index = 0;

                return true;
            }

            ready = null;
            count = 0;
            return false;
        }

        /// <summary>
        /// Optional: Reset processor
        /// </summary>
        public void Reset()
        {
            index = 0;
            fillingB1 = true;
        }
    }
}