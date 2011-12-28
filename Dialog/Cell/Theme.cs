//
// Theme.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
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
namespace MonoMobile.Views
{
	using System;
	using System.Drawing;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class Theme : NSObject
	{
		private UITableViewCell _Cell;

		private UIImage _CellImageIcon;
		private Uri _CellImageIconUri;

		private UIImage _CellBackgroundImage;
		private Uri _CellBackgroundUri;
		private UIColor _CellBackgroundColor;
		
		private UIImage _BackgroundImage;
		private Uri _BackgroundUri;
		private UIColor _BackgroundColor;

		public string Name { get; set; }

		public UIColor PlaceholderColor { get; set; }
		public UITextAlignment PlaceholderAlignment { get; set; }

		public UIColor SeparatorColor { get; set; }
		public UITableViewCellSeparatorStyle? SeparatorStyle { get; set; }
		public UITableViewStyle TableViewStyle { get; set; }

		public UIBarStyle? BarStyle { get; set; }
		public UIColor BarTintColor { get; set; }
		public string BarImage { get; set; }
		public bool BarTranslucent { get; set; }

		public UITableViewCell Cell 
		{ 
			get {return _Cell; }
			set { ThemeChanged(value); } 
		}
	
		public float CellHeight { get; set; }

		public UIImage CellImageIcon
		{
			get { return _CellImageIcon; }
			set
			{
				_CellImageIcon = value;
				_CellImageIconUri = null;
			}
		}

		public Uri CellImageIconUri
		{
			get { return _CellImageIconUri; }
			set {
				_CellImageIconUri = value;
				
				if (_CellImageIcon != null)
				{
					_CellImageIcon.Dispose();
					_CellImageIcon = null;
				}
			}
		}

		public UIImage CellBackgroundImage
		{
			get { return _CellBackgroundImage; }
			set {
				_CellBackgroundImage = value;
				_CellBackgroundUri = null;
				_CellBackgroundColor = null;

				ConfigureBackgroundImage();
			}
		}

		public Uri CellBackgroundUri
		{
			get { return _CellBackgroundUri; }
			set {
				_CellBackgroundUri = value;
				_CellBackgroundColor = null;
				
				if (_CellBackgroundImage != null)
				{
					_CellBackgroundImage.Dispose();
					_CellBackgroundImage = null;
				}

				ConfigureBackgroundImage();
			}
		}

		public UIColor CellBackgroundColor
		{
			get { return _CellBackgroundColor; }
			set {
				_CellBackgroundColor = value;
				_CellBackgroundUri = null;
				
				if (_CellBackgroundImage != null)
				{
					_CellBackgroundImage.Dispose();
					_CellBackgroundImage = null;
				}

				ConfigureBackgroundImage();
			}
		}

		public UIFont TextFont { get; set; }
		public UIColor TextColor { get; set; } 
		public UITextAlignment TextAlignment { get; set; }
		public SizeF TextShadowOffset { get; set; }
		public UIColor TextShadowColor { get; set; }
		public UIColor TextHighlightColor { get; set; }

		public UIFont DetailTextFont { get; set; }
		public UIColor DetailTextColor { get; set; }
		public UITextAlignment DetailTextAlignment { get; set; }
		public SizeF DetailTextShadowOffset { get; set; }
		public UIColor DetailTextShadowColor { get; set; }
		public UIColor DetailTextHighlightColor { get; set; }

		public UIFont HeaderTextFont { get; set; }
		public UITextAlignment HeaderTextAlignment { get; set; }
		public UIColor HeaderTextColor { get; set; }
		public UIColor HeaderTextBackgroundColor { get; set; }
		public SizeF HeaderTextShadowOffset { get; set; }
		public UIColor HeaderTextShadowColor { get; set; }
		public UIColor HeaderBackgroundColor { get; set; }

		public UIFont FooterTextFont { get; set; }
		public UITextAlignment FooterTextAlignment { get; set; }
		public UIColor FooterTextColor { get; set; }
		public UIColor FooterTextBackgroundColor { get; set; }
		public SizeF FooterTextShadowOffset { get; set; }
		public UIColor FooterTextShadowColor { get; set; }
		public UIColor FooterBackgroundColor { get; set; }
		
		public UIImage BackgroundImage
		{
			get 
			{ 
				return _BackgroundImage; 
			}
			set 
			{
				_BackgroundImage = value;
				_BackgroundUri = null;
				_BackgroundColor = null;
			}
		}

		public Uri BackgroundUri
		{
			get 
			{ 
				return _BackgroundUri;
			}
			set 
			{
				_BackgroundUri = value;
				_BackgroundColor = null;
				
				if (_BackgroundImage != null)
				{
					_BackgroundImage.Dispose();
					_BackgroundImage = null;
				}
			}
		}

		public UIColor BackgroundColor
		{
			get 
			{ 
				return _BackgroundColor; 
			}
			set
			{
				_BackgroundColor = value;
				_BackgroundUri = null;
				
				if (_BackgroundImage != null)
				{
					_BackgroundImage.Dispose();
					_BackgroundImage = null;
				}
			}
		}
		
		public UIColor DisabledColor { get; set; }

		public Action<RectangleF, CGContext, UITableViewCell> DrawCellViewAction { get; set; }

		public Theme()
		{
			Name = GetType().Name.Replace("Theme", string.Empty);

			PlaceholderAlignment = UITextAlignment.Right;
			DisabledColor = UIColor.FromWhiteAlpha(0.8f, 0.4f); 
			
			HeaderTextAlignment = UITextAlignment.Left;
			FooterTextAlignment = UITextAlignment.Center;

			HeaderTextBackgroundColor = UIColor.Clear;
			FooterTextBackgroundColor = UIColor.Clear;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_Cell = null;
			}

			base.Dispose(disposing);
		}

