using System;
using MonoTouch.MVVM;

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

		public InterestingViewModel()
		{
			ResizingText = "Resize me!";
		}
	}
}

