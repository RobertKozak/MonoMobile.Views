//
// View.cs: Base class for MVVM Views
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
namespace MonoTouch.MVVM
{
	using System;
	using MonoTouch.Dialog;
	using System.Collections.Generic;
	using MonoTouch.UIKit;

	public class View : PropertyNotifier, IView
	{
		public object DataContext { get; set; }
		public IRoot Root { get; set; }

		private IEnumerable<ISection> _Sections;

		public View(): this("")
		{
		}

		public View(string caption) : base()
		{
			Root = CreateRoot(caption);
		}

		public View(IRoot root) : base()
		{
			Root = root;
		}
		
		protected virtual IRoot CreateRoot(string caption)
		{
			return new RootElement<int>(caption);
		}
		
		protected virtual IEnumerable<ISection> Sections
		{
			get
			{
				if(_Sections == null)
				{
					_Sections = InitializeSections();
				}
				
				return _Sections;
			}
			set
			{
				_Sections = value;
				ResetView();
			}
		}
		
		protected virtual void ResetView()
		{
			Root.Clear();
			if (Sections != null)
				Root.Add(Sections);
		
			if (DataContext != null)
				Root.Caption = DataContext.ToString();
		}
		
		protected virtual IEnumerable<ISection> InitializeSections()
		{
			return null;
		}

		public override string ToString()
		{
			if (Root != null)
				return Root.Caption;
		
			return string.Empty;
		}
	}
	
	public abstract class View<TDataContext> : View, IView<TDataContext> where TDataContext : IViewModel, new()
	{
		private TDataContext _DataContext;
		public new TDataContext DataContext 
		{ 
			get 
			{
				return _DataContext; 
			} 
			set 
			{ 
				if (!EqualityComparer<TDataContext>.Default.Equals(_DataContext, value))
				{
					ResetDataContext(value);
				}
			} 
		}

		public View(TDataContext dataContext, string caption) : base(caption)
		{
			ResetDataContext(dataContext);
		}

		public View(string caption) : base(caption)
		{
			ResetDataContext(new TDataContext());
		}

		public View() : this("")
		{
		}

		protected void ResetDataContext(TDataContext dataContext)
		{
			if (DataContext != null)
			{
				DataContext.PropertyChanged -= (s, e) => { this.NotifyPropertyChanged (e.PropertyName); };
			}

			base.DataContext = dataContext;
			_DataContext = dataContext;

			DataContext.PropertyChanged += (s, e)=> { this.NotifyPropertyChanged(e.PropertyName); };
			ResetView();
		}
	}
}

