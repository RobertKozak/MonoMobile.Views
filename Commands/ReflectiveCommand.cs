//
// ReflectiveCommand.cs: ICommand implementation using a MethodInfo for Execute method
// and PropertyInfo for CanExecute
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
//
// Copyright 2011, Nowcom Corporation
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace MonoMobile.Views
{
	using System;
	using System.Reflection;
	using System.ComponentModel;
	using System.Linq;

	public class ReflectiveCommand : ICommand
	{
		private readonly PropertyInfo _CanExecute;
		private readonly MethodInfo _Execute;
		private readonly object _ViewModel;
		private readonly object _CanExecuteSource;
		
		public CommandOption CommandOption { get; set; }

		public event EventHandler CanExecuteChanged = (sender, e) => { };

		public ReflectiveCommand(object viewModel, string executeMethodName, string canExecutePropertyName) : this(viewModel, executeMethodName, null, canExecutePropertyName)
		{			
		}
		
		public ReflectiveCommand(object viewModel, string executeMethodName, object canExecuteSource, string canExecutePropertyName) 
		{
			if (string.IsNullOrEmpty(executeMethodName))
				throw new ArgumentNullException("executeMethodName");

			var execute = viewModel.GetType().GetMethod(executeMethodName);
			
			PropertyInfo canExecute = null;
			if (!string.IsNullOrEmpty(canExecutePropertyName))
				canExecute = viewModel.GetType().GetProperty(canExecutePropertyName);
			
			_ViewModel = viewModel;
			_Execute = execute;
			_CanExecute = canExecute;
			_CanExecuteSource = canExecuteSource;

			CreateCommand();
		}

		public ReflectiveCommand(object viewModel, MethodInfo execute, PropertyInfo canExecute) : this(viewModel, execute, null, canExecute)
		{
		}

		public ReflectiveCommand(object viewModel, MethodInfo execute, object canExecuteSource, PropertyInfo canExecute)
		{
			_ViewModel = viewModel;
			_Execute = execute;
			_CanExecute = canExecute;
			_CanExecuteSource = canExecuteSource;

			CreateCommand();
		}

		private void CreateCommand()
		{
			if (_Execute == null)
				throw new Exception("The method supplied for the Command does not exist. Check spelling.");
			
			var notifier = _ViewModel as INotifyPropertyChanged;
			if (notifier == null)
			{
				var dataContext = _ViewModel as IDataContext<object>;
				if (dataContext != null)
				{
					notifier = dataContext.DataContext as INotifyPropertyChanged;
				}
			}

			if (notifier != null && _CanExecute != null)
			{
				notifier.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == _CanExecute.Name)
						OnCanExecuteChanged();
				};
			}
		}

		public void OnCanExecuteChanged()
		{					
			CanExecuteChanged(this, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			var source = _CanExecuteSource;
			if (source == null)
				source = _ViewModel;

			if (_CanExecute != null)
				return (bool)_CanExecute.GetValue(source, null);
			
			return true;
		}

		public object Execute(object parameter)
		{
			object result = null;
			
			try
			{
				if (CanExecute(parameter))
				{
					if (_Execute.GetParameters().Any())
						result = _Execute.Invoke(_ViewModel, new object[] { parameter });
					else
						result = _Execute.Invoke(_ViewModel, null);
				}
			}
			catch(TargetInvocationException ex)
			{
				throw new Exception(string.Format("{0} method has thrown an exception: {1}", ex.InnerException.TargetSite.Name, ex.InnerException.Message), ex);
			}

			return result;
		}
	}
}

