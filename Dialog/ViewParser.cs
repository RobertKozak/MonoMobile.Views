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
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using MonoTouch.CoreLocation;
	using MonoTouch.UIKit;
	
	public class ViewParser
	{
		private class NoElement : Element
		{
			public NoElement() : base(null)
			{
			}
		}

		private List<Func<object, MemberInfo[]>> _MemberFuncMap = new List<Func<object, MemberInfo[]>>() 
		{
			(T)=>GetFields(T),
			(T)=>GetProperties(T),
			(T)=>GetMethods(T)
		};

		private CommandBarButtonItem _LeftFlexibleSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Location = BarButtonLocation.Left };
		private CommandBarButtonItem _RightFlexibleSpace = new CommandBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Location = BarButtonLocation.Right };

		private readonly NoElement _NoElement = new NoElement();
		private Dictionary<Type, Func<MemberInfo, string, object, List<Binding>, IElement>> _ElementPropertyMap;
		
		public IRoot Root { get; set; }

		public ViewParser()
		{
			BuildElementPropertyMap();
		}

		public void Parse(UIView view, string caption, Theme theme)
		{
			Type viewType = view.GetType();

			var vw = view as IView;
			if (vw != null)
			{
				var captionAttribute = viewType.GetCustomAttribute<CaptionAttribute>();
				if (captionAttribute != null)
					caption = captionAttribute.Caption;
			}
			
			Root = new RootElement(caption);
			Root.DataContext = view;

			var dataContext = view as IDataContext;
			if (dataContext != null)
				Root.DataContext = dataContext.DataContext;

			var themeable = Root as IThemeable;
			if (themeable != null)
				themeable.Theme = Theme.CreateTheme(theme);
			
			var searchbarAttribute = viewType.GetCustomAttribute<SearchbarAttribute>();
			var searchbar = Root as ISearchBar;
			if (searchbarAttribute != null && searchbar != null)
			{
				searchbar.SearchPlaceholder = searchbarAttribute.Placeholder;
				searchbar.IncrementalSearch = searchbarAttribute.IncrementalSearch;
				searchbar.EnableSearch =  searchbarAttribute.ShowImmediately;
				searchbar.IsSearchbarHidden = !searchbarAttribute.ShowImmediately;
			}

			var editStyle = viewType.GetCustomAttribute<CellEditingStyleAttribute>();
			if (editStyle != null)
			{
				Root.EditingStyle = editStyle.EditingStyle;
			}

			var sectionList = CreateSectionList(view, Root);
			sectionList.ForEach((section)=>section.Initialize());
			Root.Add(sectionList);
			
			Root.ToolbarButtons = CheckForToolbarItems(view);
			Root.NavbarButtons = CheckForNavbarItems(view);

			var notifyDataContextChanged = view as INotifyDataContextChanged;
			if (notifyDataContextChanged != null)
			{
				notifyDataContextChanged.DataContextChanged -= HandleNotifyDataContextChangedDataContextChanged;
				notifyDataContextChanged.DataContextChanged += HandleNotifyDataContextChangedDataContextChanged;
			}
		}
		
	    public static ICommand GetCommandForMember(object view, MemberInfo member)
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
				throw new Exception(string.Format("Method not found"));
			
			if (!string.IsNullOrEmpty(propertyName))
			{
				var property = view.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				if (property != null)
				{
					if (property.PropertyType == typeof(bool))
					{
						propertyInfo = property;
					}
					else
					{
						throw new Exception(string.Format("Property {0} cannot be used for CanExecute property because it does not have a return type of bool", property.Name));
					}
				}
			}

			return new ReflectiveCommand(view, methodInfo, propertyInfo) { CommandOption = commandOption };
		}

		private List<ISection> CreateSectionList(UIView view, IRoot root)
		{
			ISection lastSection = new Section() { Order = -1, Parent = root as IElement };

			var sectionList = new List<ISection>() { lastSection };

			IElement newElement = null;
			Theme theme = null;
			var themeable = root as IThemeable;
			if (themeable != null)
			{
				ThemeHelper.ApplyRootTheme(view, themeable);
				theme = themeable.Theme;
				ThemeHelper.ApplyElementTheme(theme, lastSection, null);
			}
			
			if (!(view is IView))
			{
				newElement = new UIViewElement(null, view, false);
				lastSection.Add(newElement);
			}
			else
			{
				foreach(var memberFunc in _MemberFuncMap)
				{
					var members = memberFunc(view);
		
					foreach (var member in members)
					{
						var isList = member.GetCustomAttribute<ListAttribute>() != null;

						var pullToRefreshAttribute = member.GetCustomAttribute<PullToRefreshAttribute>();
						if (pullToRefreshAttribute != null)
						{
							root.PullToRefreshCommand = GetCommandForMember(view, member);
							root.DefaultSettingsKey = pullToRefreshAttribute.SettingsKey;
						}
						var skipAttribute = member.GetCustomAttribute<SkipAttribute>(true);
						if (skipAttribute != null) continue;
						
						var sectionAttribute = member.GetCustomAttribute<SectionAttribute>();
		
						if (sectionAttribute != null)
						{	
							Theme sectionTheme = null;
							if (sectionAttribute.ThemeType != null)
								 sectionTheme = Activator.CreateInstance(sectionAttribute.ThemeType) as Theme;
							
							lastSection = new Section(sectionAttribute.Caption, sectionAttribute.Footer) 
							{ 
								Order = sectionAttribute.Order,
								IsExpandable = sectionAttribute.IsExpandable,
								ExpandState = sectionAttribute.ExpandState,
							};
							lastSection.Parent = root as IElement;

							ThemeHelper.ApplyElementTheme(root.Theme, lastSection, null); 
							ThemeHelper.ApplyElementTheme(sectionTheme, lastSection, null);

							sectionList.Add(lastSection);
						}
		
						var bindings = GetBindings(view, member);
	
						newElement = GetElementForMember(root.Theme, view, member, bindings);

						ThemeHelper.ApplyElementTheme(theme, newElement, member);
						
						IBindable bindable = null;
						
						if (newElement is ISection)
						{
							lastSection.Caption = newElement.Caption;
							lastSection.Order = newElement.Order;
							
							lastSection.Add(((ISection)newElement).Elements);
							bindable = lastSection as IBindable;
						}
						else if (newElement != null)
						{
							var editStyle = member.GetCustomAttribute<CellEditingStyleAttribute>();
							if (editStyle != null)
							{
								root.EditingStyle = editStyle.EditingStyle;
								newElement.EditingStyle = editStyle.EditingStyle;
								if (newElement.Section != null)
								{
									foreach(ISection section in newElement.Section)
									{
										foreach(var element in section.Elements)
										{
											element.EditingStyle = editStyle.EditingStyle;
										}
									}
								}
							}
							else
								newElement.EditingStyle = root.EditingStyle;

							bindable = newElement as IBindable;

							if ((isList) && newElement is IRoot)
							{
//								var sections = ((IRoot)newElement).Sections;
//								root.ViewBinding = newElement.ViewBinding;
//
//								var firstSection = sections.FirstOrDefault();
//								if (firstSection.Elements.Count > 0)
//									lastSection.Add(firstSection.Elements);
//
//								for(var index=1; index < sections.Count; index++)
//								{
//									sectionList.Add(sections[index]);
//								}
							}
							else
							{
								lastSection.Add(newElement);
							}
						}

						if (bindable != null && bindable != _NoElement && bindings.Count != 0)
						{
							foreach (Binding binding in bindings)
							{
								if (binding.TargetPath == null)
								{
									binding.TargetPath = "DataContext";	
								}
			
								var bindingRoot = bindable as IRoot;
								if (bindingRoot != null && binding.TargetPath == "DataContext")
								{
									foreach(var section in bindingRoot.Sections)
									{
										section.Parent = bindingRoot;
										BindingOperations.SetBinding(section as IBindable, binding);
									}

									BindingOperations.SetBinding(Root as IBindable, binding);
								}

								BindingOperations.SetBinding(bindable, binding);	
							}
						}
					}
				}
			}

			foreach (var section in sectionList)
			{
				var orderedList = section.Elements.OrderBy(e=>e.Order).Where((e)=>e != _NoElement).ToList();
				section.Elements = new System.Collections.ObjectModel.ObservableCollection<MonoMobile.MVVM.IElement>(orderedList);
			}

			var orderedSections = sectionList.Where(s=>s.Elements.Count > 0).OrderBy(section=>section.Order).ToList();
			return orderedSections;
		}

		private IElement GetElementForMember(Theme theme, UIView view, MemberInfo member, List<Binding> bindings)
		{
			string caption = GetCaption(member);
			IElement element = null;

			var orderAttribute = member.GetCustomAttribute<OrderAttribute>();

			Type memberType = GetTypeForMember(member);

			if (!(member is MethodInfo))
			{
				var defaultValue = member.GetCustomAttribute<DefaultValueAttribute>();
				if (defaultValue != null)
				{
					var propertyInfo = member as PropertyInfo;
					var fieldInfo = member as FieldInfo;
					if (propertyInfo != null && propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(view, defaultValue.Value, null);
					}
					
					if (fieldInfo != null)
					{
						fieldInfo.SetValue(view, defaultValue.Value);
					}
				}

			}
			
			// get a single element
			if(_ElementPropertyMap.ContainsKey(memberType))
			{
				element = _ElementPropertyMap[memberType](member, caption, view, bindings);
			}
			
			if (typeof(IElement).IsAssignableFrom(memberType))
			{
				object context = view;
				var memberValue = GetMemberValue<IElement>(ref context, member);

				//var memberValue = member.GetValue(view) as IElement;
				if (memberValue == null)
				{
					memberValue = Activator.CreateInstance(memberType) as IElement;
				}
				if (memberValue != null)
				{
					memberValue.Caption = caption;
				}

				element = memberValue;
			}

			if (element == null)
			{
				element = GetRootElementForMember(theme, view, member, bindings);
			}
			
			if (element == null)
			{
				throw new Exception(string.Format("Unknown type ({0}). Are you missing a [Root] or [List] attribute?", memberType));
			}	
			
			if (orderAttribute != null && element != null)
				element.Order = orderAttribute.Order;
			
//			var dataContext = view as IDataContext;
//			if (dataContext != null && element.DataContext == null && element.DataContext != dataContext.DataContext)
//			{
//				element.DataContext = dataContext.DataContext;
//			}

			return element;
		}
		
		private IElement GetRootElementForMember(Theme theme, UIView view, MemberInfo member, List<Binding> bindings)
		{
			Type memberType = GetTypeForMember(member);
			var caption = GetCaption(member);
			
			IElement root = null;
			Type viewType = memberType;
			Type elementType = null;

			var genericType = memberType.GetGenericArguments().FirstOrDefault();
			if (genericType != null)
				viewType = genericType;
			
			var listAttribute = member.GetCustomAttribute<ListAttribute>();
			if (listAttribute != null)
			{
				viewType = listAttribute.ViewType ?? viewType;
				elementType = listAttribute.ElementType ?? elementType;
			}

			var rootAttribute = member.GetCustomAttribute<RootAttribute>();
			if (rootAttribute != null)
			{
				viewType = rootAttribute.ViewType ?? viewType;
				elementType = rootAttribute.ElementType ?? elementType;
			}
			
			var cellEditingStyle = member.GetCustomAttribute<CellEditingStyleAttribute>(); 

			var isEnum = memberType.IsEnum;
			var isEnumCollection = typeof(EnumCollection).IsAssignableFrom(memberType);
			var isMultiselect = member.GetCustomAttribute<MultiSelectionAttribute>() != null;
			var isSelect = member.GetCustomAttribute<SelectionAttribute>() != null;
			var isView = typeof(IView).IsAssignableFrom(memberType) || typeof(IView).IsAssignableFrom(viewType);
			var isUIView = typeof(UIView).IsAssignableFrom(memberType) || typeof(UIView).IsAssignableFrom(viewType);

			var isEnumerable = typeof(IEnumerable).IsAssignableFrom(memberType) && !(isView || isUIView);
		
			var isList = member.GetCustomAttribute<ListAttribute>() != null;

			if (isEnum || isEnumCollection || isMultiselect || isSelect)
			{
				ISection section = GetSectionElementForMember(theme, view, member, bindings);
				if (!isList && section != null)
				{
					var rootElement = new RootElement() { section };
					rootElement.Caption = caption;
					rootElement.Theme = Theme.CreateTheme(Root.Theme); 
		
					rootElement.ViewBinding = section.ViewBinding;
					rootElement.Theme.CellStyle = GetCellStyle(member, UITableViewCellStyle.Value1);
					root = rootElement;
				}
				else
				{
					var sectionAttribute = member.GetCustomAttribute<SectionAttribute>();
					if (sectionAttribute != null)
					{
						section.Caption = sectionAttribute.Caption.Capitalize() ?? section.Caption.Capitalize();
						section.Order = sectionAttribute.Order > -1 ? sectionAttribute.Order : section.Order;
						section.IsExpandable = sectionAttribute.IsExpandable;
						section.ExpandState = sectionAttribute.ExpandState;
					}
					root = section as IElement;
				} 
			}
			else if (isEnumerable)
			{
				var rootElement = CreateEnumerableRoot(theme, member, caption, view, bindings);
				if (isList)
				{
					root = rootElement.Sections.FirstOrDefault() as IElement;
				}
				else
				{
					root = rootElement as IElement;
				}
			}
			else if (isView || elementType != null)
			{
				elementType = elementType ?? typeof(RootElement);

				object context = view;
				MemberInfo dataContextMember = GetMemberFromDataContext(member, ref context);
				var items = dataContextMember.GetValue(context);

				var rootElement = Activator.CreateInstance(elementType) as IElement;
				
				rootElement.Caption = caption;
				rootElement.Theme = Theme.CreateTheme(Root.Theme); 
				rootElement.ViewBinding.DataContext = context;
				rootElement.ViewBinding.MemberInfo = dataContextMember;
				rootElement.ViewBinding.ElementType = elementType;
				rootElement.ViewBinding.ViewType = viewType;
				rootElement.ViewBinding.DataContextCode = DataContextCode.Object;
				rootElement.Theme.CellStyle = GetCellStyle(member, UITableViewCellStyle.Default);

				if (cellEditingStyle != null)
					rootElement.EditingStyle = cellEditingStyle.EditingStyle;

				if (items != null)
				{
					if (items is UIView)
					{				
						rootElement.ViewBinding.View = items as UIView;
						rootElement.DataContext = items;
					}
					else
					{
						rootElement.DataContext = items;
					}
				}
				else
					rootElement.DataContext = context;

				if (genericType != null)
				{
					SetDefaultConverter(view, member, "DataContext", new EnumerableConverter(), null, bindings);
					rootElement.ViewBinding.DataContextCode = DataContextCode.ViewEnumerable;
					rootElement.ViewBinding.ViewType = viewType;
				}

				if (isList)
				{
					//var innerRoot = BindingContext.CreateRootedView(rootElement as IRoot);
					//root = innerRoot as IElement;
					Root.Sections = ((IRoot)rootElement).Sections;
					root = Root;
					root.ViewBinding = rootElement.ViewBinding;
				}
				else
				{
					root = rootElement;
				}
			}
			else if (isUIView)
			{
				object context = view;
			//	MemberInfo dataContextMember = GetMemberFromDataContext(member, ref dataContext);
				var uiView = GetMemberValue<UIView>(ref context, member);
				
				if (uiView == null)
					uiView = Activator.CreateInstance(memberType) as UIView;

				root = new UIViewElement(caption, uiView, true);
				SetDefaultConverter(view, member, "DataContext", null, null, bindings);
			}
			else if (elementType != null)
			{
				root = CreateElement(elementType, "") as IElement;
				SetDefaultConverter(view, member, "DataContext", null, null, bindings);
			}

		//	if (root != null)
		//		SetDefaultConverter(view, member, "DataContext", new ViewConverter(), null, bindings);

			return root;
		}
		
		private ISection GetSectionElementForMember(Theme theme, object view, MemberInfo member, List<Binding> bindings)
		{
			var caption = GetCaption(member);
			Type memberType = GetTypeForMember(member);
			ISection section = null;
			var isMultiselect = member.GetCustomAttribute<MultiSelectionAttribute>() != null;
			var isSelect = member.GetCustomAttribute<SelectionAttribute>() != null;

			if (memberType.IsEnum)
			{
				SetDefaultConverter(view, member, "DataContext", new EnumConverter(), memberType, bindings);
	
				var pop = member.GetCustomAttribute<PopOnSelectionAttribute>() != null;
				
				var currentValue = member.GetValue(view);
				var enumValues = Enum.GetValues(memberType);

				section = CreateEnumSection(enumValues, typeof(RadioElement), currentValue, pop, false);
			}
			else if (typeof(EnumCollection).IsAssignableFrom(memberType))
			{
				section = CreateEnumCollectionSection(member, caption, view, bindings);
			}
			else if (isMultiselect)
			{
				section = CreateMultiselectCollectionSection(member, caption, view, bindings);
			}
			else if (isSelect)
			{
				section = CreateSelectCollectionSection(member, caption, view, bindings);
			}

			return section;
		}
			
		private ISection CreateEnumSection(IEnumerable values, Type elementType, object currentValue, bool popOnSelection, bool readOnly)
		{
			var csection = new Section();

			int index = 0;
			
			csection = BindingContext.CreateSection(null, csection, values, elementType, popOnSelection);
			
			foreach(var element in csection.Elements)
			{
				var radioElement = element as RadioElement;
				if (radioElement != null)
				{
					radioElement.Item = element.DataContext;
					radioElement.Index = index;
					radioElement.PopOnSelect = popOnSelection;
					index++;
				}
				
				if (!readOnly)
				{
					if (currentValue != null && currentValue.Equals(element.DataContext))
					{
						element.DataContext = true;
					}
					element.DataContext = false;
				}
			}

			csection.ViewBinding.DataContextCode = DataContextCode.Enum;

			return csection;
		}

		private ISection CreateEnumCollectionSection(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			Type memberType = GetTypeForMember(member);
			
			object context = view;
			var dataContext = view as IDataContext;

			if (dataContext != null)
			{
				context = dataContext.DataContext;
			}
			
			SetDefaultConverter(view, member, "DataContext", new EnumCollectionConverter(), null, bindings);

			member = GetMemberFromDataContext(member, ref context);

			var csection = new Section() { IsMultiselect = true };

			var collection = member.GetValue(view);
			if (collection == null)
			{
				var collectionType = typeof(EnumCollection<>);
				var enumType = memberType.GetGenericArguments().FirstOrDefault();
				Type[] generic = { enumType };

				collection = Activator.CreateInstance(collectionType.MakeGenericType(generic));
				member.SetValue(view, collection);
			}

			var index = 0;
			var items = (EnumCollection)collection;
			foreach (var item in items.Items)
			{
				var checkboxElement = new CheckboxElement(item.Description) 
				{ 
					Item = item, 
					Index = index, 
					DataContext = item.IsChecked, 
					Group = item.GroupName
				};

				csection.Add(checkboxElement);				
				index++;
			}
			
			csection.DataContext = memberType;
			csection.ViewBinding.DataContextCode = DataContextCode.EnumCollection;

			return csection;
		}
		
		private ISection CreateMultiselectCollectionSection(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			object context = view;

			var csection = new Section() { IsMultiselect = true };
			var index = 0;

			SetDefaultConverter(view, member, "DataContext", new EnumerableConverter(), null, bindings);
		
//			var dataContextMember = GetMemberFromDataContext(member, ref context);
//			var collection = dataContextMember.GetValue(context) as IEnumerable;

			var collection = GetMemberValue<IEnumerable>(ref context, member);
			foreach (var item in collection)
			{
				var checkboxElement = new CheckboxElement(item.ToString()) { Item = item, Index = index, DataContext = false};
				
				csection.Add(checkboxElement);
				index++;
			}
			
			csection.ViewBinding.DataContextCode = DataContextCode.MultiselectCollection;
			csection.ViewBinding.ViewType = null;

			return csection;
		}

		private ISection CreateSelectCollectionSection(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			object context = view;

			var csection = new Section() { IsMultiselect = false };
			var index = 0;

			SetDefaultConverter(view, member, "DataContext", new EnumerableConverter(), null, bindings);
		
//			var dataContextMember = GetMemberFromDataContext(member, ref context);
//			var collection = dataContextMember.GetValue(context) as IEnumerable;
					
			var collection = GetMemberValue<IEnumerable>(ref context, member);
			foreach (var item in collection)
			{
				var radioElement = new RadioElement(item.ToString()) { Item = item, Index = index, DataContext = false};
				
				csection.Add(radioElement);
				index++;
			}
			
			csection.ViewBinding.DataContextCode = DataContextCode.Enumerable;
			csection.ViewBinding.ViewType = null;

			return csection;
		}

		private IRoot CreateEnumerableRoot(Theme theme, MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			var rootAttribute = member.GetCustomAttribute<RootAttribute>();
			var listAttribute = member.GetCustomAttribute<ListAttribute>();
			var viewAttribute = member.GetCustomAttribute<ViewAttribute>();

			Type elementType = null;
			
			if (listAttribute != null)
			{
				elementType = listAttribute.ElementType;
			}
			
			if (rootAttribute != null && elementType == null)
			{
				elementType = rootAttribute.ElementType;
			}

			SetDefaultConverter(view, member, "DataContext", new EnumerableConverter(), null, bindings);

			var items = GetMemberValue<IEnumerable>(ref view, member);

			if (items == null)
				throw new ArgumentNullException(member.Name, string.Format("Member of class {1} must have a value.", view.GetType().Name));

			SetDefaultConverter(view, member, "DetailTextLabel", null, null, bindings);

			var genericType = items.GetType().GetGenericArguments().SingleOrDefault();

			var isUIView = typeof(UIView).IsAssignableFrom(genericType);
 
			var readOnly = member.GetCustomAttribute<ReadOnlyAttribute>() != null;

			var section = CreateEnumSection(items, elementType, null, true, readOnly);

			var root = new RootElement(caption) { section };
			root.ViewBinding.MemberInfo = member;
			root.DataContext = items;
			root.ViewBinding.DataContextCode = DataContextCode.Enumerable;

			if (isUIView)
			{
				root.ViewBinding.ViewType = genericType;
				root.ViewBinding.DataContextCode = DataContextCode.ViewEnumerable;
			}
			else if (viewAttribute != null && viewAttribute.ViewType != null)
			{
				root.ViewBinding.ViewType = viewAttribute.ViewType;
				root.ViewBinding.DataContextCode = DataContextCode.ViewEnumerable;
			}

			if (rootAttribute != null && rootAttribute.ViewType != null)
			{
				root.ViewBinding.ViewType = rootAttribute.ViewType;
				root.ViewBinding.DataContextCode = DataContextCode.ViewEnumerable;
			}

			if (listAttribute != null)
			{
				var sectionAttribute = member.GetCustomAttribute<SectionAttribute>();
				if (sectionAttribute != null)
				{
					section.Caption = sectionAttribute.Caption.Capitalize() ?? section.Caption.Capitalize();
					section.Order = sectionAttribute.Order > -1 ? sectionAttribute.Order : section.Order;
					section.IsExpandable = sectionAttribute.IsExpandable;
					section.ExpandState = sectionAttribute.ExpandState;
				}

				root.ViewBinding.ViewType = listAttribute.ViewType ?? root.ViewBinding.ViewType;
			}
		
			root.Theme.CellStyle = GetCellStyle(member, UITableViewCellStyle.Value1);
			
			return root;
		}

		private UITableViewCellStyle GetCellStyle(MemberInfo member, UITableViewCellStyle defaultCellStyle)
		{
			var rootAttribute = member.GetCustomAttribute<RootAttribute>();
			var cellStyle = defaultCellStyle;
			if (rootAttribute != null)
				cellStyle = rootAttribute.CellStyle;

			return cellStyle;
		}

		private void SetDefaultConverter(object view, MemberInfo member, string targetPath, IValueConverter converter, object parameter, List<Binding> bindings)
		{
			var list = (from binding in bindings
				where binding.Source == view && 
					binding.SourcePath == member.Name &&
					string.IsNullOrEmpty(binding.TargetPath) && 
					binding.Converter == null
				select binding).ToList();

			list.ForEach(binding => 
			{
				binding.TargetPath = targetPath;
				binding.Converter = converter;
				binding.ConverterParameter = parameter;
			});
		}

		private static MemberInfo[] GetFields(object view)
		{
			return view.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		private static MemberInfo[] GetProperties(object view)
		{
			return view.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		private static MemberInfo[] GetMethods(object view)
		{
			return view.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(m =>
			{
				var methodInfo = m as MethodBase;
				return (methodInfo == null || !methodInfo.IsConstructor && !methodInfo.IsSpecialName);
			}).ToArray();
		}

		private List<CommandBarButtonItem> CheckForToolbarItems(object view)
		{
			var buttonList = new List<CommandBarButtonItem>();
			var members = GetMethods(view);

			foreach(var member in members)
			{
				var buttonAttribute = member.GetCustomAttribute<ToolbarButtonAttribute>();
				var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();
				var caption = captionAttribute != null ? captionAttribute.Caption : null;

				if (buttonAttribute != null)
				{
					UIView buttonView = null;
					var title = caption;
					if (buttonAttribute.ViewType != null)
					{	
						buttonView = Activator.CreateInstance(buttonAttribute.ViewType) as UIView;
					
						var tappable = buttonView as ITappable;
						if (tappable != null)
						{
							tappable.Command = GetCommandForMember(view, member); 
						}
					}

					var button = CreateCommandBarButton(view, member, title, buttonView, buttonAttribute.Style, buttonAttribute.ButtonType, buttonAttribute.Location);
					
					if (button != null)
					{		
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
	
		private List<CommandBarButtonItem> CheckForNavbarItems(object view)
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
					var button = CreateCommandBarButton(view, member, title, null, buttonAttribute.Style, buttonAttribute.ButtonType, buttonAttribute.Location);
					
					if (button != null)
					{
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

		private CommandBarButtonItem CreateCommandBarButton(object view, MemberInfo member, string title, UIView buttonView, UIBarButtonItemStyle style, UIBarButtonSystemItem? buttonType, BarButtonLocation location)
		{
			CommandBarButtonItem button = null;

			ICommand command = null;
			var methodInfo = member as MethodInfo;

			if(methodInfo != null)
				command = GetCommandForMember(view, member);

			if (!string.IsNullOrEmpty(title))
			{
				button = new CommandBarButtonItem(title, style, delegate {command.Execute(null); });
			}
			else if (buttonView != null)
			{
				button = new CommandBarButtonItem(buttonView); 
			}
			else
			{
				if (!buttonType.HasValue)
					buttonType = UIBarButtonSystemItem.Done;

				button = new CommandBarButtonItem(buttonType.Value,  delegate { command.Execute(null); });
				button.Style = style;
			}
		
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

		private List<Binding> GetBindings(object view, MemberInfo member)
		{
			var bindings = new List<Binding>();
			var bindAttributes = member.GetCustomAttributes(typeof(BindAttribute), false);

			foreach (BindAttribute bindAttribute in bindAttributes)
			{
				if (bindAttribute.DataTemplate != null)
				{
					bindings.AddRange(bindAttribute.DataTemplate.Bind());
				}
				else
				{
					bindings.Add(bindAttribute.Binding);
				}
			}
			
			var dataContextBinding = bindings.Where((b)=>b.TargetPath == "DataContext").FirstOrDefault() != null;

			if (!dataContextBinding)
			{
				bindings.Add(new Binding(member.Name, null));
			}

			foreach(var binding in bindings)
			{
				if (string.IsNullOrEmpty(binding.SourcePath))
				{
					binding.SourcePath = member.Name;
				}

				var sourceDataContext = view;
				binding.Source = sourceDataContext;
			}
			
			return bindings;
		}

		private string GetCaption(MemberInfo member)
		{
			var caption = string.Empty;
			var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();

			if(captionAttribute != null)
				caption = captionAttribute.Caption;
			else
				caption = member.Name.Capitalize();
			
			return caption;
		}

		// Returns the type for fields and properties and null for everything else
		private static Type GetTypeForMember(MemberInfo mi)
		{
			if (mi is FieldInfo)
				return ((FieldInfo)mi).FieldType;

			if (mi is PropertyInfo)
				return ((PropertyInfo)mi).PropertyType;

			if (mi is MethodInfo)
				return typeof(MethodInfo);

			return null;
		}
		
		private T GetMemberValue<T>(ref object context, MemberInfo member)
		{
			MemberInfo dataContextMember = GetMemberFromDataContext(member, ref context);
			return (T)dataContextMember.GetValue(context);
		}

		private static MemberInfo GetMemberFromDataContext(MemberInfo memberInfo, ref object dataContext)
		{
			var member = memberInfo;
			var context = dataContext as IDataContext;
			if (context != null)
			{
				if (context != null && context.DataContext != null)
				{
					dataContext = context.DataContext;
					memberInfo = dataContext.GetType().GetMember(member.Name).SingleOrDefault();
				}
				
				if (memberInfo == null)
				{
					dataContext = context;
					memberInfo = dataContext.GetType().GetMember(member.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault();
				}
			}

			return memberInfo;
		}

		private void BuildElementPropertyMap()
		{
			_ElementPropertyMap = new Dictionary<Type, Func<MemberInfo, string, object, List<Binding>, IElement>>();
			_ElementPropertyMap.Add(typeof(MethodInfo), (member, caption, view, bindings)=>
			{
				IElement element = null;
				
				var loadMoreAttribute = member.GetCustomAttribute<LoadMoreAttribute>();
				if (loadMoreAttribute != null)
				{
					var normalCaption = !string.IsNullOrEmpty(loadMoreAttribute.NormalCaption) ? loadMoreAttribute.NormalCaption: "Load More";
					var loadingCaption =  !string.IsNullOrEmpty(loadMoreAttribute.LoadingCaption) ? loadMoreAttribute.LoadingCaption: "Loading...";

					element = new LoadMoreElement(normalCaption, loadingCaption, null) { Command = GetCommandForMember(view, member) };
				}

				var searchbarAttribute = member.GetCustomAttribute<SearchbarAttribute>();
				var searchbar = Root as ISearchBar;
				if (searchbarAttribute != null && searchbar != null)
				{
					searchbar.SearchPlaceholder = searchbarAttribute.Placeholder;
					searchbar.IncrementalSearch = searchbarAttribute.IncrementalSearch;
					searchbar.EnableSearch = searchbarAttribute.ShowImmediately;
					searchbar.IsSearchbarHidden = !searchbarAttribute.ShowImmediately;
					
					var methodInfo = member as MethodInfo;
					searchbar.SearchCommand = new SearchCommand(view, methodInfo);
				}
				
				var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();
				if (buttonAttribute != null)
				{	
					var belement = CreateElement(buttonAttribute.ViewType, caption) as ButtonElement; 
					var command = GetCommandForMember(view, member) as ReflectiveCommand;
					
					command.Element = belement;
					command.CanExecuteChanged += HandleCanExecuteChanged;

					belement.Command = command;
			
					HandleCanExecuteChanged(command, EventArgs.Empty);

					element = belement;
				}

				return element ?? _NoElement;
			});

			_ElementPropertyMap.Add(typeof(CLLocationCoordinate2D), (member, caption, view, bindings) =>
			{
				IElement element = null;
		
				var mapAttribute = member.GetCustomAttribute<MapAttribute>();
				var location = (CLLocationCoordinate2D)member.GetValue(view);
				if (mapAttribute != null)
					element = new MapElement(mapAttribute.Caption, location);

				return element;
			});

			_ElementPropertyMap.Add(typeof(string), (member, caption, view, bindings)=>
			{
				IElement element = null;
	
				var passwordAttribute = member.GetCustomAttribute<PasswordAttribute>();
				var entryAttribute = member.GetCustomAttribute<EntryAttribute>();
				var multilineAttribute = member.GetCustomAttribute<MultilineAttribute>();
				var htmlAttribute = member.GetCustomAttribute<HtmlAttribute>();
		
				if (passwordAttribute != null)
				{
					element = new EntryElement(caption) { Placeholder = passwordAttribute.Placeholder, KeyboardType = passwordAttribute.KeyboardType, IsPassword = true, EditMode = entryAttribute.EditMode, AutoCorrectionType = passwordAttribute.AutoCorrectionType, AutoCapitalizationType = passwordAttribute.AutoCapitalizationType };
				} 
				else if (entryAttribute != null)
				{
					element = new EntryElement(caption) { Placeholder = entryAttribute.Placeholder, KeyboardType = entryAttribute.KeyboardType, EditMode = entryAttribute.EditMode,  AutoCorrectionType = entryAttribute.AutoCorrectionType, AutoCapitalizationType = entryAttribute.AutoCapitalizationType };
				}
				else if (multilineAttribute != null)
				{
					element = new MultilineElement(caption) { Lines = multilineAttribute.Lines };
					element.Theme.TextAlignment = UITextAlignment.Left;
				}
				else if (htmlAttribute != null)
				{
					SetDefaultConverter(view, member, "DataContext", new UriConverter(), null, bindings);
					element = new HtmlElement(caption);
				}
				else
				{
					var selement = new StringElement(caption, (string)member.GetValue(view)) { };
					
					element = selement;
				}
				
				return element;
			});

			_ElementPropertyMap.Add(typeof(Single), (member, caption, view, bindings)=>
			{
				return CreateFloatElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(Double), (member, caption, view, bindings)=>
			{
				return CreateFloatElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(Uri), (member, caption, view, bindings)=>
			{
				return new HtmlElement(caption, (Uri)member.GetValue(view));  
			});

			_ElementPropertyMap.Add(typeof(bool), (member, caption, view, bindings)=>
			{
				IElement element = null;

				var checkmarkAttribute = member.GetCustomAttribute<CheckmarkAttribute>();
				if (checkmarkAttribute != null)
				    element = new CheckboxElement(caption) {  };
				else
					element = new BooleanElement(caption) { };

				return element;
			});

			_ElementPropertyMap.Add(typeof(DateTime), (member, caption, view, bindings)=>
			{
				IElement element = null;

				SetDefaultConverter(view, member, "DataContext", new DateTimeConverter(), null, bindings);

				var dateAttribute = member.GetCustomAttribute<DateAttribute>();
				var timeAttribute = member.GetCustomAttribute<TimeAttribute>();

				if(dateAttribute != null)
					element = new DateElement(caption);// { DataContext = (DateTime)member.GetValue(view)};
				else if (timeAttribute != null)
					element = new TimeElement(caption);// { DataContext = (DateTime)member.GetValue(view)};
				else
					element = new DateTimeElement(caption);// { DataContext = (DateTime)member.GetValue(view)};
				
				return element;
			});

			_ElementPropertyMap.Add(typeof(UIImage),(member, caption, view, bindings)=>
			{
				return new ImageElement((UIImage)member.GetValue(view));
			});

			_ElementPropertyMap.Add(typeof(Int32), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(Int16), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(Int64), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(UInt32), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(UInt16), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(UInt64), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(Byte), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(SByte), (member, caption, view, bindings)=>
			{				
				return CreateIntElement(member, caption, view, bindings);
			});

			_ElementPropertyMap.Add(typeof(Decimal), (member, caption, view, bindings)=>
			{				
				return CreateFloatElement(member, caption, view, bindings);
			});
		}
		
		private IElement CreateElement(Type elementType, string caption)
		{
			var element = Activator.CreateInstance(elementType, new object[] { caption }) as IElement;

			return element;
		}

		private IElement CreateFloatElement(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			IElement element = null;
			var rangeAttribute = member.GetCustomAttribute<RangeAttribute>();
			if (rangeAttribute != null)
			{
				var floatElement = new FloatElement(caption) {  ShowCaption = rangeAttribute.ShowCaption, MinValue = rangeAttribute.Low, MaxValue = rangeAttribute.High, DataContext = rangeAttribute.Low };
				element = floatElement;
			}
			else 
			{		
				var entryAttribute = member.GetCustomAttribute<EntryAttribute>();
				string placeholder = null;
				var keyboardType = UIKeyboardType.DecimalPad;

				if (entryAttribute != null)
				{
					placeholder = entryAttribute.Placeholder;
					if(entryAttribute.KeyboardType != UIKeyboardType.Default)
						keyboardType = entryAttribute.KeyboardType;
				}

				element = new EntryElement(caption) { Placeholder = placeholder, KeyboardType = keyboardType};
			}
		
			SetDefaultConverter(view, member, "DataContext", new FloatConverter(), null, bindings);

			return element;
		}
		private IElement CreateIntElement(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			IElement element = null;

			var entryAttribute = member.GetCustomAttribute<EntryAttribute>();
			string placeholder = null;
			var keyboardType = UIKeyboardType.NumberPad;

			if(entryAttribute != null)
			{
				placeholder = entryAttribute.Placeholder;
				if(entryAttribute.KeyboardType != UIKeyboardType.Default)
					keyboardType = entryAttribute.KeyboardType;

				element = new EntryElement(caption) { Placeholder = placeholder, KeyboardType = keyboardType};
			}
			else
			{
				element = new StringElement(caption) { DataContext = member.GetValue(view).ToString() };
			}

			SetDefaultConverter(view, member, "DataContext", new IntConverter(), null, bindings);

			return element;
		}

		public void HandleCanExecuteChanged(object sender, EventArgs e)
		{
			var reflectiveCommand = sender as ReflectiveCommand;
			if (reflectiveCommand != null)
			{
				if (reflectiveCommand.CommandOption == CommandOption.Hide)
				{
					reflectiveCommand.Element.Visible = reflectiveCommand.CanExecute(null);
				}
				else
				{
					reflectiveCommand.Element.Enabled = reflectiveCommand.CanExecute(null);
				}
			}
		}
		
		private void HandleNotifyDataContextChangedDataContextChanged(object sender, DataContextChangedEventArgs e)
		{
			
		}
	}
}