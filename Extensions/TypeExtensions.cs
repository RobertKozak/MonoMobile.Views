using System.Linq;
namespace MonoMobile.MVVM
{
	using System;
	using System.Reflection;

	public static class TypeExtensions
	{
		public static T GetCustomAttribute<T>(this MemberInfo member) where T: Attribute
		{
			return GetCustomAttribute<T>(member, false);
		}

		public static T GetCustomAttribute<T>(this MemberInfo member, bool inherited) where T: Attribute
		{
			var attributes = Attribute.GetCustomAttributes(member, typeof(T), inherited);
			if (attributes.Length > 0)
				return attributes[0] as T;

			return default(T);
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
			if (member.MemberType == MemberTypes.Field)
			{
				return ((FieldInfo)member).GetValue(obj);
			}
			
			if (member.MemberType == MemberTypes.Property)
			{
				return ((PropertyInfo)member).GetValue(obj, null);
			}
			
			return null;
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

