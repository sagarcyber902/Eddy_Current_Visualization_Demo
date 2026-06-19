using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace winform2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnStart;
        private Button btnPause;
        private Button btnClear;
        private Panel topPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnStart = new Button();
            btnPause = new Button();
            btnClear = new Button();
            topPanel = new Panel();

            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;

            btnStart.Text = "Start";
            btnPause.Text = "Pause";
            btnClear.Text = "Clear";

            btnStart.Left = 10;
            btnPause.Left = 100;
            btnClear.Left = 190;

            topPanel.Controls.Add(btnStart);
            topPanel.Controls.Add(btnPause);
            topPanel.Controls.Add(btnClear);

            Controls.Add(topPanel);

            ClientSize = new Size(1250, 420);
            Text = "Eddy Current";
        }
    }
}