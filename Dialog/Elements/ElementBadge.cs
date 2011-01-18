//
// ElementBadge.cs: defines the Badge Element.
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
	using System;
	using System.Drawing;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	/// <summary>
	///    This element can be used to show an image with some text
	/// </summary>
	/// <remarks>
	///    The font can be configured after the element has been created
	///    by assignign to the Font property;   If you want to render
	///    multiple lines of text, set the MultiLine property to true.
	/// 
	///    If no font is specified, it will default to Helvetica 17.
	/// 
	///    A static method MakeCalendarBadge is provided that can 
	///    render a calendar badge like the iPhone OS.   It will compose
	///    the text on top of the image which is expected to be 57x57
	/// </remarks>
	public class BadgeElement : Element, IElementSizing
	{
		public event NSAction Tapped;
		public UILineBreakMode LineBreakMode = UILineBreakMode.TailTruncation;
		public UIViewContentMode ContentMode = UIViewContentMode.Left;
		public int Lines = 1;
		public UITableViewCellAccessory Accessory = UITableViewCellAccessory.None;
		private UIImage image;
		private UIFont font;
	
		public BadgeElement (UIImage badgeImage, string cellText)
			: this (badgeImage, cellText, null)
		{
		}

		public BadgeElement (UIImage badgeImage, string cellText, NSAction tapped) : base (cellText)
		{
			if (badgeImage == null)
				throw new ArgumentNullException ("badgeImage");
			
			image = badgeImage;
			if (tapped != null)
				Tapped += tapped;
		}		
	
		public UIFont Font
		{
			get
			{
				if (font == null)
					font = UIFont.FromName ("Helvetica", 17f);
				return font;
			}
			set
			{
				if (font != null)
					font.Dispose ();
				font = value;
			}
		}

		public override void InitializeCell(UITableView tableView)
		{
			Cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;

			Cell.Accessory = Accessory;

			var textLabel = Cell.TextLabel;
			textLabel.Text = Caption;
			textLabel.Font = Font;
			textLabel.LineBreakMode = LineBreakMode;
			textLabel.Lines = Lines;
			textLabel.ContentMode = ContentMode;

			Cell.ImageView.Image = image;
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			SizeF size = new SizeF (280, float.MaxValue);
			float height = tableView.StringSize (Caption, Font, size, LineBreakMode).Height + 10;
			
			// Image is 57 pixels tall, add some padding
			return Math.Max (height, 63);
		}

		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (Tapped != null)
				Tapped ();
			tableView.DeselectRow (path, true);
		}
		
		public static UIImage MakeCalendarBadge (UIImage template, string smallText, string bigText)
		{
			using (var cs = CGColorSpace.CreateDeviceRGB ()){
				using (var context = new CGBitmapContext (IntPtr.Zero, 57, 57, 8, 57*4, cs, CGImageAlphaInfo.PremultipliedLast)){
					//context.ScaleCTM (0.5f, -1);
					context.TranslateCTM (0, 0);
					context.DrawImage (new RectangleF (0, 0, 57, 57), template.CGImage);
					context.SetRGBFillColor (1, 1, 1, 1);
					
					context.SelectFont ("Helvetica", 10f, CGTextEncoding.MacRoman);
					
					// Pretty lame way of measuring strings, as documented:
					var start = context.TextPosition.X;					
					context.SetTextDrawingMode (CGTextDrawingMode.Invisible);
					context.ShowText (smallText);
					var width = context.TextPosition.X - start;
					
					context.SetTextDrawingMode (CGTextDrawingMode.Fill);
					context.ShowTextAtPoint ((57-width)/2, 46, smallText);
					
					// The big string
					context.SelectFont ("Helvetica-Bold", 32, CGTextEncoding.MacRoman);					
					start = context.TextPosition.X;
					context.SetTextDrawingMode (CGTextDrawingMode.Invisible);
					context.ShowText (bigText);
					width = context.TextPosition.X - start;
					
					context.SetRGBFillColor (0, 0, 0, 1);
					context.SetTextDrawingMode (CGTextDrawingMode.Fill);
					context.ShowTextAtPoint ((57-width)/2, 9, bigText);
					
					context.StrokePath ();
				
					return UIImage.FromImage (context.ToImage ());
				}
			}
		}
	}
}