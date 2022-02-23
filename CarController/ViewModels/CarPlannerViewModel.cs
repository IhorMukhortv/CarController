using CarController.Infrastructure.Providers;
using CarController.Models;
using CarController.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarController.ViewModels
{
	public class CarPlannerViewModel : BaseViewModel
	{
		private bool _isLoading = false;
		private ObservableCollection<ActionModel> _actions = new ObservableCollection<ActionModel> { new ActionModel { Direction = DirectionType.Up, Distance = 124} };
		private ActionModel _selectedAction;
		private DirectionType _lastActionDirectionSelected = DirectionType.Up;
		private int _actionDistance;

		public CarPlannerViewModel()
		{
			Initialize();
		}

		public ICommand AddActionCommand => new Command(AddAction);

		public ICommand DeleteActionCommand => new Command((x) => DeleteAction(x as ActionModel));

		public ICommand ChangeActionDirectionCommand => new Command((x) => LastActionDirectionSelected = (DirectionType)x);

		public ObservableCollection<ActionModel> Actions
		{
			get => _actions ?? new ObservableCollection<ActionModel>();
			set => SetProperty(() => Actions, ref _actions, value);
		}

		public ActionModel SelectedAction
		{
			get => _selectedAction;
			set => SetProperty(() => SelectedAction, ref _selectedAction, value);
		}

		public int ActionDistance
		{
			get => _actionDistance;
			set => SetProperty(() => ActionDistance, ref _actionDistance, value);
		}

		public DirectionType LastActionDirectionSelected
		{
			get => _lastActionDirectionSelected;
			set => SetProperty(() => LastActionDirectionSelected, ref _lastActionDirectionSelected, value);
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
				return false;
			}
			finally
			{
				IsLoading = false;
			}
		}

		private void DeleteAction(ActionModel selectedAction)
		{
			if (_actions.Contains(selectedAction))
			{
				_actions.Remove(selectedAction);
			}
		}

		private void AddAction()
		{
			var newAction = new ActionModel
			{
				Direction = _lastActionDirectionSelected,
				Distance = ActionDistance
			};

			_actions.Add(newAction);
		}
	}
}