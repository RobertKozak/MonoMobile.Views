// 
//  MemberData.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.Views
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class MemberData
	{
		private object _Value;

		public object Value { get { return GetValue(); } set { SetValue(value); } }
		public Type Type { get; set; }

		public NSString Id { get; private set; }
 
		public object DataContextSource { get; private set; }
		public MemberInfo DataContextMember { get; private set; }

		public object Source { get; private set; }
		public MemberInfo Member { get; private set; }
 
		public int Section { get; set; }
		public int Order { get; set; }
		public float RowHeight { get; set; }

		public MemberData(object source, MemberInfo member)
		{
			Source = source;
			Member = member;

			Type = member.GetMemberType();
			Id = new NSString(Type.ToString());
		}

		protected virtual object GetValue()
		{
			if (Member != null && Source != null)
			{
				var view = Source as IDataContext<object>;
				if (view != null && view.DataContext != null)
				{
					DataContextSource = view.DataContext;
					DataContextMember = DataContextSource.GetType().GetMember(Member.Name).FirstOrDefault();
					
					if (DataContextMember != null)
						return DataContextMember.GetValue(DataContextSource);
				}

				return Member.GetValue(Source);
			}

			return _Value;
		}

		protected virtual void SetValue(object value)
		{
			_Value = value;
			
			if (_Value != null)
			{
				Type = _Value.GetType();
			}
			
			if (DataContextMember != null)
				DataContextMember.SetValue(DataContextSource, value);

			Member.SetValue(Source, value);

			// Reset Type since the base TypeData sets it to the type of the DataContext 
			Type = Member.GetMemberType();
		}
	}
}

