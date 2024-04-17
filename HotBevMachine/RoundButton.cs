using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotBevMachine
{
    public class RoundButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            GraphicsPath p = new GraphicsPath();
            p.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            Region = new Region(p);
            base.OnPaint(pevent);
            
            FlatAppearance.BorderColor = BackColor;

            if (Name == "rbt1E" || Name == "rbt2E")
            {
                Pen pen = new(ForeColor, (Font.Size + 8) / 2);
                pen.Alignment = PenAlignment.Inset;
                pevent.Graphics.DrawPath(pen, p);
                pen.Dispose();
            }

            p.Dispose();
        }
    }
}
