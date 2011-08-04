// 
// ProgressHud.cs
// 
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
// Copyright 2011, Nowcom Corporation.
// 
// Original found on web: 
// http://chunkyinterface.wordpress.com/2010/09/09/mbprogresshud-for-monotouch/
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
	using System.Linq;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;

	public enum HudProgressMode
	{
		/** Progress is shown using an UIActivity_IndicatorView. This is the default. */
		Indeterminate,
		/** Progress is shown using a MBRoundProgressView. */
		Determinate, 
		Completed
	}

	public class ProgressHud : UIView
	{
		private static UIImage _CheckmarkImage = UIImage.FromResource(null, "checkmark.png");
		
		private UIWindow _Window = UIApplication.SharedApplication.Windows.FirstOrDefault();
		private UIView _Indicator;
		private float _Width;
		private float _Height;
		private NSTimer _GraceTimer;
		private NSTimer _MinShowTimer;
		private DateTime? _ShowStarted;
		private HudProgressMode? _Mode;
		private bool _UseAnimation;
		private float _YOffset = 0f;
		private float _XOffset= 0f;
		private bool _TaskInProgress;
		private UILabel _Label;
		private UILabel _DetailsLabel;
		private float _Progress;
		private string _DetailText;
		private string _TitleText = "Loading";

		public event EventHandler HudWasHidden;

		public float GraceTime = 0f;
		public float MinimumShowTime = 0f;

		public HudProgressMode Mode
		{
			get 
			{
				if (!_Mode.HasValue)
				{
					_Mode = HudProgressMode.Indeterminate;
					EnsureInvokedOnMainThread(() =>
					{
						UpdateIndicators();
						SetNeedsLayout();
						SetNeedsDisplay();
					});
				}
				return _Mode.Value;
			}
			set 
			{
				// Dont change mode if it wasn't actually changed to prevent flickering
				if (_Mode == value)
				{
					return;
				}
				_Mode = value;
				EnsureInvokedOnMainThread(() =>
				{
					UpdateIndicators();
					SetNeedsLayout();
					SetNeedsDisplay();
				});
			}
		}

		public float Progress
		{
			get 
			{ 
				return _Progress; 
			}
			set
			{
				if (_Progress != value)
					_Progress = value;

				if (Mode == HudProgressMode.Determinate)
				{
					EnsureInvokedOnMainThread(() =>
					{
						UpdateProgress();
						SetNeedsDisplay();
					});
				}
			}
		}

		public string TitleText
		{
			get { return _TitleText; }
			set {
				if (_TitleText != value)
				{
					_TitleText = value;

					EnsureInvokedOnMainThread(() =>
					{
						_Label.Text = _TitleText;
						SetNeedsLayout();
						SetNeedsDisplay();
					});
				}
			}
		}

		public string DetailText
		{
			get 
			{ 
				return _DetailText; 
			}
			set
			{
				if (_DetailText != value)
				{
					_DetailText = value;

					EnsureInvokedOnMainThread(() =>
					{
						_DetailsLabel.Text = _DetailText;
						SetNeedsLayout();
						SetNeedsDisplay();
					});
				}
			}
		}

		public float Opacity { get; set; }
		public UIFont TitleFont { get; set; }
		public UIFont DetailFont { get; set; }

		#region Accessor helpers
		private void UpdateProgress()
		{
			var indicator = _Indicator as RoundProgressView;
			if (indicator != null)
			{
				indicator.Progress = Progress;
			}
		}

		private void UpdateIndicators()
		{
			if (_Indicator != null)
			{
				_Indicator.RemoveFromSuperview();
			}
			
			_Indicator = null;
			
			if (Mode == HudProgressMode.Determinate)
			{
				_Indicator = new RoundProgressView();
			} 
			else if (Mode == HudProgressMode.Indeterminate)
			{
				_Indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
				((UIActivityIndicatorView)_Indicator).StartAnimating();
			}
			else
			{
				_Indicator = new UIImageView()
				{
					ContentMode = UIViewContentMode.ScaleAspectFill,
					Image = _CheckmarkImage,
					AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin
				};
				_Indicator.Bounds = new RectangleF(0, 0, 37, 37);
			}
			
			AddSubview(_Indicator);
		}
		#endregion
		
		#region Constants
		public const float MARGIN = 20.0f;
		public const float PADDING = 4.0f;

		public const float LABELFONTSIZE = 22.0f;
		public const float LABELDETAILSFONTSIZE = 16.0f;
		#endregion

		#region Lifecycle methods
		public ProgressHud() : this(UIApplication.SharedApplication.KeyWindow)
		{
		}

		public ProgressHud(UIWindow window) : this(window.Bounds)
		{
			_Window = window;
		}

		public ProgressHud(UIView view) : this(view.Bounds)
		{
		}

		public ProgressHud(RectangleF frame) : base(frame)
		{
			Initialize();
		}

		private void Initialize()
		{
			Mode = HudProgressMode.Indeterminate;
			
			// Add label
			_Label = new UILabel(Bounds);
			
			// Add details label
			_DetailsLabel = new UILabel(Bounds);
			
			TitleText = null;
			DetailText = null;
			Opacity = 0.9f;
			TitleFont = UIFont.BoldSystemFontOfSize(LABELFONTSIZE);
			DetailFont = UIFont.BoldSystemFontOfSize(LABELDETAILSFONTSIZE);

			AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
			
			// Transparent background
			Opaque = false;
			BackgroundColor = UIColor.Clear;
			
			// Make invisible for now
			Alpha = 0.0f;
			
			_TaskInProgress = false;
		}

		private void EnsureInvokedOnMainThread(Action action)
		{
			if (IsMainThread())
			{
				action();
				return;
			}

			BeginInvokeOnMainThread(() => action());
		}

		protected override void Dispose(bool disposing)
		{
			if (_Indicator != null)
			{
				_Indicator.RemoveFromSuperview();
				_Indicator = null;
			}

			RemoveFromSuperview();
			
			if (_Label != null)
			{
				_Label.Dispose();
				_Label = null;
			}

			if (_DetailsLabel != null)
			{
				_DetailsLabel.Dispose();
				_DetailsLabel = null;
			}

			if (_GraceTimer != null)
			{
				_GraceTimer.Dispose();
				_GraceTimer = null;
			}

			if (_MinShowTimer != null)
			{
				_MinShowTimer.Dispose();
				_MinShowTimer = null;
			}
			
			base.Dispose(disposing);
		}
		#endregion
		
		#region Layout
		public override void LayoutSubviews()
		{
			RectangleF frame = Bounds;
			
			// Compute HUD dimensions based on indicator size (add margin to HUD border)
			RectangleF indFrame = _Indicator.Bounds;
			_Width = indFrame.Size.Width + 2 * MARGIN;
			_Height = indFrame.Size.Height + 2 * MARGIN;
			
			// Position the indicator
			indFrame = new RectangleF((float)Math.Floor((frame.Size.Width - indFrame.Size.Width) / 2) + _XOffset, (float)Math.Floor((frame.Size.Height - indFrame.Size.Height) / 2) + _YOffset, indFrame.Size.Width, indFrame.Size.Height);
			_Indicator.Frame = indFrame;
			
			// Add label if label text was set 
			if (null != TitleText)
			{
				// Get size of label text
				SizeF dims = StringSize(TitleText, TitleFont);
				
				// Compute label dimensions based on font metrics if size is larger than max then clip the label width
				float lHeight = dims.Height;
				float lWidth;
				if (dims.Width <= (frame.Size.Width - 2 * MARGIN))
				{
					lWidth = dims.Width;
				} 
				else
				{
					lWidth = frame.Size.Width - 4 * MARGIN;
				}
				
				// Set label properties
				_Label.Font = TitleFont;
				_Label.AdjustsFontSizeToFitWidth = false;
				_Label.TextAlignment = UITextAlignment.Center;
				_Label.LineBreakMode = UILineBreakMode.WordWrap;
				_Label.Opaque = false;
				_Label.BackgroundColor = UIColor.Clear;
				_Label.TextColor = UIColor.White;
				_Label.Text = TitleText;
				
				// Update HUD size
				if (_Width < (lWidth + 2 * MARGIN))
				{
					_Width = lWidth + 2 * MARGIN;
				}
				
				//Set number of lines for the amount of text and re-adjust height
				_Label.Lines = (int)(dims.Width / lWidth) + 1;
				if (dims.Width % lWidth == 0)
					_Label.Lines--;
				
				lHeight = lHeight * _Label.Lines;
				
				// Move indicator to make room for the label
				indFrame = new RectangleF(indFrame.Location.X, indFrame.Location.Y - (float)(Math.Floor(lHeight / 2 + PADDING / 2)), indFrame.Width, indFrame.Height);
				_Indicator.Frame = indFrame;
				
				// Set the label position and dimensions
				RectangleF lFrame = new RectangleF((float)Math.Floor((frame.Size.Width - lWidth) / 2) + _XOffset, (float)Math.Floor(indFrame.Location.Y + indFrame.Size.Height + PADDING), lWidth, lHeight);
				_Label.Frame = lFrame;
				
				_Height = _Height + lHeight + PADDING;

				AddSubview(_Label);
				
				// Add details label delatils text was set
				if (null != DetailText)
				{
					// Get size of label text
					dims = StringSize(DetailText, DetailFont);
					
					// Compute label dimensions based on font metrics if size is larger than max then clip the label width
					lHeight = dims.Height;
					if (dims.Width <= (frame.Size.Width - 2 * MARGIN))
					{
						lWidth = dims.Width;
					} else
					{
						lWidth = frame.Size.Width - 4 * MARGIN;
					}
					
					// Set label properties
					_DetailsLabel.Font = DetailFont;
					_DetailsLabel.AdjustsFontSizeToFitWidth = false;
					_DetailsLabel.TextAlignment = UITextAlignment.Center;
					_DetailsLabel.LineBreakMode = UILineBreakMode.WordWrap;
					_DetailsLabel.Opaque = false;
					_DetailsLabel.BackgroundColor = UIColor.Clear;
					_DetailsLabel.TextColor = UIColor.White;
					_DetailsLabel.Text = DetailText;
					
					// Update HUD size
					if (_Width < lWidth)
					{
						_Width = lWidth + 2 * MARGIN;
					}
					
					//Set number of lines for the amount of text and re-adjust height
					_DetailsLabel.Lines = (int)(dims.Width / lWidth) + 1;
					if (dims.Width % lWidth== 0)
						_DetailsLabel.Lines--;
					lHeight = lHeight * _DetailsLabel.Lines;

					// Move indicator to make room for the new label
					indFrame = new RectangleF(indFrame.Location.X, indFrame.Location.Y - ((float)Math.Floor(lHeight / 2 + PADDING / 2)), indFrame.Width, indFrame.Height);
					_Indicator.Frame = indFrame;
					
					// Move first label to make room for the new label
					lFrame = new RectangleF(lFrame.Location.X, lFrame.Location.Y - ((float)Math.Floor(lHeight / 2 + PADDING / 2)), lFrame.Width, lFrame.Height);
					_Label.Frame = lFrame;
					
					// Set label position and dimensions
					RectangleF lFrameD = new RectangleF((float)Math.Floor((frame.Size.Width - lWidth) / 2) + _XOffset, lFrame.Location.Y + lFrame.Size.Height + PADDING, lWidth, lHeight);
					_DetailsLabel.Frame = lFrameD;
					
					_Height = _Height + lHeight + PADDING;

					AddSubview(_DetailsLabel);
				}
			}
			
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft)
				Transform = CGAffineTransform.MakeRotation(ToRadians(90f));
			else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
				Transform = CGAffineTransform.MakeRotation(ToRadians(-90f));
			else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown)
				Transform = CGAffineTransform.MakeRotation(ToRadians(180f));
		}

		private static IntPtr GetClassHandle(string clsName)
		{
			return (new Class(clsName)).Handle;
		}

		private float ToRadians(float degrees)
        {
            return degrees * 0.01745329f;
        }

		private static bool IsMainThread()
		{
			return Messaging.bool_objc_msgSend(GetClassHandle("NSThread"), new Selector("isMainThread").Handle);
		}
		#endregion

		#region Showing and execution
		public void Show(bool animated)
		{
			_Window.AddSubview(this);

			_UseAnimation = animated;
			
			// If the grace time is set postpone the HUD display
			if (GraceTime > 0.0)
			{
				_GraceTimer = NSTimer.CreateScheduledTimer(GraceTime, HandleGraceTimer);
			} 
			else
			{
				// ... otherwise show the HUD imediately 
				SetNeedsDisplay();
				ShowUsingAnimation(_UseAnimation);
			}
		}

		public void Hide(bool animated)
		{
			_UseAnimation = animated;
			
			// If the minShow time is set, calculate how long the hud was shown,
			// and pospone the hiding operation if necessary
			if (MinimumShowTime > 0.0 && _ShowStarted.HasValue)
			{
				double interv = (DateTime.Now - _ShowStarted.Value).TotalSeconds;
				if (interv < MinimumShowTime)
				{
					_MinShowTimer = NSTimer.CreateScheduledTimer((MinimumShowTime - interv), HandleMinShowTimer);
					return;
				}
			}
			
			// ... otherwise hide the HUD immediately
			HideUsingAnimation(_UseAnimation);
		}
		
		public Action ShowWhileAsync(Action execute, bool animated)
		{
			// Launch execution in new thread
			_TaskInProgress = true;
			
			// Show HUD view
			Show(animated);

			var thread = new System.Threading.Thread(() =>
			{
				using (NSAutoreleasePool pool = new NSAutoreleasePool())
				{
					execute();
				}
			});
				
			thread.Start();

			return ()=> Completed();
		}

		private void Completed()
		{
			EnsureInvokedOnMainThread(() =>
			{
				CleanUp();
			});
		}

		public void ShowWhileExecuting(Action execute, bool animated)
		{		
			// Launch execution in new thread
			_TaskInProgress = true;
			
			// Show HUD view
			Show(animated);

			var thread = new System.Threading.Thread(() =>
			{
				using (NSAutoreleasePool pool = new NSAutoreleasePool())
				{
					execute();

					Completed();
				}
			});
				
			thread.Start();
		}

		public void ShowCompleted(bool animated)
		{
			Mode = HudProgressMode.Completed;

			TitleText = "Completed";
			MinimumShowTime = 2.0f;
			Show(animated);
			Hide(animated);
		}
		
		private void HandleGraceTimer()
		{
			// Show the HUD only if the task is still running
			if (_TaskInProgress)
			{
				SetNeedsDisplay();
				ShowUsingAnimation(_UseAnimation);
			}
		}

		private void HandleMinShowTimer()
		{
			HideUsingAnimation(_UseAnimation);
		}

		private void Done()
		{	
			// If delegate was set make the callback
			if (HudWasHidden != null)
			{
				HudWasHidden(this, EventArgs.Empty);
			}
		}

		private void CleanUp()
		{
			_TaskInProgress = false;
			
			_Indicator = null;
			
			Hide(_UseAnimation);
		}
		#endregion
		
		#region Fade in and Fade out
		private void ShowUsingAnimation(bool animated)
		{
			_ShowStarted = DateTime.Now;
			// Fade in
			if (animated)
			{
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(0.40);
				Alpha = 1.0f;
				UIView.CommitAnimations();
			}
			else
			{
				Alpha = 1.0f;
			}
		}

		private void HideUsingAnimation(bool animated)
		{
			// Fade out
			if (animated)
			{
				UIView.BeginAnimations(null);
				UIView.SetAnimationDuration(0.4f);
				Alpha = 0.0f;
				UIView.CommitAnimations();
			} 
			else
			{
				Alpha = 0.0f;
			}
			
			Done();
		}
		#endregion
	
		#region BG Drawing
		public override void Draw(RectangleF rect)
		{
			// Center HUD
			RectangleF allRect = Bounds;
			// Draw rounded HUD bacgroud rect
			RectangleF boxRect = new RectangleF(((allRect.Size.Width - _Width) / 2) + _XOffset, ((allRect.Size.Height - _Height) / 2) + _YOffset, _Width, _Height);
			CGContext ctxt = UIGraphics.GetCurrentContext();
			FillRoundedRect(boxRect, ctxt);
		}

		private void FillRoundedRect(RectangleF rect, CGContext context)
		{
			float radius = 10.0f;
			context.BeginPath();
			context.SetGrayFillColor(0.0f, Opacity);
			context.MoveTo(rect.GetMinX() + radius, rect.GetMinY());
			context.AddArc(rect.GetMaxX() - radius, rect.GetMinY() + radius, radius, (float)(3 * Math.PI / 2), 0f, false);
			context.AddArc(rect.GetMaxX() - radius, rect.GetMaxY() - radius, radius, 0, (float)(Math.PI / 2), false);
			context.AddArc(rect.GetMinX() + radius, rect.GetMaxY() - radius, radius, (float)(Math.PI / 2), (float)Math.PI, false);
			context.AddArc(rect.GetMinX() + radius, rect.GetMinY() + radius, radius, (float)Math.PI, (float)(3 * Math.PI / 2), false);
			context.ClosePath();
			context.FillPath();
		}
		
	}
	#endregion

	public class RoundProgressView : UIProgressView
	{
		public RoundProgressView() : base(new RectangleF(0.0f, 0.0f, 37.0f, 37.0f))
		{
		}

		public override void Draw(RectangleF rect)
		{
			RectangleF allRect = Bounds;
			RectangleF circleRect = new RectangleF(allRect.Location.X + 2, allRect.Location.Y + 2, allRect.Size.Width - 4, allRect.Size.Height - 4);
			
			CGContext context = UIGraphics.GetCurrentContext();
			
			// Draw background
			context.SetRGBStrokeColor(1.0f, 1.0f, 1.0f, 1.0f);
			// white
			context.SetRGBFillColor(1.0f, 1.0f, 1.0f, 0.1f);
			// translucent white
			context.SetLineWidth(2.0f);
			context.FillEllipseInRect(circleRect);
			context.StrokeEllipseInRect(circleRect);
			
			// Draw progress
			float x = (allRect.Size.Width / 2);
			float y = (allRect.Size.Height / 2);
			context.SetRGBFillColor(1.0f, 1.0f, 1.0f, 1.0f);
			// white
			context.MoveTo(x, y);
			context.AddArc(x, y, (allRect.Size.Width - 4) / 2, -(float)(Math.PI / 2), (float)(Progress * 2 * Math.PI) - (float)(Math.PI / 2), false);
			context.ClosePath();
			context.FillPath();
		}
	}
}
