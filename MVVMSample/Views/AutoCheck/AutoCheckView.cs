using System;
using MonoMobile.MVVM;
using System.Threading;
using System.Collections.Generic;

namespace DealerCenter
{
	[Theme(typeof(DealerCenterTheme))]
	public class AutoCheckView : View
	{		
	[Section]

		public AutoCheckVehicleView AutoCheck;

		[List(ViewType = typeof(AutoCheckRecordSummaryView))]
		public AutoCheckRecordSummary RecordSummary;
		
	[Section]

		[Root(ViewType = typeof(AutoCheckHistoryView))]
		public AutoCheckHistoryViewModel History;
		
		public override void Initialize()
		{
		}
	}
}

