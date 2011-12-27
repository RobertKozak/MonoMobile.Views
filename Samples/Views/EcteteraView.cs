using System.Drawing;
namespace Samples
{
	using System;
	using MonoMobile.Views;
	using MonoTouch.UIKit;

	public class EcteteraCellView : View, ISelectable, IUpdateable, IHandleNotifyPropertyChanged//, ISizeable
	{
		public UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Default; } } 
		public UITableViewCell Cell { get; set; }
		public DialogViewController Controller { get; set; }

		private UITextField _TextField;
		private UIImageView _Image;
		
		private UIView ElementView;

		private EcteteraViewModel ViewModel
		{
			get { return ((EcteteraView)DataContext).DataContext; }
		}

		public EcteteraCellView()
		{
			_TextField = new UITextField(new RectangleF(50, 10, 250, 30));
		
			_Image = new UIImageView(new RectangleF(10, 10, 25, 25));
			
			if (ViewModel != null)
				_Image.BackgroundColor = ViewModel.Color;
		
			ElementView = new UIView(RectangleF.Empty) { BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor };

			ElementView.Add(_Image);
			ElementView.Add(_TextField);

		
			_TextField.EditingChanged += delegate
			{
				if (_TextField.Text.Length > 5)
				{
					ViewModel.Color = UIColor.Blue;
				} else
				{
					ViewModel.Color = UIColor.Red;
				}
				_Image.BackgroundColor = ViewModel.Color;
			};
		

			if (ViewModel != null)
				_TextField.Text = ViewModel.Text;

			Add(ElementView);
		}

		public void UpdateCell(UITableViewCell cell, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			cell.Accessory = UITableViewCellAccessory.None;
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			var location = new PointF(-10, -11);
			ElementView.Frame = new RectangleF(location, cell.Frame.Size);
		}

		public void HandleNotifyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_TextField != null)
				_TextField.Text = ViewModel.Text;		
		}

		public void Selected (DialogViewController controller, UITableView tableView, object item, MonoTouch.Foundation.NSIndexPath indexPath)
		{
		
		}
	

//		#region ISizeable implementation
//		public float GetHeight (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
//		{
//			return tableView.Bounds.Height;
//		}
//		#endregion
}

	public class EcteteraViewModel : ViewModel
	{
		public string Text 
		{ 
			get { return Get(()=>Text); } 
			set { Set(()=>Text, value); } 
		}

		public UIColor Color 		
		{
			get { return Get(() => Color, UIColor.Blue); }
			set { Set(() => Color, value); }
		}
	}

	public class EcteteraView: View
	{
		[Section]
		[Order(0)]
		[Caption("")]
		[CellView(typeof(EcteteraCellView))]
		public new EcteteraViewModel DataContext
		{
			get { return (EcteteraViewModel)GetDataContext(); }
			set { SetDataContext(value); }
		}

		public override void Initialize()
		{
			DataContext = new EcteteraViewModel() { Text = "Sample Text", Color = UIColor.Red };
		}
	}
}

