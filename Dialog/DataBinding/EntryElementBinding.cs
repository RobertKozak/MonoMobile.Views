//
// EntryElement.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
// Copyright 2011, Nowcom Corporation.ll, Inc.
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
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public partial class EntryElement : FocusableElement, IFocusable, ISearchable, ISelectable
	{
		private string __Text { get {return InputControl.Text; } set { InputControl.Text = value; } }

		private string __Placeholder { get {return InputControl.Placeholder; } set { InputControl.Placeholder = value; } }
		private bool __SecureTextEntry { get {return InputControl.SecureTextEntry; } set { InputControl.SecureTextEntry = value; } }
		private UIReturnKeyType __ReturnKeyboardType { get { return InputControl.ReturnKeyType; } set { InputControl.ReturnKeyType = value; } }
		private UIKeyboardType __KeyboardType { get { return InputControl.KeyboardType; } set { InputControl.KeyboardType = value; } }
		private UIFont __Font { get { return InputControl.Font; } set { InputControl.Font = value; } }
		private UITextAlignment __TextAlignment { get { return InputControl.TextAlignment; } set { InputControl.TextAlignment = value; } }

		public BindableProperty PlaceholderProperty = BindableProperty.Register("Placeholder");
		public BindableProperty IsPasswordProperty = BindableProperty.Register("IsPassword");
		public BindableProperty TextProperty = BindableProperty.Register("Text");
		public BindableProperty ReturnKeyTypeProperty = BindableProperty.Register("ReturnKeyType");
		public BindableProperty KeyboardTypeProperty = BindableProperty.Register("KeyboardType");
		public BindableProperty EditModeProperty = BindableProperty.Register("EditMode");

		public override void BindProperties()
		{
			base.BindProperties();
			
			EditModeProperty.BindTo(this, this, "EditMode");
		
			if (InputControl != null)
			{
				DataContextProperty.BindTo(this, InputControl, "Text");

				PlaceholderProperty.BindTo(this,  InputControl, "Placeholder");
				IsPasswordProperty.BindTo(this, InputControl, "SecureTextEntry");
				TextProperty.BindTo(this, InputControl, "Text");
				DetailTextAlignmentProperty.BindTo(this, InputControl, "TextAlignment");
				ReturnKeyTypeProperty.BindTo(this, InputControl,"ReturnKeyType");
				KeyboardTypeProperty.BindTo(this, InputControl, "KeyboardType");
				DetailTextFontProperty.BindTo(this, InputControl, "Font");
			}
		}
	}
}

