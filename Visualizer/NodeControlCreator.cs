using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LINVAST.Nodes;
using Newtonsoft.Json;

namespace LINVisualizer
{
    internal sealed class NodeControlCreator
    {
        public Form Parent { get; }


        public NodeControlCreator(VisualizeForm parent)
        {
            this.Parent = parent;
        }


        public (Point Top, Point Bottom) DrawNode(Graphics g, ASTNode node, int x, int y)
        {
            PropertyInfo[] props = node.GetType()
                .GetProperties()
                .Where(p => !p.CustomAttributes.Any(attr => attr.AttributeType == typeof(JsonIgnoreAttribute)))
                .ToArray()
                ;

            int maxw = 0;
            int currh = 20;
            foreach (PropertyInfo prop in props.OrderBy(p => p.Name)) {
                if (prop.Name == "Children" || prop.Name == "NodeType")
                    continue;
                string value = prop.GetValue(node)?.ToString() ?? "null";
                string text = $"{prop.Name}: {(string.IsNullOrWhiteSpace(value) ? "N/A" : value)}";

                g.DrawString(text, new Font("Consolas", 9), Brushes.Black, new Point(x, y + currh));
                currh += 20;

                maxw = Math.Max(maxw, (int)g.MeasureString(text, new Font("Consolas", 9)).Width);
            }

            g.DrawString(node.NodeType, new Font("Consolas", 11, FontStyle.Bold), Brushes.Black, new Point(x, y));
            maxw = Math.Max(maxw, (int)g.MeasureString(node.NodeType, new Font("Consolas", 11, FontStyle.Bold)).Width);

            var rect = new Rectangle(x, y, maxw, currh);
            g.DrawRectangle(new Pen(Color.Black, 2), rect);

            return (new Point(x + maxw / 2, y), new Point(x + maxw / 2, y + currh));
        }
    }
}
