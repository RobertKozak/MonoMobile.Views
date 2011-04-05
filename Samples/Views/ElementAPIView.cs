using System;
using MonoTouch.MVVM;
using System.Collections.Generic;
using MonoTouch.Dialog;
namespace Samples
{
	public class ElementAPIView : View
	{
		protected override IEnumerable<ISection> InitializeSections()
		{
			var sections = new List<ISection>()
			{
				new Section()
				{
					new EntryElement("Element API", "Add stuff here")
				}
			};

			return sections;
		}

		public ElementAPIView()
		{
		}
	}
}

