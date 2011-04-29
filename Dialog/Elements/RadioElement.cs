//
// RadioView.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Based on cdoe from MonoTouch.Dialog by Miguel de Icaza (miguel@gnome.org)
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
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using System.Linq;

	public class RadioElement : BoolElement, ISelectable
	{
		public bool PopOnSelect { get; set; }
		
		public RadioElement() : this("")
		{
		}

		public RadioElement(string caption): base(caption)
		{
		}
		
		public override void InitializeContent()
		{
			UpdateSelected();
		}

		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			if (Parent != null)
			{
				var root = (IRoot)Parent.Parent;
				if (root != null)
				{
					var radioGroup = root.Groups.FirstOrDefault() as RadioGroup;

					var section = root.Sections[indexPath.Section];
					var index = 0;
					foreach (var e in section.Elements)
					{
						var radioView = e as RadioElement;
						UpdateSelected(radioView, this == radioView);

						if (this == radioView && radioGroup != null)
						{
							radioGroup.Selected = index;
						}

						index++;
					}
					
					root.Value = this.Caption;

					var property = BindableProperty.GetBindableProperty(root, "ValueProperty");
					property.Update();

					property = BindableProperty.GetBindableProperty(root, "ItemIndexProperty");
				//	property.Update();
					
					if (radioGroup != null)
						root.ItemIndex = radioGroup.Selected;
				}
			}
			
			Value = true;
			UpdateSelected();
			
			if (PopOnSelect)
				dvc.NavigationController.PopViewControllerAnimated(true);
		}

		public void UpdateSelected(RadioElement element, bool selected)
		{
			if (element != null)
			{
				element.Value = selected;
				element.UpdateSelected();
			}
		}

		public override string ToString()
		{
			return Caption;
		}
	}
}

