using System;
using System.Collections.Generic;
using MonoMobile.Views;
using System.Collections.ObjectModel;
using MonoTouch.Foundation;

namespace Samples
{
	[Preserve(AllMembers = true)]
	public class ThemeSampleView: View
	{
		private Theme Selected;

		[List(SelectionAction = SelectionAction.Selection, SelectedItemMemberName = "Selected")]
		public ObservableCollection<Theme> Themes { get; set; }	

		public ThemeSampleView()
		{
			DataContext = new ThemeSampleViewModel();
		}
		
		[Section]
		[Button]
		public void SetSelectedTheme()
		{
			Application.CurrentDialogViewController.ResetTheme(Selected);
		}
	}
}

