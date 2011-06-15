//
// BindAttribute.cs: Bind Attribute for creating a data Binding to Elements
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Globalization;
	using MonoMobile.MVVM;

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public class BindAttribute : Attribute
	{
		internal Binding Binding;

		public BindAttribute()
		{
			Binding = new Binding(null, "DataContext");
		}

		public BindAttribute(string sourcePath)
		{
			Binding = new Binding(sourcePath, "DataContext");
		}

		public BindAttribute(string sourcePath, string targetPath)
		{
			Binding = new Binding(sourcePath, targetPath);
		}

		public string SourcePath
		{
			get { return Binding.SourcePath; }
			set { Binding.SourcePath = value; }
		}
		
		public string TargetPath
		{
			get { return Binding.TargetPath; }
			set { Binding.TargetPath = value; }
		}

		public BindingMode BindingMode
		{
			get { return Binding.Mode; }
			set { Binding.Mode = value; }
		}

		public object TargetNullValue
		{
			get { return Binding.TargetNullValue; }
			set { Binding.TargetNullValue = value; }
		}

		public object FallbackValue
		{
			get { return Binding.FallbackValue; }
			set { Binding.FallbackValue = value; }
		}

		public Type ValueConverterType
		{
			get { return null; }
			set { Binding.Converter = Activator.CreateInstance(value) as IValueConverter; }
		}

		public object ConverterParameter
		{
			get { return Binding.ConverterParameter; }
			set { Binding.ConverterParameter = value; }
		}

		public CultureInfo ConverterCulture
		{
			get { return Binding.ConverterCulture; }
			set { Binding.ConverterCulture = value; }
		}

		internal IValueConverter Converter 
		{
			get { return Binding.Converter; }
		}
	}
}