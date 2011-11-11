//// 
//// ElementDataBinding.cs
//// 
//// Author:
////   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
//// 
//// Copyright 2011, Nowcom Corporation.
//// 
////  Code licensed under the MIT X11 license
//// 
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
//// 
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
//// 
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//// 
//namespace MonoMobile.Views
//{
//	using System;
//	using System.Drawing;
//	using MonoTouch.Foundation;
//	using MonoTouch.UIKit;
//
//	[Preserve(AllMembers = true)]
//	public class ElementDataBinding : IDataBinding, IDisposable
//	{		
//		private UITableViewCellAccessory __Accessory { get { return Cell.Accessory; } set { Cell.Accessory = value; } }
//		private UIImage __ImageIcon { get { return Cell.ImageView.Image; } set { Cell.ImageView.Image = value; } }
//		private UIColor __BackgroundColor { get { return Cell.BackgroundColor; } set { Cell.BackgroundColor = value; } }
//		private UIView __BackgroundView { get { return Cell.BackgroundView; } set { Cell.BackgroundView = value; } }
//		private UILabel __TextLabel { get { return Cell.TextLabel; } }
//		private UILabel __DetailTextLabel { get { return Cell.DetailTextLabel; } }
//		private UIFont __TextLabelFont { get { return Cell.TextLabel.Font; } set { Cell.TextLabel.Font = value; } }
//		private UIFont __DetailTextLabelFont { get { return Cell.DetailTextLabel.Font; } set { Cell.DetailTextLabel.Font = value; } }
//		private UITextAlignment __TextAlignment { get { return Cell.TextLabel.TextAlignment; } set { Cell.TextLabel.TextAlignment = value; } }
//		private UITextAlignment __DetailTextAlignment { get { return Cell.DetailTextLabel.TextAlignment; } set { Cell.DetailTextLabel.TextAlignment = value; } }
//		private SizeF __TextAlignmentShadowOffset { get { return Cell.TextLabel.ShadowOffset; } set { Cell.TextLabel.ShadowOffset = value; } }
//		private SizeF __DetailTextAlignmentShadowOffset { get { return Cell.DetailTextLabel.ShadowOffset; } set { Cell.DetailTextLabel.ShadowOffset = value; } }
//		private UIColor __TextAlignmentShadowColor { get { return Cell.TextLabel.ShadowColor; } set { Cell.TextLabel.ShadowColor = value; } }
//		private UIColor __DetailTextAlignmentShadowColor { get { return Cell.DetailTextLabel.ShadowColor; } set { Cell.DetailTextLabel.ShadowColor = value; } }
//		
//		public IElement Element { get; set; }
//		public UITableViewCell Cell { get { return Element.Cell; } }
//		public UILabel TextLabel { get { return Cell.TextLabel; } }
//		public UILabel DetailTextLabel { get { return Cell.DetailTextLabel; } }
//
//		protected object DataContext 
//		{ 
//			get { return GetDataContext(); } 
//			set { SetDataContext(value); }
//		} 
//
//		public virtual object GetDataContext()
//		{
//			return Element.DataContext;
//		} 
//
//		public virtual void SetDataContext(object value)
//		{
//			if (Element.DataContext != value)
//			{
//				Element.DataContext = value;
//			}
//		}
//		
//		public BindableProperty DataContextProperty = BindableProperty.Register("DataContext");
//		
//		public BindableProperty ElementInstanceProperty = BindableProperty.Register("ElementInstance");
//		public BindableProperty AccessoryProperty = BindableProperty.Register("Accessory");
//		public BindableProperty AccessoryCommandProperty = BindableProperty.Register("AccessoryCommand");
//
//		public BindableProperty ImageIconProperty = BindableProperty.Register("ImageIcon");
//		public BindableProperty ImageIconUriProperty = BindableProperty.Register("ImageIconUri");
//		public BindableProperty BackgroundImageProperty = BindableProperty.Register("BackgroundImage");
//		public BindableProperty BackgroundUriProperty = BindableProperty.Register("BackgroundUri");
//		public BindableProperty BackgroundColorProperty = BindableProperty.Register("BackgroundColor");
//
//		public BindableProperty TextLabelProperty = BindableProperty.Register("TextLabel");
//		public BindableProperty TextFontProperty = BindableProperty.Register("Font");
//		public BindableProperty TextColorProperty = BindableProperty.Register("TextColor");
//		public BindableProperty TextAlignmentProperty = BindableProperty.Register("TextAlignment");
//		public BindableProperty TextShadowOffsetProperty = BindableProperty.Register("ShadowOffset");
//		public BindableProperty TextShadowColorProperty = BindableProperty.Register("ShadowColor");
//
//		public BindableProperty DetailTextLabelProperty = BindableProperty.Register("DetailTextLabel");
//		public BindableProperty DetailTextFontProperty = BindableProperty.Register("Font");
//		public BindableProperty DetailTextColorProperty = BindableProperty.Register("TextColor");
//		public BindableProperty DetailTextAlignmentProperty = BindableProperty.Register("TextAlignment");
//		public BindableProperty DetailTextShadowOffsetProperty = BindableProperty.Register("ShadowOffset");
//		public BindableProperty DetailTextShadowColorProperty = BindableProperty.Register("ShadowColor");
//
//		public BindableProperty CaptionProperty = BindableProperty.Register("Text");
//		public BindableProperty ThemeProperty = BindableProperty.Register("Theme");
//		public BindableProperty DetailTextProperty = BindableProperty.Register("Text");
//
//		public BindableProperty RowHeightProperty = BindableProperty.Register("RowHeight");
//
//
//		public ElementDataBinding(IElement element)
//		{
//			Element = element;
//		}
//
//		public virtual void BindProperties()
//		{		
//			DataContextProperty.BindTo(this);
//
//			ElementInstanceProperty.BindTo(Element);
//
//			AccessoryProperty.BindTo(Element);
//			AccessoryCommandProperty.BindTo(Element);
//
//			ImageIconProperty.BindTo(Element);
//			ImageIconUriProperty.BindTo(Element);
//			BackgroundImageProperty.BindTo(Element);
//			BackgroundUriProperty.BindTo(Element);
//			BackgroundColorProperty.BindTo(Element);
//
//			TextLabelProperty.BindTo(Element);
//			DetailTextLabelProperty.BindTo(Element);
//			
//			RowHeightProperty.BindTo(Element);
//
//			if (TextLabel != null)
//			{
//				CaptionProperty.BindTo(TextLabel);
//				TextFontProperty.BindTo(TextLabel);
//				TextColorProperty.BindTo(TextLabel);
//				TextAlignmentProperty.BindTo(TextLabel);
//				TextShadowOffsetProperty.BindTo(TextLabel);
//				TextShadowColorProperty.BindTo(TextLabel);
//			}
//
//			if (DetailTextLabel != null)
//			{
//				DetailTextProperty.BindTo(DetailTextLabel);
//				DetailTextFontProperty.BindTo(DetailTextLabel);
//				DetailTextColorProperty.BindTo(DetailTextLabel);
//				DetailTextAlignmentProperty.BindTo(DetailTextLabel);
//				DetailTextShadowOffsetProperty.BindTo(DetailTextLabel);
//				DetailTextShadowColorProperty.BindTo(DetailTextLabel);
//			}
//		
//			ThemeProperty.BindTo(Element);
//		}
//
//		public virtual void UpdateTargets()
//		{
//			var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(Element);
//			if (bindingExpressions != null)
//			{
//				foreach (var bindingExpression in bindingExpressions)
//				{
//					bindingExpression.UpdateTarget();
//				}
//			}
//		}
//
//		public virtual void UpdateSources()
//		{
//			var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(Element);
//			if (bindingExpressions != null)
//			{
//				foreach (var bindingExpression in bindingExpressions)
//				{
//					bindingExpression.UpdateSource();
//				}
//			}
//		}
//		
//		public virtual void UpdateDataContext()
//		{
//			DataContextProperty.Update();
//		}
//
//		public virtual void UpdateDataContext(object value)
//		{
//			DataContextProperty.Update(value);
//		}
//
//		public virtual void ClearBindings()
//		{
//			var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(Element);
//			if (bindingExpressions != null)
//				foreach (var bindingExpression in bindingExpressions)
//					BindingOperations.ClearBinding(bindingExpression.Binding.Target, bindingExpression.Binding.TargetPath);
//		}
//	
//		public void Dispose()
//		{
//			ClearBindings();
//			Dispose(true);
//			GC.SuppressFinalize(this);
//		}
//
//		protected virtual void Dispose(bool calledExplicitly)
//		{
//		}	
//	}
//}
//
