//
// ImageElement.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Drawing;
	using System.Linq;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using MonoMobile.MVVM;

	public partial class ImageElement : Element, ISelectable
	{		
		protected override void SetDataContext(object value)
		{
			if (value != null && DataContext != value && value is UIImage)
			{				
				_DataContext = value;
				_Scaled = Scale((UIImage)DataContext);
				SetNeedsDisplay();
			}
			else
			{
			//	_DataContext = MakeEmpty();
			//	_Scaled = (UIImage)DataContext;
			}
		}

		private static RectangleF rect = new RectangleF(0, 0, dimx, dimy);
		private UIImage _Scaled;
		private UIPopoverController popover;

		// Apple leaks this one, so share across all.
		private static UIImagePickerController picker;

		// Height for rows
		private const int dimx = 48;
		private const int dimy = 43;

		// radius for rounding
		private const int rad = 10;

		private static UIImage MakeEmpty()
		{
			using (var cs = CGColorSpace.CreateDeviceRGB())
			{
				using (var bit = new CGBitmapContext(IntPtr.Zero, dimx, dimy, 8, 0, cs, CGImageAlphaInfo.PremultipliedFirst))
				{
					bit.SetRGBStrokeColor(1, 0, 0, 0.5f);
					bit.FillRect(new RectangleF(0, 0, dimx, dimy));
					
					return UIImage.FromImage(bit.ToImage());
				}
			}
		}

		private UIImage Scale(UIImage source)
		{
			UIGraphics.BeginImageContext(new SizeF(dimx, dimy));
			var ctx = UIGraphics.GetCurrentContext();
			
			var img = source.CGImage;
			ctx.TranslateCTM(0, dimy);
			if (img.Width > img.Height)
				ctx.ScaleCTM(1, -img.Width / dimy);
			else
				ctx.ScaleCTM(img.Height / dimx, -1);
			
			ctx.DrawImage(rect, source.CGImage);
			
			var ret = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return ret;
		}

		public ImageElement() : base("")
		{
		}

		public ImageElement(UIImage image) : base("")
		{
			DataContext = image;
		}

		public override void UpdateCell()
		{
			if (_Scaled != null)
			{
				bool roundTop = Section.Elements.FirstOrDefault() == this;
				bool roundBottom = Section.Elements[Section.Elements.Count - 1] == this;
	
				using (var cs = CGColorSpace.CreateDeviceRGB())
				{
					using (var bit = new CGBitmapContext(IntPtr.Zero, dimx, dimy, 8, 0, cs, CGImageAlphaInfo.PremultipliedFirst))
					{
						// Clipping path for the image, different on top, middle and bottom.
						if (roundBottom)
						{
							bit.AddArc(rad, rad, rad, (float)Math.PI, (float)(3 * Math.PI / 2), false);
	
						}
						else
						{
							bit.MoveTo(0, rad);
							bit.AddLineToPoint(0, 0);
						}
						bit.AddLineToPoint(dimx, 0);
						bit.AddLineToPoint(dimx, dimy);
	
						if (roundTop)
						{
							bit.AddArc(rad, dimy - rad, rad, (float)(Math.PI / 2), (float)Math.PI, false);
							bit.AddLineToPoint(0, rad);
	
						}
						else
						{
							bit.AddLineToPoint(0, dimy);
						}
						bit.Clip();
						bit.DrawImage(rect, _Scaled.CGImage);
	
						Cell.ImageView.Image = UIImage.FromImage(bit.ToImage());
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_Scaled.Dispose();
				var disposable = DataContext as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
			base.Dispose(disposing);
		}

		class MyDelegate : UIImagePickerControllerDelegate
		{
			ImageElement container;
			UITableView table;
			NSIndexPath path;

			public MyDelegate(ImageElement container, UITableView table, NSIndexPath path)
			{
				this.container = container;
				this.table = table;
				this.path = path;
			}

			public override void FinishedPickingImage(UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
			{
				container.Picked(image);
				table.ReloadRows(new NSIndexPath[] { path }, UITableViewRowAnimation.None);
			}
		}

		void Picked(UIImage image)
		{
			DataContext = image;
			_Scaled = Scale(image);
			currentController.DismissModalViewControllerAnimated(true);
			
		}

		UIViewController currentController;
		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (picker == null)
				picker = new UIImagePickerController();
			picker.Delegate = new MyDelegate(this, tableView, path);

			switch (UIDevice.CurrentDevice.UserInterfaceIdiom)
			{
				case UIUserInterfaceIdiom.Pad:
					RectangleF useRect;
					popover = new UIPopoverController(picker);
					var cell = tableView.CellAt(path);
					if (cell == null)
						useRect = rect;
					else
						useRect = cell.Frame;
					popover.PresentFromRect(useRect, dvc.View, UIPopoverArrowDirection.Any, true);
					break;
				default:
				
				case UIUserInterfaceIdiom.Phone:
					dvc.ActivateController(picker, dvc);
					break;
			}
			currentController = dvc;
		}
	}
}

