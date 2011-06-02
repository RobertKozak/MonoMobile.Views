namespace MVVMSample
{
	using MonoMobile.MVVM;

	public class Application : MonoMobileApplication
	{
		public static void Main(string[] args)
		{
			Run("MVVM Sample", typeof(TestView), args);
		}
	}
}

