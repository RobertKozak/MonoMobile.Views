using System;
using MonoMobile.Views;
using MonoTouch.UIKit;
using System.ComponentModel;
using MonoTouch.Foundation;

namespace Samples
{
	[Preserve(AllMembers = true)]
	[Theme(typeof(WoodenTheme))]
//	[CellEditingStyle(UITableViewCellEditingStyle.Delete)]
	public class InterestingView: View
	{
		[Entry]
		[Caption("test")]
		public string ResizingText { get; set; }

		[Range(12, 30, ShowCaption = true)]
		public float CaptionSize { get; set; }

		[Entry(EditMode = EditMode.ReadOnly)]
		[DefaultValue("55")]
		public string Number { get; set; }

		[List(SelectionAction = SelectionAction.PopOnSelection)]
		[Section]
		public UITextAlignment Alignment { get; set; }

		[Section("Entry with Custom Keyboard")]
		[Entry(KeyboardType = UIKeyboardType.EmailAddress)]
		public string Text { get; set; }
		
		[List(SelectionAction = SelectionAction.PopOnSelection)]
		public UIKeyboardType KeyboardType { get; set; }
		

		[Section]
		[CellView(typeof(ThemeSampleView))]
		public ThemeSampleViewModel ThemeSample { get; set; }
		

		[ToolbarButton(Style = UIBarButtonItemStyle.Plain)]
		[Order(1)]
		[Caption("Test")]
		[Description("")]
		public void TestEditButton()
		{
		}		

		[ToolbarButton(Style = UIBarButtonItemStyle.Plain)]
		[Order(2)]
		[Caption("Test2")]
		[Description("")]
		public void TestEditButton2()
		{
		}

		[NavbarButton("View This Button", Style = UIBarButtonItemStyle.Plain)]
		public void TestAddButton()
		{
		}

		public InterestingView()
		{
			DataContext = new InterestingViewModel();
			ThemeSample = new ThemeSampleViewModel();
		}
	}	
}

