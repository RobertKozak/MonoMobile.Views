using System.Drawing;
using System.Text;
using MonoMobile.MVVM;
using MonoTouch.Foundation;
using MonoTouch.MessageUI;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DealerCenter
{
	public class AutoCheckConfirmationView : ActionSheetView
	{
		private AutoCheckView _AutoCheckView;

		protected InventoryItemViewModel Inventory { get; set; }

		public AutoCheckConfirmationView(string title, InventoryItemViewModel inventory) : base(title)
		{
			Inventory = inventory;
		}

		[Button]
		[Caption("Yes")]
		public void AutoCheckReport(int buttonIndex)
		{
			var progress = new ProgressHud() { TitleText = "Retrieving AutoCheck History Report" };
			progress.ShowWhileExecuting(() =>
			{
				Thread.Sleep(2000);
				ShowAutoCheckView();

			}, true);
		}
		
		public void ShowAutoCheckView()
		{
			CreateAutoCheckReport();
			_AutoCheckView = new AutoCheckView() { DataContext = Inventory.AutoCheck, Caption = "AutoCheck" };
			
			Application.PushView(_AutoCheckView, true);
		}

		private void CreateAutoCheckReport()
		{
			Inventory.AutoCheck = new AutoCheckViewModel();
			Inventory.AutoCheck.Inventory = Inventory;
			Inventory.AutoCheck.CountryOfAssembly = "USA";
	
			Inventory.AutoCheck.RecordSummary = new AutoCheckRecordSummary()
			{
				State = "CA",
				TitleNumber = "16E100721811",
				EventDate = new DateTime(2010, 11, 08),
				Odometer = 57454,
				NumberOfHistoricalRecords = 22
			};
	
			Inventory.AutoCheck.History = new AutoCheckHistoryViewModel()
			{
				ReportRunDate = new DateTime(2011, 6, 10),
				ProblemHistory = new List<AutoCheckHistoryItem>() 
				{ 
					new AutoCheckHistoryItem() {  Description = "Abandoned", Result = false}, 
					new AutoCheckHistoryItem() {  Description = "Damaged or Major damage incident", Result = false }, 
					new AutoCheckHistoryItem() {  Description = "Fire Damage", Result = false },
					new AutoCheckHistoryItem() {  Description = "Grey Market", Result = false },
					new AutoCheckHistoryItem() {  Description = "Hail Damage", Result = false },
					new AutoCheckHistoryItem() {  Description = "Insurance or probable total loss", Result = false },
					new AutoCheckHistoryItem() {  Description = "Junk or scraped", Result = false },
					new AutoCheckHistoryItem() {  Description = "Manufactureer Buyback/Lemon", Result = false },
					new AutoCheckHistoryItem() {  Description = "Odomometer rollback/rollover", Result = false },
					new AutoCheckHistoryItem() {  Description = "Odomometer problem", Result = false },
					new AutoCheckHistoryItem() {  Description = "Rebuilt/Rebuildable", Result = false },
					new AutoCheckHistoryItem() {  Description = "Salvage or salvage auction", Result = false },
					new AutoCheckHistoryItem() {  Description = "Water damage", Result = false },
					new AutoCheckHistoryItem() {  Description = "NHTSA crash test vehicle", Result = false },
					new AutoCheckHistoryItem() {  Description = "Frame damage", Result = true },
					new AutoCheckHistoryItem() {  Description = "Recycled", Result = true },
				},
			
				OtherHistory = new List<AutoCheckHistoryItem>() 
				{  
					new AutoCheckHistoryItem() { Description = "Failed last reported emission test", Result = false }, 
					new AutoCheckHistoryItem() { Description = "Theft", Result = false},
					new AutoCheckHistoryItem() { Description = "Theft Recovered", Result = false},
					new AutoCheckHistoryItem() { Description = "Canadian Registration", Result = false},
					new AutoCheckHistoryItem() { Description = "Storm area title/registration", Result = false},
					new AutoCheckHistoryItem() { Description = "Rental/Fleet", Result = true},
				}
			};

			//Thread.Sleep(2000);

			Inventory.AutoCheckBooked = true;
		}
		
		[Button]
		[Caption("No")]
		public override void Destroy()
		{
			base.Destroy();
		} 
	}
}

