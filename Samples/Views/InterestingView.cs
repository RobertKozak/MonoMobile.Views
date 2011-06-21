using System;
using MonoMobile.MVVM;
using MonoTouch.UIKit;
using System.ComponentModel;
using MonoTouch.Foundation;

namespace Samples
{
	[Preserve(AllMembers = true)]
	[CellEditingStyle(UITableViewCellEditingStyle.Delete)]
	public class InterestingView: View
	{
	//	[Bind("CaptionSize", "DetailTextFont", ValueConverterType = typeof(FontConverter))]
		[Bind("Alignment", "TextAlignment")]
		[Entry]
		[Caption("test")]
		public string ResizingText 
		{
			get;// { return Get(() => ResizingText); }
			set;// { Set(() => ResizingText, value); }
		}

		[Range(12, 30, ShowCaption = true)]
		public float CaptionSize 
		{
			get;// { return Get(()=>CaptionSize, 17); }
			set;// { Set(()=>CaptionSize, value); }
		}

		[Bind("CaptionSize")]
		[Entry(EditMode = EditMode.ReadOnly)]
		[DefaultValue("55")]
		public string Number 
		{
			get;// { return Get (() => Number, "Testing number"); }
			set;// { Set (() => Number, value); }
		}

		[PopOnSelection]
		[Section]
		public UITextAlignment Alignment
		{
			get;// { return Get (() => Alignment, UITextAlignment.Left); }
			set;// { Set (() => Alignment, value); }
		}

		[Section("Entry with Custom Keyboard")]
		[Bind("KeyboardType", "Entry.KeyboardType")]
		[Entry(KeyboardType = UIKeyboardType.EmailAddress)]
		public string Text
		{
			get;// { return Get (() => DataContext.SampleText); }
			set;// { Set (() => DataContext.SampleText, value); }
		}
		
		[PopOnSelection]
		public UIKeyboardType KeyboardType 
		{
			get;// { return Get (() => KeyboardType, UIKeyboardType.Default); }
			set;// { Set (() => KeyboardType, value); }
		}
		

		[Section]
		[Root(ViewType = typeof(ThemeSampleView))]
		public ThemeSampleViewModel ThemeSample { get; set; }
		

		[ToolbarButton(Style = UIBarButtonItemStyle.Plain)]
		//[DisplayOrder(1)]
		[Order(1)]
		[Caption("Test")]
		[Description("")]
		public void TestEditButton()
		{
		}		
		[ToolbarButton(Style = UIBarButtonItemStyle.Plain)]
		//[DisplayOrder(1)]
		[Order(2)]
		[Caption("Test2")]
		[Description("")]
		public void TestEditButton2()
		{
		}

		[NavbarButton("View This Button", Style = UIBarButtonItemStyle.Plain)]
		//[Caption("Test1")]
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

