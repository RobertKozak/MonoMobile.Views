//
// RangeAttribute.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corportation
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
namespace MonoMobile.Views
{
	using System;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class RangeAttribute : CellViewTemplate
	{
		public override Type CellViewType { get { return typeof(SliderCellView); } }

		public RangeAttribute(float low, float high)
		{
			Low = low;
			High = high;
		}

		public float Low, High;
		public bool ShowCaption;

		[Preserve(AllMembers = true)]
		class SliderCellView : CellView<float>, IAccessoryView
		{ 
			public UISlider Slider { get; set; }
		
			public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Default; } }

			public SliderCellView(RectangleF frame) : base(RectangleF.Empty)
			{
				Slider = new UISlider(new RectangleF(0, 0, 100, frame.Height)) 
				{ 
					AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
					BackgroundColor = UIColor.Clear, 
					Continuous = true,
					
					Tag = 1 
				};

				Slider.ValueChanged += (sender, e) => 
				{
					DataContext.Value = Slider.Value;
				};

				Add(Slider);
			}

			public override void UpdateCell(UITableViewCell cell, MonoTouch.Foundation.NSIndexPath indexPath)
			{	
				cell.TextLabel.Text = string.Empty;
			
				var rangeAttribute = DataContext.Member.GetCustomAttribute<RangeAttribute>();
				if (rangeAttribute != null)
				{
					Slider.MaxValue = rangeAttribute.High;
					Slider.MinValue = rangeAttribute.Low;
					ShowCaption = rangeAttribute.ShowCaption;
				}
			
				if (ShowCaption)
				{
					cell.TextLabel.Text = Caption;
				}

				Slider.Value = (float)DataContext.Value;
			}
		
//		public override void LayoutSubviews()
//		{
//			base.LayoutSubviews();
//			float x = 10;
//			if (ShowCaption)
//			{	
//				x = StringSize(Caption, Cell.TextLabel.Font, Bounds.Size, UILineBreakMode.TailTruncation).Width + 20;
//			}
//		
//			Slider.Frame = new RectangleF(x, 0, Bounds.Width - x - 10, Bounds.Height); 
//		}
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
}