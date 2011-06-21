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
	public abstract partial class Element : UIView, IElement, IImageUpdated, IThemeable, IBindable, ISizeable
	{		
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

		private bool _Visible;
		private int _OldRow;
		private DisabledCellView _DisabledCellView; 
		
		public Element ElementInstance { get { return this; } }

		public NSString Id { get; set; }
		public int Order { get; set; }
		public int Index { get; set; }
		
		protected virtual void SetDataContext(object value)
		{
			if (_DataContext != value)
			{
				_DataContext = value;
			
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
					ThemeChanged();
				}
			}
		}

		public UITableViewCellAccessory? Accessory 
		{
			get { return Theme.Accessory; }
			set { Theme.Accessory = value; ThemeChanged(); }
		}

		public UIImage ImageIcon
		{
			get { return Theme.CellImageIcon; }
			set { Theme.CellImageIcon = value; ThemeChanged(); }
		}

		public Uri ImageIconUri
		{
			get { return Theme.CellImageIconUri; }
			set { Theme.CellImageIconUri = value; ThemeChanged(); }
		}

		public UIImage BackgroundImage
		{
			get { return Theme.CellBackgroundImage; }
			set { Theme.CellBackgroundImage = value; ThemeChanged(); }
		}

		public Uri BackgroundUri
		{
			get { return Theme.CellBackgroundUri; }
			set { Theme.CellBackgroundUri = value; ThemeChanged(); }
		}
		
		public new UIColor BackgroundColor
		{
			get { return Theme.CellBackgroundColor; }
			set { Theme.CellBackgroundColor = value; ThemeChanged(); }
		}
		
		public UILabel DetailTextLabel
		{
			get { return Theme.DetailTextLabel; }
			set { Theme.DetailTextLabel = value; ThemeChanged(); }
		}

		public UIFont DetailTextFont
		{
			get { return Theme.DetailTextFont; }
			set { Theme.DetailTextFont = value; ThemeChanged(); }
		}

		public UIColor DetailTextColor
		{
			get { return Theme.DetailTextColor; }
			set { Theme.DetailTextColor = value; ThemeChanged(); }
		}
		
		public UITextAlignment DetailTextAlignment
		{
			get { return Theme.DetailTextAlignment; }
			set { Theme.DetailTextAlignment = value; ThemeChanged(); }
		}
		
		public SizeF DetailTextShadowOffset
		{
			get { return Theme.DetailTextShadowOffset; }
			set { Theme.DetailTextShadowOffset = value; ThemeChanged(); }
		}

		public UIColor DetailTextShadowColor
		{
			get { return Theme.DetailTextShadowColor; }
			set { Theme.DetailTextShadowColor = value; ThemeChanged(); }
		}

		public UILabel TextLabel
		{
			get { return Theme.TextLabel; }
			set { Theme.TextLabel = value; ThemeChanged(); }
		}

		public UIFont TextFont
		{
			get { return Theme.TextFont; }
			set { Theme.TextFont = value; ThemeChanged(); }
		}

		public UIColor TextColor
		{
			get { return Theme.TextColor; }
			set { Theme.TextColor = value; ThemeChanged(); }
		}
			
		public UITextAlignment TextAlignment
		{
			get { return Theme.TextAlignment; }
			set { Theme.TextAlignment = value; ThemeChanged(); }
		}

		public SizeF TextShadowOffset
		{
			get { return Theme.TextShadowOffset; }
			set { Theme.TextShadowOffset = value; ThemeChanged(); }
		}

		public UIColor TextShadowColor
		{
			get { return Theme.TextShadowColor; }
			set { Theme.TextShadowColor = value; ThemeChanged();}
		}
		
		public virtual void InitializeTheme()
		{
		}
		
		public virtual void ThemeChanged()
		{
			if (TextLabel != null)
			{
				if (TextFont != null)
					TextLabel.Font = TextFont;
				
				TextLabel.TextAlignment = TextAlignment;
				TextLabel.TextColor = TextColor != null ? TextColor : TextLabel.TextColor;
				
				if (TextShadowColor != null)
					TextLabel.ShadowColor = TextShadowColor;

				if (TextShadowOffset != SizeF.Empty)
					TextLabel.ShadowOffset = TextShadowOffset;

				if (Theme.TextHighlightColor != null)
					TextLabel.HighlightedTextColor = Theme.TextHighlightColor;
			}
			
			if (DetailTextLabel != null)
			{
				if (DetailTextFont != null)
					DetailTextLabel.Font = DetailTextFont;
				
				DetailTextLabel.TextAlignment = DetailTextAlignment;
				DetailTextLabel.TextColor = DetailTextColor != null ? DetailTextColor : DetailTextLabel.TextColor;
				
				if (DetailTextShadowColor != null)
					DetailTextLabel.ShadowColor = DetailTextShadowColor;

				if (DetailTextShadowOffset != SizeF.Empty)
					DetailTextLabel.ShadowOffset = DetailTextShadowOffset;

				if (Theme.DetailTextHighlightColor != null)
					DetailTextLabel.HighlightedTextColor = Theme.DetailTextHighlightColor;
			}
			
			if (Cell != null)
			{
				if (BackgroundColor != null)
				{
					Cell.BackgroundColor = BackgroundColor == null ? UIColor.White : BackgroundColor;
				}
				else if (BackgroundUri != null)
				{
					var img = ImageLoader.DefaultRequestImage(BackgroundUri, this);
					Cell.BackgroundColor = img != null ? UIColor.FromPatternImage(img) : UIColor.White;
				}
				else if (BackgroundImage != null)
				{
					Cell.BackgroundColor = UIColor.FromPatternImage(BackgroundImage);
				} else
				{
					Cell.BackgroundColor = UIColor.White;
				}
				
				if (ImageIconUri != null)
				{
					var img = ImageLoader.DefaultRequestImage(ImageIconUri, this);
					
					if (img != null)
					{
						var small = img.Scale(new SizeF(32, 32));
						small = small.RemoveSharpEdges(5);
						
						Cell.ImageView.Image = small;
						Cell.ImageView.Layer.MasksToBounds = false;
						Cell.ImageView.Layer.ShadowOffset = new SizeF(2, 2);
						Cell.ImageView.Layer.ShadowRadius = 2f;
						Cell.ImageView.Layer.ShadowOpacity = 0.8f;
					}
				}
				else if (ImageIcon != null)
					Cell.ImageView.Image = ImageIcon;
				
				if (Accessory.HasValue)
					Cell.Accessory = Accessory.Value;
				
				Theme.ClearBackground();
			}
			
			SetNeedsDisplay();
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
		
		public UIView ContentView { get; set; }
		
		private ViewBinding _ViewBinding;
		public ViewBinding ViewBinding 
		{ 
			get { return _ViewBinding; }
			set { _ViewBinding = value; }
		}
		
		public UITableView TableView { get; set; }

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
		}
		
		public Element(string caption, Binding binding): this(caption)
		{

		}
		public RectangleF ContentFrame 
		{ 
			get { var frame = base.Frame; frame.Location = new PointF(0, 0); return frame; } 
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
			
			TextLabel = Cell.TextLabel;
			TextFont = TextLabel.Font;
			TextColor = TextLabel.TextColor;
			TextAlignment = TextLabel.TextAlignment;
			TextShadowColor = TextLabel.ShadowColor;
			TextShadowOffset = TextLabel.ShadowOffset;
			
			DetailTextLabel = Cell.DetailTextLabel;
			if (DetailTextLabel != null)
			{
				DetailTextLabel.Text = string.Empty;
				DetailTextFont = UIFont.SystemFontOfSize(17);
				DetailTextColor = DetailTextLabel.TextColor;
				DetailTextAlignment = DetailTextLabel.TextAlignment;
				DetailTextShadowColor = DetailTextLabel.ShadowColor;
				DetailTextShadowOffset = DetailTextLabel.ShadowOffset;
			}

			InitializeTheme();
			
			InitializeCell(tableView);
			
			Theme.Cell = Cell;
			
			BindProperties();
			UpdateTargets();
			UpdateSources();
			
			if (!Enabled) 
				SetDisabled(Cell);
			
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
				TextLabel.Text = Caption;
			}

			var selectable = this as ISelectable;
			Cell.SelectionStyle = selectable != null ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;			

			ThemeChanged();
			
			CreateContentView();
		}

		protected virtual void CreateContentView()
		{
			if (ContentView != null)
				ContentView.RemoveFromSuperview();

			InitializeContent();

			if (Cell != null)
			{	
				if (ContentView != null)
				{
					ContentView.Frame = Cell.RecalculateContentFrame(ContentView.Frame, ShowCaption);
					Cell.ContentView.AddSubview(ContentView);
				}
			}
			
			var elementView = ContentView as IElement;
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
					
					if (Section != null)
					{
						if(_Visible && !Section.Elements.Contains(this))
							Section.Insert(_OldRow, this);
						else
						{
							_OldRow = IndexPath.Row;
							Section.Remove(this);
						}
					}
				}
			}
		}

		protected void SetDisabled(UITableViewElementCell cell)
		{
			if (cell != null)
			{
				_DisabledCellView = new DisabledCellView(cell);
				cell.AddSubview(_DisabledCellView);
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

			return tableView.RowHeight;
		}	
	}
}