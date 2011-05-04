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
namespace MonoMobile.MVVM
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	/// <summary>
	/// Generic base version of Section
	/// </summary>
	[Preserve(AllMembers=true)]
	public partial class Section : StringElement, ISection, IEnumerable
	{
		private UIView _Header, _Footer;

		public List<IElement> Elements { get; set; }
		
		public new IRoot Root { get { return Parent as IRoot; } }
		
		public bool IsMultiselect { get; set; }

		/// <summary>
		///  Constructs a Section without header or footers.
		/// </summary>
		public Section() : this((string)null)
		{
		}

		/// <summary>
		///  Constructs a Section with the specified header
		/// </summary>
		/// <param name="caption">
		/// The header to display
		/// </param>
		public Section(string caption) : base(caption)
		{
			Elements = new List<IElement>();
			BindProperties();
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
		public Section(string caption, string footer) : this(caption)
		{
			FooterText = footer;
		}

		public Section(UIView header) : this((string)null)
		{
			HeaderView = header;
		}

		public Section(UIView header, UIView footer) : this((string)null)
		{
			HeaderView = header;
			FooterView = footer;
		}

		/// <summary>
		///    The section header, as a string
		/// </summary>
		public string HeaderText { get; set; }

		/// <summary>
		/// The section footer, as a string.
		/// </summary>
		public string FooterText { get; set; }

		/// <summary>
		/// The section's header view.  
		/// </summary>
		public UIView HeaderView
		{
			get { return _Header; }
			set { _Header = value; }
		}

		/// <summary>
		/// The section's footer view.
		/// </summary>
		public UIView FooterView
		{
			get { return _Footer; }
			set { _Footer = value; }
		}
		
		public override void ThemeChanged()
		{
			if (Theme != null)
			{
				if (HeaderView != null)
				{
					var headerLabel = HeaderView.Subviews[0] as UILabel;
		
					headerLabel.Text = Caption;
		
					if (Theme.HeaderTextFont != null)
						headerLabel.Font = Theme.HeaderTextFont;
					if (Theme.HeaderTextColor != null)
						headerLabel.TextColor = Theme.HeaderTextColor;
					if (Theme.HeaderTextShadowColor != null)
						headerLabel.ShadowColor = Theme.HeaderTextShadowColor;
					if (Theme.HeaderTextShadowOffset != SizeF.Empty)
						headerLabel.ShadowOffset = Theme.HeaderTextShadowOffset;
				}

				if (FooterView != null)
				{
					var footerLabel = FooterView as UILabel;
					
					footerLabel.Text = FooterText.Replace("\n","");
					
					if (Theme.FooterTextFont != null)
						footerLabel.Font = DetailTextFont;
					if (Theme.FooterTextColor != null)
						footerLabel.TextColor = Theme.FooterTextColor;
					if (Theme.FooterTextShadowColor != null)
						footerLabel.ShadowColor = Theme.FooterTextShadowColor;
					if (Theme.FooterTextShadowOffset != SizeF.Empty)
						footerLabel.ShadowOffset = Theme.FooterTextShadowOffset;
				}
			}
		}

		/// <summary>
		/// Adds a new child Element to the Section
		/// </summary>
		/// <param name="element">
		/// An element to add to the section.
		/// </param>
		public void Add(IElement element)
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
		public int Add(IEnumerable<IElement> elements)
		{
			int count = 0;
			foreach (var e in elements)
			{
				Add(e);
				count++;
			}
			return count;
		}
//
//		/// <summary>
//		/// Use to add a UIView to a section, it makes the section opaque, to
//		/// get a transparent one, you must manually call UIViewElement
//		public void Add(UIView view)
//		{
//			if (view == null)
//				return;
//			Add(new UIViewElement(null, view, false));
//		}
//
//		/// <summary>
//		///   Adds the UIViews to the section.
//		/// </summary>fparent
//		/// <param name="views">
//		/// An enumarable list that can be produced by something like:
//		///    from x in ... select (UIView) new UIFoo ();
//		/// </param>
//		public void Add(IEnumerable<UIView> views)
//		{
//			foreach (var v in views)
//				Add(v);
//		}

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
		public void Insert(int idx, UITableViewRowAnimation anim, params IElement[] newElements)
		{
			if (newElements == null)
				return;
			
			int pos = idx;
			foreach (var e in newElements)
			{
				Elements.Insert(pos++, e);
				e.Parent = this;
			}
			
			if (Root != null && Root.TableView != null)
			{
				if (anim == UITableViewRowAnimation.None)
					Root.TableView.ReloadData();
				else
					InsertVisual(idx, anim, newElements.Length);
			}
		}

		public int Insert(int idx, UITableViewRowAnimation anim, IEnumerable<IElement> newElements)
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

			if (Root != null && Root.TableView != null)
			{
				if (anim == UITableViewRowAnimation.None)
					Root.TableView.ReloadData();
				else
					InsertVisual(idx, anim, pos - idx);
			}
			return count;
		}

		void InsertVisual(int idx, UITableViewRowAnimation anim, int count)
		{
			if (Root == null || Root.TableView == null)
				return;
			
			int sidx = Root.IndexOf(this as ISection);
			var paths = new NSIndexPath[count];
			for (int i = 0; i < count; i++)
				paths[i] = NSIndexPath.FromRowSection(idx + i, sidx);
			
			Root.TableView.InsertRows(paths, anim);
		}

		public void Insert(int index, params IElement[] newElements)
		{
			Insert(index, UITableViewRowAnimation.None, newElements);
		}

		public void Remove(IElement e)
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
			
			if (start + count > Elements.Count)
				count = Elements.Count - start;
			
			Elements.RemoveRange(start, count);
			
			if (Root == null || Root.TableView == null)
				return;
			
			int sidx = Root.IndexOf(this as ISection);
			var paths = new NSIndexPath[count];
			for (int i = 0; i < count; i++)
				paths[i] = NSIndexPath.FromRowSection(start + i, sidx);
			Root.TableView.DeleteRows(paths, anim);

			foreach(var element in Elements)
			{
				if (element.Cell != null)
					element.Cell.SetNeedsDisplay();
			}
		}

		/// <summary>
		/// Enumerator to get all the elements in the Section.
		/// </summary>
		/// <returns>
		/// A <see cref="IEnumerator"/>
		/// </returns>
		public new IEnumerator GetEnumerator()
		{
			foreach (var e in Elements)
				yield return e;
		}

		public int Count
		{
			get { return Elements.Count; }
		}

		public IElement this[int idx]
		{
			get { return Elements[idx]; }
		}

		public void Clear()
		{
			foreach (var e in Elements)
				e.Dispose();
			Elements = new List<IElement>();
			
			if (Root != null && Root.TableView != null)
				Root.TableView.ReloadData();
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

