//
// Element.cs: 
//
// Author:
//  Miguel de Icaza (miguel@gnome.org)
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
	using System.Drawing;
	using MonoMobile.MVVM.Utilities;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public abstract class Element : DisposableObject, IElement, IImageUpdated, IThemeable, IBindable, ISizeable, INotifyDataContextChanged
	{		
		public Guid DebugId = Guid.NewGuid();

		protected object _DataContext;
		public object DataContext 
		{ 
			get 
			{
				return _DataContext;
			} 
			set 
			{ 
				SetDataContext(value);
			}
		}

		public IDataBinding DataBinding { get; set; }

		private bool _Visible;
		private int _OldRow;
		private DisabledCellView _DisabledCellView; 
		
		public Element ElementInstance { get { return this; } }

		public NSString Id { get; set; }
		public int Order { get; set; }
		public int Index { get; set; }
		
		public event DataContextChangedEvent DataContextChanged;

		protected virtual void SetDataContext(object value)
		{
			if (_DataContext != value)
			{
				var oldDataContext = _DataContext;
				_DataContext = value;

				var handler = DataContextChanged;
				if (handler != null)
				{
					handler(this, new DataContextChangedEventArgs(oldDataContext, value));
				}

				DataBinding.UpdateDataContext();
				OnDataContextChanged();
			}
		}

		/// <summary>
		/// Returns the IndexPath of a given element.   This is only valid for leaf elements,
		/// it does not work for a toplevel IRoot or a Section of if the Element has
		/// not been attached yet.
		/// </summary>
		public NSIndexPath IndexPath
		{
			get 
			{
				if (Section == null || Root == null)
					return null;
				
				int row = 0;
				foreach (var element in Section.Elements)
				{
					if (element == this)
					{
						int nsect = 0;
						foreach (var sect in Root.Sections)
						{
							if (Section == sect)
							{
								return NSIndexPath.FromRowSection(row, nsect);
							}
							nsect++;
						}
					}
					row++;
				}
				return null;
			}
		}
		
		private float _RowHeight;
		public float RowHeight
		{
			get { return _RowHeight == 0 ? 44 : _RowHeight; }
			set 
			{ 
				if (_RowHeight != value) 
				{ 
					_RowHeight = value;
					TableView.ReloadRows(new NSIndexPath[] { IndexPath }, UITableViewRowAnimation.None);
				} 
			}
		}

		public UITableViewElementCell Cell { get; set; }
		public UITableViewCellEditingStyle EditingStyle { get; set; }

		private Theme _Theme;
		public Theme Theme 
		{
			get 
			{
				if (_Theme == null)
				{
					_Theme = new Theme();
				}

				return _Theme;
			}
			set 
			{ 
				if (_Theme != value)
				{
					_Theme = value;
					Theme.ThemeChanged(Cell);
				}
			}
		}

		public UITableViewCellAccessory? Accessory 
		{
			get { return Theme.Accessory; }
			set { Theme.Accessory = value; Theme.ThemeChanged(Cell); }
		}
		
		public ICommand AccessoryCommand { get; set; }

		public UIImage ImageIcon
		{
			get { return Theme.CellImageIcon; }
			set { Theme.CellImageIcon = value; Theme.ThemeChanged(Cell); }
		}

		public Uri ImageIconUri
		{
			get { return Theme.CellImageIconUri; }
			set { Theme.CellImageIconUri = value; Theme.ThemeChanged(Cell); }
		}

		public UIImage BackgroundImage
		{
			get { return Theme.CellBackgroundImage; }
			set { Theme.CellBackgroundImage = value; Theme.ThemeChanged(Cell); }
		}

		public Uri BackgroundUri
		{
			get { return Theme.CellBackgroundUri; }
			set { Theme.CellBackgroundUri = value; Theme.ThemeChanged(Cell); }
		}
		
		public UIColor BackgroundColor
		{
			get { return Theme.CellBackgroundColor; }
			set { Theme.CellBackgroundColor = value; Theme.ThemeChanged(Cell); }
		}
		
		public UILabel TextLabel { get; set; }
		public UILabel DetailTextLabel { get; set; } 

		public virtual void InitializeTheme()
		{
			TextLabel = Cell.TextLabel;
			DetailTextLabel = Cell.DetailTextLabel;
			
			if (DetailTextLabel != null)
			{
				DetailTextLabel.BackgroundColor = UIColor.Clear;
				DetailTextLabel.Lines = 0;
			}

			TextLabel.BackgroundColor = UIColor.Clear;

			Theme.ThemeChanged(Cell);
		}

		/// <summary>
		/// Handle to the container object.
		/// </summary>
		/// <remarks>
		/// For sections this points to a IRoot, for every
		/// other object this points to a Section and it is null
		/// for the root IRoot.
		/// </remarks>
		public IElement Parent { get; set; }

		public IContainer Container { get { return Parent as IContainer; } }
		public ISection Section { get { return Parent as ISection; } }
		public IRoot Root
		{
			get
			{
				if (Section == null)
					return null;
				
				return Section.Parent as IRoot;
			}
		}

		public string Caption { get; set; }
		public bool ShowCaption { get; set; }
		
		public UIView ElementView { get; set; }
		
		private ViewBinding _ViewBinding;
		public ViewBinding ViewBinding 
		{ 
			get { return _ViewBinding; }
			set { _ViewBinding = value; }
		}
		
		private UITableView _TableView;
		public UITableView TableView 
		{ 
			get { return _TableView; } 
			set { _TableView = value; }
		}

		public Element(string caption) : base()
		{
			Id = new NSString(GetType().FullName);
			Caption = caption;
			ShowCaption = !string.IsNullOrEmpty(Caption);
			Theme.CellStyle = UITableViewCellStyle.Default;
			ViewBinding = new ViewBinding();
			Visible = true;
			Enabled = true;
			EditingStyle = UITableViewCellEditingStyle.None;
			
			DataBinding = new ElementDataBinding(this);
		}
		
		public Element(string caption, Binding binding): this(caption)
		{

		}
		
		public virtual bool Matches(string text)
		{
			var captionMatch = false;
			var detailTextMatch = false;

			if (!string.IsNullOrEmpty(Caption))
				captionMatch = Caption.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1;

			if (DetailTextLabel != null && !string.IsNullOrEmpty(DetailTextLabel.Text))
				detailTextMatch = DetailTextLabel.Text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1;

			return captionMatch || detailTextMatch;
		}
		
		protected override void Dispose(bool disposing)
		{
			var disposable = DataBinding as IDisposable;
			if (disposable != null)
				disposable.Dispose();

			base.Dispose(disposing);
			ViewBinding = null;
		}

		protected void RemoveTag(int tag)
		{
			var viewToRemove = Cell.ContentView.ViewWithTag(tag);
			if (viewToRemove != null)
			{
				viewToRemove.RemoveFromSuperview();
			}
		}
		
		public virtual void Initialize()
		{
		}

		public virtual UITableViewElementCell GetCell(UITableView tableView)
		{
			TableView = tableView;
			
			Cell = tableView.DequeueReusableCell(Id) as UITableViewElementCell;
			
			if (Cell == null)
			{
				Cell = NewCell();
			}
			else
				Cell.Element = this;
			
			InitializeTheme();
			
			InitializeCell(TableView);
			
			if (DataBinding != null)
			{
				DataBinding.BindProperties();
				DataBinding.UpdateTargets();
				DataBinding.UpdateSources();
			}
			
			if (!Enabled)
				SetDisabled(Cell);
			
			if (!Visible)
				SetVisible(Cell);
			
			UpdateCell();

			return Cell;
		}

		public virtual UITableViewElementCell NewCell()
		{
			var cell = new UITableViewElementCell(Theme.CellStyle, Id, this);
			cell.Element = this;
			return cell;
		}

		public virtual void InitializeCell(UITableView tableView)
		{
			RemoveTag(1);

			if (ShowCaption)
			{
				Cell.TextLabel.Text = Caption;
			}

			var selectable = this as ISelectable;
			Cell.SelectionStyle = selectable != null ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;			
			
			Theme.Cell = Cell;

			CreateElementView();
		}

		protected virtual void CreateElementView()
		{
			if (ElementView != null)
				ElementView.RemoveFromSuperview();

			InitializeContent();

			if (Cell != null)
			{	
				if (ElementView != null)
				{
					ElementView.Frame = Cell.RecalculateContentFrame(ElementView.Frame, ShowCaption);
					Cell.ContentView.AddSubview(ElementView);
				}
			}
			
			var elementView = ElementView as IElement;
			if (elementView != null && elementView.Caption != Caption)
			{
				var title = Caption;
				elementView.Caption = title;
			}
		}
		
		public virtual void InitializeContent()
		{
		}
		
		public virtual void UpdateCell()
		{
		}
		
		private bool _Enabled;
		public bool Enabled 
		{
			get { return _Enabled; } 
			set 
			{
				if (_Enabled != value)
				{
					_Enabled = value;

					if (_Enabled)
					{
						if (_DisabledCellView != null)
						{
							_DisabledCellView.RemoveFromSuperview();
							_DisabledCellView.Dispose();
							_DisabledCellView = null;
							Cell.SetNeedsDisplay();
						}
					}
					else
					{
						SetDisabled(Cell);
					}
				
				}
			}
		}
		
		public bool Visible
		{
			get { return _Visible;}
			set 
			{
				if (_Visible != value)
				{
					_Visible = value;

					SetVisible(Cell);	
				}
			}
		}

		protected void SetDisabled(UITableViewElementCell cell)
		{
			if (cell != null)
			{
				if (_DisabledCellView != null)
				{
					_DisabledCellView.RemoveFromSuperview();
					_DisabledCellView.Dispose();
					_DisabledCellView = null;
				}

				_DisabledCellView = new DisabledCellView(cell);
				cell.AddSubview(_DisabledCellView);
				cell.SetNeedsDisplay();
			}
		}
		
		protected void SetVisible(UITableViewElementCell cell)
		{
			if (cell != null)
			{
				if (Section != null)
				{
					if (_Visible && !Section.Elements.Contains(this))
						Section.Insert(_OldRow, this);
					else
					{
						_OldRow = IndexPath.Row;
						Section.Remove(this);
					}
				}

				cell.SetNeedsDisplay();
			}
		}

		protected virtual void OnDataContextChanged()
		{
		}

		void IImageUpdated.UpdatedImage(Uri uri)
		{
			if (uri == null)
				return;

			if (Root == null || Root.TableView == null)
				return;
			
			Root.TableView.ReloadRows(new NSIndexPath[] { IndexPath }, UITableViewRowAnimation.None);
		}

		public virtual float GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			if (Theme.CellHeight != 0)
				return Theme.CellHeight;
			
			return RowHeight;
		}	
	}
}