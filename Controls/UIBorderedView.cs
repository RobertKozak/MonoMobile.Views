// 
//  UIBorderedView.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.Views
{
	using System;
	using MonoTouch.UIKit;
	using System.Drawing;

	public partial class UIBorderedView : UIView
	{
		private UIActivityIndicatorView _ActivityIndicatorView;
		
		public UIImageView ImageView { get; set; }
		public int CornerRadius { get; set; }
		public UIColor ShadowColor { get; set; }
		public float ShadowOpacity { get; set; }
		public float ShadowRadius { get; set; }
		public SizeF ShadowOffset { get; set; }
		public int BorderWidth { get; set; }

		public UIBorderedView(IntPtr handle) : base(handle)
		{
			Initialize();
			CreateCurveAndShadow();
		}

		public UIBorderedView(RectangleF frame) : base(frame)
		{
			Initialize();
			CreateCurveAndShadow();
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				if (ImageView != null)
				{
					ImageView.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		public void AddImage(UIImage image, bool animate)
		{
			if (image != null)
			{
				InvokeOnMainThread(() =>
				{
					var bounds = Bounds;
					ImageView = new UIImageView(new RectangleF(BorderWidth, BorderWidth, Bounds.Width - (BorderWidth * 2), Bounds.Height - (BorderWidth * 2)));
					ImageView.Layer.CornerRadius = 5.0f;
					ImageView.Layer.MasksToBounds = true;
					ImageView.Layer.BorderWidth = 1;
					ImageView.Layer.BorderColor = UIColor.LightGray.CGColor;
					ImageView.Image = image;
					ImageView.Alpha = 0f;
	
					Add(ImageView);
				
					_ActivityIndicatorView.StopAnimating();

					UIView.BeginAnimations("fadeIn");
					UIView.SetAnimationDuration(animate ? 0.6f : 0f);
					ImageView.Alpha = 1.0f;
					UIView.CommitAnimations();
				});
			}
		}

		private void Initialize()
		{
			_ActivityIndicatorView = new UIActivityIndicatorView(Bounds);
			_ActivityIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
			_ActivityIndicatorView.HidesWhenStopped = true;
			_ActivityIndicatorView.StartAnimating();

			Add(_ActivityIndicatorView);

			CornerRadius = 10;
			ShadowColor = UIColor.DarkGray;
			ShadowOpacity = 1.0f;
			ShadowRadius = 1.0f;
			ShadowOffset = new SizeF(0, 0);
			BackgroundColor = UIColor.White;
		}

		private void CreateCurveAndShadow()
		{
			Layer.MasksToBounds = false;
			Layer.ShadowColor = ShadowColor.CGColor;
			Layer.ShadowOpacity = ShadowOpacity;
			Layer.ShadowRadius = ShadowRadius;
			Layer.ShadowOffset = ShadowOffset; 	
			Layer.CornerRadius = CornerRadius;	
		}
	}
}

