// 
// ListAttribute.cs
// 
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
// Copyright 2011 - 2012, Nowcom Corporation.
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
	
	public enum DisplayMode
	{
		List,
		RootCell,
		Collapsable,
	}

	public enum SelectionAction
	{
		NavigateToView,
		PopOnSelection,
		Selection,
		Multiselection,
		Custom,
		None
	} 

	public enum UnselectionBehavior
	{
		SetSelectedToCurrentValue,
		SetSelectedToPreviousValueOrNull,
		SetSelectedToNull
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class ListAttribute : Attribute
	{
		public ListAttribute()
		{
		}
		
		public ListAttribute(DisplayMode displayMode)
		{
			DisplayMode = displayMode;
		}

		public string SelectedItemMemberName { get; set; }
		public string SelectedItemsMemberName { get; set; }
		public Type SelectedAccessoryViewType { get; set; }		
		public Type UnselectedAccessoryViewType { get; set; }
		public DisplayMode DisplayMode { get; set; }
		public SelectionAction SelectionAction { get; set; }
		public UnselectionBehavior UnselectionBehavior { get; set; }
		public bool ReplaceCaptionWithSelection { get; set; }
	}
}
