using MonoMobile.MVVM;

namespace Samples
{
	using System;
	using MonoTouch.UIKit;
	using System.Drawing;
	using MonoTouch.CoreAnimation;
	using MonoTouch.CoreGraphics;
	using MonoMobile.MVVM.Controls;

	public class TestView : View
	{
		private UILabel label;

		public TestView()
		{
			this.BackgroundColor = UIColor.White;
			Frame = new RectangleF(0, 0, 400, 400);
			label = new UILabel (Frame);
			label.Text = "";
			label.Font = UIFont.BoldSystemFontOfSize(17);
			label.BackgroundColor = this.BackgroundColor;
			AddSubview (label);

		var color = UIColor.FromRGB(88, 170, 34);
		UIButton btn = new UIGlassyButton(new RectangleF(0, 0, 250, 42)) { Color = UIColor.Red, Title ="Red Button" };
			UIButton btn2 = new UIGlassyButton (new RectangleF (0, 50, 250, 42)) { Color = UIColor.Gray, Title = "Gray Button" };
			var btn3 = new UIGlassyButton (new RectangleF (0, 100, 250, 42)) { Color = UIColor.Black, Title = "Black Button", HighlightColor = UIColor.Blue };
			UIButton btn4 = new UIGlassyButton (new RectangleF (0, 150, 250, 42)) { Color = color, Title = "Green Button" };


			AddSubview (btn);
			AddSubview (btn2);
			AddSubview (btn3);
			AddSubview (btn4);

			btn.TouchDown += HandleBtnTouchDown;
			btn2.TouchDown += HandleBtnTouchDown;
			btn3.TouchDown += HandleBtnTouchDown;
			btn4.TouchDown += HandleBtnTouchDown;
		}

		void HandleBtnTouchDown (object sender, EventArgs e)
		{
			Console.WriteLine("Clicked");
		}


	}
}
