namespace Samples
{
	using MonoMobile.MVVM;
	
	public class Application : MonoMobileApplication
	{
		static void Main(string[] args)
		{
			Run("Sample", typeof(TestView), args);
		//	Run("AppDelegate", args);
		}	
	}
}

