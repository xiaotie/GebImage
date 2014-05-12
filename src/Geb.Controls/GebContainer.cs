using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace Geb.Controls
{
    public class GebContainer : Control
    {
        public GebContainer()
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
            element.IsRootDisplayObject = true;
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
                item.SetInvalidated(false);
                item.Draw(g);
            }
        }

        public void InitEvents()
        {
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GebContainer_MouseDown);
            this.MouseUp += GebContainer_MouseUp;
        }

       private  void GebContainer_MouseUp(object sender, MouseEventArgs e)
        {
            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                find.OnMouseEvent("MouseUp", e.Location.X, e.Location.Y);
            }
        }

        private void GebContainer_MouseDown(object sender, MouseEventArgs e)
        {
            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                find.OnMouseEvent("MouseDown", e.Location.X, e.Location.Y);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
