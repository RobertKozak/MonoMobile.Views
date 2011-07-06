namespace DealerCenter
{
	using System.Drawing;
	using System.Text;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.MessageUI;
	using MonoTouch.UIKit;

	public class SendActionSheetView : ActionSheetView
	{
		private bool ShowSendToDealerCenter { get; set; }
		private bool ShowAddToFavorites { get; set; }
		private bool ShowRemoveFromFavorites { get; set; }

		protected InventoryItemViewModel Inventory { get; set; }

		public SendActionSheetView(string title, InventoryItemViewModel inventory) : base(title)
		{
			Inventory = inventory;
			ShowAddToFavorites = !Inventory.Favorite; 
			ShowRemoveFromFavorites = Inventory.Favorite;
			ShowSendToDealerCenter = Inventory.IsLocal;
		}

		[Button("ShowSendToDealerCenter")]
		[Caption("Send To DealerCenter")]
		public virtual void SendToDealerCenter()
		{
			Alert.Show("", "Send to DealerCenter");
		}


		[Button]
		public virtual void SendToEmail()
		{
			if (MFMailComposeViewController.CanSendMail)
			{
				NSData photo = null;
				
				if (Inventory.Icon != null)
				{
					photo = Inventory.Icon.AsPNG();
				}
				
				var mailController = new MFMailComposeViewController();
				mailController.SetSubject(Inventory.ToString());
				mailController.SetMessageBody(GetMailBody(), false);
				
				if (photo != null)
					mailController.AddAttachmentData(photo, "image/png", string.Format("{0}.png", Inventory.ToString()));
				
				mailController.Finished += HandleMailControllerFinished;
				
				Application.NavigationController.PresentModalViewController(mailController, true);
			} 
			else
			{
				Alert.Show("No email account found", "Please add an email account in Settings.");
			}
		}

		[Button("ShowAddToFavorites")]
		public virtual void AddToFavorites()
		{
			Inventory.Favorite = true;
		}
 
		[Button("ShowRemoveFromFavorites")]
		public virtual void RemoveFromFavorites()
		{
			Inventory.Favorite = false;
		}

		[Button]
		public virtual void EditPhotos()
		{
			var picker = new PhotoPickerController(image => SelectedImage(image));
			Application.NavigationController.PresentModalViewController(picker, true);
		}
		[Button]
		public virtual void EditVehicle()
		{

		}
//		[Button]
//		public virtual void TakePhoto()
//		{
//			var picker = new PhotoPickerController(image => SelectedImage(image));
//			picker.SourceType = UIImagePickerControllerSourceType.Camera;
//
//			Application.NavigationController.PresentModalViewController(picker, true);
//		}

		[Button]
		public override void Cancel()
		{
		}
		
		private void SelectedImage(UIImage image)
		{
			Application.NavigationController.DismissModalViewControllerAnimated(true);
			
			if (image != null)
				Inventory.Icon = image.Scale(new SizeF(64, 64));

			Application.CurrentDialogViewController.View.SetNeedsDisplay();
		}

		private string GetMailBody()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("{0}\n", Inventory.ToString());
			sb.AppendFormat("{0}\n\n", Inventory.Trim);
			
			sb.AppendFormat("Mileage: {0}\n", Inventory.Mileage);
			sb.AppendFormat("Transmission: {0}\n", Inventory.Transmission);
			sb.AppendFormat("Drivetrain: {0}\n", Inventory.Drivetrain);
			sb.AppendFormat("Engine: {0}\n", Inventory.Engine);
			sb.AppendFormat("VIN: {0}\n\n", Inventory.Vin);
			sb.AppendFormat("Price: {0}", "$14,000");
			
			return sb.ToString();
		}

		private void HandleMailControllerFinished(object sender, MFComposeResultEventArgs e)
		{
			e.Controller.DismissModalViewControllerAnimated(true);
		}
	}
}

