//
// EnumExtensions.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
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
namespace System
{
    using System.ComponentModel;
    using System.Linq;

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return GetDescriptionValue(value.ToString(), value.GetType());
        }

        public static string GetDescriptionValue(string name, Type enumType)
        {
            if (enumType == null || !enumType.IsEnum)
            {
                throw new ArgumentException("is not an enum", "enumType");
            }

            string value = string.Empty;

            var enumFields = from field in enumType.GetFields()
                             where field.IsLiteral && field.Name == name
                             select field;

            var enumField = enumFields.FirstOrDefault();
            if (enumField != null)
            {
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(enumField, typeof(DescriptionAttribute));

                value = attribute != null ? attribute.Description : enumField.Name;
            }

            return value;
        }

		public static int GetValueFromString(Type enumType, string descriptionValue)
		{
			int? result = null;
			
			var index = 0;
			foreach (var enumField in enumType.GetFields())
			{
				if (enumField.IsSpecialName)
					continue;
				
				var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(enumField, typeof(DescriptionAttribute));
				if (attribute != null && attribute.Description == descriptionValue)
				{
					result = index;
					break;
				}
				
				index++;
			}
			
			if (result == null)
				result = (int)Enum.Parse(enumType, descriptionValue);
			
			return result.Value;
		}
    }
}
