//
// EntryElement.cs
//
// Author:
//  Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.ll, Inc.
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
#define DATABINDING
namespace MonoMobile.MVVM
{
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public partial class EntryElement : StringElement, IFocusable, ISearchable, ISelectable
	{
		private string __Text { get {return Entry.Text; } set { Entry.Text = value; } }
#if DATABINDING
		private string __Placeholder { get {return Entry.Placeholder; } set { Entry.Placeholder = value; } }
		private bool __SecureTextEntry { get {return Entry.SecureTextEntry; } set { Entry.SecureTextEntry = value; } }
		private UIReturnKeyType __ReturnKeyboardType { get { return Entry.ReturnKeyType; } set { Entry.ReturnKeyType = value; } }
		private UIKeyboardType __KeyboardType { get { return Entry.KeyboardType; } set { Entry.KeyboardType = value; } }
		private UIFont __Font { get { return Entry.Font; } set { Entry.Font = value; } }
		
		public BindableProperty PlaceholderProperty = BindableProperty.Register("Placeholder");
		public BindableProperty IsPasswordProperty = BindableProperty.Register("IsPassword");
		public BindableProperty TextProperty = BindableProperty.Register("Text");
		public BindableProperty ReturnKeyTypeProperty = BindableProperty.Register("ReturnKeyType");
		public BindableProperty KeyboardTypeProperty = BindableProperty.Register("KeyboardType");
		public BindableProperty EditModeProperty = BindableProperty.Register("EditMode");
#endif
		public override void BindProperties()
		{
#if DATABINDING
			base.BindProperties();
			
			EditModeProperty.BindTo(this, () => EditMode);
#endif			
			if (Entry != null)
			{
				ValueProperty.BindTo(this, () => Entry.Text);

#if DATABINDING
				DetailTextFontProperty.BindTo(this, ()=>Entry.Font);
				PlaceholderProperty.BindTo(this, () => Entry.Placeholder);
				IsPasswordProperty.BindTo(this, () => Entry.SecureTextEntry);
				TextProperty.BindTo(this, () => Entry.Text);
				DetailTextAlignmentProperty.BindTo(this, () => Entry.TextAlignment);
				ReturnKeyTypeProperty.BindTo(this, () => Entry.ReturnKeyType);
				KeyboardTypeProperty.BindTo(this, () => Entry.KeyboardType);
				DetailTextFontProperty.BindTo(this, () => Entry.Font);
#endif
			}
		}
	}
}

