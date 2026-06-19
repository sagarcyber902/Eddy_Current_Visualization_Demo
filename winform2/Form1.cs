using winform2.Core;
using winform2.Model;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace winform2
{
    public partial class Form1 : Form
    {
        private RenderSurface impedanceSurface;
        private RenderSurface graphSurface;
        private SignalController controller = new();

        private Stopwatch renderTimer = Stopwatch.StartNew();
        private const int TargetFrameMs = 16;

        public Form1()
        {
            InitializeComponent();

            impedanceSurface = new RenderSurface
            {
                Width = 303,
                Height = 303,
                Left = 10,
                Top = 60,
                IsImpedance = true
            };

            graphSurface = new RenderSurface
            {
                Width = 905,
                Height = 229,
                Left = 320,
                Top = 60
            };

            Controls.Add(impedanceSurface);
            Controls.Add(graphSurface);

            

            // ✅ Bucket for impedance
            controller.OnBucket += OnBucket;

            btnStart.Click += (s, e) =>
            {
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();

                double[] arrD = new double[]
            {
                0, 0, 42, -6, -1, 1, -5, 5, 1, 0, -5, 3, 2, 0, -2, 0, -2, 1, 2, -1, 1, -1,
                -1, 0, 0, 0, 2, 0, 0, 0, 4, -2, 4, -2, 3, -1, 1, -1, -4, 2, 0, 0, 3, -2, -4, 2, 7, -15, 7, -511, 61, -41, 10, 114, 203, -101, -41, 211, 611, 8,
                 -2, -7, -2, 2, 6, -2, -3, 5, -2, -1, 3, 4, -1, 4, -1, -2, 1, 2, 0, 4, -2, -3, 1, 3, -1, 0, 0, -2, 0, 0, 0,
                1, 0, 0, 0, 11, 0, 11, 0, -11, 0, 0, 0, 2, 0, 0, 0, 1, -1, 1, -1, -2, 1, -1, 0, 3, -2, -1, 0, 4, -2, 5, -3, 4, -2, 0, -1,
                2, 0, 0, -1, 12, -9, 8, -5, -11, 9, -4, 1, 1, 3, 3, -4, -2, 2, 0, -2, 3, 1, 0, 0, -2, -1, 0, 1, -1, 0, 0, -2, 1,
                2, -1, 2, -1, 0, 0, 5, 0, -1, 3, -3, -8, 3, 1, -4, 2, 0, -1, 4, -2, -2, 5, 4, -1, 2, 0, -2, 2, -1, 0, 2, -2, -2, 0, 2,
                -1, 2, -1, -2, 1, -1, 0, 3, -1, 0, 0, -2, 1, 2, -1, 2, -1, 0, 0, -2, 0, -1, 0, 4, -1, 0, 0, -5, 3, -3, 2, 8, -7, 0, 0,
                5, -5, -3, 1, -6, 2, 5, 0, -2, -7, -1, 6, -2, 6, 3, -1, -3, 2, -1, 0, 3, -3, -3, 0, -5, 0, 4, -2, 4, -1, -4, -1, -7, -2,
                4, 2, -11, -5, 1, -5, 17, 16, -8, -1, 5, 8, -5, -7, -2, 9, 4, -1, -5, 4, -4, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 1, 0, 1, 0, -3, 2, -1, 0, -5, 5, -7, 7, -8, 7, 2, 0, 3, -1, -2, 0, 4, 10, 2, 10, 0, 9, 5, -4, -7, 1,
                -4, 4, 7, -5, -2, -1, -3, 4, 2, -1, -6, 3, -5, 3, 6, -4, 3, 0, -6, 3, 0, -1, 7, -3, -5, 2, 3, 6, -2, -5, -12, -2, -3, 0,
                32, 11, -4, 1, -14, -8, -2, -10, -2, -8, -2, 4, 6, -2, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0
               , 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, -2, 0, -2, 0, -7, 6, -5, 4, 4, -3, 2, 0, -7, 4, -8, 4, 3, -8, 1, 3, 3, -2, 3, -2, -6, 2, -4,
                -1, 6, -3, -2, 3, -6, 3, 5, -3, 4, -2, -2, 2, 4, -3, -1, 0, -3, 4, 3, -2, 6, 1, -5, -1, -4, -9, -5, -6, 20, 21, -8, -9, -7,
                -8, -4, -2, -11, 3, 4, -1, 6, 0, -2, 0, 4, -3, -2, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 1, 0, 1, 0, 1, 0, -1, 1,
                -1, 1, -2, 1, -1, 1, -6, 4, -3, 3, 3, -1, 0, 2, -10, 2, -5, 0, 7, 0, 0, 0, -11, 0, 10, -1, 8, -2, -1, 5, 4, -4, 1, 0, -5, 3,
                0, -2, -4, 4, -4, 5, -3, 5, 2, 0, -8, -2, 0, -1, 8, 3, -9, -5, 19, 0, 27, 4, 30, 13, -4, -2, 1, -8, -1, 2, 1, 6, 1, -2, -4, 3,
                1, -1, 2, -1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, -1, 0, 1, 0, -1, 0, -2, 1, -2, 2, 0, 0, -4, 3, -4, 3, -3, 3, 4, -3,
                 0, 0, 6, -2, 4, -3, 5, -3, -1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, -1, 1, 0, 1, 3, -3, 3, -3, -6, 5,
                2, 0, 4, -2, -1, 5, -3, -11, 6, 4, 6, 3, 2, -2, -4, 2, -2, 0, 2, -3, 0, -4, 1, 5, -1, -1, 5, 2, 0, 2, -5, -4, 2, 0, 0, 4, -2,
                -3, -3, -5, -3, -3, 16, 9, -2, -7, -9, -10, -6, -2, 11, -2, 1, 7, -8, 2, 1, -5, 3, -4, 1, 0, 3, -2, 1, 0, -1, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, -1, 0, 0, 2, 0, 2, 0, -3, 1, 0, 0, 4, -2, 0, 0, -3, 2, -1, 0, 6, -4, -1, 0, 8, 6, 9, 6, -6, -6, -5, -5, 8, 1, 3, 4,
                -3, -1, -1, 0, 2, -1, 0, 1, -6, -5, 6, 2, 3, 3, -2, 0, -4, -6, 4, 2, 3, 4, -3, -3, -13, -1, -3, 2, 12, 4, 1, -3, -4, 3, 2, 0, 3,
                -3, 4, -3, 4, -2, -1, 0, 1, 0, 1, 0, -1, 0, 1, 0, 2, -1, -2, 0, 3, -2, 0, 0, -4, 3, 1, 0, -2, 3, -4, 4, -4, 3, 3, -3, 8, -7, -4,
                4, -8, 4, 3, -3, 4, -1, -2, 0, 7, 5, -3, -3, -2, -3, 0, 0, 3, 0, 1, 1, -4, 1, 0, -3, -10, -9, -9, -6, 11, 6, 1, 3, -5, -3, 2, -1,
                3, 4, -2, 2, 8, -3, 6, -2, -6, 4, 1, 0, -2, 1, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
                 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, -2, 0, -2, 0, -7, 6, -5, 4, 4, -3, 2, 0, -7, 4, -8, 4, 3, -8, 1, 3, 3, -2, 3, -2, -6, 2, -4,
                -1, 6, -3, -2, 3, -6, 3, 5, -3, 4, -2, -2, 2, 4, -3, -1, 0, -3, 4, 3, -2, 6, 1, -5, -1, -4, -9, -5, -6, 20, 21, -8, -9, -7,
                -8, -4, -2, -11, 3, 4, -1, 6, 0, -2, 0, 4, -3, -2, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 1, 0, 1, 0, 1, 0, -1, 1,
                -1, 1, -2, 1, -1, 1, -6, 4, -3, 3, 3, -1, 0, 2, -10, 2, -5, 0, 7, 0, 0, 0, -11, 0, 10, -1, 8, -2, -1, 5, 4, -4, 1, 0, -5, 3,
                0, -21, -4, 41, -4, 5, -3, 51, 2, 0, -8, -21, 0, -1, 81, 3, -9, -5, 119, 0, 271, 4, 301, 13, -41, -2, 1, -8, -1, 2, 1, 6, 1, -2, -4, 3,
                11, -11, 21, -1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, -1, 0, 1, 0, -1, 0, -2, 1, -2, 2, 0, 0, -4, 3, -4, 3, -3, 3, 4, -3,
                -7, 71, 0, 0, -9, -3, -101, -1, 18, 8, -1, -10, 1, -101, 1, 10, 110, 3, 0, 3, -1, 3, 3, -2, -4, 1, -2, 0, 3, 0, -1, 0, -4, -4, 0,
                -31, 1, 41, -21, 0, -21, -4, 1, 0, 8, 1, 41, 0, -47, -8, 110, 0, 42, -6, -1, 1, -5, 5, 1, 0, -5, 3, 2, 0, -2, 0, -2, 1, 2, -1, 1, -1,
                -1, 0, 0, 0, 2, 0, 0, 0, 4, -2, 4, -2, 3, -1, 1, -1, -4, 2, 0, 0, 3, -2, -4, 2, 7, -15, 7, -511, 61, -41, 10, 114, 203, -101, -41, 211, 611, 8,
                 -2, -7, -2, 2, 6, -2, -3, 5, -2, -1, 3, 4, -1, 4, -1, -2, 1, 2, 0, 4, -2, -3, 1, 3, -1, 0, 0, -2, 0, 0, 0, 0 };

                controller.Start(arrD);
            };

            btnPause.Click += (s, e) => controller.Pause();

            btnClear.Click += (s, e) =>
            {
                controller.Clear();
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();
            };

            // 🔥 Render loop
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            Application.Idle += RenderLoop;
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
        }

       

        // 🔥 BUCKET-BASED IMPEDANCE
        private void OnBucket(Sample[] b, int c)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => OnBucket(b, c));
                return;
            }

            // ✅ impedance (same as before)
            impedanceSurface.SetBucket(b, c);

            // 🔥 graph also bucket-wise
            for (int i = 0; i < c; i++)
            {
                graphSurface.PushSample(b[i]);
            }
        }

        // 🔥 60 FPS RENDER LOOP
        private void RenderLoop(object sender, EventArgs e)
        {
            while (IsAppIdle())
            {
                if (renderTimer.ElapsedMilliseconds < TargetFrameMs)
                    continue;

                renderTimer.Restart();

                graphSurface.Invalidate();
                impedanceSurface.Invalidate();
            }
        }

        // 🔧 Win32 idle check
        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint a, uint b, uint c);

        private bool IsAppIdle()
        {
            return !PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
        }
    }
}