//
// RootElementDataTemplate.cs
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
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class RootElementDataTemplate : ElementDataTemplate
	{		
		public BindableProperty PullToRefreshCommandProperty = BindableProperty.Register("PullToRefreshCommand");
		
		public BindableProperty ToolbarButtonsProperty = BindableProperty.Register("ToolbarButtons");
		public BindableProperty NavbarButtonsProperty = BindableProperty.Register("NavbarButtons");
		
		public BindableProperty EnableSearchProperty = BindableProperty.Register("EnableSearch");
		public BindableProperty IncrementalSearchProperty = BindableProperty.Register("IncrementalSearch");
		public BindableProperty SearchPlaceholderProperty = BindableProperty.Register("SearchPlaceholder");
		public BindableProperty SearchCommandProperty = BindableProperty.Register("SearchCommand");

		public RootElementDataTemplate(IElement element) : base(element)
		{
		}
 
		public override void BindProperties()
		{
			//base.BindProperties();
			
//			if (DetailTextLabel != null)
//				DataContextProperty.BindTo(DetailTextLabel, "Text");
	
			DataContextProperty.BindTo(Element);

			PullToRefreshCommandProperty.BindTo(Element);
			ToolbarButtonsProperty.BindTo(Element);
			NavbarButtonsProperty.BindTo(Element);

			EnableSearchProperty.BindTo(Element);
			IncrementalSearchProperty.BindTo(Element);
			SearchPlaceholderProperty.BindTo(Element);
			SearchCommandProperty.BindTo(Element);

		
			AccessoryProperty.BindTo(Element);
			ImageIconProperty.BindTo(Element);
			ImageIconUriProperty.BindTo(Element);
			BackgroundImageProperty.BindTo(Element);
			BackgroundUriProperty.BindTo(Element);
			BackgroundColorProperty.BindTo(Element);
		}
	}
}