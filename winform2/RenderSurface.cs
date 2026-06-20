using System.ComponentModel;
using winform2.Model;

namespace winform2
{
    public class RenderSurface : Control
    {
        private const int BufferSize = 700;

        private readonly Sample[] buffer = new Sample[BufferSize];
        private int head = 0;
        private int count = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsImpedance { get; set; }

        private Sample[] currentBucket;
        private int bucketCount;

        public RenderSurface()
        {
            DoubleBuffered = true;
        }

        public void PushSample(Sample s)
        {
            buffer[head] = s;
            head = (head + 1) % BufferSize;

            if (count < BufferSize)
                count++;
        }

        public void SetBucket(Sample[] bucket, int count)
        {
            currentBucket = bucket;
            bucketCount = count;
        }

        public void ClearAll()
        {
            head = 0;
            count = 0;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.Black);

            if (IsImpedance)
                DrawImpedance(g);
            else
                DrawGraph(g);
        }

        private void DrawImpedance(Graphics g)
        {
            float cx = Width / 2f;
            float cy = Height / 2f;

            g.DrawLine(Pens.Gray, 0, cy, Width, cy);
            g.DrawLine(Pens.Gray, cx, 0, cx, Height);

            g.DrawEllipse(Pens.Green, cx - 100, cy - 100, 200, 200);
            g.DrawEllipse(Pens.Green, cx - 20, cy - 20, 40, 40);

            if (currentBucket == null) return;

            for (int i = 1; i < bucketCount; i++)
            {
                var p1 = new PointF(cx + currentBucket[i - 1].X, cy - currentBucket[i - 1].Y);
                var p2 = new PointF(cx + currentBucket[i].X, cy - currentBucket[i].Y);

                g.DrawLine(Pens.Lime, p1, p2);
            }
        }

        private void DrawGraph(Graphics g)
        {
            float left = 40;
            float bottom = Height - 20;
            float right = Width - 10;
            float top = 10;

            float height = bottom - top;

            // Axis
            g.DrawLine(Pens.Gray, left, bottom, right, bottom);
            g.DrawLine(Pens.Gray, left, bottom, left, top);

            // ✅ Proper grid
            int divisions = 10;
            float stepY = height / divisions;

            for (int i = 0; i <= divisions; i++)
            {
                float y = bottom - i * stepY;
                g.DrawLine(Pens.DimGray, left, y, right, y);
            }

            if (count < 2) return;

            float step = (right - left) / BufferSize;
            PointF prev = default;

            for (int i = 0; i < count; i++)
            {
                int idx = (head - count + i + BufferSize) % BufferSize;
                float px = left + (count - 1 - i) * step;

                float py = bottom - (float)buffer[idx].Z; // RAW

                if (py < top) py = top;
                if (py > bottom) py = bottom;

                var cur = new PointF(px, py);

                if (i > 0)
                    g.DrawLine(Pens.Lime, prev, cur);

                prev = cur;
            }
        }
    }
}