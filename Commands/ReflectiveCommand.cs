//
// ReflectiveCommand.cs: ICommand implementation using a MethodInfo for Execute method
// and PropertyInfo for CanExecute
//
// Author:
//   Robert Kozak (rkozak@nowcom.com)
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
namespace MonoTouch.MVVM
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

		public event EventHandler CanExecuteChanged = (sender, e) => { };

		public ReflectiveCommand(object viewModel, MethodInfo execute, PropertyInfo canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_ViewModel = viewModel;
			_Execute = execute;
			_CanExecute = canExecute;

			var notifier = viewModel as INotifyPropertyChanged;
			if (notifier != null && _CanExecute != null)
			{
				notifier.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == _CanExecute.Name)
						CanExecuteChanged(this, EventArgs.Empty);
				};
			}
		}

		public bool CanExecute(object parameter)
		{
			if (_CanExecute != null)
				return (bool)_CanExecute.GetValue(_ViewModel, null);

			return true;
		}

		public void Execute(object parameter)
		{
			if (CanExecute(parameter))
			{
				if (_Execute.GetParameters().Any())
					_Execute.Invoke(_ViewModel, new object[] { parameter });
				else
					_Execute.Invoke(_ViewModel, null);
			}
		}
	}
}

