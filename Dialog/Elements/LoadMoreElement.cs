//
// LoadMoreElement.cs
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
	using System;
	using System.Drawing;
	using System.Linq;
	using System.Threading;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using MonoMobile.Views;
	
	[Obsolete("No longer supported. Please use ProgressElement instead.")]
	[Preserve(AllMembers = true)]
	public class LoadMoreElement : Element, ISelectable, ITappable
	{
		public string NormalCaption { get; set; }
		public string LoadingCaption { get; set; }
		public ICommand Command { get; set; }
		public object CommandParameter { get; set; }
		
		private UIActivityIndicatorView _ActivityIndicator;

		private const int pad = 10;
		private const int indicatorSize = 20;
	
		public LoadMoreElement(string normalCaption, string loadingCaption, ICommand command) : base (normalCaption)
		{
			NormalCaption = normalCaption;
			LoadingCaption = loadingCaption;
			Command = command;
			
			Theme.TextFont = UIFont.BoldSystemFontOfSize(15f);
			Theme.TextAlignment = UITextAlignment.Center;

			_ActivityIndicator = new UIActivityIndicatorView()
			{
				ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White,
				Hidden = true,
				Tag = 1,
				Frame = new RectangleF(indicatorSize * 2, pad, indicatorSize, indicatorSize)
			};

			_ActivityIndicator.StopAnimating();
		}
		
		public bool Animating
		{
			get
			{
				return _ActivityIndicator.IsAnimating;
			}
			set
			{
				// Doesn't work due to a bug in MonoTouch that should be fixed in 4.0 
				//float hue, saturation, brightness, alpha;
				//TextColor.GetHSBA(out hue, out saturation, out brightness, out alpha);
				var brightness = 1 - Theme.TextColor.CGColor.Components.FirstOrDefault();


				if (brightness > 0.5f)
					_ActivityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
				else
					_ActivityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;

				if (value)
				{
					Cell.TextLabel.Text = LoadingCaption;

					_ActivityIndicator.Hidden = false;
					_ActivityIndicator.StartAnimating();
				}
				else
				{
					_ActivityIndicator.StopAnimating();
					_ActivityIndicator.Hidden = true;
					Cell.TextLabel.Text = NormalCaption;
				}
			}
		}
				
		public override void InitializeCell(UITableView tableView)
		{
			base.InitializeCell(tableView);
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			Theme.TextAlignment = UITextAlignment.Center;

			Cell.ContentView.AddSubview(_ActivityIndicator);
		}
		
		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{			
			if (Animating)
				return;
			
			if (Command != null)
			{
				Animating = true;

				var executeCommandThread = new Thread(ExecuteCommandThread as ThreadStart);
				executeCommandThread.Start();
			}
		}

		private void ExecuteCommandThread()
		{
			using (var pool = new NSAutoreleasePool())
			{
				pool.InvokeOnMainThread(delegate
				{
					if (Command.CanExecute(CommandParameter))
						Command.Execute(CommandParameter);
					
					Animating = false;
				});
			}
		}
		
		public override float GetHeight(UITableView tableView, NSIndexPath indexPath)
		{
			SizeF size = new SizeF(280, 37);
			float height = size.Height;
			
			if (Theme.TextFont != null)
			{
				var fontHeight = tableView.StringSize(NormalCaption, Theme.TextFont, size, UILineBreakMode.TailTruncation).Height;
				if (fontHeight == 0) fontHeight = 17;
				height = fontHeight + (2 * pad);
			}
			return height;
		}
	}
}

