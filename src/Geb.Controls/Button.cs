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

        public override void Create()
        {
            base.Create();
            Label label = new Label();
            label.Text = Text;
            this.Add(label);
        }
    }
}
