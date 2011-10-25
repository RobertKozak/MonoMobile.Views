//
// CheckboxView.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Based on code from MonoTouch.Dialog by Miguel de Icaza (miguel@gnome.org)
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
	using System.Linq;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using MonoMobile.Views;
	
	[Preserve(AllMembers = true)]
	public class CheckboxElement : BoolElement, ISelectable
	{		
		public string Group { get; set; }

		public CheckboxElement(string caption) : base(caption)
		{
			DataBinding = new CheckboxElementDataBinding(this);
		}
		
		public override void UpdateCell()
		{
			base.UpdateCell();
			//There is a visual artifact when using Checkmark and UITableViewCellSelectionStyle.Blue;
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			//DataContext = (bool)DataContextProperty.Value;
			DataBinding.UpdateDataContext();

			UpdateSelected();
		}
		
		public override void UpdateSelected()
		{
			Cell.AccessoryView = null;
			Cell.Accessory = (bool)DataContext ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			Cell.TextLabel.Text = Caption;

			var containsItem = false;
			if (Container != null)
			{
				containsItem = Container.SelectedItems.Contains(Item);

				if ((bool)DataContext && !containsItem)
				{
					Container.SelectedItems.Add(Item);
					Container.SelectedItem = Item;
					((IContainer)Root).SelectedItems.Add(Item);
					((IContainer)Root).SelectedItem = Item;
				}
				
				if (!(bool)DataContext && containsItem)
				{
					Container.SelectedItems.Remove(Item);
					((IContainer)Root).SelectedItems.Remove(Item);
				}
			
				((IContainer)Root).IsMultiselect = Container.IsMultiselect;

				var enumBinder = Item as EnumItem;
				if (enumBinder != null)
				{
					enumBinder.IsChecked = (bool)DataContext;
					Root.DataContext = Container.SelectedItems.Count.ToString();
				}
			
				var property = BindableProperty.GetBindableProperty(Container, "SelectedItemProperty");
				if (property != null)
					property.Update();

				property = BindableProperty.GetBindableProperty(Container, "SelectedItemsProperty");
				if (property != null)
					property.Update();

				property = BindableProperty.GetBindableProperty(Root, "DataContextProperty");
				if (property != null)
					property.Update();

				property = BindableProperty.GetBindableProperty(Container, "IndexProperty");
				if (property != null)
					property.Update();
			}
		}

		public override string ToString()
		{
			return Caption;
		}
	
		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			DataContext = !(bool)DataContext;
			DataBinding.UpdateDataContext();
			UpdateSelected();
		}

	}
}

