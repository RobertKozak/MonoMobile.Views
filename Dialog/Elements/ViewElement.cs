using System.Collections;
using System.Collections.Generic;
using MonoTouch.Foundation;
//
// ViewElement.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com)
//
// Copyright 2011, Nowcom Corporation
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
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;

	public class ViewElement: StringElement, IRoot
	{
		public object DataContext{ get; set; }
		public UITableViewCellStyle CellStyle { get; set; }
		public List<Section> Sections {	get; set; }
		public bool UnevenRows { get; set; }
		public UITableView TableView { get; set; }
		public Group Group { get; set; }
		public Func<IRoot, UIViewController> createOnSelected;


		public ViewElement(string caption) : base(caption)
		{
			CellStyle = UITableViewCellStyle.Value1;
			Sections= new List<Section>();
		}

		public override UITableViewElementCell NewCell()
		{
			var cell = new UITableViewElementCell(CellStyle, Id);
			cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
			
			return cell;
		}

		public override void InitializeCell (UITableView tableView)
		{
			base.InitializeCell(tableView);

			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			Cell.TextLabel.Text = Caption;
			Cell.DetailTextLabel.Text = ToString();
		}

		public void Add(Section section)
		{
			if (section == null)
				return;

			Sections.Add(section);
			section.Parent = this;
			if (TableView == null)
				return;

			TableView.InsertSections(MakeIndexSet(Sections.Count - 1, 1), UITableViewRowAnimation.None);
		}

		public void Add(IEnumerable<Section> sections)
		{
			foreach (var s in sections)
				Add(s);
		}

		public void Clear()
		{
			foreach (var s in Sections)
				s.Dispose();
			Sections = new List<Section>();
			if (TableView != null)
				TableView.ReloadData();
		}

		public int IndexOf(Section target)
		{
			int idx = 0;
			foreach (Section s in Sections)
			{
				if (s == target)
					return idx;
				idx++;
			}
			return -1;
		}

		public int Count
		{
			get { return Sections.Count; }
		}

		public void Prepare()
		{
			foreach (Section s in Sections)
			{
				foreach (Element e in s.Elements)
				{
					if (UnevenRows == false && e is IElementSizing)
						UnevenRows = true;
				}
			}
		}
		public override string ToString()
		{
			var value = string.Empty;
			var view = DataContext as IView;

			if (view != null)
				value = view.ToString();

			return value;
		}

		private NSIndexSet MakeIndexSet(int start, int count)
		{
			NSRange range;
			range.Location = start;
			range.Length = count;
			return NSIndexSet.FromNSRange(range);
		}

		protected virtual void PrepareDialogViewController(UIViewController dvc)
		{
		}

		protected virtual UIViewController MakeViewController()
		{
			if (createOnSelected != null)
				return createOnSelected(this);
			
			return new DialogViewController(this, true) { Autorotate = true };
		}

		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			tableView.DeselectRow(path, false);
			var newDvc = MakeViewController();
			PrepareDialogViewController(newDvc);
			dvc.ActivateController(newDvc, dvc);
		}
	}

