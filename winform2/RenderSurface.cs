using System;
using System.Drawing;
using System.Windows.Forms;
using winform2.Model;
using System.ComponentModel;

namespace winform2
{
    public class RenderSurface : Control
    {
        private const int BufferSize = 1000;

        private readonly Sample[] buffer = new Sample[BufferSize];
        private int head = 0;
        private int count = 0;

        private Sample[] liveBuffer = new Sample[40];
        private int liveCount = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsImpedance { get; set; }

        private Sample[]? currentBucket = null;
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

            liveBuffer[liveCount % liveBuffer.Length] = s;
            liveCount++;
        }

        public void SetBucket(Sample[] bucket, int count)
        {
            currentBucket = bucket;
            bucketCount = count;

            liveCount = 0;
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

        // ============================
        // 🔥 IMPEDANCE PLANE
        // ============================
        private void DrawImpedance(Graphics g)
        {
            float cx = Width / 2f;
            float cy = Height / 2f;

            using var axisPen = new Pen(Color.Gray, 1);
            using var circlePen = new Pen(Color.Green, 1.5f);

            // Axis
            g.DrawLine(axisPen, 0, cy, Width, cy);
            g.DrawLine(axisPen, cx, 0, cx, Height);

            // Circles
            g.DrawEllipse(circlePen, cx - 100, cy - 100, 200, 200);
            g.DrawEllipse(circlePen, cx - 20, cy - 20, 40, 40);

            if (currentBucket == null || bucketCount < 2)
                return;

            // 🔥 Bucket drawing
            for (int i = 1; i < bucketCount; i++)
            {
                var p1 = MapToScreen(currentBucket[i - 1], cx, cy);
                var p2 = MapToScreen(currentBucket[i], cx, cy);

                g.DrawLine(Pens.Lime, p1, p2);
            }

            // 🔥 Live preview
            int points = Math.Min(liveCount, liveBuffer.Length);

            for (int i = 1; i < points; i++)
            {
                var p1 = MapToScreen(liveBuffer[(i - 1) % liveBuffer.Length], cx, cy);
                var p2 = MapToScreen(liveBuffer[i % liveBuffer.Length], cx, cy);

                g.DrawLine(Pens.Yellow, p1, p2);
            }
        }

        // 🔥 CLIP TO CONTROL
        private PointF MapToScreen(Sample s, float cx, float cy)
        {
            float x = cx + s.X;   // ✅ NO scaling
            float y = cy - s.Y;   // ✅ NO scaling

            // optional clipping (safe)
            x = Math.Max(0, Math.Min(Width, x));
            y = Math.Max(0, Math.Min(Height, y));

            return new PointF(x, y);
        }

        // ============================
        // 🔥 GRAPH WITH SCALE
        // ============================
        private void DrawGraph(Graphics g)
        {
            float left = 40f;
            float bottom = Height - 20f;
            float right = Width - 10f;
            float top = 10f;

            using var axisPen = new Pen(Color.Gray, 1);
            using var gridPen = new Pen(Color.FromArgb(40, 255, 255, 255));

            // Axis
            g.DrawLine(axisPen, left, bottom, right, bottom);
            g.DrawLine(axisPen, left, bottom, left, top);

            // 🔥 Simple grid (NO scaling math)
            for (int i = 0; i < 10; i++)
            {
                float y = bottom - i * 20; // fixed spacing
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

                // ✅ RAW VALUE (NO scaling)
                float py = bottom - (float)s.Z;

                // optional clipping (safe)
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