namespace MonoTouch.MVVM
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

		public static PropertyInfo GetNestedProperty(this Type sourceType, ref object obj, string path, bool allowPrivateProperties)
		{
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
			
			if (allowPrivateProperties)
				bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
			
			Type type = sourceType;
			
			if (path.Contains(".")) 
			{
				PropertyInfo info = null;
				var properties = path.Split('.');
				for (int index = 0; index < properties.Length; index++) 
				{
					var property = properties[index];
					info = type.GetProperty(property, bindingFlags);
					if (info != null) 
					{
						type = info.PropertyType;
						
						if (obj != null && index < properties.Length - 1) 
						{
							try 
							{
								obj = info.GetValue(obj, null);
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
				return type.GetProperty (path, bindingFlags);
		}
	}
}

