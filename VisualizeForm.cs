using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LINVAST.Nodes;

namespace LINVAST.Visualizer
{
    public partial class VisualizeForm : Form
    {
        private readonly NodeControlCreator cc;
        private readonly ASTNode ast;
        private readonly TrackBar tb;


        public VisualizeForm(ASTNode ast)
        {
            this.InitializeComponent();
            this.cc = new NodeControlCreator(this);
            this.ast = ast;

            this.Text = "AST Visualizer";
            this.Size = new Size(1200, 800);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(6000, 20000);

            this.tb = new TrackBar() {
                Minimum = 100,
                Maximum = 100000,
                Value = 1200,
                LargeChange = 1000,
                SmallChange = 100,
                Parent = this,
                Location = new Point(10, 10),
                Size = new Size(300, 20)
            };
            this.tb.ValueChanged += this.ResizeScroll;
        }


        protected override void OnResize(EventArgs _) => this.Invalidate();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            this.Draw(e.Graphics, this.ast, 0, this.AutoScrollMinSize.Width, 10);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            this.tb.Location = new Point(10, 10);
        }


        private (Point Top, Point Bottom) Draw(Graphics g, ASTNode node, int s, int w, int h)
        {
            (Point Top, Point Bottom) loc = this.cc.DrawNode(g, node, s + w / 2, h);

            int cx = s;
            int cw = w / (node.Children.Count > 1 ? node.Children.Count : 1);
            foreach (ASTNode child in node.Children) {
                (Point Top, Point Bottom) cloc = this.Draw(g, child, cx, cw, h + 100);
                g.DrawLine(new Pen(Color.Black, 1), loc.Bottom, cloc.Top);
                cx += cw;
            }

            return loc;
        }

        private void ResizeScroll(object? sender, EventArgs e)
        {
            if (sender is TrackBar sb)
                this.AutoScrollMinSize = new Size(sb.Value, this.AutoScrollMinSize.Height);
            this.Invalidate();
        }
    }
}
