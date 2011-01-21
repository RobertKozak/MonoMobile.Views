//
// Section.cs
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public interface ISection : IDisposable
	{
		List<Element> Elements { get; set; }
		string Caption { get; set; }
		int Order { get; set; }
		Element Parent { get; set; }
		bool IsMultiselect { get; set; }

		SizeF EntryAlignment { get; set; }
		string Header { get; set; }
		string Footer { get; set; }
		UIView HeaderView { get; set; }
		UIView FooterView { get; set; }

		void Add(Element element );
		int Add(IEnumerable<Element> elements);
		void Add(UIView view);
		void Add(IEnumerable<UIView> views);

		void Insert(int idx, UITableViewRowAnimation anim, params Element[] newElements);

		int Insert(int idx, UITableViewRowAnimation anim, IEnumerable<Element> newElements);
		void Insert(int index, params Element[] newElements);

		void Remove(Element e);
		void Remove(int idx);
		void RemoveRange(int start, int count);
		void RemoveRange(int start, int count, UITableViewRowAnimation anim);

		IEnumerator GetEnumerator();

		int Count { get; }
		Element this[int idx] { get; }

		void Clear();
	}

	/// <summary>
	/// Generic base version of Section
	/// </summary>
	[Preserve(AllMembers=true)]
	public class Section : Element, ISection, IEnumerable
	{
		private object header, footer;
		public List<Element> Elements { get; set; }

		// X corresponds to the alignment, Y to the height of the password
		public SizeF EntryAlignment { get; set; }
		public bool IsMultiselect { get; set; }

		/// <summary>
		///  Constructs a Section without header or footers.
		/// </summary>
		public Section() : base(null)
		{
			Elements = new List<Element>();
		}

		/// <summary>
		///  Constructs a Section with the specified header
		/// </summary>
		/// <param name="caption">
		/// The header to display
		/// </param>
		public Section(string caption) : base(caption)
		{
			Elements = new List<Element>();
		}

		/// <summary>
		/// Constructs a Section with a header and a footer
		/// </summary>
		/// <param name="caption">
		/// The caption to display (or null to not display a caption)
		/// </param>
		/// <param name="footer">
		/// The footer to display.
		/// </param>
		public Section(string caption, string footer) : base(caption)
		{
			Footer = footer;
			Elements = new List<Element>();
		}

		public Section(UIView header) : base(null)
		{
			HeaderView = header;
			Elements = new List<Element>();
		}

		public Section(UIView header, UIView footer) : base(null)
		{
			HeaderView = header;
			FooterView = footer;
			Elements = new List<Element>();
		}

		/// <summary>
		///    The section header, as a string
		/// </summary>
		public string Header
		{
			get { return header as string; }
			set { footer = value; }
		}

		/// <summary>
		/// The section footer, as a string.
		/// </summary>
		public string Footer
		{
			get { return footer as string; }

			set { footer = value; }
		}

		/// <summary>
		/// The section's header view.  
		/// </summary>
		public UIView HeaderView
		{
			get { return header as UIView; }
			set { header = value; }
		}

		/// <summary>
		/// The section's footer view.
		/// </summary>
		public UIView FooterView
		{
			get { return footer as UIView; }
			set { footer = value; }
		}

		/// <summary>
		/// Adds a new child Element to the Section
		/// </summary>
		/// <param name="element">
		/// An element to add to the section.
		/// </param>
		public void Add(Element element)
		{
			if (element == null)
				return;
			
			Elements.Add(element);
			element.Parent = this;
			
			if (Parent != null)
				InsertVisual(Elements.Count - 1, UITableViewRowAnimation.None, 1);
		}

		/// <summary>
		///    Add version that can be used with LINQ
		/// </summary>
		/// <param name="elements">
		/// An enumerable list that can be produced by something like:
		///    from x in ... select (Element) new MyElement (...)
		/// </param>
		public int Add(IEnumerable<Element> elements)
		{
			int count = 0;
			foreach (var e in elements)
			{
				Add(e);
				count++;
			}
			return count;
		}

		/// <summary>
		/// Use to add a UIView to a section, it makes the section opaque, to
		/// get a transparent one, you must manually call UIViewElement
		public void Add(UIView view)
		{
			if (view == null)
				return;
			Add(new UIViewElement(null, view, false));
		}

		/// <summary>
		///   Adds the UIViews to the section.
		/// </summary>fparent
		/// <param name="views">
		/// An enumarable list that can be produced by something like:
		///    from x in ... select (UIView) new UIFoo ();
		/// </param>
		public void Add(IEnumerable<UIView> views)
		{
			foreach (var v in views)
				Add(v);
		}

		/// <summary>
		/// Inserts a series of elements into the Section using the specified animation
		/// </summary>
		/// <param name="idx">
		/// The index where the elements are inserted
		/// </param>
		/// <param name="anim">
		/// The animation to use
		/// </param>
		/// <param name="newElements">
		/// A series of elements.
		/// </param>
		public void Insert(int idx, UITableViewRowAnimation anim, params Element[] newElements)
		{
			if (newElements == null)
				return;
			
			int pos = idx;
			foreach (var e in newElements)
			{
				Elements.Insert(pos++, e);
				e.Parent = this;
			}
			var root = Parent as IRoot;
			if (Parent != null && root.TableView != null)
			{
				if (anim == UITableViewRowAnimation.None)
					root.TableView.ReloadData();
				else
					InsertVisual(idx, anim, newElements.Length);
			}
		}

		public int Insert(int idx, UITableViewRowAnimation anim, IEnumerable<Element> newElements)
		{
			if (newElements == null)
				return 0;
			
			int pos = idx;
			int count = 0;
			foreach (var e in newElements)
			{
				Elements.Insert(pos++, e);
				e.Parent = this;
				count++;
			}
			var root = Parent as IRoot;
			if (root != null && root.TableView != null)
			{
				if (anim == UITableViewRowAnimation.None)
					root.TableView.ReloadData();
				else
					InsertVisual(idx, anim, pos - idx);
			}
			return count;
		}

		void InsertVisual(int idx, UITableViewRowAnimation anim, int count)
		{
			var root = Parent as IRoot;
			
			if (root == null || root.TableView == null)
				return;
			
			int sidx = root.IndexOf(this as ISection);
			var paths = new NSIndexPath[count];
			for (int i = 0; i < count; i++)
				paths[i] = NSIndexPath.FromRowSection(idx + i, sidx);
			
			root.TableView.InsertRows(paths, anim);
		}

		public void Insert(int index, params Element[] newElements)
		{
			Insert(index, UITableViewRowAnimation.None, newElements);
		}

		public void Remove(Element e)
		{
			if (e == null)
				return;
			for (int i = Elements.Count; i > 0;)
			{
				i--;
				if (Elements[i] == e)
				{
					RemoveRange(i, 1);
					return;
				}
			}
		}

		public void Remove(int idx)
		{
			RemoveRange(idx, 1);
		}

		/// <summary>
		/// Removes a range of elements from the Section
		/// </summary>
		/// <param name="start">
		/// Starting position
		/// </param>
		/// <param name="count">
		/// Number of elements to remove from the section
		/// </param>
		public void RemoveRange(int start, int count)
		{
			RemoveRange(start, count, UITableViewRowAnimation.Fade);
		}

		/// <summary>
		/// Remove a range of elements from the section with the given animation
		/// </summary>
		/// <param name="start">
		/// Starting position
		/// </param>
		/// <param name="count">
		/// Number of elements to remove form the section
		/// </param>
		/// <param name="anim">
		/// The animation to use while removing the elements
		/// </param>
		public void RemoveRange(int start, int count, UITableViewRowAnimation anim)
		{
			if (start < 0 || start >= Elements.Count)
				return;
			if (count == 0)
				return;
			
			var root = Parent as IRoot;
			
			if (start + count > Elements.Count)
				count = Elements.Count - start;
			
			Elements.RemoveRange(start, count);
			
			if (root == null || root.TableView == null)
				return;
			
			int sidx = root.IndexOf(this as ISection);
			var paths = new NSIndexPath[count];
			for (int i = 0; i < count; i++)
				paths[i] = NSIndexPath.FromRowSection(start + i, sidx);
			root.TableView.DeleteRows(paths, anim);
		}

		/// <summary>
		/// Enumerator to get all the elements in the Section.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerator"/>
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			foreach (var e in Elements)
				yield return e;
		}

		public int Count
		{
			get { return Elements.Count; }
		}

		public Element this[int idx]
		{
			get { return Elements[idx]; }
		}

		public void Clear()
		{
			foreach (var e in Elements)
				e.Dispose();
			Elements = new List<Element>();
			
			var root = Parent as IRoot;
			if (root != null && root.TableView != null)
				root.TableView.ReloadData();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Parent = null;
				Clear();
				Elements = null;
			}
		}

		public override void InitializeCell(UITableView tableView)
		{
			Cell.TextLabel.Text = "Section was used for Element";
		}
	}
}

