using CarController.Views;
using Xamarin.Forms;

namespace CarController
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(CarPlannerPage), typeof(CarPlannerPage));
			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
			Routing.RegisterRoute(nameof(BluetoothSettingsPage), typeof(BluetoothSettingsPage));
		}
	}
}