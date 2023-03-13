namespace Xamarin.Essentials
{
	public static class AppInfo
	{
		public static string PackageName { get; } = System.AppDomain.CurrentDomain.FriendlyName;
	}
}
