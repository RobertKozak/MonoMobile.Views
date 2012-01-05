// 
// StringExtensions.cs
// 
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
// Copyright 2011, Nowcom Corporation.
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
	using System.Text;
	using System.Linq;
	using MonoTouch.UIKit;

	public static class StringExtensions
	{
		public static string Capitalize(this string name)
		{
			if (string.IsNullOrEmpty(name))
				return string.Empty;

			var sb = new StringBuilder(name.Length);
			bool nextUp = true;
			
			foreach (char c in name)
			{
				if (nextUp)
				{
					sb.Append(Char.ToUpper(c));
					nextUp = false;
					
				} else
				{
					if (c == '_')
					{
						sb.Append(' ');
						continue;
					}
					if (Char.IsUpper(c))
						sb.Append(' ');
					sb.Append(c);
				}
			}
			
			return sb.ToString();
		}

		public static int NumberOfLines(this UIFont font, string caption, float width)
		{
			using (var label = new UILabel())
			{	
				var linefeeds = caption.Count(ch => ch == '\n');
				var size = label.StringSize(caption, font);
				
				label.Lines = 1 + ((int)(size.Width / width)) + linefeeds;
				if (size.Width % width == 0)
				{
					label.Lines--;
				}

				return label.Lines;
			}
		}

		public static UIColor ToUIColor(this string hexString)
		{
			var result = UIColor.Clear;
		
			if (!string.IsNullOrEmpty(hexString))
			{
				var colorString = hexString.Replace("#", "");
				byte alpha, red, blue, green;
				
				alpha = 255;

				switch (colorString.Length)
				{
					case 3: // #RGB
						{
							red = (Byte)Convert.ToInt16(colorString.Substring(0, 1), 16);
							green = (Byte)Convert.ToInt16(colorString.Substring(1, 1), 16);
							blue = (Byte)Convert.ToInt16(colorString.Substring(2, 1), 16);
							break;
						}
					case 4: // #ARGB
						{
							alpha = (Byte)Convert.ToInt16(colorString.Substring(0, 1), 16);
							red = (Byte)Convert.ToInt16(colorString.Substring(1, 1), 16);
							green = (Byte)Convert.ToInt16(colorString.Substring(2, 1), 16);
							blue = (Byte)Convert.ToInt16(colorString.Substring(3, 1), 16);
							break;
						}
					case 6: // #RRGGBB
						{
							red = (Byte)Convert.ToInt16(colorString.Substring(0, 2), 16);
							green = (Byte)Convert.ToInt16(colorString.Substring(2, 2), 16);
							blue = (Byte)Convert.ToInt16(colorString.Substring(4, 2), 16);
							break;
						}
					case 8: // #AARRGGBB
						{
							alpha = (Byte)Convert.ToInt16(colorString.Substring(0, 2), 16);
							red = (Byte)Convert.ToInt16(colorString.Substring(2, 2), 16);
							green = (Byte)Convert.ToInt16(colorString.Substring(4, 2), 16);
							blue = (Byte)Convert.ToInt16(colorString.Substring(6, 2), 16);
							break;
						}
					default:
						{
							throw new Exception(string.Format("Invalid color value \rColor value {0} is invalid. It should be a hex value of the form #RBG, #ARGB, #RRGGBB, or #AARRGGBB", hexString));
						}
				}
		
				result = UIColor.FromRGBA(red, green, blue, alpha);
			}

			return result;
		}

	}
}

