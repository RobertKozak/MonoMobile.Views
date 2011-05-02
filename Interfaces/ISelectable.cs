namespace MonoMobile.MVVM
{
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public interface ISelectable
	{
		/// <summary>
		/// Invoked when the given element has been tapped by the user.
		/// </summary>
		/// <param name="dvc">
		/// The <see cref="DialogViewController"/> where the selection took place
		/// </param>
		/// <param name="tableView">
		/// The <see cref="UITableView"/> that contains the element.
		/// </param>
		/// <param name="path">
		/// The <see cref="NSIndexPath"/> that contains the Section and Row for the element.
		/// </param>
		void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path);
	}
	
	public interface IListSelectable
	{
		/// <summary>
		/// Invoked when the given element has been tapped by the user.
		/// </summary>
		/// <param name="listView">
		/// The <see cref="ListView"/> where the selection took place
		/// </param>
		/// <param name="tableView">
		/// The <see cref="UITableView"/> that contains the element.
		/// </param>
		/// <param name="path">
		/// The <see cref="NSIndexPath"/> that contains the Section and Row for the element.
		/// </param>
		void Selected(ListView listView, NSIndexPath path);
	}
	
	
}

