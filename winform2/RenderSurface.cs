using System;
using System.Drawing;
using System.Windows.Forms;
using winform2.Model;

namespace winform2
{
    public class RenderSurface : Control
    {
        private const int BufferSize = 1000;

        private readonly Sample[] buffer = new Sample[BufferSize];
        private int head = 0;
        private int count = 0;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool IsImpedance { get; set; }

        private Sample[] currentBucket = null;
        private int bucketCount = 0;

        public RenderSurface()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

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
            Array.Clear(buffer, 0, buffer.Length);
            currentBucket = null;
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

            using var axisPen = new Pen(Color.Gray, 1);
            using var circlePen = new Pen(Color.Green, 1.5f);

            // ✅ Axis
            g.DrawLine(axisPen, 0, cy, Width, cy);
            g.DrawLine(axisPen, cx, 0, cx, Height);

            // ✅ Circles (restored)
            // 🔥 Big circle
            float r1 = 100f;
            g.DrawEllipse(circlePen, cx - r1, cy - r1, 2 * r1, 2 * r1);

            // 🔥 Small circle (reduced radius)
            float r2 = 20f;
            g.DrawEllipse(circlePen, cx - r2, cy - r2, 2 * r2, 2 * r2);

            // ✅ No data yet
            if (currentBucket == null || bucketCount < 2)
                return;

            // ✅ Draw ONLY current bucket
            for (int i = 1; i < bucketCount; i++)
            {
                var s1 = currentBucket[i - 1];
                var s2 = currentBucket[i];

                PointF p1 = new(cx + s1.X, cy - s1.Y);
                PointF p2 = new(cx + s2.X, cy - s2.Y);

                g.DrawLine(Pens.Lime, p1, p2);
            }
        }

        private void DrawGraph(Graphics g)
        {


            float left = 40f;
            float bottom = Height - 20f;
            float right = Width - 10f;
            float top = 10f;

            // ✅ Axis pen
            using var axisPen = new Pen(Color.Gray, 1);

            // 🔥 X-axis (time)
            g.DrawLine(axisPen, left, bottom, right, bottom);

            // 🔥 Y-axis (magnitude)
            g.DrawLine(axisPen, left, bottom, left, top);

            // Optional: grid lines (nice improvement)
            using var gridPen = new Pen(Color.FromArgb(40, 255, 255, 255));

            for (int i = 1; i <= 4; i++)
            {
                float y = bottom - i * (bottom - top) / 5;
                g.DrawLine(gridPen, left, y, right, y);
            }
            if (count < 2) return;
            float step = (right - left) / BufferSize;

            PointF prev = default;

            for (int i = 0; i < count; i++)
            {
                int idx = (head - 1 - i + BufferSize) % BufferSize;
                var s = buffer[idx];

                float px = left + i * step;
                float py = bottom - (float)s.Z;

                var cur = new PointF(px, py);

                if (i > 0)
                    g.DrawLine(Pens.Lime, prev, cur);

                prev = cur;
            }
        }
    }
}