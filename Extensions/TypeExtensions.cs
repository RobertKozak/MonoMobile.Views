//
// TypeExtensions.cs
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
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
	using System.Linq;
	using System.Reflection;

	public static class TypeExtensions
	{
		public static T GetCustomAttribute<T>(this MemberInfo member) where T: Attribute
		{
			return GetCustomAttribute<T>(member, false);
		}

		public static T GetCustomAttribute<T>(this MemberInfo member, bool inherited) where T: Attribute
		{
			Attribute attribute = default(T);
			var attributes = Attribute.GetCustomAttributes(member, typeof(T), inherited);
			if (attributes.Length > 0)
			{
				attribute = attributes.Where(a=>a.GetType() == typeof(T)).FirstOrDefault();
				if (attribute == null)
					attribute = attributes.FirstOrDefault() as T;
			}
			return (T)attribute;
		}

		public static MemberInfo GetNestedMember(this Type sourceType, ref object obj, string path, bool allowPrivateMembers)
		{
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			
			if (allowPrivateMembers)
				bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			
			Type type = sourceType;
			
			if (path.Contains("."))
			{
				MemberInfo info = null;
				var members = path.Split('.');
				for (int index = 0; index < members.Length; index++)
				{
					var member = members[index];
					info = type.GetMember(member, bindingFlags).FirstOrDefault();
					if (info != null)
					{
						if (obj != null && index < members.Length - 1)
						{
							try
							{
								if (info.MemberType == MemberTypes.Field)
									obj = ((FieldInfo)info).GetValue(obj);

								if (info.MemberType == MemberTypes.Property)
									obj = ((PropertyInfo)info).GetValue(obj, null);
								
								if (obj != null)
									type = obj.GetType();
							}
							catch (TargetInvocationException)
							{
							}
						}
					}
					else
						break;
				}	
				return info;
			}
			else
				return type.GetMember(path, bindingFlags).FirstOrDefault();
		}

		public static Type GetMemberType(this MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).FieldType;
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)member).PropertyType;
			}
			
			return null;
		}

		public static void SetValue(this MemberInfo member, object obj, object value)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				((FieldInfo)member).SetValue(obj, value);
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				((PropertyInfo)member).SetValue(obj, value, null);
			}
		}

		public static object GetValue(this MemberInfo member, object obj)
		{
			object result = null;

			if (member.MemberType == MemberTypes.Field)
			{
				result = ((FieldInfo)member).GetValue(obj);
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				result = ((PropertyInfo)member).GetValue(obj, null);
			}
			
			return result;
		}

		public static object TryGetValue(this MemberInfo member, object obj)
		{
			object result = null;
			
			try
			{
				if (member.MemberType == MemberTypes.Field)
				{
					result = ((FieldInfo)member).GetValue(obj);
				}
			
				if (member.MemberType == MemberTypes.Property)
				{
					result = ((PropertyInfo)member).GetValue(obj, null);
				}
			}
			catch (ArgumentException)
			{
			}
			
			return result;
		}

		public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
		{
			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
				if (it.IsGenericType)
					if (it.GetGenericTypeDefinition() == genericType)
						return true;
			
			Type baseType = givenType.BaseType;
			if (baseType == null)
				return false;
			
			return baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericType || IsAssignableToGenericType(baseType, genericType);
		}
	}
}

