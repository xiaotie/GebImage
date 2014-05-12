using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Geb.Controls
{
    public class Button : Container
    {
        public String Text { get; set; }

        public Button()
        {
            IsChildrenMouseEnable = false;
        }

        public override void OnMouseEvent(String eventName, double x, double y)
        {
            switch(eventName)
            {
                case "MouseDown":
                    this.BackgroundColor = Color.Red;
                    break;
                case "MouseUp":
                    this.BackgroundColor = Color.White;
                    break;
            }
            this.Invalidate();
        }

        public override void Create()
        {
            base.Create();
            Label label = new Label();
            label.Text = Text;
            this.Add(label);
        }
    }
}