		public static Theme CreateTheme(Theme theme)
		{
			var newTheme = CreateTheme();

			newTheme.MergeTheme(theme);
			if (theme != null)
			{
				theme.Dispose();
			}

			return newTheme;
		}
		
		public static Theme CreateTheme()
		{
			var newTheme = new Theme();
			return newTheme;
		}

		public static Theme CreateTheme(Type themeType)
		{
			var newTheme = Activator.CreateInstance(themeType) as Theme;
			return newTheme;
		}

		public void MergeTheme(Theme theme)
		{
			if (theme != null)
			{	
				Name = theme.Name;

				if (theme.CellHeight != 0 && CellHeight != theme.CellHeight)
				{
					CellHeight = theme.CellHeight;
				}
				
				if (theme.CellBackgroundColor != null)
				{
					CellBackgroundColor = theme.CellBackgroundColor.Clone();
				}
				
				if (theme.CellBackgroundUri != null)
				{
					CellBackgroundUri = theme.CellBackgroundUri;
				}
				
				if (theme.CellBackgroundImage != null)
				{
					CellBackgroundImage = theme.CellBackgroundImage;
				}

				
				if (theme.TextFont != null)
				{
					TextFont = theme.TextFont.Clone();
				}
				

				if (theme.TextColor != null)
				{
					TextColor = theme.TextColor.Clone();
				}
				
				if (theme.TextShadowOffset != SizeF.Empty)
				{
					TextShadowOffset = theme.TextShadowOffset;
				}
				
				if (theme.TextShadowColor != null)
				{
					TextShadowColor = theme.TextShadowColor.Clone();
				}
				
				if (theme.TextHighlightColor != null)
				{
					TextHighlightColor = theme.TextHighlightColor.Clone();
				}
				
				TextAlignment = theme.TextAlignment;

				
				if (theme.DetailTextFont != null)
				{
					DetailTextFont = theme.DetailTextFont.Clone();
				}
				

				if (theme.DetailTextColor != null)
				{
					DetailTextColor = theme.DetailTextColor.Clone();
				}

				if (theme.DetailTextShadowOffset != SizeF.Empty)
				{
					DetailTextShadowOffset = theme.DetailTextShadowOffset;
				}
				
				if (theme.DetailTextShadowColor != null)
				{
					DetailTextShadowColor = theme.DetailTextShadowColor.Clone();
				}
				
				if (theme.DetailTextHighlightColor != null)
				{
					DetailTextHighlightColor = theme.DetailTextHighlightColor.Clone();
				}
				
				DetailTextAlignment = theme.DetailTextAlignment;

				
				if (theme.PlaceholderColor != null)
				{
					PlaceholderColor = theme.PlaceholderColor.Clone();
				}
				
				if (theme.PlaceholderAlignment != UITextAlignment.Right)
				{
					PlaceholderAlignment = theme.PlaceholderAlignment;
				}

				if (theme.SeparatorColor != null)
				{
					SeparatorColor = theme.SeparatorColor.Clone();
				}

				if (theme.SeparatorStyle.HasValue)
				{
					SeparatorStyle = theme.SeparatorStyle;
				}

				TableViewStyle = theme.TableViewStyle;				

				if (theme.BarStyle.HasValue)
				{
					BarStyle = theme.BarStyle;
				}

				if (theme.BarTintColor != null)
				{
					BarTintColor = theme.BarTintColor.Clone();
				}
		
				if (theme.BarImage != null)
				{
					BarImage = theme.BarImage;
				}
		
				if (theme.BarTranslucent)
				{
					BarTranslucent = theme.BarTranslucent;
				}

			
				HeaderTextAlignment = theme.HeaderTextAlignment;
				
				if (theme.HeaderTextFont != null)
				{
					HeaderTextFont = theme.HeaderTextFont.Clone();
				}
				
				if (theme.HeaderTextColor != null)
				{
					HeaderTextColor = theme.HeaderTextColor.Clone();
				}
				
				if (theme.HeaderTextBackgroundColor != null)
				{
					HeaderTextBackgroundColor = theme.HeaderTextBackgroundColor.Clone();
				}

				if (theme.HeaderTextShadowOffset != SizeF.Empty)
				{
					HeaderTextShadowOffset = theme.HeaderTextShadowOffset;
				}
				
				if (theme.HeaderTextShadowColor != null)
				{
					HeaderTextShadowColor = theme.HeaderTextShadowColor.Clone();
				}
				
				if (theme.HeaderBackgroundColor != null)
				{
					HeaderBackgroundColor = theme.HeaderBackgroundColor.Clone();
				}
				

				FooterTextAlignment = theme.FooterTextAlignment;
				
				if (theme.FooterTextFont != null)
				{
					FooterTextFont = theme.FooterTextFont.Clone();
				}
				
				if (theme.FooterTextColor != null)
				{
					FooterTextColor = theme.FooterTextColor.Clone();
				}
				
				if (theme.FooterTextBackgroundColor != null)
				{
					FooterTextBackgroundColor = theme.FooterTextBackgroundColor.Clone();
				}

				if (theme.FooterTextShadowOffset != SizeF.Empty)
				{
					FooterTextShadowOffset = theme.FooterTextShadowOffset;
				}
				
				if (theme.FooterTextShadowColor != null)
				{
					FooterTextShadowColor = theme.FooterTextShadowColor.Clone();
				}
			
				if (theme.FooterBackgroundColor != null)
				{
					FooterBackgroundColor = theme.FooterBackgroundColor.Clone();
				}
			

				if (theme.DrawCellViewAction != null)
				{
					DrawCellViewAction = theme.DrawCellViewAction;
				}

				if (theme.BackgroundColor != null)
				{
					BackgroundColor = theme.BackgroundColor.Clone();
				}
				
				if (theme.BackgroundUri != null)
				{
					BackgroundUri = theme.BackgroundUri;
				}
				
				if (theme.BackgroundImage != null)
				{
					BackgroundImage = theme.BackgroundImage;
				}
			}
		}

