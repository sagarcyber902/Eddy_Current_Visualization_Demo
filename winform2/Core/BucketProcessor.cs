using System.Diagnostics;
using winform2.Model;

namespace winform2.Core
{
    public class BucketProcessor
    {
        private const int Size = 40;
      

        private readonly Sample[] bucket1 = new Sample[Size];
        private readonly Sample[] bucket2 = new Sample[Size];

        private bool fillingB1 = true;
        private int index = 0;

        private readonly Stopwatch sw = Stopwatch.StartNew();

        public bool Add(Sample s, out Sample[] ready, out int count)
        {
            ready = null;
            count = 0;

            if (fillingB1)
                bucket1[index] = s;
            else
                bucket2[index] = s;

            index++;

            // FULL BUCKET
            if (index >= Size)
            {
                ready = fillingB1 ? bucket1 : bucket2;
                count = Size;

                Swap();
                return true;
            }
            return false;
        }

        private void Swap()
        {
            fillingB1 = !fillingB1;
            index = 0;
            sw.Restart();
        }

        public void Reset()
        {
            index = 0;
            fillingB1 = true;
            sw.Restart();
        }
    }
}