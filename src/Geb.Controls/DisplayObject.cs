using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Controls
{
    public class DisplayObject
    {
        public double X, Y;
        public double Width = 100, Height = 30;

        public Color BackgroundColor = Color.Gray;
        public Border Border { get; private set; }
        public Boolean IsRoot;

        protected bool _invalidated = false;

        public Action<DisplayObject> OnInvalidate;

        public Boolean Captured { get; set; }

        public virtual void SetInvalidated(Boolean value)
        {
            this._invalidated = value;
        }

        public void Invalidate()
        {
            if (_invalidated == true)
            {
                _invalidated = false;
                if (Parent != null) Parent.Invalidate();
                if (OnInvalidate != null) OnInvalidate(this);
            }
        }

        public virtual void OnMouseEvent(String eventName, double x, double y)
        {
        }

        public DisplayObject Parent;

        public virtual DisplayObject HitTest(double x, double y)
        {
            if (x < X || y < Y || x > X + this.Width || y > Y + Height) return null;
            else return this;
        }

        public virtual void Create()
        {
        }

        protected void DrawBackground(Graphics g)
        {
            if(BackgroundColor.A > 0)
            {
                RectangleF r = new RectangleF(new Point(), new SizeF((float)Width, (float)Height));
                g.FillRectangle(new SolidBrush(BackgroundColor), r.X, r.Y, r.Width, r.Height);
            }
            
            if(Border.Thickness > 0 && Border.Color.A > 0)
            {
                RectangleF r = new RectangleF(new Point(), new SizeF((float)Width, (float)Height));
                g.DrawRectangle(new Pen(new SolidBrush(Border.Color)), r.X, r.Y, r.Width, r.Height);
            }
        }

        public virtual void Draw(Graphics g)
        {
            if (_invalidated == true) return;
            _invalidated = true;
        }
    }
}
