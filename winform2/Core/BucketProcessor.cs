using winform2.Model;


namespace winform2.Core
{
    public class BucketProcessor
    {
        private const int Size = 20;

        private readonly Sample[] bucket1 = new Sample[Size];
        private readonly Sample[] bucket2 = new Sample[Size];

        private Sample[] filling;
        private Sample[] ready;

        private int index = 0;
        private volatile bool hasReady = false;

        public BucketProcessor()
        {
            filling = bucket1;
            ready = bucket2;
        }

        public void Add(Sample s)
        {
            filling[index++] = s;

            if (index >= Size)
            {
                var temp = filling;
                filling = ready;
                ready = temp;

                index = 0;
                hasReady = true;
            }
        }

        public bool TryGetReady(out Sample[]? buffer)
        {
            if (hasReady)
            {
                buffer = ready;
                hasReady = false;
                return true;
            }

            buffer = null;
            return false;
        }
    }
}