//
// RadioElement.cs
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
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
namespace MonoTouch.Dialog
{
	using System;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class RadioElement : BooleanElement
	{
		internal bool PopOnSelect;

		public RadioElement(string caption, bool popOnSelect) : base(caption)
		{
			PopOnSelect = popOnSelect;
		}

		public RadioElement(string caption) : base(caption)
		{
		}

		public override void InitializeCell(UITableView tableView)
		{
			var root = (IRoot)Parent.Parent;
			bool selected = false;
			var radioGroup = root.Group as RadioGroup;

			if (radioGroup == null)
				throw new Exception ("The IRoot's Group is null or is not a RadioGroup");
			else
				selected = Index == radioGroup.Selected;

			Cell.Accessory = selected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			Cell.TextLabel.Text = Caption;
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			if (Parent != null)
			{
				var root = (IRoot)Parent.Parent;
				if (root != null)
				{
					var section = root.Sections[indexPath.Section];
					foreach (var e in section.Elements)
						if (e is RadioElement)
							((RadioElement)e).Value = false;
				}
			}

			Value = true;

			base.Selected(dvc, tableView, indexPath);
			
			if (PopOnSelect)
				dvc.NavigationController.PopViewControllerAnimated(true);
		}

		protected override void OnValueChanged()
		{
			if (Cell != null)
				Cell.Accessory = Value ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			if (Parent != null)
			{
				var root = (IRoot)Parent.Parent;
				if(root != null)
				{
					var radioGroup = root.Group as RadioGroup;
					if (radioGroup != null && Value)
					{
						radioGroup.Selected = Index;
					}
				}
			}

			base.OnValueChanged();
		}

		public override string ToString()
		{
			return Caption;
		}
	}
}

