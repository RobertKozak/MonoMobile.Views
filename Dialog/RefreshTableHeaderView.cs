//
// RefreshTableHeaderView.cs:
//
// Author:
//  Miguel de Icaza
//
// Code to support pull-to-refresh based on Martin Bowling's TweetTableView
// which is based in turn in EGOTableViewPullRefresh code which was created
// by Devin Doty and is Copyrighted 2009 enormego and released under the
// MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files(the
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
	using MonoTouch.UIKit;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.CoreAnimation;
	using MonoTouch.CoreGraphics;
	
	public enum RefreshStatus
	{
		ReleaseToReload = 0,
		PullToReload = 1,
		Loading = 2
	}

	public class RefreshTableHeaderView : UIView
	{
		private static readonly UIImage _ArrowImage = UIImage.FromResource(null, "arrow.png");
		private UIActivityIndicatorView _Activity;
		private UILabel _LastUpdateLabel, _StatusLabel;
		private UIImageView _ArrowView;		
		private DateTime _LastUpdateTime;
		private readonly string _DefaultSettingsKey = "RefreshTableHeaderView.LastUpdateTime";
		
		public bool IsFlipped { get; set; }

		public RefreshTableHeaderView(RectangleF rect, string settingsKey) : base(rect)
		{
			if (!string.IsNullOrEmpty(settingsKey))
			{
				_DefaultSettingsKey = settingsKey;
			}

			Initialize(); 
		}

		public RefreshTableHeaderView(RectangleF rect) : base(rect)
		{
			Initialize();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_ArrowImage.Dispose();

				if (_Activity != null)
				{
					_Activity.Dispose();
					_Activity = null;
				}

				if (_LastUpdateLabel != null)
				{
					_LastUpdateLabel.Dispose();
					_LastUpdateLabel = null;
				}

				if (_StatusLabel != null)
				{
					_StatusLabel.Dispose();
					_StatusLabel = null;
				}

				if (_ArrowView != null)
				{
					_ArrowView.Dispose();
					_ArrowView = null;
				}
			}

			base.Dispose(disposing);
		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			var bounds = Bounds;
			
			_LastUpdateLabel.Frame = new RectangleF(0, bounds.Height - 30, bounds.Width, 20);
			_StatusLabel.Frame = new RectangleF(0, bounds.Height - 48, bounds.Width, 20);
			_ArrowView.Frame = new RectangleF(20, bounds.Height - 65, 30, 55);
			_Activity.Frame = new RectangleF(25, bounds.Height - 38, 20, 20);
		}
		
		private readonly RefreshStatus _Status = (RefreshStatus)(-1);
		
		public virtual void SetStatus(RefreshStatus status)
		{
			if (_Status == status)
				return;
			
			string s = "Release to refresh";
	
			switch (status)
			{
				case RefreshStatus.Loading:
					s = "Loading"; 
					break;
					
				case RefreshStatus.PullToReload:
					s = "Pull down to refresh";
					break;

				default:
					break;
			}
			_StatusLabel.Text = s;
		}
		
		public void Flip(bool animate)
		{
			UIView.BeginAnimations(null);
			UIView.SetAnimationDuration(animate ? .18f : 0);
			_ArrowView.Layer.Transform = IsFlipped 
				? CATransform3D.MakeRotation((float)Math.PI, 0, 0, 1) 
				: CATransform3D.MakeRotation((float)Math.PI * 2, 0, 0, 1);
				
			UIView.CommitAnimations();
			IsFlipped = !IsFlipped;
		}
		
		public DateTime LastUpdate 
		{
			get
			{
				return _LastUpdateTime;
			}
			set
			{
				_LastUpdateTime = value;
				if (value == DateTime.MinValue)
				{
					_LastUpdateLabel.Text = "Last Updated: never";
				} 
				else 
				{
					NSUserDefaults.StandardUserDefaults[_DefaultSettingsKey] = new NSString(value.ToString("G"));
					NSUserDefaults.StandardUserDefaults.Synchronize();
					_LastUpdateLabel.Text = String.Format("Last Updated: {0:g}", value);
				}
			}
		}
		
		public void SetActivity(bool active)
		{
			if (active)
			{
				_Activity.StartAnimating();
				_ArrowView.Hidden = true;
				SetStatus(RefreshStatus.Loading);
			} 
			else 
			{
				_Activity.StopAnimating();
				_ArrowView.Hidden = false;
			}
		}

		private void Initialize()
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			
			BackgroundColor = new UIColor(0.88f, 0.9f, 0.92f, 1);
			BackgroundColor = UIColor.Red;

			_LastUpdateLabel = new UILabel()
			{
				Font = UIFont.SystemFontOfSize(13f),
				TextColor = new UIColor(0.47f, 0.50f, 0.57f, 1),
				ShadowColor = UIColor.White, 
				ShadowOffset = new SizeF(0, 1),
				BackgroundColor = BackgroundColor,
				Opaque = true,
				TextAlignment = UITextAlignment.Center,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin
			};
			AddSubview(_LastUpdateLabel);
			
			_StatusLabel = new UILabel()
			{
				Font = UIFont.BoldSystemFontOfSize(14),
				TextColor = new UIColor(0.47f, 0.50f, 0.57f, 1),
				ShadowColor = _LastUpdateLabel.ShadowColor,
				ShadowOffset = new SizeF(0, 1),
				BackgroundColor = BackgroundColor,
				Opaque = true,
				TextAlignment = UITextAlignment.Center,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin
			};
			AddSubview(_StatusLabel);
			SetStatus(RefreshStatus.PullToReload);
			
			_ArrowView = new UIImageView()
			{
				ContentMode = UIViewContentMode.ScaleAspectFill,
				Image = _ArrowImage,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin
			};
			_ArrowView.Layer.Transform = CATransform3D.MakeRotation((float) Math.PI, 0, 0, 1);
			AddSubview(_ArrowView);
			
			_Activity = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray) 
			{
				HidesWhenStopped = true,
				AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin
			};
			AddSubview(_Activity);

			if (NSUserDefaults.StandardUserDefaults[_DefaultSettingsKey] != null) 
			{
				LastUpdate = Convert.ToDateTime(NSUserDefaults.StandardUserDefaults[_DefaultSettingsKey].ToString());
			}
			else
				LastUpdate = DateTime.MinValue;
		}
	}
}
