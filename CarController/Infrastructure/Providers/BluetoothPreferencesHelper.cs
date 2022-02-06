using CarController.Models.Settings;
using Xamarin.Essentials;

namespace CarController.Infrastructure.Providers
{
	public class BluetoothPreferencesHelper
	{
		private const string BluetoothConnectionIdKey = "BluetoothDeviceId";
		private const string BluetoothDeviceNameKey = "BluetoothDeviceName";

		public static BluetoothSettingsModel TryGetBluetoothSettings(out bool result, out string errorMessage)
		{
			var existConnectionId = Preferences.ContainsKey(BluetoothConnectionIdKey);
			var existDeviceName = Preferences.ContainsKey(BluetoothDeviceNameKey);

			if (!existConnectionId)
			{
				result = false;
				errorMessage = "Not exist connection";
				return null;
			}

			var connectionId = Preferences.Get(BluetoothConnectionIdKey, string.Empty);
			var deviceName = string.Empty;
			if (existDeviceName)
			{
				deviceName = Preferences.Get(BluetoothDeviceNameKey, string.Empty);
			}

			var resultModel = new BluetoothSettingsModel
			{
				ConnectionId = connectionId,
				DeviceName = deviceName
			};

			result = true;
			errorMessage = string.Empty;
			return resultModel;
		}

		public static bool SetBluetoothSettings(BluetoothSettingsModel value)
		{
			if (string.IsNullOrEmpty(value?.ConnectionId))
			{
				return false;
			}

			Preferences.Set(BluetoothConnectionIdKey, value.ConnectionId);

			if (!string.IsNullOrEmpty(value.DeviceName))
			{
				Preferences.Set(BluetoothDeviceNameKey, value.DeviceName);
			}

			return true;
		}
	}
}