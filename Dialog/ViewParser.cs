// 
// ViewParser.cs
// 
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
// Copyright 2011, Nowcom Corporation.
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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	public class ViewParser : NSObject
	{
		private static readonly CommandBarButtonItem _LeftFlexibleSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Location = BarButtonLocation.Left };
		private static readonly CommandBarButtonItem _RightFlexibleSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Location = BarButtonLocation.Right };
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public UITableViewSource Parse(DialogViewController controller, object view, MemberInfo member)
		{
			UITableViewSource source = null;
			
			if (view != null)
			{
				view = GetActualView(view);
				controller.RootView = view;

				controller.ToolbarButtons = CheckForToolbarItems(view);
				controller.NavbarButtons = CheckForNavbarItems(view);

				if (member != null)
				{
					using (var memberData = new MemberData(view, member))
					{
						source = ParseList(controller, memberData, null); 
					}
				}
					
				if (source == null)
				{
					source = ParseView(controller, view);
				}

				InitializeSearch(view, source);
			}
			
			return source;
		}
		
		public UITableViewSource ParseView(DialogViewController controller, object view)
		{
			var members = view.GetType().GetMembers(false);

			var sections = new SortedList<int, Section>();
			var memberLists = new SortedList<int, SortedList<int, MemberData>>();
			var sectionIndex = 0;
			var memberOrder = 0;
			
			foreach (var member in members)
			{
				var attributes = member.GetCustomAttributes(false); 

				var memberData = new MemberData(view, member) { Section = sectionIndex };				

				var defaultValueAttribute = member.GetCustomAttribute<DefaultValueAttribute>();
				if (defaultValueAttribute != null)
				{
					memberData.Value = defaultValueAttribute.Value;
				}

				var pullToRefreshAttribute = member.GetCustomAttribute<PullToRefreshAttribute>();
				if (pullToRefreshAttribute != null)
				{
					((DialogViewController)controller).PullToRefreshCommand = GetCommandForMember(view, member);
					((DialogViewController)controller).RefreshKey = pullToRefreshAttribute.SettingsKey;
					((DialogViewController)controller).EnablePullToRefresh = true;
				}

				var toolbarButtonAttribute = member.GetCustomAttribute<ToolbarButtonAttribute>();
				var navbarButtonAttribute = member.GetCustomAttribute<NavbarButtonAttribute>();
				var skipAttribute = member.GetCustomAttribute<SkipAttribute>();

				if (skipAttribute != null || 
					toolbarButtonAttribute != null || 
					navbarButtonAttribute != null || 	
					pullToRefreshAttribute != null || 
					(attributes.Length == 0 && typeof(MethodInfo) == memberData.Type))
				{
					memberData.Dispose();
					continue;
				}
				
				var themeAttribute = member.GetCustomAttribute<ThemeAttribute>();
				if (themeAttribute != null)
				{
					var theme = Theme.CreateTheme(themeAttribute.ThemeType);
					if (theme != null && theme.CellHeight > 0)
					{
						memberData.RowHeight = theme.CellHeight;
						theme.Dispose();
					}
				}
				else
				{
					var themeable = view as IThemeable;
					if (themeable != null && themeable.Theme != null && themeable.Theme.CellHeight > 0)
					{
						memberData.RowHeight = themeable.Theme.CellHeight;
					}
				}

				var rowHeightAttribute = member.GetCustomAttribute<RowHeightAttribute>();
				if (rowHeightAttribute != null)
					memberData.RowHeight = rowHeightAttribute.RowHeight;
				
				var listAttribute = member.GetCustomAttribute<ListAttribute>();
				var isList = (listAttribute != null && listAttribute.DisplayMode == DisplayMode.List) && 
					!typeof(string).IsAssignableFrom(memberData.Type) && 
					(typeof(IEnumerable).IsAssignableFrom(memberData.Type) || 
					typeof(Enum).IsAssignableFrom(memberData.Type));
				
				var orderAttribute = member.GetCustomAttribute<OrderAttribute>();
				if (orderAttribute != null)
				{
					// make sure assigned order is an even number to fit in between the default order 
					// allowing the values int.MinValue and int.MaxValue for the first and Last positions
					memberData.Order = orderAttribute.Order > int.MaxValue / 2 ? int.MaxValue : orderAttribute.Order * 2;
				}
				else
				{				
					// make sure all default memberOrder is odd;
					memberOrder = memberOrder + (memberOrder % 2) + 1;
					memberData.Order = memberOrder;
				}

				var sectionAttribute = member.GetCustomAttribute<SectionAttribute>();
				if (sectionAttribute != null || isList)
				{
					if (sections.Count > 0)
					{
						sectionIndex++;
					}

					memberData.Section = sectionIndex;

					if (sectionAttribute != null && orderAttribute != null)
					{
						memberData.Section = orderAttribute.Order == 0 ? sectionIndex : orderAttribute.Order;
					}
					else
					{
						memberData.Section = sectionIndex;
					}
				}
				
				var viewTypes = GetViewTypes(memberData);

				if (!sections.ContainsKey(memberData.Section))
				{
					var section = CreateSection(controller, memberData, viewTypes);
					sections.Add(memberData.Section, section);
				}
				else
				{ 
					if (viewTypes != null)
					{
						IList<Type> list = null;
						var key = memberData.Id.ToString();
	
						var viewTypesList = sections[memberData.Section].ViewTypes;
						if (viewTypesList.ContainsKey(key))
						{
							list = viewTypesList[key];
						}
						else
						{
							list = new List<Type>();
							viewTypesList.Add(key, list);
						}
	
						foreach(var viewType in viewTypes)
						{
							if (!list.Contains(viewType))
							{
								list.Add(viewType);
							}	
						}
					}
				}

				if (memberLists.ContainsKey(memberData.Section))
				{
					memberLists[memberData.Section].Add(memberData.Order, memberData); 
				}
				else
				{
					var sortedList = new SortedList<int, MemberData>();
					sortedList.Add(memberData.Order, memberData);
					memberLists.Add(memberData.Section, sortedList);
				}
			}
			
			foreach(var kvp in memberLists)
			{
				var listSources = new SortedList<int, ListSource>();	

				var index = 0;
				var list = kvp.Value.Values.ToList();
				list.ForEach(data => data.Order = index++);

				foreach(var memberData in list)
				{
					var viewTypes = GetViewTypes(memberData);

					if ((!typeof(string).IsAssignableFrom(memberData.Type) && typeof(IEnumerable).IsAssignableFrom(memberData.Type)) || typeof(Enum).IsAssignableFrom(memberData.Type))
					{
						var listSource = ParseList(controller, memberData, viewTypes) as ListSource; 

						listSource.MemberData = memberData;
						listSource.Sections[0].Index = memberData.Section;

						listSources.Add(memberData.Order, listSource);
					}
					else
						listSources.Add(memberData.Order, null);
		
					sections[memberData.Section].ListSources = listSources;
					sections[memberData.Section].Index = memberData.Section;

					var lastListSource = listSources.Values.Last();
					if (lastListSource != null)
						memberData.DataContextBinder = new DataContextBinder(controller, lastListSource.Sections[0]);
				}

				sections[kvp.Key].DataContext = list;
			}
			
			var keyIndex = 0;
			var sectionList = sections.Select(kvp => kvp.Value).ToDictionary((value) => keyIndex++);

			// If there is only one list property return the ListSource rather than create a ViewSource
			if (sectionList.Count == 1 && sectionList[0].DataContext.Count == 1 && sectionList[0].ListSources[0] != null && !sectionList[0].ListSources[0].IsRootCell)
			{
				sectionList[0].ListSources[0].TableViewStyle = UITableViewStyle.Plain;
				return sectionList[0].ListSources[0];
			}

			var source = new ViewSource(controller) { Sections = sectionList };

			return source;
		}

		public static UITableViewSource ParseList(DialogViewController controller, MemberData memberData, List<Type> viewTypes)
		{
			object memberValue = memberData.Value;
			var member = memberData.Member;
			var type = memberData.Type;

			var isList = typeof(IEnumerable).IsAssignableFrom(type) || typeof(Enum).IsAssignableFrom(type);
			if (isList)
			{
				var data = type.CreateGenericListFromEnumerable(memberValue);
				var source = new ListSource(controller, (IList)data, viewTypes);

				if (source != null)
				{
					if (type.IsEnum)
					{
						source.SelectedItem = memberValue;
						source.SelectedItems.Add(source.SelectedItem);
					}
					else if (data != null && data.Count == 1)
					{
						source.SelectedItem = data[0];
						source.SelectedItems.Add(source.SelectedItem);
					}

					source.Caption = GetCaption(member);

					var listAttribute = member.GetCustomAttribute<ListAttribute>();
					if (listAttribute != null)
					{
						source.DisplayMode = listAttribute.DisplayMode;
						source.SelectionAction = listAttribute.SelectionAction;

						source.IsMultiselect = listAttribute.SelectionAction == SelectionAction.Multiselection;
						source.IsSelectable = source.SelectionAction != SelectionAction.NavigateToView;
						source.IsNavigable = listAttribute.DisplayMode != DisplayMode.Collapsable || listAttribute.SelectionAction == SelectionAction.NavigateToView;

						source.SelectedAccessoryViewType = listAttribute.SelectedAccessoryViewType;
						source.UnselectedAccessoryViewType = listAttribute.UnselectedAccessoryViewType;
						source.UnselectionBehavior = listAttribute.UnselectionBehavior;

						source.ReplaceCaptionWithSelection = listAttribute.ReplaceCaptionWithSelection;
						
						if (!string.IsNullOrEmpty(listAttribute.SelectedItemMemberName)) 
						{
							source.SelectedItemMemberName = listAttribute.SelectedItemMemberName;
						}
						if (!string.IsNullOrEmpty(listAttribute.SelectedItemsMemberName))
						{
							source.SelectedItemsMemberName = listAttribute.SelectedItemsMemberName;
						}
					}						
	
					source.PopOnSelection = source.SelectionAction == SelectionAction.PopOnSelection;
					
					var memberAttributes = member.GetCustomAttributes(false);
					foreach(var memberAttribute in memberAttributes)
					{
						var navigable = memberAttribute as INavigable;
						if (navigable != null)
						{
							source.IsSelectable = false;
							source.NavigationViewType = navigable.NavigateToViewType;
						}
					}

					source.IsRootCell = source.DisplayMode != DisplayMode.List;

					return source;
				}
			}

			return null;
		}
		
	    public static ReflectiveCommand GetCommandForMember(object view, MemberInfo member)
		{
			string propertyName = string.Empty;
			PropertyInfo propertyInfo = null;
			var commandOption = CommandOption.Disable;

			var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();
			if (buttonAttribute != null)
			{
				propertyName = buttonAttribute.CanExecutePropertyName;
				commandOption = buttonAttribute.CommandOption;
			}

			var toolbarButtonAttribute = member.GetCustomAttribute<ToolbarButtonAttribute>();
			if (toolbarButtonAttribute != null)
			{
				propertyName = toolbarButtonAttribute.CanExecutePropertyName;
				commandOption = toolbarButtonAttribute.CommandOption;
			}

			var navbarButtonAttribute = member.GetCustomAttribute<NavbarButtonAttribute>();
			if (navbarButtonAttribute != null)
			{
				propertyName = navbarButtonAttribute.CanExecutePropertyName;
				commandOption = navbarButtonAttribute.CommandOption;
			}

			var methodInfo = member as MethodInfo;

			if (methodInfo == null)
				throw new Exception(string.Format("Method not found {0}", member.Name));
			
			object source = view;
			if (!string.IsNullOrEmpty(propertyName))
			{
				PropertyInfo property = source.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				
				if (property == null)
				{
					var dataContext = view as IDataContext<object>;
					if (dataContext != null)
					{
						var vm = dataContext.DataContext;
						if (vm != null)
						{
							source = vm;
							property = source.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
						}
					}
				}

				if (property != null)
				{
					if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
					{
						propertyInfo = property;
					}
					else
					{
						throw new Exception(string.Format("Property {0} cannot be used for CanExecute property because it does not have a return type of bool", property.Name));
					}
				}
			}

			return new ReflectiveCommand(view, methodInfo, source, propertyInfo) { CommandOption = commandOption };
		}

		public List<CommandBarButtonItem> CheckForToolbarItems(object view)
		{
			var buttonList = new List<CommandBarButtonItem>();
			var members = GetMethods(view);

			foreach(var member in members)
			{
				var buttonAttribute = member.GetCustomAttribute<ToolbarButtonAttribute>();
				var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();

				if (buttonAttribute != null)
				{
					var caption = captionAttribute != null ? captionAttribute.Caption : !buttonAttribute.ButtonType.HasValue && buttonAttribute.CellViewType == null ? member.Name.Capitalize() : null;
					
					UIView buttonView = null;
					var title = caption;
					if (buttonAttribute.CellViewType != null)
					{	
						buttonView = Activator.CreateInstance(buttonAttribute.CellViewType) as UIView;
						
						CheckForInstanceProperties(view, member, buttonView);

						var tappable = buttonView as ICommandButton;
						if (tappable != null)
						{
							tappable.Command = GetCommandForMember(view, member); 
							tappable.Command.CanExecuteChanged += HandleCanExecuteChanged;
						}
					}

					var button = CreateCommandBarButton(view, member, title, buttonView, buttonAttribute.Style, buttonAttribute.ButtonType, buttonAttribute.Location);
					
					if (button != null)
					{		
						if (button.Command != null)
						{
							button.Command.CanExecuteChanged += HandleCanExecuteChanged;
						}

						if (button.Location == BarButtonLocation.Center)
							buttonList.Add(_LeftFlexibleSpace);

						buttonList.Add(button);

						if (button.Location == BarButtonLocation.Center)
							buttonList.Add(_RightFlexibleSpace);
					}
				}
			}
			
			if (buttonList.Count > 0)
			{
				var sortedList = buttonList.OrderBy(button=>button.Order).Distinct().ToList();
				return sortedList;
			}	

			return null;
		}
	
		public List<CommandBarButtonItem> CheckForNavbarItems(object view)
		{
			var buttonList = new List<CommandBarButtonItem>();
			var members = GetMethods(view);
			foreach(var member in members)
			{
				var buttonAttribute = member.GetCustomAttribute<NavbarButtonAttribute>();
				var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();
				var caption = captionAttribute != null ? captionAttribute.Caption : null;
				
				if (buttonAttribute != null)
				{
					var memberName = buttonAttribute.ButtonType.HasValue ? null : member.Name.Capitalize();

					var title = caption ?? memberName;

					if (buttonAttribute.CellViewType != null)
					{	
						UIView buttonView = Activator.CreateInstance(buttonAttribute.CellViewType) as UIView;
						
						CheckForInstanceProperties(view, member, buttonView);

						var tappable = buttonView as ICommandButton;
						if (tappable != null)
						{
							tappable.Command = GetCommandForMember(view, member); 
							tappable.Command.CanExecuteChanged += HandleCanExecuteChanged;
						}
					}

					var button = CreateCommandBarButton(view, member, title, null, buttonAttribute.Style, buttonAttribute.ButtonType, buttonAttribute.Location);
					
					if (button != null)
					{
						if (button.Command != null)
						{
							button.Command.CanExecuteChanged += HandleCanExecuteChanged;
						}

						buttonList.Add(button);
					}
				}
			}
			
			if (buttonList.Count > 0)
			{
				var sortedList = buttonList.OrderBy(button=>button.Tag).ToList();
				return sortedList;
			}	

			return null;
		}

		public static string GetCaption(MemberInfo member)
		{
			var caption = member.Name;
			var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();

			if (captionAttribute != null)
			{
				caption = captionAttribute.Caption;
			}
			else
			{
				if (caption == "DataContext")
				{
					var propertyInfo = member as PropertyInfo;
					if (propertyInfo != null)
					{
						caption = propertyInfo.PropertyType.Name.Split('.').LastOrDefault();
					}
	
					var fieldInfo = member as FieldInfo;
					if (fieldInfo != null)
					{
						caption = fieldInfo.FieldType.Name.Split('.').LastOrDefault();
					}
				}

				caption = caption.Capitalize();
			}

			return caption;
		}
		
		private static Section CreateSection(DialogViewController controller, MemberData memberData, List<Type> viewTypes)
		{
			var listSources = new SortedList<int, ListSource>();
			listSources.Add(memberData.Order, null);

			var memberOrder = 0;
			memberData.Order = memberOrder;

			var sectionMembers = new List<MemberData>();
					
			var section = new Section(controller) { DataContext = sectionMembers };
			
			var sectionAttribute = memberData.Member.GetCustomAttribute<SectionAttribute>();
			if (sectionAttribute != null)
			{
				section.HeaderText = sectionAttribute.Caption;
				section.FooterText = sectionAttribute.Footer;
			}
			section.ViewTypes.Add(memberData.Id.ToString(), viewTypes);
			return section;
		}
		
		private static CommandBarButtonItem CreateCommandBarButton(object view, MemberInfo member, string title, UIView buttonView, UIBarButtonItemStyle style, UIBarButtonSystemItem? buttonType, BarButtonLocation location)
		{
			CommandBarButtonItem button = null;

			ReflectiveCommand command = null;
			var methodInfo = member as MethodInfo;

			if(methodInfo != null)
				command = GetCommandForMember(view, member);

			if (!string.IsNullOrEmpty(title))
			{
				button = new CommandBarButtonItem(title, style, (sender, e) => command.Execute(null));
			}
			else if (buttonView != null)
			{
				button = new CommandBarButtonItem(buttonView); 
			}
			else
			{
				if (!buttonType.HasValue)
					buttonType = UIBarButtonSystemItem.Done;

				button = new CommandBarButtonItem(buttonType.Value,  (sender, e) => command.Execute(null));
				button.Style = style;
			}
		
			command.CommandButton = button;
			button.Enabled = true;
			button.Location = location;
			button.Command = command;

			var orderAttribute = member.GetCustomAttribute<OrderAttribute>();
			if (orderAttribute != null)
				button.Order = orderAttribute.Order;
			else 
				button.Order = 0;

			return button;
		}

		private static void InitializeSearch(object view, UITableViewSource source)
		{
			var searchbarAttribute = view.GetType().GetCustomAttribute<SearchbarAttribute>();
			var searchbar = source as ISearchBar;
			if (searchbarAttribute != null && searchbar != null)
			{
				searchbar.SearchPlaceholder = searchbarAttribute.Placeholder;
				searchbar.IncrementalSearch = searchbarAttribute.IncrementalSearch;
				searchbar.EnableSearch = searchbarAttribute.ShowImmediately;
				searchbar.IsSearchbarHidden = !searchbarAttribute.ShowImmediately;
		
					
				var methods = GetMethods(view);
				foreach (var method in methods)
				{
					var attribute = method.GetCustomAttribute<SearchbarAttribute>();
					if (attribute != null)
					{
						searchbar.SearchPlaceholder = attribute.Placeholder;
						searchbar.IncrementalSearch = attribute.IncrementalSearch;
						searchbar.EnableSearch = attribute.ShowImmediately;
						searchbar.IsSearchbarHidden = !attribute.ShowImmediately;

						searchbar.SearchCommand = new SearchCommand(view, method as MethodInfo);
						break;
					};
				}
			}
		}
		
		private static object GetActualView(object view)
		{
			if (view != null && !(view is IView))
			{
				var type = view.GetType();
				var actualView = ViewContainer.GetExactView(type);
				
				if (actualView == null)
				{
					var viewAttribute = type.GetCustomAttribute<ViewAttribute>();
					if (viewAttribute != null)
					{
						actualView = viewAttribute.ViewType;
					}
				}

				if (actualView != null)
				{
					var newView = Activator.CreateInstance(actualView);
					var dc = newView as IDataContext<object>;
					if (dc != null)
					{
						dc.DataContext = view;
					}

					return newView;
				}
			}

			return view;
		}

		private static List<Type> GetViewTypes(MemberData memberData)
		{
			List<Type> viewTypeList = new List<Type>();

			var memberInfo = memberData.Member;
			if (memberInfo != null)
			{
				var cellViewAttributes = memberInfo.GetCustomAttributes<CellViewAttribute>();
				if (cellViewAttributes.Length > 0)
				{
					viewTypeList = (from attribute in cellViewAttributes select attribute.ViewType).ToList();
					var viewAttributesList = cellViewAttributes.ToList();
					viewAttributesList.ForEach((attribute) => 
					{
						var sizeable = attribute as ISizeable;
						if (sizeable != null)
						{
							memberData.RowHeight = sizeable.GetRowHeight();
						}
					});
				}

				var cellViewTemplates = memberInfo.GetCustomAttributes<Attribute>().Where(attribute=> typeof(CellViewTemplate).IsAssignableFrom(attribute.GetType())).Cast<CellViewTemplate>().ToList();
				if (cellViewTemplates.Count > 0)
				{
					cellViewTemplates.ForEach(attribute=>viewTypeList.Add(attribute.CellViewType));
				}
			}

			if (viewTypeList.Count == 0)
				return null;

			return viewTypeList;
		}

		private static MemberInfo[] GetMethods(object view)
		{
			return GetMethods(view.GetType()); 
		}

		private static MemberInfo[] GetMethods(Type type)
		{
			return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(m =>
			{
				var methodInfo = m as MethodBase;
				return (methodInfo == null || !methodInfo.IsConstructor && !methodInfo.IsSpecialName);
			}).ToArray();
		}

		private static void CheckForInstanceProperties(object view, MemberInfo member, UIView elementView)
		{
			var cellViewTemplate = member.GetCustomAttribute<CellViewTemplate>(true);
			if (cellViewTemplate != null)
			{
				if (!string.IsNullOrEmpty(cellViewTemplate.InstancePropertyName))
				{
					var instanceProperty = view.GetType().GetProperty(cellViewTemplate.InstancePropertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
					if (instanceProperty != null)
					{
						UIView instanceView = elementView;

						instanceProperty.SetValue(view, instanceView);
					}
				}				
			}
		}
		
		public void HandleCanExecuteChanged(object sender, EventArgs e)
		{
			var reflectiveCommand = sender as ReflectiveCommand;
			if (reflectiveCommand != null)
			{
				if (reflectiveCommand.CommandOption == CommandOption.Hide)
				{
					reflectiveCommand.CommandButton.Hidden = !reflectiveCommand.CanExecute(null);
				}
				else
				{
					reflectiveCommand.CommandButton.Enabled = reflectiveCommand.CanExecute(null);
				}

				MonoMobileApplication.CurrentDialogViewController.UpdateSource();
			}
		}
	}
}