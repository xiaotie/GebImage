using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Controls
{
    public class DisplayObject : IComparable<DisplayObject>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public int ZIndex { get; set; }
        
        public double Width = 100, Height = 30;

        private Color _backColor = Color.Gray;

        public Color BackColor
        {
            get { return _backColor; }
            set {
                _backColor = value;
                _needDraw = false;
            }
        }
        public Border Border { get; set; }
        public Boolean IsRoot;

        protected bool _needDraw = false;

        public Action<DisplayObject> OnInvalidate;

        public Boolean Capture { get; set; }

        public virtual void SetInvalidated(Boolean value)
        {
            this._needDraw = value;
        }

        public void Invalidate()
        {
            if (_needDraw == true)
            {
                _needDraw = false;
                if (Parent != null) Parent.Invalidate();
                if (OnInvalidate != null) OnInvalidate(this);
            }
        }

        protected internal virtual void OnMouseDown(MouseEventArgs e)
        {
        }

        protected internal virtual void OnMouseUp(MouseEventArgs e)
        {
        }

        protected internal virtual void OnMouseMove(MouseEventArgs e)
        {
        }

        private Boolean _hide;

        public Boolean Visible
        {
            get { return !_hide; }
            set { _hide = !value; }
        }

        public PointD LocalToGlobal(PointD pos)
        {
            if (IsRoot == true || Parent == null) return pos + new PointD(X,Y);
            return Parent.LocalToGlobal(pos + new PointD(X,Y));
        }

        public PointD GlobalToLocal(PointD pos)
        {
            if (IsRoot == true || Parent == null) return pos - new PointD(X, Y);
            return Parent.GlobalToLocal(pos - new PointD(X, Y));
        }

        public DisplayObject Parent;

        public virtual DisplayObject HitTest(double x, double y)
        {
            if (x < 0 || y < 0 || x > this.Width || y > Height) return null;
            else return this;
        }

        public virtual void Create()
        {
        }

        protected void DrawBackground(Graphics g)
        {
            if(BackColor.A > 0)
            {
                RectangleF r = new RectangleF(new Point(), new SizeF((float)Width, (float)Height));
                g.FillRectangle(new SolidBrush(BackColor), r.X, r.Y, r.Width, r.Height);
            }
            
            if(Border.Thickness > 0 && Border.Color.A > 0)
            {
                Pen pen = new Pen(new SolidBrush(Border.Color));
                RectangleF r = new RectangleF(new Point(), new SizeF((float)Width - 1, (float)Height - 1));
                if (Border.BorderTopVisible == true)
                    g.DrawLine(pen, r.Left, r.Top, r.Right, r.Top);
                if (Border.BorderBottomVisible == true)
                    g.DrawLine(pen, r.Left, r.Bottom, r.Right, r.Bottom);
                if (Border.BorderLeftVisible == true)
                    g.DrawLine(pen, r.Left, r.Top, r.Left, r.Bottom);
                if (Border.BorderRightVisible == true)
                    g.DrawLine(pen, r.Right, r.Top, r.Right, r.Bottom);
            }
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(Graphics g)
        {
            if (this.Visible == false) return;

            if (_needDraw == true) return;
            _needDraw = true;
            DrawBackground(g);
        }

        public int CompareTo(DisplayObject other)
        {
            return this.ZIndex.CompareTo(other.ZIndex);
        }
    }
}
