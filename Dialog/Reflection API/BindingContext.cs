//
// BindingContext.cs
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
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
namespace MonoTouch.Dialog
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using MonoTouch.CoreLocation;
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;


	public class BindingContext : DisposableObject
	{
		private Dictionary<Type, Func<MemberInfo, string, object, Element>> _ElementPropertyMap;

		public IRoot Root { get; set; }

		public BindingContext(object dataContext, string title)
		{
			if (dataContext == null)
				throw new ArgumentNullException("dataContext");
			
			var view = dataContext as IView;
			if (view != null && view.Root != null && view.Root.Count > 0)
				Root = view.Root;
			else
			{
				BuildElementPropertyMap();
				Root = new RootElement<int>(title);
				Populate(dataContext, Root);
			}
		}
		
		private void Populate(object dataContext, IRoot root)
		{
			root.Add(CreateSectionList(dataContext, root));
		}

		private List<ISection> CreateSectionList(object dataContext, IRoot root)
		{
			ISection lastSection = new Section() { Order = -1, Parent = root as Element };

			var sectionList = new List<ISection>() { lastSection };
			Element newElement = null;

			var members = GetMembers(dataContext);

			foreach (var member in members)
			{
				var skipAttribute = member.GetCustomAttribute<SkipAttribute>(true);
				if(skipAttribute != null) continue;

				var inline = member.GetCustomAttribute<InlineAttribute>() != null;
				var isList = member.GetCustomAttribute<ListAttribute>() != null;
				var sectionAttribute = member.GetCustomAttribute<SectionAttribute>();
				var toolbarButtonAttribute = member.GetCustomAttribute<ToolbarButtonAttribute>();
				var editButtonAttribute = member.GetCustomAttribute<EditButtonAttribute>();

				if (toolbarButtonAttribute != null)
				{
					var button = new UIBarButtonItem(toolbarButtonAttribute.ButtonType, delegate { (member as MethodInfo).Invoke(dataContext, new object[] {}); });
					if(root.ToolbarButtons == null)
						root.ToolbarButtons = new List<UIBarButtonItem>();

					root.ToolbarButtons.Add(button);
				}

				if (editButtonAttribute != null)
				{
					var button = new UIBarButtonItem(toolbarButtonAttribute.ButtonType, delegate { (member as MethodInfo).Invoke(dataContext, new object[] {}); });
					//root.EditButton = button;
				}

				if (sectionAttribute != null)
				{
					lastSection = new Section(sectionAttribute.Caption, sectionAttribute.Footer) { Order = sectionAttribute.Order };
					lastSection.Parent = root as Element;
					sectionList.Add(lastSection);
				}

				newElement = GetElementForMember(dataContext, member);

				if ((inline || isList) && newElement is IRoot)
				{
					foreach(var element in ((IRoot)newElement).Sections[0].Elements)
						lastSection.Add(element);

					root.Group = ((IRoot)newElement).Group;
				}
				else
				{
					lastSection.Add(newElement);
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

		private Element GetElementForMember(object dataContext, MemberInfo member)
		{
			string caption = null;
			Element element = null;
			//MemberInfo last_radio_index = null;
			var bindings = new List<Binding>();

			var captionAttribute = member.GetCustomAttribute<CaptionAttribute>();
			var orderAttribute = member.GetCustomAttribute<OrderAttribute>();
			var bindAttributes = member.GetCustomAttributes(typeof(BindAttribute), false);
			var popOnSelectionAttribute = member.GetCustomAttribute<PopOnSelectionAttribute>();

		//	var memberDataContext = GetDataContextForMember(dataContext, ref member);
			var memberDataContext = dataContext;

			if(captionAttribute != null)
				caption = captionAttribute.Caption;
			else
				caption = MakeCaption(member.Name);

			Type memberType = GetTypeForMember(member);

			if (!(member is MethodInfo))
			{
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
		//			else
					{
						var sourceDataContext = memberDataContext;
						var sourceProperty = sourceDataContext.GetType().GetNestedProperty(ref sourceDataContext, binding.SourcePath, true);
						if (sourceProperty == null)
						{
							sourceDataContext = dataContext;
							sourceProperty = sourceDataContext.GetType().GetNestedProperty(ref sourceDataContext, binding.SourcePath, true);
						}

						binding.Source = sourceDataContext;
					}
				}
			}

			if(_ElementPropertyMap.ContainsKey(memberType))
				element = _ElementPropertyMap[memberType](member, caption, dataContext);
			else if (memberType.IsEnum)
			{
				SetDefaultConverter(member, "Value", new EnumConverter() { PropertyType = memberType }, bindings);
				
				var csection = new Section();
				var currentValue = GetValue(member, memberDataContext);
				int index = 0;
				int selected = 0;

				var pop = popOnSelectionAttribute != null;

				foreach(Enum value in Enum.GetValues(memberType))
				{
					if (currentValue == value)
						selected = index;
					csection.Add(new RadioElement(value.GetDescription(), pop) { Index = index, Value = false});
					index++;
				}

				element = new RootElement<int>(caption, new RadioGroup(memberType.FullName, selected)) { csection };
				element.Caption = caption;
				((IRoot)element).Group = new RadioGroup(memberType.FullName, selected) { EnumType = memberType };
				((IRoot)element).CellStyle = UITableViewCellStyle.Value1;
			}
			else if (typeof(EnumCollection).IsAssignableFrom(memberType))
			{
				SetDefaultConverter(member, "Value", new EnumItemsConverter(), bindings);

				var csection = new Section() { IsMultiselect = true };
				var collection = GetValue(member, memberDataContext);
				if (collection == null)
				{
					var collectionType = typeof(EnumCollection<>);
					var enumType = memberType.GetGenericArguments()[0];
					Type[] generic = { enumType };

					collection = Activator.CreateInstance(collectionType.MakeGenericType(generic));
					(member as PropertyInfo).SetValue(memberDataContext, collection, new object[] {});
				}

				var index = 0;
				var items = (EnumCollection)collection;
				foreach (var item in items.AllValues)
				{
					csection.Add(new CheckboxElement(item.Description, item.IsChecked, item.GroupName) { Index = index });
					index++;
				}
				
				element = new RootElement<int>(caption) { csection };
				((IRoot)element).CellStyle = GetCellStyle(member, UITableViewCellStyle.Value1);
			}
			else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(memberType))
			{
				SetDefaultConverter(member, "Value", new EnumerableConverter(), bindings);
				var listBox = new ListBoxElement();
				int index = 0;

				Type viewType = memberType.GetGenericArguments()[0];
				
				var rootAttribute = member.GetCustomAttribute<RootAttribute>();

				var items = (IEnumerable)GetValue(member, memberDataContext);
				foreach (var e in items)
				{
					Element newElement = null;

					newElement = CreateGenericRoot(viewType, null, e);
					if (rootAttribute != null)
						((IRoot)newElement).ElementType = rootAttribute.DataTemplateType; 

					//Populate(e, (IRoot)newElement);
										
					listBox.Add(newElement);
					index++;
				}

				element = CreateGenericRoot(memberType, listBox, null);
				element.Caption = caption;
				((IRoot)element).CellStyle = GetCellStyle(member, UITableViewCellStyle.Default);
			}
			else
			{
				var nested = GetValue(member, memberDataContext);
				if (nested != null)
				{
			//		if(nested is IView)
			//			SetDefaultConverter(member, "Value", new ViewConverter(), bindings);
					
					var newRoot = CreateGenericRoot(memberType, null, nested);
					newRoot.Caption = caption;
					((IRoot)newRoot).CellStyle = GetCellStyle(member, UITableViewCellStyle.Default);

				//	Populate(nested, (IRoot)newRoot);
					element = newRoot;
				}
			}

			if (orderAttribute != null)
				element.Order = orderAttribute.Order;

			var bindable = element as IBindable;
			if (bindable != null && bindings.Count != 0) //&& !(bindable is IRoot)
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

		private static object GetValue(MemberInfo mi, object o)
		{
			var fi = mi as FieldInfo;
			if (fi != null)
				return fi.GetValue(o);
			var pi = mi as PropertyInfo;
			if (pi != null)
			{
				var getMethod = pi.GetGetMethod();
				var value = getMethod.Invoke(o, new object[] {});
				return value;
			}

			return null;
		}

		private object GetDataContextForMember(object dataContext, ref MemberInfo member)
		{
			var explicitDataContext = dataContext;
			
			var view = dataContext as IView;
			if (view != null && view.DataContext != null)
			{
				explicitDataContext = view.DataContext;
			}
			
			var explicitMember = explicitDataContext.GetType().GetProperty(member.Name);
			if (explicitMember != null)
			{
				member = explicitMember;
				return explicitDataContext;
			} 
			else
			{
				return dataContext;
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
			_ElementPropertyMap = new Dictionary<Type, Func<MemberInfo, string, object, Element>>();
			_ElementPropertyMap.Add(typeof(MethodInfo), (member, caption, dataContext)=>
			{
				Element element = null;
	
				var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();
				if (buttonAttribute != null)
				{	
					element = new ButtonElement(caption)
					{
						BackgroundColor = buttonAttribute.BackgroundColor,
						TextColor = buttonAttribute.TextColor,
						Command = GetCommandForMember(dataContext, member)
					};
				}

				return element;
			});

			_ElementPropertyMap.Add(typeof(CLLocationCoordinate2D), (member, caption, dataContext)=>
			{
				Element element = null;
		
				var mapAttribute = member.GetCustomAttribute<MapAttribute>();
				var location = (CLLocationCoordinate2D)GetValue(member, dataContext);
				if (mapAttribute != null)
					element = new MapElement(mapAttribute.Caption, mapAttribute.Value, location);

				return element;
			});

			_ElementPropertyMap.Add(typeof(string), (member, caption, dataContext)=>
			{
				Element element = null;
	
				var passwordAttribute = member.GetCustomAttribute<PasswordAttribute>();
				var entryAttribute = member.GetCustomAttribute<EntryAttribute>();
				var multilineAttribute = member.GetCustomAttribute<MultilineAttribute>();
				var htmlAttribute = member.GetCustomAttribute<HtmlAttribute>();
				var alignmentAttribute = member.GetCustomAttribute<AlignmentAttribute>();
		
				if (passwordAttribute != null)
					element = new EntryElement(caption, passwordAttribute.Placeholder, true);
				else if (entryAttribute != null)
					element = new EntryElement(caption, entryAttribute.Placeholder) { KeyboardType = entryAttribute.KeyboardType };
				else if (multilineAttribute != null)
					element = new MultilineElement(caption);
				else if (htmlAttribute != null)
					element = new HtmlElement(caption);
				else
				{
					var selement = new StringElement(caption, (string)GetValue(member, dataContext));
					
					if (alignmentAttribute != null)
						selement.Alignment = alignmentAttribute.Alignment;
					
					element = selement;
				}

				var tappable = element as ITappable;
				if (tappable != null)
					((ITappable)element).Command = GetCommandForMember(dataContext, member);
				
				return element;
			});

			_ElementPropertyMap.Add(typeof(float), (member, caption, dataContext)=>
			{
				Element element = null;
				var rangeAttribute = member.GetCustomAttribute<RangeAttribute>();
				if (rangeAttribute != null)
				{
					var floatElement = new FloatElement() {  };
					floatElement.Caption = caption;
					element = floatElement;
					
					floatElement.MinValue = rangeAttribute.Low;
					floatElement.MaxValue = rangeAttribute.High;
					floatElement.ShowCaption = rangeAttribute.ShowCaption;
				}
				else 
				{		
					var entryAttribute = member.GetCustomAttribute<EntryAttribute>();
					var placeholder = "";
					var keyboardType = UIKeyboardType.NumberPad;

					if(entryAttribute != null)
					{
						placeholder = entryAttribute.Placeholder;
						if(entryAttribute.KeyboardType != UIKeyboardType.Default)
							keyboardType = entryAttribute.KeyboardType;
					}

					element = new EntryElement(caption, placeholder, "") { KeyboardType = keyboardType };
				}

				return element;
			});

			_ElementPropertyMap.Add(typeof(bool), (member, caption, dataContext)=>
			{
				Element element = null;

				var checkboxAttribute = member.GetCustomAttribute<CheckboxAttribute>();
				if (checkboxAttribute != null)
				    element = new CheckboxElement(caption) {  };
				else
					element = new BooleanElement(caption) {  };

				return element;
			});

			_ElementPropertyMap.Add(typeof(DateTime), (member, caption, dataContext)=>
			{
				Element element = null;

				var dateAttribute = member.GetCustomAttribute<DateAttribute>();
				var timeAttribute = member.GetCustomAttribute<TimeAttribute>();

				if(dateAttribute != null)
					element = new DateElement(caption);
				else if (timeAttribute != null)
					element = new TimeElement(caption);
				else
					element = new DateTimeElement(caption);
				
				return element;
			});

			_ElementPropertyMap.Add(typeof(UIImage),(member, caption, dataContext)=>
			{
				return new ImageElement();
			});

			_ElementPropertyMap.Add(typeof(int), (member, caption, dataContext)=>
			{
				return new StringElement(caption) { Value = GetValue(member, dataContext).ToString() };
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

		private Element CreateGenericRoot(Type type, Section section, object value)
		{
			var rootType = typeof(RootElement<>);
			Type[] generic = { type };
			
			var genericType = rootType.MakeGenericType(generic);

			var root = Activator.CreateInstance(genericType);
			root.GetType().GetProperty("Value").SetValue(root, value, null);
			((IRoot)root).Add(section);
			
			var element = root as Element;
			return element;
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
		private MemberInfo[] GetMembers(object dataContext)
		{
			return dataContext.GetType().GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(m =>
			{
				var methodInfo = m as MethodBase;
				//Bug 662867: var skip = m.GetCustomAttribute<SkipAttribute>(true) != null;
				var skip = m.Name == "ToString";
				return (methodInfo == null || !methodInfo.IsConstructor && !methodInfo.IsSpecialName) && m.MemberType != MemberTypes.Field && !skip;
			}).ToArray();
		}

	    private ICommand GetCommandForMember(object dataContext, MemberInfo member)
		{
			var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();
			if (buttonAttribute != null)
			{
				var context = dataContext;
				var methodInfo = member as MethodInfo;
				var methodName = buttonAttribute.MethodName;

				if (methodInfo == null)
				{
					if (string.IsNullOrEmpty(methodName))
						methodName = member.Name;

					methodInfo = context.GetType().GetMethod(methodName);
					if (methodInfo == null)
					{
						var memberInfo = methodInfo as MemberInfo;
						//context = GetDataContextForMember(dataContext, ref memberInfo);
						context = dataContext;
						methodInfo = memberInfo as MethodInfo;
					}
				}

				if (methodInfo == null)
					throw new Exception(string.Format("Method not found : {0}", methodName));

				return new ReflectiveCommand(context, methodInfo, null);
			}

			return null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}
	}
}
