using System;
using System.Collections.Generic;
using System.Text;

namespace Geb.Controls
{
    public enum MouseButtons
    {
        Left,
        Middle,
        None,
        Right
    }

    public class MouseEventArgs
    {
        public double StageX, StageY;

        public MouseButtons Button;
        public DisplayObject Owner;
        public double X, Y, Delta;
        public static MouseEventArgs Create(System.Windows.Forms.MouseEventArgs e, DisplayObject sender)
        {
            PointD p = sender.GlobalToLocal(new PointD(e.Location.X, e.Location.Y));
            MouseEventArgs me = new MouseEventArgs();
            me.X = p.X;
            me.Y = p.Y;
            me.StageX = e.Location.X;
            me.StageY = e.Location.Y;

            switch(e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    me.Button = MouseButtons.Left;
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    me.Button = MouseButtons.Right;
                    break;
                case System.Windows.Forms.MouseButtons.None:
                    me.Button = MouseButtons.None;
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    me.Button = MouseButtons.Middle;
                    break;
            }
            me.Delta = e.Delta;
            me.Owner = sender;
            return me;
        }
    }
}
