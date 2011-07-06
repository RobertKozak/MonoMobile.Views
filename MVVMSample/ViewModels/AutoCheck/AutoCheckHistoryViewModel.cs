namespace DealerCenter
{
	using System;
	using System.Collections.Generic;
	using MonoMobile.MVVM;

	public class AutoCheckHistoryViewModel : ViewModel
	{
		[Section]
		public DateTime ReportRunDate 
		{
			get { return Get(() => ReportRunDate); }
			set { Set(() => ReportRunDate, value); }
		}

		public List<AutoCheckHistoryItem> ProblemHistory
		{
			get { return Get(() => ProblemHistory); }
			set { Set(() => ProblemHistory, value); }
		}

		public List<AutoCheckHistoryItem> OtherHistory
		{
			get { return Get(() => OtherHistory); }
			set { Set(() => OtherHistory, value); }
		}
		
		public string PDF
		{
			get { return Get(() => PDF); }
			set { Set(() => PDF, value); }
		}

		public override string ToString()
		{
			return ReportRunDate.ToShortDateString();
		}
	}
}

