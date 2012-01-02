// 
//  CellView.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.Views
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class CellView<T> : UIView, IDataContext<MemberData>, IInitializable, IInitializeCell, ICellViewTemplate, IUpdateable, ICaption, IThemeable, IHandleNotifyPropertyChanged
	{
		private T _Value;
		public T Value { get { return GetValue(); } set { SetValue(value); } }
		public static Type ValueType { get { return typeof(T); } }

		public CellViewTemplate CellViewTemplate { get; set; }

		public Theme Theme { get; set; }

		public virtual UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Default; } }
		public UITableViewCell Cell { get; set; }
		public DialogViewController Controller { get; set; }

		private MemberData _DataContext;
		public MemberData DataContext { get { return GetDataContext(); } set { SetDataContext(value); } }
		
		public int Order { get; set; }
		public string Caption { get; set; }
		public bool ShowCaption { get; set; } 
		
		public CellView(RectangleF frame) : base(frame)
		{
			ShowCaption = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Controller = null;
				Cell = null;
				
				if (DataContext != null)
				{
					_DataContext.RemoveNotifyPropertyChangedHandler(_DataContext.Source, this);
					_DataContext.RemoveNotifyPropertyChangedHandler(_DataContext.DataContextSource, this);
					DataContext.Dispose();
				}
				
				Value = default(T);
 
				if (Theme != null)
				{
					Theme.Dispose();
					Theme = null;
				}
			}

			base.Dispose(disposing);
		}

		public virtual void Initialize()
		{
		}

		public virtual void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
		}
		
		public virtual void HandleNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (DataContext.CanHandleNotifyPropertyChanged(e.PropertyName))
			{
				SetValue(_DataContext.Value); 
				//Controller.TableView.ReloadData();
			}
		}

		protected virtual MemberData GetDataContext()
		{
			return _DataContext;
		}
		
		protected virtual void SetDataContext(MemberData value)
		{			
			if (_DataContext != value)
			{
				_DataContext = value;
				var newValue = _DataContext.Value;
				SetValue(newValue);
			}
			
			if (_DataContext != null)
			{
//				if (_DataContext.Source != null)
//					_DataContext.AddNotifyPropertyChangedHandler(_DataContext.Source, this);

				if (_DataContext.DataContextSource != null)
					_DataContext.AddNotifyPropertyChangedHandler(_DataContext.DataContextSource, this);
			}
		}

		protected virtual T GetValue()
		{
//			if (_DataContext != null)
//			{
//				IValueConverter valueConverter = null;
//				var valueConvertible = CellViewTemplate as IValueConvertible;
//				if (valueConvertible != null)
//				{
//					valueConverter = valueConvertible.ValueConverter;
//				}
//
//				_Value = (T)_DataContext.Convertback(_DataContext.Value, ValueType, valueConverter);
//			}

			return _Value;
		}
		
		protected virtual void SetValue(object value)
		{	
			if (_DataContext != null)
			{
				IValueConverter valueConverter = null;
				var valueConvertible = CellViewTemplate as IValueConvertible;
				if (valueConvertible != null)
				{
					valueConverter = valueConvertible.ValueConverter;
				}

				_Value = (T)_DataContext.Convert(_DataContext.Value, ValueType, valueConverter);
				return;
			}

			_Value = (T)value;
		}

		public virtual void InitializeTheme(UITableViewCell cell)
		{
		}

		public virtual void ApplyTheme(UITableViewCell cell)
		{
		}
	}
}