namespace MonoMobile.Views
{
	using System;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class PhotoPickerController : UIImagePickerController
	{
		class PhotoControllerDelegate : UIImagePickerControllerDelegate
		{
			private Action<UIImage> _SelectedAction;
			private UIImage _SelectedImage;

			public PhotoControllerDelegate(Action<UIImage> selectedAction)
			{
				_SelectedAction = selectedAction;
			}
		
			public override void FinishedPickingImage(UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
			{
				_SelectedImage = image;
				_SelectedAction(_SelectedImage);
			}
		}
		
		public PhotoPickerController(Action<UIImage> selectedAction)
		{
			Delegate = new PhotoControllerDelegate(selectedAction);
		}
	}
}

