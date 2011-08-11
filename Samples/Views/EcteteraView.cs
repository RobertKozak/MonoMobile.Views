using System.Drawing;
namespace Samples
{
	using System;
	using MonoMobile.Views;
	using MonoTouch.UIKit;

	public class EcteteraElement : RootElement, ISelectable//, ISizeable
	{
		private UITextField _TextField;
		private UIImageView _Image;

		private EcteteraViewModel ViewModel
		{
			get { return ((EcteteraView)DataContext).DataContext; }
		}

		public override void InitializeCell(UITableView tableView)
		{
			base.InitializeCell (tableView);
			Cell.Accessory = UITableViewCellAccessory.None;
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
		}

		public override void InitializeContent()
		{
			_TextField = new UITextField(new RectangleF(50, 10, 250, 30));
		
			_Image = new UIImageView(new RectangleF(10, 10, 25, 25));
			
			if (ViewModel != null)
				_Image.BackgroundColor = ViewModel.Color;

			base.InitializeContent();
		
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
		}
		
		public override void UpdateCell()
		{
			base.UpdateCell();
			var location = new PointF(-10, -11);
			ElementView.Frame = new RectangleF(location, TableView.Frame.Size);
		}

		protected override void OnDataContextChanged()
		{
			if (_TextField != null)
				_TextField.Text = ViewModel.Text;
		}

		public void Selected(DialogViewController dvc, UITableView tableView, MonoTouch.Foundation.NSIndexPath path)
		{
			// do nothing
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
		[Section(Order = 0)]
		[Caption("")]
		[Root(ElementType = typeof(EcteteraElement))]
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

