using System;
using MonoMobile.MVVM;

namespace DealerCenter
{
	public class AutoCheckRecordSummary : ViewModel
	{
		public string State
		{
			get { return Get(() => State); }
			set { Set(() => State, value); }
		}

		public string TitleNumber
		{
			get { return Get(() => TitleNumber); }
			set { Set(() => TitleNumber, value); }
		}

		public DateTime EventDate
		{
			get { return Get(() => EventDate); }
			set { Set(() => EventDate, value); }
		}

		public int Odometer
		{
			get { return Get(() => Odometer); }
			set { Set(() => Odometer, value); }
		}

		public int NumberOfHistoricalRecords
		{
			get { return Get(() => NumberOfHistoricalRecords); }
			set { Set(() => NumberOfHistoricalRecords, value); }
		}
	}
}

