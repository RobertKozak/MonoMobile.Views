//
// UICommand.cs: Generic Command implementation
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

	public class UICommand<T>: ICommand
	{
		private Action<T> _ExecuteMethod { get ; set; }
		private Func<T, bool> _CanExecuteMethod { get; set;}

		public UICommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
		{
			if (executeMethod == null)
				throw new ArgumentNullException("Execute method cannot be null");

			_ExecuteMethod = executeMethod;
			_CanExecuteMethod = canExecuteMethod;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(T parameter)
		{
			if (_CanExecuteMethod == null)
				return true;

			return _CanExecuteMethod(parameter);
		}

		public void Execute(T parameter)
		{
			_ExecuteMethod(parameter);
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			Execute((T)parameter);
		}
	}
}

