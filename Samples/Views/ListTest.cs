using System;
using MonoMobile.Views;
using System.Collections.ObjectModel;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
namespace Samples
{
	[Theme(typeof(HoneyDoTheme))]
	public class ListTest : View
	{
		protected new ObservableCollection<MovieViewModel> DataContext
		{
			get { return (ObservableCollection<MovieViewModel>)GetDataContext(); }
			set { SetDataContext(value); }
		}

		public override void Initialize()
		{
			Add();
		}
		
		[ToolbarButton(MonoTouch.UIKit.UIBarButtonSystemItem.Action)]
		public void Clear()
		{
			DataContext = null;
		}

		[ToolbarButton(MonoTouch.UIKit.UIBarButtonSystemItem.Add)]
		public void Add()
		{
			var dataModel = new MovieDataModel();
			dataModel.Load(20);
			Console.WriteLine("Data Loaded " + DateTime.Now);
		DataContext = dataModel.Movies;

		}

		[ToolbarButton]
		public void One()
		{
			DataContext.Insert(2, new MovieViewModel(){});	
		}
	}
}

