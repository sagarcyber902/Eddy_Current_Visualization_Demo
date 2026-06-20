using System.Runtime.InteropServices;
using winform2.Core;
using winform2.Model;



namespace winform2
{
    public partial class Form1 : Form
    {
        private RenderSurface graph;
        private RenderSurface impedance;
        private SignalController controller = new();

        public Form1()
        {
            InitializeComponent();

            impedance = new RenderSurface
            {
                Width = 303,
                Height = 303,
                Left = 10,
                Top = 60,
                IsImpedance = true
            };

            graph = new RenderSurface
            {
                Width = 905,
                Height = 229,
                Left = 320,
                Top = 60
            };

            Controls.Add(impedance);
            Controls.Add(graph);

            controller.OnBucket += OnBucket;

            btnStart.Click += (s, e) =>
            {
                var data = DataProvider.GetData();

                controller.Start(data);
            };

            Application.Idle += RenderLoop;
        }

        private void OnBucket(Sample[] b, int c)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => OnBucket(b, c));
                return;
            }

            impedance.SetBucket(b, c);

            for (int i = 0; i < c; i++)
                graph.PushSample(b[i]);
        }

        private void RenderLoop(object sender, EventArgs e)
        {
            while (IsIdle())
            {
                graph.Invalidate();
                impedance.Invalidate();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MSG { public IntPtr h; public uint m; public IntPtr w; public IntPtr l; public uint t; public Point p; }

        [DllImport("user32.dll")]
        static extern bool PeekMessage(out MSG msg, IntPtr hWnd, uint a, uint b, uint c);

        private bool IsIdle() => !PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
    }
}