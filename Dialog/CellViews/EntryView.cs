// 
//  EntryView.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
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
	using System.Drawing;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class EntryView: FocusableView
	{	
		public UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }

		public string Placeholder { get; set; }
		public bool IsPassword { get; set; }
		public string Text { get; set; }
		public UIReturnKeyType ReturnKeyType { get; set; }
		public UIKeyboardType KeyboardType { get; set; }
		public UITextAutocorrectionType AutocorrectionType { get; set; }
		public UITextAutocapitalizationType AutocapitalizationType { get; set; }
		public bool ClearOnEdit { get; set; }

		public EntryView(RectangleF frame) : base(frame)
		{
			KeyboardType = UIKeyboardType.Default;
			EditModeValue = EditMode.WithCaption;

			InputView = new UIPlaceholderTextField(new RectangleF(5, 0, frame.Width - 10, frame.Height)) 
			{ 
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
	
				BackgroundColor = UIColor.Clear, 
		//PlaceholderColor = Theme.PlaceholderColor, 
		//PlaceholderAlignment = Theme.PlaceholderAlignment,
				PlaceholderAlignment = UITextAlignment.Right,
				VerticalAlignment = UIControlContentVerticalAlignment.Center,
				AutocorrectionType = AutocorrectionType,
				AutocapitalizationType = AutocapitalizationType,
				Tag = 1 
			};
		
			InputView.Started += (s, e) =>
			{
				if (ClearOnEdit)
					InputView.Text = string.Empty;

				//DataContextProperty.ConvertBack<string>();				
			};
			
			InputView.ShouldReturn = delegate
			{
				InputView.ResignFirstResponder();

				return true;
			};
				
			InputView.EditingDidEnd += delegate 
			{
				DataContext.Value = InputView.Text;
			};
							
			InputView.EditingDidEndOnExit += delegate {
				InputView.ResignFirstResponder();
			};
			Add(InputView);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && InputView != null)
			{
				InputView.Dispose();
				InputView = null;
			}
			
			base.Dispose(disposing);
		}
		
		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			InputView.TableView = Controller.TableView;
			
			base.UpdateCell(cell, indexPath);

			ShowCaption = EditModeValue != EditMode.NoCaption;
			cell.TextLabel.Text = string.Empty;

			if (ShowCaption)
			{
				cell.TextLabel.Text = Caption;
			}
			
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			InputView.Text = string.Empty;

			SetKeyboard();

			IsPassword = false;

			var entryAttribute = DataContext.Member.GetCustomAttribute<EntryAttribute>();
			if (entryAttribute != null)
			{
				AutocapitalizationType = entryAttribute.AutocapitalizationType;
				AutocorrectionType = entryAttribute.AutocorrectionType;
				EditModeValue = entryAttribute.EditMode;
				KeyboardType = entryAttribute.KeyboardType;
				Placeholder = entryAttribute.Placeholder;
				ClearOnEdit = entryAttribute.ClearOnEdit;
			}

			var passwordAttribute = DataContext.Member.GetCustomAttribute<PasswordAttribute>();
			if (passwordAttribute != null)
			{
				AutocapitalizationType = passwordAttribute.AutocapitalizationType;
				AutocorrectionType = passwordAttribute.AutocorrectionType;
				EditModeValue = passwordAttribute.EditMode;
				KeyboardType = passwordAttribute.KeyboardType;
				Placeholder = passwordAttribute.Placeholder;
				ClearOnEdit = passwordAttribute.ClearOnEdit;

				IsPassword = true;
			}
			
			var readOnlyAttribute = DataContext.Member.GetCustomAttribute<ReadOnlyAttribute>();
			if (readOnlyAttribute != null)
			{
				EditModeValue = EditMode.ReadOnly;
			}

			if (EditModeValue != EditMode.ReadOnly)
			{
				cell.DetailTextLabel.Text = string.Empty;
				cell.DetailTextLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
				cell.TextLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			}
			
			if (EditModeValue == EditMode.NoCaption)
			{
				InputView.Placeholder = Caption;
			}
			else
			{
				InputView.Placeholder = Placeholder;
			}
				
			InputView.SecureTextEntry = IsPassword;
			//	InputView.Font = Theme.DetailTextFont;
			InputView.KeyboardType = KeyboardType;
			//	InputView.TextAlignment = Theme.DetailTextAlignment;
			InputView.TextAlignment = UITextAlignment.Right;
			InputView.ReturnKeyType = ReturnKeyType;

			//	if (Theme.DetailTextColor != null)
			//		InputView.TextColor = Theme.DetailTextColor;

			InputView.InputAccessoryView = new UIKeyboardToolbar(this);

			InputView.ReturnKeyType = UIReturnKeyType.Default;

			if (DataContext.Value != null)
			{
				InputView.Text = DataContext.Value.ToString();
			}
		}

		public override void HandleNotifyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			InputView.Text = (string)DataContext.Value;
		}

		protected override void SetDataContext(MemberData value)
		{
			base.SetDataContext(value);

			var propertyInfo = DataContext.Member as PropertyInfo;
			if (propertyInfo != null)
			{
				EditModeValue = !propertyInfo.CanWrite ? EditMode.ReadOnly : EditModeValue;
			}

			SetKeyboard();
		}
		
		private void SetKeyboard()
		{
			KeyboardType = UIKeyboardType.Default;

			var typeCode = Convert.GetTypeCode(DataContext.Value);
			switch (typeCode)
			{
				case TypeCode.Decimal : KeyboardType = UIKeyboardType.DecimalPad; break;
				case TypeCode.Double : KeyboardType = UIKeyboardType.DecimalPad; break;
				case TypeCode.Single : KeyboardType = UIKeyboardType.DecimalPad; break;

				case TypeCode.Int16 : KeyboardType = UIKeyboardType.NumberPad; break;
				case TypeCode.Int32 : KeyboardType = UIKeyboardType.NumberPad; break;
				case TypeCode.Int64 : KeyboardType = UIKeyboardType.NumberPad; break;
				case TypeCode.UInt16 : KeyboardType = UIKeyboardType.NumberPad; break;
				case TypeCode.UInt32 : KeyboardType = UIKeyboardType.NumberPad; break;
				case TypeCode.UInt64 : KeyboardType = UIKeyboardType.NumberPad; break;
				case TypeCode.DateTime : KeyboardType = UIKeyboardType.NumbersAndPunctuation; break;	
			}
		}
	}
}

