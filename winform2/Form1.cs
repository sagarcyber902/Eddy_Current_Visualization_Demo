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

            

            
            controller.OnBucket += OnBucket;

            btnStart.Click += (s, e) =>
            {
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();

                var data = DataProvider.GetTestData();

                controller.Start(data);
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