//	public class BaseRootElement : Element<IView>, IEnumerable
//	{
//		int summarySection, summaryElement;
//		public bool UnevenRows;
//		public Func<BaseRootElement, UIViewController> createOnSelected;
//		internal UITableView TableView;
//		private UILabel DetailLabel;
//
//
//		public BaseRootElement(string caption) : base(caption)
//		{
//			summarySection = -1;
//			Sections = new List<Section>();
//		}
//
//		/// <summary>
//		/// Initializes a RootSection with a caption and a callback that will
//		/// create the nested UIViewController that is activated when the user
//		/// taps on the element.
//		/// </summary>
//		/// <param name="caption">
//		///  The caption to render.
//		/// </param>
//		public BaseRootElement(string caption, Func<BaseRootElement, UIViewController> createOnSelected) : base(caption)
//		{
//			summarySection = -1;
//			this.createOnSelected = createOnSelected;
//			Sections = new List<Section>();
//		}
//
//		/// <summary>
//		///   Initializes a RootElement with a caption with a summary fetched from the specified section and leement
//		/// </summary>
//		/// <param name="caption">
//		/// The caption to render cref="System.String"/>
//		/// </param>
//		/// <param name="section">
//		/// The section that contains the element with the summary.
//		/// </param>
//		/// <param name="element">
//		/// The element index inside the section that contains the summary for this RootSection.
//		/// </param>
//		public BaseRootElement(string caption, int section, int element) : base(caption)
//		{
//			summarySection = section;
//			summaryElement = element;
//		}
//
//		internal List<Section> Sections = new List<Section>();
//
//		public int Count
//		{
//			get { return Sections.Count; }
//		}
//
//		public Section this[int idx]
//		{
//			get { return Sections[idx]; }
//		}
//
//		internal int IndexOf(Section target)
//		{
//			int idx = 0;
//			foreach (Section s in Sections)
//			{
//				if (s == target)
//					return idx;
//				idx++;
//			}
//			return -1;
//		}
//
//		internal virtual void Prepare()
//		{
//			int current = 0;
//			foreach (Section s in Sections)
//			{
//				foreach (Element e in s.Elements)
//				{
//					if (UnevenRows == false && e is IElementSizing)
//						UnevenRows = true;
//				}
//			}
//		}
//
//		/// <summary>
//		/// Adds a new section to this RootElement
//		/// </summary>
//		/// <param name="section">
//		/// The section to add, if the root is visible, the section is inserted with no animation
//		/// </param>
//		public void Add(Section section)
//		{
//			if (section == null)
//				return;
//			
//			Sections.Add(section);
//			section.Parent = this;
//			if (TableView == null)
//				return;
//			
//			TableView.InsertSections(MakeIndexSet(Sections.Count - 1, 1), UITableViewRowAnimation.None);
//		}
//
//		//
//		// This makes things LINQ friendly;  You can now create RootElements
//		// with an embedded LINQ expression, like this:
//		// new RootElement ("Title") {
//		//     from x in names
//		//         select new Section (x) { new StringElement ("Sample") }
//		//
//		public void Add(IEnumerable<Section> sections)
//		{
//			foreach (var s in sections)
//				Add(s);
//		}
//
//		private NSIndexSet MakeIndexSet(int start, int count)
//		{
//			NSRange range;
//			range.Location = start;
//			range.Length = count;
//			return NSIndexSet.FromNSRange(range);
//		}
//
//		/// <summary>
//		/// Inserts a new section into the RootElement
//		/// </summary>
//		/// <param name="idx">
//		/// The index where the section is added <see cref="System.Int32"/>
//		/// </param>
//		/// <param name="anim">
//		/// The <see cref="UITableViewRowAnimation"/> type.
//		/// </param>
//		/// <param name="newSections">
//		/// A <see cref="Section[]"/> list of sections to insert
//		/// </param>
//		/// <remarks>
//		///    This inserts the specified list of sections (a params argument) into the
//		///    root using the specified animation.
//		/// </remarks>
//		public void Insert(int idx, UITableViewRowAnimation anim, params Section[] newSections)
//		{
//			if (idx < 0 || idx > Sections.Count)
//				return;
//			if (newSections == null)
//				return;
//
//			if (TableView != null)
//				TableView.BeginUpdates();
//			
//			int pos = idx;
//			foreach (var s in newSections)
//			{
//				s.Parent = this;
//				Sections.Insert(pos++, s);
//			}
//
//			if (TableView == null)
//				return;
//			
//			TableView.InsertSections(MakeIndexSet(idx, newSections.Length), anim);
//			TableView.EndUpdates();
//		}
//
//		/// <summary>
//		/// Inserts a new section into the RootElement
//		/// </summary>
//		/// <param name="idx">
//		/// The index where the section is added <see cref="System.Int32"/>
//		/// </param>
//		/// <param name="newSections">
//		/// A <see cref="Section[]"/> list of sections to insert
//		/// </param>
//		/// <remarks>
//		///    This inserts the specified list of sections (a params argument) into the
//		///    root using the Fade animation.
//		/// </remarks>
//		public void Insert(int idx, Section section)
//		{
//			Insert(idx, UITableViewRowAnimation.None, section);
//		}
//
//		/// <summary>
//		/// Removes a section at a specified location
//		/// </summary>
//		public void RemoveAt(int idx)
//		{
//			RemoveAt(idx, UITableViewRowAnimation.Fade);
//		}
//
//		/// <summary>
//		/// Removes a section at a specified location using the specified animation
//		/// </summary>
//		/// <param name="idx">
//		/// A <see cref="System.Int32"/>
//		/// </param>
//		/// <param name="anim">
//		/// A <see cref="UITableViewRowAnimation"/>
//		/// </param>
//		public void RemoveAt(int idx, UITableViewRowAnimation anim)
//		{
//			if (idx < 0 || idx >= Sections.Count)
//				return;
//			
//			Sections.RemoveAt(idx);
//			
//			if (TableView == null)
//				return;
//			
//			TableView.DeleteSections(NSIndexSet.FromIndex(idx), anim);
//		}
//
//		public void Remove(Section s)
//		{
//			if (s == null)
//				return;
//			int idx = Sections.IndexOf(s);
//			if (idx == -1)
//				return;
//			RemoveAt(idx, UITableViewRowAnimation.Fade);
//		}
//
//		public void Remove(Section s, UITableViewRowAnimation anim)
//		{
//			if (s == null)
//				return;
//			int idx = Sections.IndexOf(s);
//			if (idx == -1)
//				return;
//			RemoveAt(idx, anim);
//		}
//
//		public void Clear()
//		{
//			foreach (var s in Sections)
//				s.Dispose();
//			Sections = new List<Section>();
//			if (TableView != null)
//				TableView.ReloadData();
//		}
//
//		protected override void Dispose(bool disposing)
//		{
//			if (disposing)
//			{
//				if (Sections == null)
//					return;
//				
//				TableView = null;
//				Clear();
//				Sections = null;
//			}
//		}
//
//		/// <summary>
//		/// Enumerator that returns all the sections in the RootElement.
//		/// </summary>
//		/// <returns>
//		/// A <see cref="IEnumerator"/>
//		/// </returns>
//		public IEnumerator GetEnumerator()
//		{
//			foreach (var s in Sections)
//				yield return s;
//		}
//
//		public override UITableViewElementCell NewCell ()
//		{
//			var style = summarySection == -1 ? UITableViewCellStyle.Default : UITableViewCellStyle.Value1;
//			
//			var cell = new UITableViewElementCell(style, Id);
//			cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
//
//			return cell;
//		}
//
//		public override void InitializeCell(UITableView tableView)
//		{
//			Cell.TextLabel.Text = Caption;
//		    if (summarySection != -1 && summarySection < Sections.Count)
//			{
//				var s = Sections[summarySection];
//				if (summaryElement < s.Elements.Count)
//					Cell.DetailTextLabel.Text = s.Elements[summaryElement].ToString();
//			}
//
//			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//
//			DetailLabel = Cell.DetailTextLabel;
//
////			if (DetailLabel != null)
////				Value = FromString(DetailLabel.Text);
//		}
//
//		/// <summary>
//		///    This method does nothing by default, but gives a chance to subclasses to
//		///    customize the UIViewController before it is presented
//		/// </summary>
//		protected virtual void PrepareDialogViewController(UIViewController dvc)
//		{
//		}
//
//		/// <summary>
//		/// Creates the UIViewController that will be pushed by this RootElement
//		/// </summary>
//		protected virtual UIViewController MakeViewController()
//		{
//			if (createOnSelected != null)
//				return createOnSelected(this);
//			
//			return new DialogViewController(this as BaseRootElement, true) { Autorotate = true };
//		}
//
//		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
//		{
//			tableView.DeselectRow(path, false);
//			var newDvc = MakeViewController();
//			PrepareDialogViewController(newDvc);
//			dvc.ActivateController(newDvc, dvc);
//		}
//
//		public void Reload(Section section, UITableViewRowAnimation animation)
//		{
//			if (section == null)
//				throw new ArgumentNullException("section");
//			if (section.Parent == null || section.Parent != this)
//				throw new ArgumentException("Section is not attached to this root");
//			
//			int idx = 0;
//			foreach (var sect in Sections)
//			{
//				if (sect == section)
//				{
//					TableView.ReloadSections(new NSIndexSet((uint)idx), animation);
//					return;
//				}
//				idx++;
//			}
//		}
//
//		public void Reload(Element element, UITableViewRowAnimation animation)
//		{
//			if (element == null)
//				throw new ArgumentNullException("element");
//			var section = element.Parent as Section;
//			if (section == null)
//				throw new ArgumentException("Element is not attached to this root");
//			var root = section.Parent as RootElement;
//			if (root == null)
//				throw new ArgumentException("Element is not attached to this root");
//			var path = element.IndexPath;
//			if (path == null)
//				return;
//			TableView.ReloadRows(new NSIndexPath[] { path }, animation);
//		}
//
//		protected override void OnValueChanged()
//		{
//			base.OnValueChanged();
//			if (DetailLabel != null)
//				DetailLabel.Text = ToString();
//		}
//
//	}
}


