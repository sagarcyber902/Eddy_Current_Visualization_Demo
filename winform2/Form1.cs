
using winform2.Core;
using winform2.Model;

namespace winform2
{
    public partial class Form1 : Form
    {
        private RenderSurface impedanceSurface;
        private RenderSurface graphSurface;
        private SignalController controller = new();

        public Form1()
        {
            InitializeComponent();

            // UI setup
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

            // 🔥 UI ONLY subscribes
            controller.OnSample += OnSample;
            controller.OnBucket += OnBucket;

            // Buttons → controller only
            btnStart.Click += (s, e) =>
            {
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();
                controller.Start();
            };

            btnPause.Click += (s, e) =>
            {
                controller.Pause();
            };

            btnClear.Click += (s, e) =>
            {
                controller.Clear();
                graphSurface.ClearAll();
                impedanceSurface.ClearAll();
            };
        }

        // 🔥 PURE UI METHODS

        private void OnSample(Sample s)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => OnSample(s));
                return;
            }

            graphSurface.PushSample(s);
            graphSurface.Invalidate();
        }

        private void OnBucket(Sample[] b, int c)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => OnBucket(b, c));
                return;
            }

            impedanceSurface.SetBucket(b, c);
            impedanceSurface.Invalidate();
        }
    }
}