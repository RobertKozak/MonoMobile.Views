using MVVMExample;
namespace DealerCenter
{
	using MonoMobile.MVVM;	

	public class Application : MonoMobileApplication
	{
		public new static void Main(string[] args)
		{
#if Pieceable
			args = new string[] { "", "-RegisterForSystemEvents" };
#endif
			ResumeFromBackgroundAction = () => { PresentModelView(typeof(ReloginView)); };
			Run("DealerCenter", typeof(InventoryListView), args);
		}
	}
}

