using Android.Bluetooth;
using CarController.Infrastructure.Providers;
using CarController.Models.Settings;
using CarController.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarContol.ViewModels
{
	public class BluetoothSettingsViewModel : BaseViewModel
	{
		private BluetoothSettingsModel _bluetoothSettingsModel;
		private ObservableCollection<BluetoothSettingsModel> _bluetoothSettingsModels;
		private string _errors;
		private string _info;

		private bool _isLoading = false;

		public BluetoothSettingsViewModel()
		{
			Initialize();
		}

		public ICommand InitializeBluetoothAdapterCommand => new Command((x) => RefresfSettings());
		public ICommand ConnectCommand => new Command(async (x) => await ConnectAsync());

		public ObservableCollection<BluetoothSettingsModel> BluetoothSettingsModels
		{
			get => _bluetoothSettingsModels ?? new ObservableCollection<BluetoothSettingsModel>();
			set => SetProperty(() => BluetoothSettingsModels, ref _bluetoothSettingsModels, value);
		}

		public BluetoothSettingsModel BluetoothSettingsModel
		{
			get => _bluetoothSettingsModel;
			set => SetProperty(() => BluetoothSettingsModel, ref _bluetoothSettingsModel, value);
		}

		public string Errors
		{
			get => _errors;
			set => SetProperty(() => Errors, ref _errors, value);
		}

		public string Info
		{
			get => _info;
			set => SetProperty(() => Info, ref _info, value);
		}

		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(() => IsLoading, ref _isLoading, value);
		}

		private async Task<bool> ConnectAsync()
		{
			if (BluetoothSettingsModel == null)
			{
				Errors = "Please select device";
				return false;
			}

			Errors = string.Empty;

			try
			{
				await BluetoothConnectionProvider.ConnectToDeviceAsync(_bluetoothSettingsModel.ConnectionId);
				BluetoothPreferencesHelper.SetBluetoothSettings(_bluetoothSettingsModel);
				Info = "Connected Success";
				return true;
			}
			catch (Exception ex)
			{
				Errors = ex.Message;
				return false;
			}
		}

		private async Task<bool> Initialize()
		{
			RefresfSettings();
			if (BluetoothPreferencesHelper.TryGetBluetoothSettings(out var result, out var error) is BluetoothSettingsModel setting && result)
			{
				BluetoothSettingsModel = setting;
				return await ConnectAsync();
			}

			return true;
		}

		private bool RefresfSettings()
		{
			IsLoading = true;

			try
			{
				return RefresfSettingsInternal();
			}
			finally
			{
				IsLoading = false;
			}
		}

		private bool RefresfSettingsInternal()
		{
			var adapter = BluetoothAdapter.DefaultAdapter;

			if (adapter == null || !adapter.IsEnabled)
			{
				Errors = "Please check your Bluetooth will be on";
				return false;
			}

			Errors = string.Empty;

			var devices = adapter.BondedDevices.Select(x => new BluetoothSettingsModel
			{
				ConnectionId = x.Address,
				DeviceName = x.Name
			});

			BluetoothSettingsModels = new ObservableCollection<BluetoothSettingsModel>(devices.ToList());

			var settings = BluetoothPreferencesHelper.TryGetBluetoothSettings(out var result, out var error);
			if (!result)
			{
				Errors = error;
				return false;
			}

			return true;
		}
	}
}