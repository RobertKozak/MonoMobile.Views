using System;
using MonoMobile.Views;
using System.Collections.Generic;
namespace Samples
{
	public class EnumerableTest: View
	{
		protected List<object> SelectedThemes = new List<object>();
		public Theme MyThemeName;

		[List(SelectionAction = SelectionAction.Multiselection, SelectedItemMemberName = "MyTheme", SelectedItemsMemberName = "SelectedThemes")]
		public List<Theme> MyList = new List<Theme>() { new FrostedTheme(), new NavbarTheme(), new BackgroundImageTheme() }; 
		
		public TestEnum Selected;
		[List(SelectionAction = SelectionAction.Selection, SelectedItemMemberName = "Selected")]
		public List<TestEnum> EnumTest { get; set; }
	

		[Button]
		public void Test()
		{
		}

		public EnumerableTest()
		{
			DataContext = new EnumerableTestModel();
		}

	}

	public class EnumerableTestModel: ViewModel
	{
		public Theme Selected
		{
			get { return Get(()=>Selected); }
			set { Set(()=>Selected, value); }
		}
	}
}

