using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CarController.ViewModels.Base
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string Title { get; set; }

		protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
		{
			var memberExpression = (MemberExpression)projection.Body;
			OnPropertyChanged(memberExpression.Member.Name);
		}

		protected void OnPropertyChanged(string propName)
		{
			if (PropertyChanged == null)
			{
				return;
			}

			PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		protected void SetProperty<T>(Expression<Func<T>> propertyExpression, ref T backingField, T newValue)
		{
			if (backingField == null && newValue == null)
			{
				return;
			}

			if (backingField == null || !backingField.Equals(newValue))
			{
				backingField = newValue;
				OnPropertyChanged(propertyExpression);
			}
		}
	}
}