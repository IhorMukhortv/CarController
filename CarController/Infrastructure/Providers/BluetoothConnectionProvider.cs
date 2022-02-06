using Android.Bluetooth;
using Java.Util;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarController.Infrastructure.Providers
{
	public static class BluetoothConnectionProvider
	{
		private static BluetoothSocket bluetoothSocket;

		private static object LockObject => new object();

		public static bool IsConnected => bluetoothSocket?.IsConnected ?? false;

		public static async Task<bool> ConnectToDeviceFromPreferences()
		{
			var settings = BluetoothPreferencesHelper.TryGetBluetoothSettings(out var result, out var error);

			if (!result)
			{
				return false;
			}

			return await ConnectToDeviceAsync(settings.ConnectionId);
		}

		public static async Task<bool> ConnectToDeviceAsync(string connectionId)
		{
			if (IsConnected)
			{
				Disconnect();
			}

			var adapter = BluetoothAdapter.DefaultAdapter;
			var device = adapter.BondedDevices.FirstOrDefault(x => x.Address == connectionId);

			var uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
			BluetoothSocket _socket;

			if ((int)Android.OS.Build.VERSION.SdkInt >= 10) 
			{
				_socket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
			}
			else
			{
				_socket = device.CreateRfcommSocketToServiceRecord(uuid);
			}

			await _socket.ConnectAsync();

			bluetoothSocket = _socket;

			return true;
		}

		public static void Disconnect()
		{
			bluetoothSocket.Close();
		}

		public static async Task<bool> SendMessageAsync(string message)
		{
			if (!IsConnected)
			{
				return false;
			}

			await SendMessageInternalAsync(message);

			return true;
		}

		private static Task SendMessageInternalAsync(string message)
		{
			try
			{
				var cmd = Encoding.ASCII.GetBytes(message+Environment.NewLine);
				return bluetoothSocket.OutputStream.WriteAsync(cmd, 0, cmd.Length);
			}
			catch (Exception ex)
			{
				return Task.CompletedTask;
			}
		}

		private static void Reconect()
		{
			if (IsConnected)
			{
				return;
			}

			var device = bluetoothSocket.RemoteDevice;
			bluetoothSocket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
		}
	}
}