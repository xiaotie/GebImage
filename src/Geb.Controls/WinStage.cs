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

        private bool _inited;

        public Action<WinStage> OnInit;

        public List<DisplayObject> DisplayObjects = new List<DisplayObject>();

        public DisplayObject HitTest(double x, double y)
        {
            for (int i = DisplayObjects.Count - 1; i >= 0; i -- )
            {
                DisplayObject item = DisplayObjects[i];
                DisplayObject m = item.HitTest(x - item.X, y - item.Y);
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
            DisplayObjects.Sort();
        }

        public Boolean Contains(DisplayObject element)
        {
            return DisplayObjects.Contains(element);
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

            if(_inited == false)
            {
                _inited = true;
                InitEvents();
                if (OnInit != null) OnInit(this);
            }

            var g = e.Graphics;
            
            foreach (DisplayObject item in DisplayObjects)
            {
                item.Update();

                var oldM = g.Transform;
                g.Transform = new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, oldM.OffsetX + (float)item.X, oldM.OffsetY + (float)item.Y);
                item.SetInvalidated(false); 
                item.Draw(g);
                g.Transform = oldM;
            }
        }

        internal void InitEvents()
        {
            this.MouseDown += GebContainer_MouseDown;
            this.MouseUp += GebContainer_MouseUp;
            this.MouseMove += GebContainer_MouseMove;
        }

        private DisplayObject _captureObj;

        private void UpdateCursor(DisplayObject ui = null)
        {
            if (ui == null) this.Cursor = System.Windows.Forms.Cursors.Default;
            switch(ui.Cursor)
            {
                case Cursors.Arrow: this.Cursor = System.Windows.Forms.Cursors.Arrow; break;
                case Cursors.AppStarting: this.Cursor = System.Windows.Forms.Cursors.AppStarting; break;
                case Cursors.Cross: this.Cursor = System.Windows.Forms.Cursors.Cross; break;
                case Cursors.Hand: this.Cursor = System.Windows.Forms.Cursors.Hand; break;
                case Cursors.Help: this.Cursor = System.Windows.Forms.Cursors.Help; break;
                case Cursors.HSplit: this.Cursor = System.Windows.Forms.Cursors.HSplit; break;
                case Cursors.IBeam: this.Cursor = System.Windows.Forms.Cursors.IBeam; break;
                case Cursors.No: this.Cursor = System.Windows.Forms.Cursors.No; break;
                case Cursors.NoMove2D: this.Cursor = System.Windows.Forms.Cursors.NoMove2D; break;
                case Cursors.NoMoveHoriz: this.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz; break;
                case Cursors.NoMoveVert: this.Cursor = System.Windows.Forms.Cursors.NoMoveVert; break;
                case Cursors.PanEast: this.Cursor = System.Windows.Forms.Cursors.PanEast; break;
                case Cursors.PanNE: this.Cursor = System.Windows.Forms.Cursors.PanNE; break;
                case Cursors.PanNorth: this.Cursor = System.Windows.Forms.Cursors.PanNorth; break;
                case Cursors.PanNW: this.Cursor = System.Windows.Forms.Cursors.PanNW; break;
                case Cursors.PanSE: this.Cursor = System.Windows.Forms.Cursors.PanSE; break;
                case Cursors.PanSouth: this.Cursor = System.Windows.Forms.Cursors.PanSouth; break;
                case Cursors.PanSW: this.Cursor = System.Windows.Forms.Cursors.PanSW; break;
                case Cursors.PanWest: this.Cursor = System.Windows.Forms.Cursors.PanWest; break;
                case Cursors.SizeAll: this.Cursor = System.Windows.Forms.Cursors.SizeAll; break;
                case Cursors.SizeNESW: this.Cursor = System.Windows.Forms.Cursors.SizeNESW; break;
                case Cursors.SizeNS: this.Cursor = System.Windows.Forms.Cursors.SizeNS; break;
                case Cursors.SizeNWSE: this.Cursor = System.Windows.Forms.Cursors.SizeNWSE; break;
                case Cursors.SizeWE: this.Cursor = System.Windows.Forms.Cursors.SizeWE; break;
                case Cursors.UpArrow: this.Cursor = System.Windows.Forms.Cursors.UpArrow; break;
                case Cursors.VSplit: this.Cursor = System.Windows.Forms.Cursors.VSplit; break;
                case Cursors.WaitCursor: this.Cursor = System.Windows.Forms.Cursors.WaitCursor; break;
                case Cursors.Default:
                default:
                    this.Cursor = System.Windows.Forms.Cursors.Default; break;
            }
        }

        private void GebContainer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_captureObj != null && _captureObj.Capture == true)
            {
                MouseEventArgs me = MouseEventArgs.Create(e, _captureObj);
                _captureObj.OnMouseMove(me);
                UpdateCursor(_captureObj);
                return;
            }

            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                MouseEventArgs me = MouseEventArgs.Create(e, find);
                find.OnMouseMove(me);
                UpdateCursor(find);
            }
        }

        private void GebContainer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           Capture = false;
           if (_captureObj != null && _captureObj.Capture == true)
           {
               MouseEventArgs me = MouseEventArgs.Create(e, _captureObj);
               _captureObj.OnMouseUp(me);
               return;
           }

           DisplayObject find = HitTest(e.Location.X, e.Location.Y);
           if (find != null)
           {
               MouseEventArgs me = MouseEventArgs.Create(e, find);
               find.OnMouseUp(me);
           }
        }

        private void GebContainer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Capture = true;
            DisplayObject find = HitTest(e.Location.X, e.Location.Y);
            if (find != null)
            {
                MouseEventArgs me = MouseEventArgs.Create(e, find);
                find.OnMouseDown(me);
                if (find.Capture == true) _captureObj = find;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
