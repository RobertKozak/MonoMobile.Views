//
// BindingContext.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Based on code from MonoTouch.Dialog by Miguel de Icaza (miguel@gnome.org)
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
	using MonoMobile.MVVM;
	using MonoTouch.CoreLocation;
	using MonoTouch.UIKit;

	public class BindingContext : DisposableObject
	{
		private Dictionary<Type, Func<MemberInfo, string, object, List<Binding>, IElement>> _ElementPropertyMap;

		public IRoot Root { get; set; }

		public BindingContext(object view, string title) : this(view, title, null)
		{
		}

		public BindingContext(object view, string title, Theme currentTheme)
		{
			if (view == null)
				throw new ArgumentNullException("view");

			BuildElementPropertyMap();
			Root = new RootElement(title);
			Root.ViewBinding.DataContext = view;
			var themeable = Root as IThemeable;
			if (themeable != null)
				themeable.Theme = currentTheme;
			
			Populate(view, Root);

			var viewContext = view as IBindingContext;
			if (viewContext != null)
			{
				viewContext.BindingContext = this;
				var dataContext = view as IDataContext;
				if (dataContext != null)
				{
					var vmContext = dataContext.DataContext as IBindingContext;
					if (vmContext != null)
					{
						vmContext.BindingContext = this;
					}
				}
			}
		}
	
		private void Populate(object view, IRoot root)
		{
			var enableSearchAttribute = view.GetType().GetCustomAttribute<EnableSearchAttribute>();
			var searchbar = root as ISearchBar;
			if (enableSearchAttribute != null && searchbar != null)
			{
				searchbar.AutoHideSearch = enableSearchAttribute.AutoHide;
				searchbar.SearchPlaceholder = enableSearchAttribute.Placeholder;
				searchbar.IncrementalSearch = enableSearchAttribute.IncrementalSearch;
				searchbar.EnableSearch = true;
			}

			root.Add(CreateSectionList(view, root));
			
			root.ToolbarButtons = CheckForToolbarItems(view);
			root.NavbarButtons = CheckForNavbarItems(view);
		}

		private List<ISection> CreateSectionList(object view, IRoot root)
		{
			var memberFuncMap = new List<Func<object, MemberInfo[]>>() 
			{
				(T)=>GetFields(T),
				(T)=>GetProperties(T),
				(T)=>GetMethods(T)
			};

			ISection lastSection = new Section() { Order = -1, Parent = root as IElement };

			var sectionList = new List<ISection>() { lastSection };

			IElement newElement = null;
			Theme theme = null;
			var themeable = root as IThemeable;
			if (themeable != null)
			{
				ApplyRootTheme(view, themeable);
				theme = themeable.Theme;
				ApplyElementTheme(theme, lastSection, null);
			}

			foreach(var memberFunc in memberFuncMap)
			{
				var members = memberFunc(view);
	
				foreach (var member in members)
				{
					var pullToRefreshAttribute = member.GetCustomAttribute<PullToRefreshAttribute>();
					if (pullToRefreshAttribute != null)
					{
						root.PullToRefreshCommand = GetCommandForMember(view, member);
						root.DefaultSettingsKey = pullToRefreshAttribute.SettingsKey;
					}
					var skipAttribute = member.GetCustomAttribute<SkipAttribute>(true);
					if (skipAttribute != null) continue;
					
					var inline = member.GetCustomAttribute<InlineAttribute>() != null;
					var isRoot = member.GetCustomAttribute<RootAttribute>() != null;
					var listAttribute = member.GetCustomAttribute<ListAttribute>();		
					var isList = listAttribute != null;
					var sectionAttribute = member.GetCustomAttribute<SectionAttribute>();
	
					if (sectionAttribute != null)
					{	
						Theme sectionTheme = null;
						if (sectionAttribute.ThemeType != null)
							 sectionTheme = Activator.CreateInstance(sectionAttribute.ThemeType) as Theme;
						
						lastSection = new Section(sectionAttribute.Caption, sectionAttribute.Footer) { Order = sectionAttribute.Order };
						lastSection.Parent = root as IElement;
						ApplyElementTheme(root.RootTheme, lastSection, null);

						ApplyElementTheme(sectionTheme, lastSection, null);
						sectionList.Add(lastSection);
					}
	
					newElement = GetElementForMember(view, member);
					
					if(newElement != null)
					{
						newElement.Theme = Theme.CreateTheme(root.RootTheme);
						ApplyElementTheme(root.RootTheme, newElement, member);

						var context = newElement as IView;
						var displayInline = (inline || !isRoot) && newElement is IRoot; //&& context != null;
						
						if (isList)
						{
							var newRoot = newElement as IRoot;
							Type viewType = newRoot.ViewBinding.ViewType ?? listAttribute.ViewType;

							string caption = null;
							if (!(view is IDataTemplate))
								caption = newElement.Caption;

							lastSection = new Section(caption,null) { };
							lastSection.Parent = root as IElement;
							ApplyElementTheme(root.RootTheme, lastSection, null);
							sectionList.Add(lastSection);
			
							IEnumerable datacontext = null;
							if (view is IDataTemplate)
								datacontext = ((IDataTemplate)view).Items;
							else 
								datacontext = (IEnumerable)GetValue(member, view);

							foreach (var e in datacontext)
							{
								IElement element = null;

								if (e is IViewModel)
								{
									element = new RootElement(e.ToString());
									element.ViewBinding.ViewType = viewType;
									element.ViewBinding.DataContext = e;
									
									((IRoot)element).RootTheme = Theme.CreateTheme(root.RootTheme);
									element.Theme = ((IRoot)element).RootTheme;

									if (listAttribute.ThemeType != null)
									{
										var listTheme = Activator.CreateInstance(listAttribute.ThemeType) as Theme;
										var rootTheme = Theme.CreateTheme(((IRoot)element).RootTheme);
										rootTheme.MergeTheme(listTheme);
										((IRoot)element).Theme = rootTheme;
									}
								}
								else
								{
									var bindingContext = new BindingContext(e, newElement.Caption, newElement.Theme);
									element = bindingContext.Root.Sections[0] as IElement;
								}
		
								lastSection.Add(element);
							}

						}
						else if (inline)
						{	
							var inlineSection = new Section(string.Empty, null) {  };
							ApplyElementTheme(newElement.Theme, inlineSection, null);
							inlineSection.Parent = root as IElement;
							sectionList.Add(inlineSection);
	
							IRoot inlineRoot = newElement as IRoot;
							if (newElement.ViewBinding.DataContextCode == DataContextCode.Object)
							{
								var bindingContext = new BindingContext(newElement.ViewBinding.CurrentView, newElement.Caption, newElement.Theme);
								inlineRoot = bindingContext.Root;
							}

							inlineSection.Caption = newElement.Caption;
	
							foreach(var element in inlineRoot.Sections[0].Elements)
								inlineSection.Add(element);

							//root.Groups.Add(inlineRoot.Groups[0]);
						}
						else
						{
							lastSection.Add(newElement);
						}
					}
				}
			}
			
			foreach (var section in sectionList)
			{
				var orderedList = section.Elements.OrderBy(e=>e.Order).ToList();
				section.Elements = orderedList;
			}

			var orderedSections = sectionList.Where(s=>s.Elements.Count > 0).OrderBy(section=>section.Order).ToList();
			return orderedSections;
		}

		private IElement GetElementForMember(object view, MemberInfo member)
		{
			string caption = null;
			IElement element = null;
			var bindings = new List<Binding>();

			var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();
			var orderAttribute = member.GetCustomAttribute<OrderAttribute>();
			var bindAttributes = member.GetCustomAttributes(typeof(BindAttribute), false);

			var memberDataContext = view;

			if(captionAttribute != null)
				caption = captionAttribute.Caption;
			else
				caption = MakeCaption(member.Name);

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

				foreach (BindAttribute bindAttribute in bindAttributes)
				{
					bindings.Add(bindAttribute.Binding);
				}
				
				var valueBinding = bindings.Where((b)=>b.TargetPath == "Value").FirstOrDefault() != null;

				if (!valueBinding)
				{
					bindings.Add(new Binding(member.Name, null));
				}

				foreach(var binding in bindings)
				{
					if (string.IsNullOrEmpty(binding.SourcePath))
					{
						binding.SourcePath = member.Name;
					}

					var sourceDataContext = memberDataContext;
					binding.Source = sourceDataContext;
				}
			}

			if(_ElementPropertyMap.ContainsKey(memberType))
			{
				element = _ElementPropertyMap[memberType](member, caption, view, bindings);
			}
			else if (memberType.IsEnum)
			{
				element = CreateEnumRoot(member, caption, memberDataContext, bindings);
			}
			else if (typeof(EnumCollection).IsAssignableFrom(memberType))
			{
				element = CreateEnumCollectionRoot(member, caption, memberDataContext, bindings);
			}
			else if (typeof(IEnumerable).IsAssignableFrom(memberType) && !typeof(IView).IsAssignableFrom(memberType))
			{
				element = CreateEnumerableRoot(member, caption, memberDataContext, bindings);
			}
			else
			{
				element = new RootElement() { };

				var nested = GetValue(member, view) as UIView;
				var viewAttribute = member.GetCustomAttribute<ViewAttribute>();

				if (viewAttribute != null && viewAttribute.ViewType != null)
					element.ViewBinding.ViewType = viewAttribute.ViewType;
				else
					element.ViewBinding.ViewType = memberType;

				element.ViewBinding.DataContext = view;

				element.ViewBinding.MemberInfo = member;
				element.ViewBinding.View = nested;

				element.Caption = caption;

				element.Theme.CellStyle = GetCellStyle(member, UITableViewCellStyle.Default);
			}			
			if (orderAttribute != null && element != null)
				element.Order = orderAttribute.Order;

			var bindable = element as IBindable;
			if (bindable != null && bindings.Count != 0)
			{
				foreach (Binding binding in bindings)
				{
					if (binding.TargetPath == null)
					{
						binding.TargetPath = "Value";
					}

					BindingOperations.SetBinding(bindable, binding.TargetPath, binding);
				}
			}

			return element;
		}
		
		public IElement CreateEnumRoot(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			Type memberType = GetTypeForMember(member);

			SetDefaultConverter(member, "Value", new MonoMobile.MVVM.EnumConverter() { PropertyType = memberType }, bindings);

			var csection = new Section() { Opaque = false };
			var currentValue = GetValue(member, view);
			int index = 0;
			int selected = 0;
	
			ApplyRootTheme(view, csection);

			var pop = member.GetCustomAttribute<PopOnSelectionAttribute>() != null;
			
			var enumValues = Enum.GetValues(memberType);
			foreach(Enum value in enumValues)
			{
				if (currentValue == value)
					selected = index;

				var radioElement = new RadioElement(value.GetDescription()) { };
				radioElement.Index = index;
				radioElement.PopOnSelect = pop;
				radioElement.Value = selected == index;
				radioElement.Opaque = false;
				
				ApplyElementTheme(Root.RootTheme, radioElement, member);

				csection.Add(radioElement);
				index++;
			}

			var element = new RootElement(caption, new RadioGroup(memberType.FullName, selected)) { csection };
			element.Caption = caption;
			element.ViewBinding.DataContext = memberType;
			element.ViewBinding.DataContextCode = DataContextCode.Enum;
			element.Opaque = false;

			var radioGroup = new RadioGroup(memberType.FullName, selected) { EnumType = memberType };
			((IRoot)element).Groups = new List<Group>() { radioGroup };
			element.Theme = Theme.CreateTheme(Root.RootTheme);
			element.Theme.CellStyle = UITableViewCellStyle.Value1;

			return element;
		}
		
		public IElement CreateEnumCollectionRoot(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			Type memberType = GetTypeForMember(member);

			SetDefaultConverter(member, "Value", new EnumItemsConverter(), bindings);

			var csection = new Section() { IsMultiselect = true, Opaque = false };
			ApplyRootTheme(view, csection);

			var collection = GetValue(member, view);
			if (collection == null)
			{
				var collectionType = typeof(EnumCollection<>);
				var enumType = memberType.GetGenericArguments()[0];
				Type[] generic = { enumType };

				collection = Activator.CreateInstance(collectionType.MakeGenericType(generic));
				(member as PropertyInfo).SetValue(view, collection, new object[] {});
			}

			var index = 0;
			var items = (EnumCollection)collection;
			foreach (var item in items.AllValues)
			{
				var checkboxElement = new CheckboxElement(item.Description) { Index = index, Value = item.IsChecked, Group = item.GroupName};
				ApplyRootTheme(view, checkboxElement);

				csection.Add(checkboxElement);
				
				index++;
			}
			
			var element = new RootElement(caption) { csection };
			element.ViewBinding.DataContextCode = DataContextCode.EnumCollection;

			element.Theme.CellStyle = UITableViewCellStyle.Value1;

			return element;
		}
		
		public IElement CreateEnumerableRoot(MemberInfo member, string caption, object view, List<Binding> bindings)
		{
			SetDefaultConverter(member, "Value", new EnumerableConverter(), bindings);
			Type memberType = GetTypeForMember(member);

			var rootAttribute = member.GetCustomAttribute<RootAttribute>();
			var listAttribute = member.GetCustomAttribute<ListAttribute>();

			var items = (IEnumerable)GetValue(member, view);
			
			var element = new RootElement(caption);
			element.ViewBinding.DataContext = view;
			element.ViewBinding.MemberInfo = member;
			element.ViewBinding.ViewType = memberType;

			var dataTemplate = view as IDataTemplate;
			if (dataTemplate != null)
			{
				element.ViewBinding.DataContext = items;
				element.ViewBinding.DataContextCode = DataContextCode.Enumerable;

				rootAttribute = dataTemplate.CustomAttributes.FirstOrDefault((o)=>o.GetType() == typeof(RootAttribute)) as RootAttribute;
				listAttribute = dataTemplate.CustomAttributes.FirstOrDefault((o)=>o.GetType() == typeof(ListAttribute)) as ListAttribute;
			}
			else
			{
				dataTemplate = new StandardListView() { Items = items };
				
				List<object> customAttributes = new List<object>(member.GetCustomAttributes(false));
				var propertyInfo = dataTemplate.GetType().GetProperty("Items");
				var itemAttributes = propertyInfo.GetCustomAttributes(false);
				customAttributes.AddRange(itemAttributes);
				dataTemplate.CustomAttributes = customAttributes;
				
				element.ViewBinding.DataContext = dataTemplate;
				element.ViewBinding.DataContextCode = DataContextCode.Object;
				element.ViewBinding.MemberInfo = propertyInfo;
			}

			element.Theme.CellStyle = GetCellStyle(member, UITableViewCellStyle.Default);

			if(rootAttribute != null)
			{
				element.ViewBinding.ViewType = rootAttribute.ViewType;
			}
	
			if(listAttribute != null)
			{
				element.ViewBinding.ViewType = listAttribute.ViewType ?? element.ViewBinding.ViewType;
			}

			return element;
		}

		public static object GetValue(MemberInfo mi, object o)
		{
			var fi = mi as FieldInfo;
			if (fi != null)
			{
				return fi.GetValue(o);
			}

			var pi = mi as PropertyInfo;
			if (pi != null)
			{
				var value = pi.GetValue(o,new object[] {}); 
				return value;
			}

			return null;
		}

		public void Fetch()
		{
			foreach(var section in Root.Sections)
			{
				foreach(var element in section)
				{
					var bindingExpressions = BindingOperations.GetBindingExpressionsForElement(element); 
					foreach(var expression in bindingExpressions)
					{
						expression.UpdateSource();
					}
				}
			}
		}

		private object GetDataContextForMember(object view, ref MemberInfo member)
		{
			var explicitDataContext = view;
			
			var context = view as IDataContext;
			if (context != null && context.DataContext != null)
			{
				explicitDataContext = context.DataContext;
			}
			
			var explicitMember = explicitDataContext.GetType().GetProperty(member.Name);
			if (explicitMember != null)
			{
				member = explicitMember;
				return explicitDataContext;
			} 
			else
			{
				return view;
			}
		}

		private static void SetValue(MemberInfo mi, object o, object val)
		{
			var fi = mi as FieldInfo;
			if (fi != null)
			{
				fi.SetValue(o, val);
				return;
			}
			var pi = mi as PropertyInfo;
			var setMethod = pi.GetSetMethod();
			setMethod.Invoke(o, new object[] { val });
		}

		private List<CommandBarButtonItem> CheckForToolbarItems(object view)
		{
			var buttonList = new List<CommandBarButtonItem>();
			var members = GetMethods(view);
			foreach(var member in members)
			{
				var toolbarButtonAttribute = member.GetCustomAttribute<ToolbarButtonAttribute>(false);
				var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();
				var caption = captionAttribute != null ? captionAttribute.Caption : null;
				
				if (toolbarButtonAttribute != null)
				{
					var title = caption ?? toolbarButtonAttribute.Title;

					ICommand command = null;
					var methodInfo = member as MethodInfo;

					if(methodInfo != null)
						command = new ReflectiveCommand(view, member as MethodInfo, null);

					CommandBarButtonItem button = null;

					if(!string.IsNullOrEmpty(title))
					{
						button = new CommandBarButtonItem(title, toolbarButtonAttribute.Style, delegate {command.Execute(null); });
					}
					else
					{
						button = new CommandBarButtonItem(toolbarButtonAttribute.ButtonType,  delegate {command.Execute(null); });
						button.Style = toolbarButtonAttribute.Style;
					}
				
					button.Enabled = true;
					button.Location = toolbarButtonAttribute.Location;

					var orderAttribute = member.GetCustomAttribute<OrderAttribute>();
					if (orderAttribute != null)
						button.Order = orderAttribute.Order;
					else 
						button.Order = 0;
					
					buttonList.Add(button);
				}
			}
			
			if (buttonList.Count > 0)
			{
				var sortedList = buttonList.OrderBy(button=>button.Tag).ToList();
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
				var navbarButtonAttribute = member.GetCustomAttribute<NavbarButtonAttribute>(false);
				var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();
				var caption = captionAttribute != null ? captionAttribute.Caption : null;

				if (navbarButtonAttribute != null)
				{
					var title = caption ?? navbarButtonAttribute.Title;

					ICommand command = null;
					var methodInfo = member as MethodInfo;

					if(methodInfo != null)
						command = new ReflectiveCommand(view, member as MethodInfo, null);
					
					CommandBarButtonItem button = null;
					if(!string.IsNullOrEmpty(title))
					{
						button = new CommandBarButtonItem(title, navbarButtonAttribute.Style, delegate {command.Execute(null); });
					}
					else
					{
						button = new CommandBarButtonItem(navbarButtonAttribute.ButtonType, delegate {command.Execute(null); });
						button.Style = navbarButtonAttribute.Style;
					}
				
					button.Enabled = true;
					button.Location = navbarButtonAttribute.Location;

					var orderAttribute = member.GetCustomAttribute<OrderAttribute>();
					if (orderAttribute != null)
						button.Order = orderAttribute.Order;
					else 
						button.Order = 0;
					
					buttonList.Add(button);
				}
			}
			
			if (buttonList.Count > 0)
			{
				var sortedList = buttonList.OrderBy(button=>button.Tag).ToList();
				return sortedList;
			}	

			return null;
		}

		private static string MakeCaption(string name)
		{
			var sb = new StringBuilder(name.Length);
			bool nextUp = true;

			foreach (char c in name)
			{
				if (nextUp)
				{
					sb.Append(Char.ToUpper(c));
					nextUp = false;

				} else
				{
					if (c == '_')
					{
						sb.Append(' ');
						continue;
					}
					if (Char.IsUpper(c))
						sb.Append(' ');
					sb.Append(c);
				}
			}
			return sb.ToString();
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

		private void BuildElementPropertyMap()
		{
			_ElementPropertyMap = new Dictionary<Type, Func<MemberInfo, string, object, List<Binding>, IElement>>();
			_ElementPropertyMap.Add(typeof(MethodInfo), (member, caption, view, bindings)=>
			{
				IElement element = null;
	
				var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();
				if (buttonAttribute != null)
				{	
					var belement = new ButtonElement(caption)
					{
						Command = GetCommandForMember(view, member)
					};
					
					((ReflectiveCommand)belement.Command).Element = belement;

					belement.Command.CanExecuteChanged += HandleCanExecuteChanged;
					element = belement;
				}
				
				var loadMoreAttribute = member.GetCustomAttribute<LoadMoreAttribute>();
				if (loadMoreAttribute != null)
				{
					var normalCaption = !string.IsNullOrEmpty(loadMoreAttribute.NormalCaption) ? loadMoreAttribute.NormalCaption: "Load More";
					var loadingCaption =  !string.IsNullOrEmpty(loadMoreAttribute.LoadingCaption) ? loadMoreAttribute.LoadingCaption: "Loading...";

					element = new LoadMoreElement(normalCaption, loadingCaption, null) { Command = GetCommandForMember(view, member) };
				}

				var enableSearchAttribute = member.GetCustomAttribute<EnableSearchAttribute>();
				var searchbar = Root as ISearchBar;
				if (enableSearchAttribute != null && searchbar != null)
				{
					searchbar.AutoHideSearch = enableSearchAttribute.AutoHide;
					searchbar.SearchPlaceholder = enableSearchAttribute.Placeholder;
					searchbar.IncrementalSearch = enableSearchAttribute.IncrementalSearch;
					searchbar.EnableSearch = true;
					
					var methodInfo = member as MethodInfo;
					searchbar.SearchCommand = new SearchCommand(view, methodInfo);
				}

				return element;
			});

			_ElementPropertyMap.Add(typeof(CLLocationCoordinate2D), (member, caption, view, bindings)=>
			{
				IElement element = null;
		
				var mapAttribute = member.GetCustomAttribute<MapAttribute>();
				var location = (CLLocationCoordinate2D)GetValue(member, view);
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
					element = new MultilineElement(caption);
				else if (htmlAttribute != null)
				{
					SetDefaultConverter(member, "Value", new UriConverter(), bindings);
					element = new HtmlElement(caption);
				}
				else
				{
					var selement = new StringElement(caption, (string)GetValue(member, view)) { };
					
					element = selement;
				}
				
				return element;
			});

			_ElementPropertyMap.Add(typeof(float), (member, caption, view, bindings)=>
			{
				IElement element = null;
				var rangeAttribute = member.GetCustomAttribute<RangeAttribute>();
				if (rangeAttribute != null)
				{
					var floatElement = new FloatElement(caption) {  ShowCaption = rangeAttribute.ShowCaption, MinValue = rangeAttribute.Low, MaxValue = rangeAttribute.High, Value = rangeAttribute.Low };
					element = floatElement;
				}
				else 
				{		
					var entryAttribute = member.GetCustomAttribute<EntryAttribute>();
					string placeholder = null;
					var keyboardType = UIKeyboardType.NumberPad;

					if(entryAttribute != null)
					{
						placeholder = entryAttribute.Placeholder;
						if(entryAttribute.KeyboardType != UIKeyboardType.Default)
							keyboardType = entryAttribute.KeyboardType;
					}

					element = new EntryElement(caption) { Placeholder = placeholder, KeyboardType = keyboardType};
				}

				return element;
			});
			
			_ElementPropertyMap.Add(typeof(Uri), (member, caption, view, bindings)=>
			{
				return new HtmlElement(caption, (Uri)GetValue(member, view));  
			});

			_ElementPropertyMap.Add(typeof(bool), (member, caption, view, bindings)=>
			{
				IElement element = null;

				var checkboxAttribute = member.GetCustomAttribute<CheckboxAttribute>();
				if (checkboxAttribute != null)
				    element = new CheckboxElement(caption) {  };
				else
					element = new BooleanElement(caption) { };

				return element;
			});

			_ElementPropertyMap.Add(typeof(DateTime), (member, caption, view, bindings)=>
			{
				IElement element = null;

				var dateAttribute = member.GetCustomAttribute<DateAttribute>();
				var timeAttribute = member.GetCustomAttribute<TimeAttribute>();

				if(dateAttribute != null)
					element = new DateElement(caption) { Value = (DateTime)GetValue(member, view)};
				else if (timeAttribute != null)
					element = new TimeElement(caption) { Value = (DateTime)GetValue(member, view)};
				else
					element = new DateTimeElement(caption) { Value = (DateTime)GetValue(member, view)};
				
				return element;
			});

			_ElementPropertyMap.Add(typeof(UIImage),(member, caption, view, bindings)=>
			{
				return new ImageElement();
			});

			_ElementPropertyMap.Add(typeof(int), (member, caption, view, bindings)=>
			{
				SetDefaultConverter(member, "Value", new IntConverter(), bindings);
				return new StringElement(caption) { Value = GetValue(member, view).ToString() };
			});
		}
		
		private UITableViewCellStyle GetCellStyle(MemberInfo member, UITableViewCellStyle defaultCellStyle)
		{
			var rootAttribute = member.GetCustomAttribute<RootAttribute>();
			var cellStyle = defaultCellStyle;
			if (rootAttribute != null)
				cellStyle = rootAttribute.CellStyle;

			return cellStyle;
		}

		private void SetDefaultConverter(MemberInfo member, string targetPath, IValueConverter converter, List<Binding> bindings)
		{
			foreach (var binding in bindings)
			{
				if (binding.SourcePath == member.Name && binding.Converter == null)
				{
					binding.TargetPath = targetPath;
					binding.Converter = converter;
				}
			}
		}
		private MemberInfo[] GetFields(object view)
		{
			return view.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		private MemberInfo[] GetProperties(object view)
		{
			return view.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		private MemberInfo[] GetMethods(object view)
		{
			return view.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(m =>
			{
				var methodInfo = m as MethodBase;
				return (methodInfo == null || !methodInfo.IsConstructor && !methodInfo.IsSpecialName);
			}).ToArray();
		}

	    private ICommand GetCommandForMember(object view, MemberInfo member)
		{
			string propertyName = string.Empty;
			PropertyInfo propertyInfo = null;
			var commandOption = CommandOption.Disable;

			var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();
			if (buttonAttribute != null)
			{
				propertyName = buttonAttribute.PropertyName;
				commandOption = buttonAttribute.CommandOption;
			}

			var methodInfo = member as MethodInfo;

			if (methodInfo == null)
				throw new Exception(string.Format("Method not found"));
			
			if (!string.IsNullOrEmpty(propertyName))
			{
				var property = view.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				if (property != null)
				{
					var value =  property.GetValue(view);
					if (value != null && value.GetType() == typeof(bool))
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
		

			return null;
		}
		private void ApplyRootTheme(object view, IThemeable element)
		{
			Theme theme = null;
			var themeAttributes = view.GetType().GetCustomAttributes(typeof(ThemeAttribute), true);
			
			foreach(ThemeAttribute themeAttribute in themeAttributes)
			{
				if (element != null) 
				{
					if (themeAttribute != null && themeAttribute.ThemeType != null)
					{
						theme = Activator.CreateInstance(themeAttribute.ThemeType) as Theme;
						element.Theme.MergeTheme(theme);
					}
				}
			}
			
			var root = element as IRoot;
			if (root != null)
				root.RootTheme = element.Theme;
		}
		
		private void ApplyElementTheme(Theme theme, IThemeable element, MemberInfo member)
		{
			if (theme != null)
			{
				var newTheme = Theme.CreateTheme(theme);
				newTheme.MergeTheme(element.Theme);

				element.Theme = newTheme;
			}

			if (member != null)
				ApplyMemberTheme(member, element);
		}
		
		private void ApplyMemberTheme(MemberInfo member, IThemeable themeableElement)
		{
			var themeAttribute = member.GetCustomAttribute<ThemeAttribute>();

			if (themeableElement != null && themeAttribute != null && themeAttribute.ThemeType != null)
			{
				var theme = Activator.CreateInstance(themeAttribute.ThemeType) as Theme;
			
				if (themeAttribute.ThemeUsage == ThemeUsage.Merge)
					themeableElement.Theme.MergeTheme(theme); 
				else
					themeableElement.Theme = theme;
			}
		}
		
		private void HandleCanExecuteChanged(object sender, EventArgs e)
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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			base.Dispose(disposing);
		}
	}
}
