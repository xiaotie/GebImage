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
        public Color BackgroundColor = Color.White;
        public double BackgroundAlpha = 1;
        public Color ForegroundColor = Color.Gray;
        public double ForegroundAlpha = 1;
        public Boolean IsRootDisplayObject;

        protected bool _invalidated = false;

        public Action<DisplayObject> OnInvalidate;

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
            PointD pos = GetDrawContextPostion();
            pos = new PointD { X = x - pos.X, Y = y - pos.Y };
            System.Windows.Forms.MessageBox.Show(String.Format("{0}({1},{2})", eventName, pos.X, pos.Y));
        }

        public DisplayObject Parent;

        public virtual DisplayObject HitTest(double x, double y)
        {
            PointD pos = GetDrawContextPostion();
            if (x < pos.X || y < pos.Y || x > pos.X + this.Width || y > pos.Y + this.Height) return null;
            else return this;
        }

        public virtual void Create()
        {
        }

        protected virtual PointD GetDrawContextPostion(DisplayObject child)
        {
            PointD pos = GetDrawContextPostion();
            return new PointD { X = pos.X + child.X, Y = pos.Y + child.Y } ;
        }

        protected virtual PointD GetDrawContextPostion()
        {
            if (Parent == null) return IsRootDisplayObject ? new PointD { X = X, Y = Y } : new PointD();
            else return Parent.GetDrawContextPostion(this);
        }

        protected void DrawRectBackground(Graphics g, PointD pos)
        {
            RectangleF r = new RectangleF(pos.ToPointF(), new SizeF((float)Width, (float)Height));
            g.FillRectangle(new SolidBrush(BackgroundColor), r.X, r.Y, r.Width, r.Height);
        }

        public virtual void Draw(Graphics g)
        {
            if (_invalidated == true) return;
            _invalidated = true;
        }
    }
}
