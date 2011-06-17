using System;
using MonoMobile.MVVM;
using System.Collections.Generic;
namespace Samples
{
	public class EnumerableTest: View
	{
		protected Element Element;

		protected List<object> SelectedThemes = new List<object>();
		
		[Bind("Selected")]
		public string MyThemeName;

	//	[Bind("Selected", "SelectedItem")]
		[Bind("SelectedThemes", "SelectedItems")]
		[Bind("Element", "ElementInstance")]
		[MultiSelection]
		public List<Theme> MyList = new List<Theme>() { new FrostedTheme(), new NavbarTheme(), new BackgroundImageTheme() }; 
		
		public TestEnum Selected;
		//[Bind("Selected", "SelectedItem", ValueConverterType = typeof(EnumCollection), ConverterParameter = typeof(TestEnum))]
		[Bind("Selected", "SelectedItem")]
		public EnumCollection<TestEnum> EnumTest { get; set; }
	

		[Button]
		public void Test()
		{

			//Console.WriteLine(Selected.FieldName);
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

