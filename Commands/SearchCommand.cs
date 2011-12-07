//
// SearchCommand.cs:
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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MonoMobile.Views;

	public class SearchCommand
	{
		private readonly MethodInfo _Execute;
		private readonly object _ViewModel;
		
		public SearchCommand(object viewModel, MethodInfo execute)
		{
			_ViewModel = viewModel;
			_Execute = execute;
		}

		public Dictionary<int, Section> Execute(IDictionary<int, Section> sections, string searchText)
		{
			Dictionary<int, Section> result = null;
			
			try
			{
				if (_Execute.GetParameters().Any())
					result = _Execute.Invoke(_ViewModel, new object[] { sections, searchText }) as Dictionary<int, Section>;
			}
			catch(Exception ex) 
			{
				throw new Exception(string.Format("{0} in {2}", ex.Message,_Execute.Name));
			}

			return result;
		}
	}
}

