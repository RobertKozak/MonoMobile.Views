using System;
using MonoMobile.Views;

namespace Samples
{
	public class InterestingViewModel : ViewModel
	{
		public string ResizingText
		{
			get { return Get(()=>ResizingText); }
			set { Set(()=>ResizingText, value); }
		}

		public string SampleText
		{
			get { return Get (() => SampleText, "Test"); }
			set { Set (() => SampleText, value); }
		}
		public float CaptionSize 
		{
			get { return Get(() => CaptionSize); }
			set { Set(() => CaptionSize, value); }	
		}

		public InterestingViewModel()
		{
			ResizingText = "Resize me!";
		}
	}
}

