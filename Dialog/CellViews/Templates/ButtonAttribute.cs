//
// ButtonAttribute.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corportation
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
	using System.Drawing;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	public enum CommandOption
	{
		Hide,
		Disable
	}
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, Inherited = false)]
	public class ButtonAttribute : CellViewTemplate
	{
		public override Type CellViewType { get { return typeof(ButtonCellView); } }
		
		public ButtonAttribute()
		{
		}

		public ButtonAttribute(string commandMemberName)
		{
			CommandOption = CommandOption.Hide;
			CommandMemberName = commandMemberName;
		}
		
		public string CommandMemberName;
		public string CanExecutePropertyName;
		public CommandOption CommandOption;
		public ICommand Command;
		public object CommandParameter;
	}

	[Preserve(AllMembers = true)]
	public class ButtonCellView : CellView<object>, ISelectable, ICommandButton
	{
		private ButtonAttribute ButtonData { get { return CellViewTemplate as ButtonAttribute; } }
		
		public string CommandMemberName 
		{ 
			get { return ButtonData != null ? ButtonData.CommandMemberName : string.Empty; } 
			set { if (ButtonData != null) ButtonData.CommandMemberName = value; } 
		}
		
		public ICommand Command 
		{ 
			get { return ButtonData != null ? ButtonData.Command : null; } 
			set { if (ButtonData != null) ButtonData.Command = value; } 
		}
		
		public object CommandParameter 
		{ 
			get { return ButtonData != null ? ButtonData.CommandParameter : null; } 
			set { if (ButtonData != null) ButtonData.CommandParameter = value; } 
		}
 
		public bool Enabled { get; set; }

		public ButtonCellView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.Accessory = UITableViewCellAccessory.None;

			cell.TextLabel.Text = Caption;
			cell.TextLabel.TextAlignment = UITextAlignment.Center;
		}

		public virtual void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{
			if (ButtonData != null)
			{
				if (ButtonData.Command != null)
				{
					ButtonData.Command.Execute(ButtonData.CommandParameter);
				}
				else
				{
					if (DataContext != null)
					{
						var method = DataContext.Member as MethodInfo;
						method.Invoke(DataContext.Source, null);
					}
				}
			}

			tableView.DeselectRow(indexPath, true);
		}
	}
}

