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

            // ✅ Real-time graph
            controller.OnSample += OnSample;

            // ✅ Bucket for impedance
            controller.OnBucket += OnBucket;

            btnStart.Click += (s, e) =>
            {
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();
                controller.Start();
            };

            btnPause.Click += (s, e) => controller.Pause();

            btnClear.Click += (s, e) =>
            {
                controller.Clear();
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();
            };

            // 🔥 Render loop
            Application.Idle += RenderLoop;
        }

        // 🔥 REAL-TIME GRAPH (NO DELAY)
        private void OnSample(Sample s)
        {
            graphSurface.PushSample(s);
        }

        // 🔥 BUCKET-BASED IMPEDANCE
        private void OnBucket(Sample[] b, int c)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => OnBucket(b, c));
                return;
            }

            impedanceSurface.SetBucket(b, c);
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