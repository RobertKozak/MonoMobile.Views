// 
// UICustomTextField.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Drawing;
	using System.Linq;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
//	[Preserve(AllMembers = true)]
//	public class UICustomTextField : UIView
//	{
//		private UIPlaceholderTextField _TextField;
//
//		private int _Lines;
//		private UIColor _PlaceholderColor;
//		private UITextAlignment _PlaceholderAlignment;
//		private string _Placeholder;
//		private EditMode _EditMode;
//		private bool _SecureTextEntry;
//		private string _Text;
//		private UIReturnKeyType _ReturnKeyType;
//		private UIKeyboardType _KeyboardType;
//		private UITextAutocorrectionType _AutocorrectionType;
//		private UITextAutocapitalizationType _AutocapitalizationType;
//		private	UIColor _TextColor;
//		private UIFont _Font;
//		private UITextAlignment _TextAlignment;
//		private UIView _InputView;
//		private UIView _InputAccessoryView;
//		
//		public IDataTemplate DataTemplate { get; set; }
//
//		public UIColor PlaceholderColor
//		{
//			get { return _PlaceholderColor; }
//			set
//			{
//				if (_PlaceholderColor != value)
//				{
//					_PlaceholderColor = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UITextAlignment PlaceholderAlignment
//		{
//			get { return _PlaceholderAlignment; }
//			set
//			{
//				if (_PlaceholderAlignment != value)
//				{
//					_PlaceholderAlignment = value;
//					RecreateTextControl();
//				}
//			}
//		}
//		
//		public string Placeholder
//		{
//			get { return _Placeholder; }
//			set
//			{
//				if (_Placeholder != value)
//				{
//					_Placeholder = value;
//					RecreateTextControl();
//				}
//			}
//		}
//		
//		public EditMode EditMode 
//		{
//			get { return _EditMode; }
//			set
//			{
//				if (_EditMode != value)
//				{
//					_EditMode = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public bool SecureTextEntry 
//		{
//			get { return _SecureTextEntry; }
//			set
//			{
//				if (_SecureTextEntry != value)
//				{
//					_SecureTextEntry = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public string Text 
//		{
//			get { return _Text; }
//			set
//			{
//				if (_Text != value)
//				{
//					_Text = value;
//					
//					if (_TextField != null)
//					{
//						_TextField.Text = _Text;
//					}
//				}
//			}
//		}
//		
//		public UIReturnKeyType ReturnKeyType
//		{
//			get { return _ReturnKeyType; }
//			set
//			{
//				if (_ReturnKeyType != value)
//				{
//					_ReturnKeyType = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UIKeyboardType KeyboardType 
//		{
//			get { return _KeyboardType; }
//			set
//			{
//				if (_KeyboardType != value)
//				{
//					_KeyboardType = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UITextAutocorrectionType AutocorrectionType
//		{
//			get { return _AutocorrectionType; }
//			set
//			{
//				if (_AutocorrectionType != value)
//				{
//					_AutocorrectionType = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UITextAutocapitalizationType AutocapitalizationType
//		{
//			get { return _AutocapitalizationType; }
//			set
//			{
//				if (_AutocapitalizationType != value)
//				{
//					_AutocapitalizationType = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UIColor TextColor
//		{
//			get { return _TextColor; }
//			set
//			{
//				if (_TextColor != value)
//				{
//					_TextColor = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UIFont Font 
//		{
//			get { return _Font; }
//			set
//			{
//				if (_Font != value)
//				{
//					_Font = value;
//					RecreateTextControl();
//				}
//			}
//		}
//		
//		public UITextAlignment TextAlignment
//		{
//			get { return _TextAlignment; }
//			set
//			{
//				if (_TextAlignment != value)
//				{
//					_TextAlignment = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public int Lines
//		{
//			get { return _Lines; }
//			set
//			{
//				if (_Lines != value)
//				{
//					_Lines = value;
//					RecreateTextControl();
//				}
//			}
//		}
//		
//		public new UIView InputView
//		{
//			get { return _InputView; }
//			set
//			{
//				if (_InputView != value)
//				{
//					_InputView = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public new UIView InputAccessoryView
//		{
//			get { return _InputAccessoryView; }
//			set
//			{
//				if (_InputAccessoryView != value)
//				{
//					_InputAccessoryView = value;
//					RecreateTextControl();
//				}
//			}
//		}
//
//		public UICustomTextField(): base() { RecreateTextControl(); }
//		public UICustomTextField(NSCoder coder): base(coder) { RecreateTextControl(); }
//		public UICustomTextField(NSObjectFlag t): base(t) { RecreateTextControl(); }
//		public UICustomTextField(IntPtr handle): base (handle) { RecreateTextControl(); }
//		public UICustomTextField(RectangleF frame): base(frame) { RecreateTextControl(); }
//		
//		public override void LayoutSubviews()
//		{
//			base.LayoutSubviews();
//
//			if (_TextField != null)
//			{
//				_TextField.Frame = Bounds;
//			}
//		}
//
//		protected void RecreateTextControl()
//		{
//			if (_TextField != null)
//			{
//				_TextField.RemoveFromSuperview();
//				_TextField.Dispose();
//				_TextField = null;
//			}
//			
//			_TextField = new UIPlaceholderTextField(Bounds)
//			{
//				BackgroundColor = BackgroundColor, 
//				AutocorrectionType = AutocorrectionType,
//				AutocapitalizationType = AutocapitalizationType, 
//				SecureTextEntry = SecureTextEntry, 
//				KeyboardType = KeyboardType, 
//				TextAlignment = TextAlignment,
//				ReturnKeyType = ReturnKeyType,
//				Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize),
//				Tag = 1,
//			};
//
//			if (TextColor != null)
//				_TextField.TextColor = TextColor;
//			
//			if (Font != null)
//				_TextField.Font = Font;
//		
//			if (InputView != null)
//				_TextField.InputView = InputView;
//			
//			if (InputAccessoryView != null)
//				_TextField.InputAccessoryView = InputAccessoryView;
//			
//
//			_TextField.Ended += delegate 
//			{
//				DataTemplate.UpdateDataContext(_TextField.Text);
//			};
//
//			Add(_TextField);
//		}
//
//		public override bool ResignFirstResponder()
//		{
//			if (_TextField != null)
//				_TextField.ResignFirstResponder();
//
//			return base.ResignFirstResponder();
//		}
//		
//		public override bool BecomeFirstResponder()
//		{
//			if (_TextField != null)
//				_TextField.BecomeFirstResponder();
//
//			return base.BecomeFirstResponder();
//		}
//	}

	public class UIPlaceholderTextField: UITextField
	{
		public UIColor PlaceholderColor { get; set; }
		public UITextAlignment PlaceholderAlignment { get; set; }

		public UIPlaceholderTextField(): base() { Initialize(); }
		public UIPlaceholderTextField(NSCoder coder): base(coder) {}
		public UIPlaceholderTextField(NSObjectFlag t): base(t) {}
		public UIPlaceholderTextField(IntPtr handle): base (handle) {}
		public UIPlaceholderTextField(RectangleF frame): base(frame) { Initialize(); }
		
		protected void Initialize()
		{
			Delegate = new UIPlaceholderTextFieldDelegate();
		}

		public override void DrawPlaceholder(RectangleF rect)
		{
			if (PlaceholderColor == null)
				PlaceholderColor = UIColor.FromWhiteAlpha(0.7f, 1.0f);

			PlaceholderColor.SetColor();

			DrawString(Placeholder, rect, UIFont.SystemFontOfSize(UIFont.LabelFontSize), UILineBreakMode.Clip, PlaceholderAlignment);
		}

		private class UIPlaceholderTextFieldDelegate : UITextFieldDelegate 
		{  
//			public override bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString) 
//			{  
//				var newText = string.Concat(textField.Text, replacementString);
//				var cell = textField.Superview.Superview as UITableViewElementCell;
//				var element = cell.Element as Element;
//				
//				var constrainedSize = new SizeF(textField.Bounds.Width, 9999);
//				var newSize = element.DetailTextLabel.StringSize(newText, element.DetailTextLabel.Font, constrainedSize , UILineBreakMode.WordWrap);
//
//Console.WriteLine("Width = "+newSize.Width);
//Console.WriteLine("Height = "+newSize.Height);
//
//				if (newSize.Height + 23 > element.RowHeight) 
//					element.RowHeight = newSize.Height + 23;
//				
//				var template = ((Element)element).DataTemplate;
//
//				template.UpdateSources();
//				return true;  
//			}
		}  
	} 
}
