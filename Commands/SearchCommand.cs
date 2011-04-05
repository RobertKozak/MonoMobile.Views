namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Dialog;

	public class SearchCommand
	{
		private readonly MethodInfo _Execute;
		private readonly object _ViewModel;
		
		public SearchCommand(object viewModel, MethodInfo execute)
		{
			_ViewModel = viewModel;
			_Execute = execute;
		}

		public List<ISection> Execute(ISection[] sections, string searchText)
		{
			List<ISection> result = null;
			
			try
			{
				if (_Execute.GetParameters().Any())
					result = _Execute.Invoke(_ViewModel, new object[] { sections, searchText }) as List<ISection>;
			}
			catch(Exception ex) 
			{
				throw new Exception(string.Format("{0} in {2}", ex.Message,_Execute.Name));
			}

			return result;
		}
	}
}

