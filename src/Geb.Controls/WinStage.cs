using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Controls
{
    public class WinStage : Control
    {
        public WinStage()
        {
            this.DoubleBuffered = true;
        }

        public List<DisplayObject> DisplayObjects = new List<DisplayObject>();

        public DisplayObject HitTest(double x, double y)
        {
            foreach (DisplayObject item in DisplayObjects)
            {
                DisplayObject m = item.HitTest(x, y);
                if (m != null) return m;
            }
            return null;
        }

        public void Add(DisplayObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (Controls == null) DisplayObjects = new List<DisplayObject>();
            element.IsRoot = true;
            element.OnInvalidate = (DisplayObject item) => { this.Invalidate(); };
            element.Create();
            DisplayObjects.Add(element);
        }

        public void Remove(DisplayObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            if (Controls != null && this.DisplayObjects.Contains(element) == true)
            {
                element.Parent = null;
                DisplayObjects.Remove(element);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            var g = e.Graphics;
            
            foreach (DisplayObject item in DisplayObjects)
            {
                var oldM = g.Transform;
                g.Transform = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, oldM.OffsetX + (float)item.X, oldM.OffsetY + (float)item.Y);
                item.SetInvalidated(false); 
                item.Draw(g);
                g.Transform = oldM;
            }
        }

        public void InitEvents()
        {
            this.MouseDown += GebContainer_MouseDown;
            this.MouseUp += GebContainer_MouseUp;
            this.MouseMove += GebContainer_MouseMove;
        }

        private DisplayObject _captureObj;

        private void GebContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_captureObj != null && _captureObj.Captured == true)
            {
                _captureObj.OnMouseEvent("MouseMove", e.Location.X - _captureObj.X, e.Location.Y - _captureObj.Y);
                return;
            }

            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                find.OnMouseEvent("MouseMove", e.Location.X - find.X, e.Location.Y - find.Y);
            }
        }

       private  void GebContainer_MouseUp(object sender, MouseEventArgs e)
       {
           if (_captureObj != null && _captureObj.Captured == true)
           {
               _captureObj.OnMouseEvent("MouseUp", e.Location.X - _captureObj.X, e.Location.Y - _captureObj.Y);
               _captureObj = null;
               return;
           }

           _captureObj = null;
            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                find.OnMouseEvent("MouseUp", e.Location.X - find.X, e.Location.Y - find.Y);
            }
        }

        private void GebContainer_MouseDown(object sender, MouseEventArgs e)
        {
            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                if (find.Captured == true) _captureObj = find;
                find.OnMouseEvent("MouseDown", e.Location.X - find.X, e.Location.Y - find.Y);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
