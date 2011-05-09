//
// RootElement.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	/// <summary>
	///    RootElements are responsible for showing a full configuration page.
	/// </summary>
	/// <remarks>
	///    At least one RootElement is required to start the MonoTouch.Dialogs
	///    process.   
	/// 
	///    RootElements can also be used inside Sections to trigger
	///    loading a new nested configuration page.   When used in this mode
	///    the caption provided is used while rendered inside a section and
	///    is also used as the Title for the subpage.
	/// 
	///    If a RootElement is initialized with a section/element value then
	///    this value is used to locate a child Element that will provide
	///    a summary of the configuration which is rendered on the right-side
	///    of the display.
	/// 
	///    RootElements are also used to coordinate radio elements.  The
	///    RadioElement members can span multiple Sections (for example to
	///    implement something similar to the ring tone selector and separate
	///    custom ring tones from system ringtones).
	/// 
	///    Sections are added by calling the Add method which supports the
	///    C# 4.0 syntax to initialize a RootElement in one pass.
	/// </remarks>
	public partial class RootElement : StringElement, IEnumerable, IRoot, ISelectable, ISearchable, ISearchBar
	{		
		public bool EnableSearch { get; set; }
		public bool AutoHideSearch { get; set; }
		public bool IncrementalSearch { get; set; }
		public string SearchPlaceholder { get; set; }
		public SearchCommand SearchCommand { get; set; }
		
		public ICommand PullToRefreshCommand { get; set; }
		public string DefaultSettingsKey { get; set; }

		public DialogViewController Controller {get; set;}
		
		//public string NavbarImage { get; set; }

		public List<CommandBarButtonItem> ToolbarButtons { get; set; } 
		public List<CommandBarButtonItem> NavbarButtons { get; set; }

		private Func<IRoot, UIViewController> _ViewControllerFactory;

		public List<Group> Groups { get; set; }
		public bool UnevenRows { get; set; }

		[Preserve]
		public RootElement(): this(null)
		{
		}

		/// <summary>
		///  Initializes a RootSection with a caption
		/// </summary>
		/// <param name="caption">
		///  The caption to render.
		/// </param>
		public RootElement(string caption) : base(caption)
		{
			Sections = new List<ISection>();
			Groups = new List<Group>();
		}

		/// <summary>
		/// Initializes a RootSection with a caption and a callback that will
		/// create the nested UIViewController that is activated when the user
		/// taps on the element.
		/// </summary>
		/// <param name="caption">
		///  The caption to render.
		/// </param>
		public RootElement(string caption, Func<IRoot, UIViewController> viewControllerFactory) : this(caption)
		{
			this._ViewControllerFactory = viewControllerFactory;
			//Sections = new List<ISection>();
		}

		/// <summary>
		/// Initializes a RootElement that renders the summary based on the radio settings of the contained elements. 
		/// </summary>
		/// <param name="caption">
		/// The caption to ender
		/// </param>
		/// <param name="group">
		/// The group that contains the checkbox or radio information.  This is used to display
		/// the summary information when a RootElement is rendered inside a section.
		/// </param>
		public RootElement(string caption, Group group) : this(caption)
		{
			Groups.Add(group);
		}

		public List<ISection> Sections { get; set; }

		public NSIndexPath PathForRadio()
		{
			RadioGroup radio = Groups.FirstOrDefault() as RadioGroup;
			if (radio == null)
				return null;
			
			uint current = 0, section = 0;
			foreach (ISection s in Sections)
			{
				uint row = 0;
				
				foreach (var e in s.Elements)
				{
					if (!(e is RadioElement))
						continue;
					
					if (current == ItemIndex)
					{
						return NSIndexPath.Create(section, row);
					}
					row++;
					current++;
				}
				section++;
			}
			return null;
		}

		public int Count
		{
			get { return Sections.Count; }
		}

		public ISection this[int idx]
		{
			get { return Sections[idx]; }
		}

		public int IndexOf(ISection target)
		{
			int idx = 0;
			foreach (ISection s in Sections)
			{
				if (s == target)
					return idx;
				idx++;
			}
			return -1;
		}

		public void Prepare()
		{
			foreach (ISection s in Sections)
			{
				foreach (var e in s.Elements)
				{
					if (UnevenRows == false && e is ISizeable)
						UnevenRows = true;
				}
			}
		}

		/// <summary>
		/// Adds a new section to this RootElement
		/// </summary>
		/// <param name="section">
		/// The section to add, if the root is visible, the section is inserted with no animation
		/// </param>
		public void Add(ISection section)
		{
			if (section == null)
				return;

			Sections.Add(section);
			section.Parent = this;
			if (TableView == null)
				return;

			TableView.InsertSections(MakeIndexSet(Sections.Count - 1, 1), UITableViewRowAnimation.None);
		}

		//
		// This makes things LINQ friendly;  You can now create RootElements
		// with an embedded LINQ expression, like this:
		// new RootElement ("Title") {
		//     from x in names
		//         select new Section (x) { new StringElement ("Sample") }
		//
		public void Add(IEnumerable<ISection> sections)
		{
			foreach (var s in sections)
				Add(s);
		}

		private NSIndexSet MakeIndexSet(int start, int count)
		{
			NSRange range;
			range.Location = start;
			range.Length = count;
			return NSIndexSet.FromNSRange(range);
		}

		/// <summary>
		/// Inserts a new section into the RootElement
		/// </summary>
		/// <param name="idx">
		/// The index where the section is added <see cref="System.Int32"/>
		/// </param>
		/// <param name="anim">
		/// The <see cref="UITableViewRowAnimation"/> type.
		/// </param>
		/// <param name="newSections">
		/// A <see cref="Section[]"/> list of sections to insert
		/// </param>
		/// <remarks>
		///    This inserts the specified list of sections (a params argument) into the
		///    root using the specified animation.
		/// </remarks>
		public void Insert(int idx, UITableViewRowAnimation anim, params ISection[] newSections)
		{
			if (idx < 0 || idx > Sections.Count)
				return;
			if (newSections == null)
				return;
			
			if (TableView != null)
				TableView.BeginUpdates();
			
			int pos = idx;
			foreach (var s in newSections)
			{
				s.Parent = this;
				Sections.Insert(pos++, s);
			}
			
			if (TableView == null)
				return;
			
			TableView.InsertSections(MakeIndexSet(idx, newSections.Length), anim);
			TableView.EndUpdates();
		}

		/// <summary>
		/// Inserts a new section into the RootElement
		/// </summary>
		/// <param name="idx">
		/// The index where the section is added <see cref="System.Int32"/>
		/// </param>
		/// <param name="newSections">
		/// A <see cref="Section[]"/> list of sections to insert
		/// </param>
		/// <remarks>
		///    This inserts the specified list of sections (a params argument) into the
		///    root using the Fade animation.
		/// </remarks>
		public void Insert(int idx, ISection section)
		{
			Insert(idx, UITableViewRowAnimation.None, section);
		}

		/// <summary>
		/// Removes a section at a specified location
		/// </summary>
		public void RemoveAt(int idx)
		{
			RemoveAt(idx, UITableViewRowAnimation.Fade);
		}

		/// <summary>
		/// Removes a section at a specified location using the specified animation
		/// </summary>
		/// <param name="idx">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="anim">
		/// A <see cref="UITableViewRowAnimation"/>
		/// </param>
		public void RemoveAt(int idx, UITableViewRowAnimation anim)
		{
			if (idx < 0 || idx >= Sections.Count)
				return;
			
			Sections.RemoveAt(idx);
			
			if (TableView == null)
				return;
			
			TableView.DeleteSections(NSIndexSet.FromIndex(idx), anim);
		}

		public void Remove(ISection s)
		{
			if (s == null)
				return;
			int idx = Sections.IndexOf(s);
			if (idx == -1)
				return;
			RemoveAt(idx, UITableViewRowAnimation.Fade);
		}

		public void Remove(ISection s, UITableViewRowAnimation anim)
		{
			if (s == null)
				return;
			int idx = Sections.IndexOf(s);
			if (idx == -1)
				return;
			RemoveAt(idx, anim);
		}

		public void Clear()
		{
			foreach (var s in Sections)
				s.Dispose();
			Sections = new List<ISection>();
			if (TableView != null)
				TableView.ReloadData();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Sections == null)
					return;
				
				TableView = null;
				Clear();
				Sections = null;
			}
		}

		/// <summary>
		/// Enumerator that returns all the sections in the RootElement.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerator"/>
		/// </returns>
		public new IEnumerator GetEnumerator()
		{
			foreach (var s in Sections)
				yield return s;
		}

		/// <summary>
		/// The currently selected Radio item in the whole Root.
		/// </summary>
		public int ItemIndex
		{
			get {
				var radio = Groups.FirstOrDefault() as RadioGroup;
				if (radio != null)
					return radio.Selected;
				return -1;
			}
			set {
				var radio = Groups.FirstOrDefault() as RadioGroup;
				if (radio != null)
				{
					radio.Selected = value;
				}
			}
		}

		public override UITableViewElementCell NewCell()
		{
			var style = Theme.CellStyle;
			
			var cell = new UITableViewElementCell(style, Id, this);

			return cell;
		}

		public override void InitializeCell(UITableView tableView)
		{
			DetailTextAlignment = UITextAlignment.Right;
			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			TextAlignment = UITextAlignment.Left;

			base.InitializeCell(tableView);

			Cell.TextLabel.Text = Caption;
		}

		/// <summary>
		///    This method does nothing by default, but gives a chance to subclasses to
		///    customize the UIViewController before it is presented
		/// </summary>
		protected virtual void PrepareDialogViewController(UIViewController dvc)
		{
		}

		/// <summary>
		/// Creates the UIViewController that will be pushed by this RootElement
		/// </summary>
		protected virtual UIViewController MakeViewController(UITableViewStyle style)
		{
			if (_ViewControllerFactory != null)
				return _ViewControllerFactory(this);

			var dvc = new DialogViewController(style, this, true) { Autorotate = true };

			return dvc;
		}

		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			var root = BindingContext.CreateRootedView(this);
			
			if (root != null)
				((RootElement)root).ActivateController(dvc, tableView, path);
			else	
				ActivateController(dvc, tableView, path);
		}

		public void ActivateController(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{	
			var newDvc = MakeViewController(dvc.Style);
			PrepareDialogViewController(newDvc);
			dvc.ActivateController(newDvc, dvc);
			
			tableView.DeselectRow(path, false);
		}

		public void Reload(ISection section, UITableViewRowAnimation animation)
		{
			if (section == null)
				throw new ArgumentNullException("section");
			if (section.Parent == null || section.Parent != this)
				throw new ArgumentException("Section is not attached to this root");
			
			int idx = 0;
			foreach (var sect in Sections)
			{
				if (sect == section)
				{
					TableView.ReloadSections(new NSIndexSet((uint)idx), animation);
					return;
				}
				idx++;
			}
		}

		public void Reload(IElement element, UITableViewRowAnimation animation)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			if (element.Section == null)
				throw new ArgumentException("Element is not attached to this root");

			if (element.Root == null)
				throw new ArgumentException("Element is not attached to this root");

			var path = element.IndexPath;
			if (path == null)
				return;
			TableView.ReloadRows(new NSIndexPath[] { path }, animation);
		}

		protected override void OnValueChanged()
		{
			base.OnValueChanged();
			
			if (ContentView is IView && ContentView != null)
			{
				var binding = new BindingContext(ContentView, Caption, Theme);
				if (binding.Root != null)
				{
					Sections = binding.Root.Sections;
				}
			}
			if (Sections != null)
			{
				var radio = Groups.FirstOrDefault() as RadioGroup;
				var section = Sections.FirstOrDefault();
				if (radio != null && section != null && !section.IsMultiselect) 
				{
					var selected = EnumExtensions.GetValueFromString(radio.EnumType, (string)Value);
					radio.Selected = selected;
					foreach (RadioElement element in section)
					{
						element.UpdateSelected(element, false);
					}

					((RadioElement)section[selected]).Value = true;
				}
			}

			if (Groups != null && DetailTextLabel != null)
			{
				DetailTextLabel.Text = ToString();
			}
		}

		public override string ToString()
		{
			if (ContentView != null) 
			{
				return ContentView.ToString();
			}

			if (Value != null)
			{
				return Value.ToString();
			}

			return string.Empty;
		}

		protected int FromString(string value)
		{
			var element = Sections.FirstOrDefault().Elements.SingleOrDefault((e)=>e.ToString() == value);
			return Sections.FirstOrDefault().Elements.IndexOf(element);
		}
	}
}
