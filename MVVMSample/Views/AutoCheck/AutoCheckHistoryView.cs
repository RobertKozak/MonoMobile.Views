namespace DealerCenter
{
	using System;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using System.Collections.Generic;
	
	[Preserve(AllMembers = true)]
	public class AutoCheckHistoryView: View
	{
	[Section]
		[Bind(ValueConverterType = typeof(DateTimeToStringConverter))]
		public string ReportRunDate;
		
	[Section("Problem")]
		[ReadOnly]
		[List(ElementType = typeof(AutoCheckHistoryElement))]
		public List<AutoCheckHistoryItem> ProblemHistory;
		
	[Section("Other Information")]
		[ReadOnly]
		[List(ElementType = typeof(AutoCheckHistoryElement))]
		public List<AutoCheckHistoryItem> OtherHistory;
		
	[Section(Order = 5)]
		[Html]
		public string TermsAndConditions = "http://www.autocheck.com/consumers/disclaimer.do";
		
//Add later
//	[Section(Order = 6)]
//		[Html]
//		[Caption("PDF")]
//		public string PDF = "http://www.nowcom.com";

		public AutoCheckHistoryView()
		{
		}
	}
}