		public void ThemeChanged(UITableViewCell cell)
		{
			_Cell = cell;

			if (_Cell == null)
			{
				return;
			}

			if (_Cell.TextLabel != null)
			{
				if (TextFont != null)
				{
					_Cell.TextLabel.Font = TextFont;
				}

				_Cell.TextLabel.TextAlignment = TextAlignment;
				_Cell.TextLabel.TextColor = TextColor ?? _Cell.TextLabel.TextColor;

				if (TextShadowColor != null)
				{
					_Cell.TextLabel.ShadowColor = TextShadowColor;
				}
				
				if (TextShadowOffset != SizeF.Empty)
				{
					_Cell.TextLabel.ShadowOffset = TextShadowOffset;
				}
				
				if (TextHighlightColor != null)
				{
					_Cell.TextLabel.HighlightedTextColor = TextHighlightColor;
				}
			}
			
			if (_Cell.DetailTextLabel != null)
			{
				if (DetailTextFont != null)
				{
					_Cell.DetailTextLabel.Font = DetailTextFont;
				}

				_Cell.DetailTextLabel.TextAlignment = DetailTextAlignment;
				_Cell.DetailTextLabel.TextColor = DetailTextColor ?? _Cell.DetailTextLabel.TextColor;

				if (DetailTextShadowColor != null)
				{
					_Cell.DetailTextLabel.ShadowColor = DetailTextShadowColor;
				}
				
				if (DetailTextShadowOffset != SizeF.Empty)
				{
					_Cell.DetailTextLabel.ShadowOffset = DetailTextShadowOffset;
				}
				
				if (DetailTextHighlightColor != null)
				{
					_Cell.DetailTextLabel.HighlightedTextColor = DetailTextHighlightColor;
				}
			}

			if (CellBackgroundColor != null)
			{
				_Cell.BackgroundColor = CellBackgroundColor;
			}
			else if (CellBackgroundImage != null)
			{
				_Cell.BackgroundColor = UIColor.FromPatternImage(CellBackgroundImage);
			}
			else
			{
				_Cell.BackgroundColor = UIColor.White;
			}
			
			ConfigureBackgroundImage();
			_Cell.SetNeedsDisplay();
		}

		private void ConfigureBackgroundImage()
		{
			if (Cell != null)
			{
				if (Cell.BackgroundColor == null)
				{	
					if (BackgroundImage != null)
					{
						Cell.BackgroundColor = UIColor.FromPatternImage(BackgroundImage);
					}
			
					if (BackgroundColor != null)
					{
						Cell.BackgroundColor = BackgroundColor;
					}
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}

