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
                    foreach (DisplayObject item in Controls)
                    {
                        DisplayObject m = item.HitTest(x, y);
                        if (m != null) return m;
                    }
                }
            }
            return match;
        }

        public override void SetInvalidated(Boolean value)
        {
            this._invalidated = value;
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
            if (_invalidated == true) return;

            SetInvalidated(false);
            _invalidated = true;

            this.DrawRectBackground(g, this.GetDrawContextPostion());

            if (Controls != null)
            {
                foreach (DisplayObject item in Controls)
                {
                    item.Draw(g);
                }
            }
        }
    }
}
