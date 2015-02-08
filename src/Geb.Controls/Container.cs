using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Geb.Controls
{
    public class Container:DisplayObject
    {
        public List<DisplayObject> Controls { get; set; }

        public Boolean IsChildrenMouseEnable = true;

        public Container()
            : base()
        { }

        public override DisplayObject HitTest(double x, double y)
        {
            DisplayObject match = base.HitTest(x, y);
            if (match == null) return null;
            else
            {
                if (Controls != null && IsChildrenMouseEnable == true)
                {
                    for (int i = Controls.Count - 1; i >= 0; i--)
                    {
                        DisplayObject item = Controls[i];
                        DisplayObject m = item.HitTest(x - item.X, y - item.Y);
                        if (m != null) return m;
                    }
                }
            }
            return match;
        }

        public override void SetInvalidated(Boolean value)
        {
            this._painted = value;
            if (Controls == null) return;
            foreach (DisplayObject item in this.Controls)
            {
                item.SetInvalidated(value);
            }
        }

        public override void Create()
        {
            if (Controls == null) return;
            foreach(DisplayObject item in Controls)
            {
                item.Create();
            }
        }

        public void Add(DisplayObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (Controls == null) Controls = new List<DisplayObject>();
            element.Parent = this;
            Controls.Add(element);
            Controls.Sort();
        }

        public void Remove(DisplayObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (Controls != null && this.Controls.Contains(element) == true)
            {
                element.Parent = null;
                Controls.Remove(element);
            }
        }

        public override void Draw(Graphics g)
        {
            if (_painted == true || Visible == false) return;

            SetInvalidated(false);
            _painted = true;
            
            this.DrawBackground(g);

            if (Controls != null)
            {
                foreach (DisplayObject item in Controls)
                {
                    item.Update();
                    var oldM = g.Transform;
                    g.Transform = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, oldM.OffsetX + (float)item.X, oldM.OffsetY + (float)item.Y);
                    item.Draw(g);
                    g.Transform = oldM;
                }
                    g.ResetClip();
            }
        }
    }
}
