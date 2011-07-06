using System;
using MonoMobile.MVVM;
using MonoTouch.Foundation;

namespace DealerCenter
{
	[Preserve(AllMembers = true)]
	[Theme(typeof(RowHeightTheme))]
	public class AutoCheckRecordSummaryView : View
	{
		[Caption("State / Province")]
		public string State { get; set; }
		
		public string TitleNumber { get; set; }
		
		[Bind(ValueConverterType = typeof(DateTimeToStringConverter))]
		public string EventDate { get; set; }
		
		[Bind(ValueConverterType = typeof(IntConverter))]
		public string Odometer { get; set; }
		
		[Bind(ValueConverterType = typeof(IntConverter))]
		public string NumberOfHistoricalRecords { get; set; }
	}
}

