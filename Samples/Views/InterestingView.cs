using System;
using MonoTouch.MVVM;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Samples
{
	public class InterestingView: View<InterestingViewModel>
	{
		[Bind("CaptionSize", "Entry.Font", ValueConverterType = typeof(FontConverter))]
		[Bind("Alignment", "Entry.TextAlignment")]
		[Entry]
		[Caption(" ")]
		public string ResizingText 
		{
			get { return Get(()=>DataContext.ResizingText); }
			set { Set(()=>DataContext.ResizingText, value); }
		}

		[Range(12, 30)]
		public float CaptionSize 
		{
			get { return Get(()=>CaptionSize, 17); }
			set { Set(()=>CaptionSize, value); }
		}

		[Bind(SourcePath = "CaptionSize")]
		[Entry]
		public string Number 
		{
			get { return Get (() => Number); }
			set { Set (() => Number, value); }
		}

		[PopOnSelection]
		[Section]
		public UITextAlignment Alignment
		{
			get { return Get (() => Alignment); }
			set { Set (() => Alignment, value); }
		}

		[Section("Entry with Custom Keyboard")]
		[Bind("KeyboardType", "Entry.KeyboardType")]
		[Entry]
		public string Text
		{
			get { return Get (() => DataContext.SampleText); }
			set { Set (() => DataContext.SampleText, value); }
		}
		
		[PopOnSelection]
		public UIKeyboardType KeyboardType 
		{
			get { return Get (() => KeyboardType, UIKeyboardType.Default); }
			set { Set (() => KeyboardType, value); }
		}
	}	
}

