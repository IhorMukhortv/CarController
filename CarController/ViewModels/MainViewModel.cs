using CarController.Infrastructure.Background;
using CarController.Infrastructure.Providers;
using CarController.Models;
using CarController.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarContol.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private string _errors;

		private bool _isLoading = false;

		private IList<DirectionType> _sendCommandsList = new List<DirectionType>() { DirectionType.None, DirectionType.None };

		private readonly DataSender _dataSender;

		public MainViewModel()
		{
			_dataSender = new DataSender(async () => await SendCommandsAsync());
			Initialize();
		}

		public ICommand RightLeftCommand => new Command((x) => ClickedOnDirectionAsync((DirectionType)x, false));

		public ICommand UpDownCommand => new Command((x) => ClickedOnDirectionAsync((DirectionType)x, true));

		public ICommand RefreshCommand => new Command((x) => RefreshAsync());

		public string Errors
		{
			get => _errors;
			set => SetProperty(() => Errors, ref _errors, value);
		}

		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(() => IsLoading, ref _isLoading, value);
		}

		private async Task<bool> Initialize()
		{
			if (await InitialaizeConnection())
			{
				//_dataSender.StartSending();
			}

			return true;
		}

		private async Task<bool> InitialaizeConnection()
		{
			Errors = string.Empty;
			if (BluetoothConnectionProvider.IsConnected)
			{
				return true;
			}

			IsLoading = true;

			try
			{
				return await BluetoothConnectionProvider.ConnectToDeviceFromPreferences();
			}
			catch (Exception ex)
			{
				Errors = ex.Message;
				return false;
			}
			finally
			{
				IsLoading = false;
			}
		}

		private async void RefreshAsync()
		{
			IsLoading = true;
			await InitialaizeConnection();
			_dataSender.StartSending();
			IsLoading = false;
		}

		private async void ClickedOnDirectionAsync(DirectionType direction, bool isUpDown)
		{
			var indexChanged = isUpDown ? 0 : 1;
			_sendCommandsList[indexChanged] = direction;
			await SendCommandInternalAsync();
		}		

		private async Task SendCommandsAsync()
		{
			while (true)
			{
				await SendCommandInternalAsync();
			}
		}

		private async Task SendCommandInternalAsync()
		{
			var sendCommand = string.Join(";", _sendCommandsList.Select(x => ((int)x).ToString()));
			if (!await BluetoothConnectionProvider.SendMessageAsync(sendCommand))
			{
				_dataSender.StopSending();
			}

			await Task.Delay(200);
		}
	}
}