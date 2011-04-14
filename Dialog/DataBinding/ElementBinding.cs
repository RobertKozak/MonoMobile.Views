// 
//  ElementBinding.cs
// 
//  Author:
//  Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
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
#if DATABINDING
namespace MonoMobile.MVVM
{
	public abstract partial class Element
	{
		public BindableProperty AccessoryProperty = BindableProperty.Register("Accessory");
		public BindableProperty ImageIconProperty = BindableProperty.Register("ImageIcon");
		public BindableProperty ImageIconUriProperty = BindableProperty.Register("ImageIconUri");
		public BindableProperty BackgroundImageProperty = BindableProperty.Register("BackgroundImage");
		public BindableProperty BackgroundUriProperty = BindableProperty.Register("BackgroundImageUri");
		public BindableProperty BackgroundColorProperty = BindableProperty.Register("BackgroundColor");

		public BindableProperty TextLabelProperty = BindableProperty.Register("TextLabel");
		public BindableProperty TextFontProperty = BindableProperty.Register("TextFont");
		public BindableProperty TextColorProperty = BindableProperty.Register("TextColor");
		public BindableProperty TextAlignmentProperty = BindableProperty.Register("TextAlignment");
		public BindableProperty TextShadowOffsetProperty = BindableProperty.Register("TextShadowOffset");
		public BindableProperty TextShadowColorProperty = BindableProperty.Register("TextShadowColor");

		public BindableProperty DetailTextLabelProperty = BindableProperty.Register("DetailTextLabel");
		public BindableProperty DetailTextFontProperty = BindableProperty.Register("DetailTextFont");
		public BindableProperty DetailTextColorProperty = BindableProperty.Register("DetailTextColor");
		public BindableProperty DetailTextAlignmentProperty = BindableProperty.Register("DetailTextAlignment");
		public BindableProperty DetailTextShadowOffsetProperty = BindableProperty.Register("DetailTextShadowOffset");
		public BindableProperty DetailTextShadowColorProperty = BindableProperty.Register("DetailTextShadowColor");

		public virtual void BindProperties()
		{		
		}

		protected virtual void UpdateTargets()
		{
			var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(this);
			if (bindingExpressions != null)
			{
				foreach (var bindingExpression in bindingExpressions)
				{
					bindingExpression.UpdateTarget();
				}
			}
		}
	}
}
#endif

