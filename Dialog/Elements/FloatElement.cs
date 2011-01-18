//
// FloatElement.cs
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace MonoTouch.Dialog
{
	using System.Drawing;
	using MonoTouch.UIKit;

	/// <summary>
	///  Used to display a slider on the screen.
	/// </summary>
	public class FloatElement : Element<float>
	{
		public bool ShowCaption;
		public float MinValue, MaxValue;
		private UISlider slider;

		public FloatElement(float value) : base(null)
		{
			MinValue = 0;
			MaxValue = 1;
			Value = value;
		}

		public FloatElement() : base(null)
		{
			MinValue = 0;
			MaxValue = 1;
		}
		public override void InitializeCell(UITableView tableView)
		{
			RemoveTag(1);

			SizeF captionSize = new SizeF(0, 0);
			if (Caption != null && ShowCaption)
			{
				Cell.TextLabel.Text = Caption;
				captionSize = Cell.TextLabel.StringSize(Caption, UIFont.FromName(Cell.TextLabel.Font.Name, UIFont.LabelFontSize));
				captionSize.Width += 10;
				// Spacing
			}

			if (slider == null)
			{
				slider = new UISlider(new RectangleF(10f + captionSize.Width, 12f, 280f - captionSize.Width, 7f)) { BackgroundColor = UIColor.Clear, MinValue = this.MinValue, MaxValue = this.MaxValue, Continuous = true, Value = this.Value, Tag = 1 };
				slider.ValueChanged += delegate { Value = slider.Value; };
			}

			Cell.ContentView.AddSubview(slider);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				slider.Dispose();
				slider = null;
			}
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			if (slider != null)
				slider.Value = Value;
		}
	}

}

