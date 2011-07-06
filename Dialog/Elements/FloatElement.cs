//
// FloatView.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Based on cdoe from MonoTouch.Dialog by Miguel de Icaza (miguel@gnome.org)
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
namespace MonoMobile.MVVM
{
	using System.Drawing;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class FloatElement : Element
	{
		public UISlider Slider { get; set; }
	
		public float MinValue { get; set; }
		public float MaxValue { get; set; }

		public FloatElement(string caption) : base(caption)
		{
			MinValue = 0;
			MaxValue = 1;

			DataTemplate = new FloatElementDataTemplate(this);
		}

		public FloatElement(RectangleF frame) : this("")
		{
			Frame = frame;
		}
		
		public override UITableViewElementCell NewCell()
		{
			var cell = base.NewCell();
			cell.Accessory = UITableViewCellAccessory.None;

			return cell;
		}

		public override void InitializeContent()
		{
			Slider = new UISlider() { BackgroundColor = UIColor.Clear, Continuous = true, Tag = 1 };
			Slider.Frame = new RectangleF(0, 0, Cell.Bounds.Width, Cell.Bounds.Height);
			Slider.ValueChanged += delegate 
			{
				DataTemplate.UpdateDataContext();
			};

			Slider.MaxValue = MaxValue;
			Slider.MinValue = MinValue;
			
			ContentView = Slider;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Slider != null)
			{
				Slider.Dispose();
				Slider = null;
			}
			
			base.Dispose(disposing);
		}
	}
}

