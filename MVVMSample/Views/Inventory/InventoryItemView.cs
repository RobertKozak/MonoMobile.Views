namespace DealerCenter
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Theme(typeof(GroupedTheme))]
	public class InventoryItemView: View, IDataContext
	{	
		private AutoCheckConfirmationView AutoCheckConfirmation;
		private SendActionSheetView SendActionSheetView;

	[Section(Order = 0)]
		[Root(ElementType=typeof(InventoryMiniView), ViewType=typeof(InventoryItemView))]
		public new InventoryItemViewModel DataContext 
		{ 
			get { return (InventoryItemViewModel)GetDataContext(); } 
			set { SetDataContext(value); } 
		}

	[Section(Order = 2)]

		[Root(ViewType = typeof(KelleyBookView))]
		[Bind("KelleyBooked", "ImageIcon", ValueConverterType = typeof(BookedConverter), BindingMode = BindingMode.OneWay)]
		public KelleyViewModel Kelley;

		[Caption("NADA")]
		[Root(ViewType = typeof(NadaBookView))]
		[Bind("NADABooked", "ImageIcon", ValueConverterType = typeof(BookedConverter), BindingMode = BindingMode.OneWay)]
		public NadaViewModel Nada;

		[Root(ViewType = typeof(BlackBookView))]
		[Bind("BlackBookBooked", "ImageIcon", ValueConverterType = typeof(BookedConverter), BindingMode = BindingMode.OneWay)]
		public BlackBookViewModel BlackBook;

	[Section(Order = 3)]
		[Button]
		[Bind("AutoCheckBooked", "ImageIcon", ValueConverterType=typeof(BookedConverter), BindingMode = BindingMode.OneWay)]
		[Bind("AutoCheckBooked", "Accessory", ValueConverterType=typeof(AccessoryConverter), BindingMode = BindingMode.OneWay)]
		[Caption("AutoCheck")]
		public void AutoCheck()
		{
			if (DataContext.AutoCheckBooked)
			{
				AutoCheckConfirmation.ShowAutoCheckView();
			}
			else
				AutoCheckConfirmation.ShowInView(Application.NavigationController.View);
		}		

		[NavbarButton(UIBarButtonSystemItem.Action)]
		public void Action()
		{
			SendActionSheetView.ShowInView(Application.NavigationController.View);
		}

		public InventoryItemView()
		{
		}
		
		public override void Initialize()
		{		
			var title = "The cost is $5 for a summary report. The report will expire 30 days from today.\n\nDo you want to get the AutoCheck Summary report?";
			AutoCheckConfirmation = new AutoCheckConfirmationView(title, DataContext);

			SendActionSheetView = new SendActionSheetView(string.Empty, DataContext);
		}

		public override string ToString()
		{
			return string.Format ("{1} {2} {3}",DataContext.Year, DataContext.Make, DataContext.Model);
		}
	}
	
	[Preserve(AllMembers = true)]
	public class BookedConverter : IValueConverter
	{
		private UIImage _Booked = UIImage.FromFile("Images/SphereGreen.png");
		private UIImage _NotBooked = UIImage.FromFile("Images/SphereRed.png");

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && (bool)value)
			{
				return _Booked;
			}
			
			return _NotBooked;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value == _Booked);  
		}
	}

	[Preserve(AllMembers = true)]
	public class AccessoryConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && (bool)value)
			{
				return UITableViewCellAccessory.DisclosureIndicator;				
			}
			
			return UITableViewCellAccessory.None;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}

