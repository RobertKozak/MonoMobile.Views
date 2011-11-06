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
	using System.Reflection;
	using MonoMobile.Views.Utilities;
	using MonoTouch.CoreGraphics;
	using MonoTouch.UIKit;
	
	public class Theme
	{
		private UITableViewElementCell _Cell;

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
		public UITableViewStyle? TableViewStyle { get; set; }

		public UIBarStyle? BarStyle { get; set; }
		public UIColor BarTintColor { get; set; }
		public string BarImage { get; set; }
		public bool BarTranslucent { get; set; }

		public UITableViewElementCell Cell 
		{ 
			get {return _Cell; }
			set { InitializePropertiesFromCell(value); } 
		}
	
		public UITableViewCellAccessory? Accessory { get; set; }
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
		public SizeF HeaderTextShadowOffset { get; set; }
		public UIColor HeaderTextShadowColor { get; set; }
		public UIColor HeaderBackgroundColor { get; set; }

		public UIFont FooterTextFont { get; set; }
		public UITextAlignment FooterTextAlignment { get; set; }
		public UIColor FooterTextColor { get; set; }
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

		public Action<RectangleF, CGContext, UITableViewElementCell> DrawElementViewAction { get; set; }

		public Theme()
		{
			Name = GetType().Name.Replace("Theme", string.Empty);

			PlaceholderAlignment = UITextAlignment.Right;
			DisabledColor = UIColor.FromWhiteAlpha(0.8f, 0.4f); 
		}
		
		public static Theme CreateTheme(Theme theme)
		{
			var newTheme = new Theme();

			newTheme.MergeTheme(theme);

			return newTheme;
		}
		
		public void MergeTheme(Theme theme)
		{
			if (theme != null)
			{	
				Name = theme.Name;

				if (theme.CellHeight != 0 && CellHeight != theme.CellHeight)
					CellHeight = theme.CellHeight;

				if (theme.Accessory != Accessory)
					Accessory = theme.Accessory;
				
//				if (theme.CellImageIcon != null)
//					CellImageIcon = theme.CellImageIcon;
//				
//				if (theme.CellImageIconUri != null)
//					CellImageIconUri = theme.CellImageIconUri;
				
				if (theme.CellBackgroundColor != null)
					CellBackgroundColor = theme.CellBackgroundColor;
				
				if (theme.CellBackgroundUri != null)
					CellBackgroundUri = theme.CellBackgroundUri;
				
				if (theme.CellBackgroundImage != null)
					CellBackgroundImage = theme.CellBackgroundImage;

				
				if (theme.TextFont != null)
					TextFont = theme.TextFont;
				
				if (theme.TextColor != null)
					TextColor = theme.TextColor;
				
				if (theme.TextShadowOffset != SizeF.Empty)
					TextShadowOffset = theme.TextShadowOffset;
				
				if (theme.TextShadowColor != null)
					TextShadowColor = theme.TextShadowColor;
				
				if (theme.TextHighlightColor != null)
					TextHighlightColor = theme.TextHighlightColor;
				
				TextAlignment = theme.TextAlignment;

				
				if (theme.DetailTextFont != null)
					DetailTextFont = theme.DetailTextFont;
				
				if (theme.DetailTextColor != null)
					DetailTextColor = theme.DetailTextColor;
				
				if (theme.DetailTextShadowOffset != SizeF.Empty)
					DetailTextShadowOffset = theme.DetailTextShadowOffset;
				
				if (theme.DetailTextShadowColor != null)
					DetailTextShadowColor = theme.DetailTextShadowColor;
				
				if (theme.DetailTextHighlightColor != null)
					DetailTextHighlightColor = theme.DetailTextHighlightColor;
				
				DetailTextAlignment = theme.DetailTextAlignment;

				
				if (theme.PlaceholderColor != null)
					PlaceholderColor = theme.PlaceholderColor;
				
				if (theme.PlaceholderAlignment != UITextAlignment.Right)
					PlaceholderAlignment = theme.PlaceholderAlignment;

				if (theme.SeparatorColor != null)
					SeparatorColor = theme.SeparatorColor;

				if (theme.SeparatorStyle.HasValue)
					SeparatorStyle = theme.SeparatorStyle;

				if (theme.TableViewStyle.HasValue)
					TableViewStyle = theme.TableViewStyle;				

				if (theme.BarStyle.HasValue)
					BarStyle = theme.BarStyle;

				if (theme.BarTintColor != null)
					BarTintColor = theme.BarTintColor;
		
				if (theme.BarImage != null)
					BarImage = theme.BarImage;
		
				if (theme.BarTranslucent)
					BarTranslucent = theme.BarTranslucent;

			
				HeaderTextAlignment = theme.HeaderTextAlignment;
				
				if (theme.HeaderTextFont != null)
					HeaderTextFont = theme.HeaderTextFont;
				
				if (theme.HeaderTextColor != null)
					HeaderTextColor = theme.HeaderTextColor;
				
				if (theme.HeaderTextShadowOffset != SizeF.Empty)
					HeaderTextShadowOffset = theme.HeaderTextShadowOffset;
				
				if (theme.HeaderTextShadowColor != null)
					HeaderTextShadowColor = theme.HeaderTextShadowColor;
				
				if (theme.HeaderBackgroundColor != null)
					HeaderBackgroundColor = theme.HeaderBackgroundColor;
				

				FooterTextAlignment = theme.FooterTextAlignment;
				
				if (theme.FooterTextFont != null)
					FooterTextFont = theme.FooterTextFont;
				
				if (theme.FooterTextColor != null)
					FooterTextColor = theme.FooterTextColor;
				
				if (theme.FooterTextShadowOffset != SizeF.Empty)
					FooterTextShadowOffset = theme.FooterTextShadowOffset;
				
				if (theme.FooterTextShadowColor != null)
					FooterTextShadowColor = theme.FooterTextShadowColor;
			
				if (theme.FooterBackgroundColor != null)
					FooterBackgroundColor = theme.FooterBackgroundColor;
			

				if (theme.DrawElementViewAction != null)
				{
					DrawElementViewAction = theme.DrawElementViewAction;
				}

				if (theme.BackgroundColor != null)
					BackgroundColor = theme.BackgroundColor;
				
				if (theme.BackgroundUri != null)
					BackgroundUri = theme.BackgroundUri;
				
				if (theme.BackgroundImage != null)
					BackgroundImage = theme.BackgroundImage;
			}
		}

		private void InitializePropertiesFromCell(UITableViewElementCell cell)
		{
			if (cell != null)
			{
				_Cell = cell;
 
				if (Cell.Accessory != UITableViewCellAccessory.None && Accessory == UITableViewCellAccessory.None)
					Accessory = Cell.Accessory;

				if (Cell.ImageView != null && CellImageIcon == null)
					CellImageIcon = Cell.ImageView.Image;
				
				if (cell.TextLabel != null)
				{
					if (cell.TextLabel.Font.PointSize > 0)
						TextFont = cell.TextLabel.Font;
	
					TextAlignment = cell.TextLabel.TextAlignment;
	
					TextColor = cell.TextLabel.TextColor;

					TextShadowOffset = cell.TextLabel.ShadowOffset;
	
					if (cell.TextLabel.ShadowColor != null)
						TextShadowColor = cell.TextLabel.ShadowColor;
				}
				
				if (cell.DetailTextLabel != null)
				{
					if (cell.DetailTextLabel.Font.PointSize > 0)
						DetailTextFont = cell.DetailTextLabel.Font;
					
					DetailTextAlignment = cell.DetailTextLabel.TextAlignment;
	
					DetailTextColor = cell.DetailTextLabel.TextColor;

					DetailTextShadowOffset = cell.DetailTextLabel.ShadowOffset;
					
					if (cell.DetailTextLabel.ShadowColor != null)
						DetailTextShadowColor = cell.DetailTextLabel.ShadowColor;
				}
				
				Cell.SetNeedsDisplay();
			}
		}

		public void ThemeChanged(UITableViewCell cell)
		{
			if (cell == null)
				return;

			if (cell.TextLabel != null)
			{
				if (TextFont != null)
					cell.TextLabel.Font = TextFont;
				
				cell.TextLabel.TextAlignment = TextAlignment;
				cell.TextLabel.TextColor = TextColor ?? cell.TextLabel.TextColor;
				
				if (TextShadowColor != null)
					cell.TextLabel.ShadowColor = TextShadowColor;
				
				if (TextShadowOffset != SizeF.Empty)
					cell.TextLabel.ShadowOffset = TextShadowOffset;
				
				if (TextHighlightColor != null)
					cell.TextLabel.HighlightedTextColor = TextHighlightColor;
			}
			
			if (cell.DetailTextLabel != null)
			{
				if (DetailTextFont != null)
					cell.DetailTextLabel.Font = DetailTextFont;
				
				cell.DetailTextLabel.TextAlignment = DetailTextAlignment;
				cell.DetailTextLabel.TextColor = DetailTextColor ?? cell.DetailTextLabel.TextColor;
				
				if (DetailTextShadowColor != null)
					cell.DetailTextLabel.ShadowColor = DetailTextShadowColor;
				
				if (DetailTextShadowOffset != SizeF.Empty)
					cell.DetailTextLabel.ShadowOffset = DetailTextShadowOffset;
				
				if (DetailTextHighlightColor != null)
					cell.DetailTextLabel.HighlightedTextColor = DetailTextHighlightColor;
			}
			
			var elementCell = cell as UITableViewElementCell;
			IImageUpdated element = null;
			if (elementCell != null)
				element = elementCell.Element as IImageUpdated;

			if (CellBackgroundColor != null)
			{
				cell.BackgroundColor = CellBackgroundColor;
			}
			else if (element != null && CellBackgroundUri != null)
			{
				var img = ImageLoader.DefaultRequestImage(CellBackgroundUri, element);
				cell.BackgroundColor = img != null ? UIColor.FromPatternImage(img) : UIColor.White;
			}
			else if (CellBackgroundImage != null)
			{
				cell.BackgroundColor = UIColor.FromPatternImage(BackgroundImage);
			} 
			else
			{
				cell.BackgroundColor = UIColor.White;
			}
			
			if (element != null && CellImageIconUri != null)
			{
				var img = ImageLoader.DefaultRequestImage(CellImageIconUri, element);
				
				if (img != null)
				{
					var small = img.Scale(new SizeF(32, 32));
					small = small.RemoveSharpEdges(5);
					
					cell.ImageView.Image = small;
					cell.ImageView.Layer.MasksToBounds = false;
					cell.ImageView.Layer.ShadowOffset = new SizeF(2, 2);
					cell.ImageView.Layer.ShadowRadius = 2f;
					cell.ImageView.Layer.ShadowOpacity = 0.8f;
				}
			}
			else if (CellImageIcon != null)
				cell.ImageView.Image = CellImageIcon;
			
			if (Accessory.HasValue)
				cell.Accessory = Accessory.Value;
			else
				cell.Accessory = UITableViewCellAccessory.None;
			
			cell.SetNeedsDisplay();
		}
		
		public static void ApplyRootTheme(object view, IThemeable element)
		{
			Theme theme = null;
			var themeAttributes = view.GetType().GetCustomAttributes(typeof(ThemeAttribute), true);
			
			foreach (ThemeAttribute themeAttribute in themeAttributes)
			{
				if (element != null)
				{
					if (themeAttribute != null && themeAttribute.ThemeType != null)
					{
						theme = Activator.CreateInstance(themeAttribute.ThemeType) as Theme;
						element.Theme.MergeTheme(theme);
					}
				}
			}
		}

		public static void ApplyElementTheme(Theme theme, IThemeable element, MemberInfo member)
		{
			if (theme != null)
			{
				var newTheme = Theme.CreateTheme(theme);
				newTheme.MergeTheme(element.Theme);
				
				element.Theme = newTheme;
			}
			
			if (member != null)
				ApplyMemberTheme(member, element);
			
			var root = element as IRoot;
			if (root != null)
			{
				foreach (var s in root.Sections)
					foreach (var e in s.Elements)
						ApplyElementTheme(element.Theme, e, null);
			}
			
			var section = element as ISection;
			if (section != null)
			{
				foreach (var e in section.Elements)
					ApplyElementTheme(element.Theme, e, null);
			}
		}

		private static void ApplyMemberTheme(MemberInfo member, IThemeable themeableElement)
		{
			var themeAttribute = member.GetCustomAttribute<ThemeAttribute>();
			
			if (themeableElement != null && themeAttribute != null && themeAttribute.ThemeType != null)
			{
				var theme = Activator.CreateInstance(themeAttribute.ThemeType) as Theme;
				
				if (themeAttribute.ThemeUsage == ThemeUsage.Merge)
				{
					themeableElement.Theme.MergeTheme(theme);
				} else
					themeableElement.Theme = Theme.CreateTheme(theme);
			}
		}

		public override string ToString()
		{
			return Name;
		}

	}
}

