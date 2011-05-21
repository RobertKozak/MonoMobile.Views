using System;
using MonoMobile.MVVM;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
namespace Samples
{
	public class ElementAPIView : UIView
	{
		//public string Text { get { return Get(()=>Text); } set { Set(()=>Text, value); } }
		public string Text { get; set; }
		public string Text2 { get; set; }

		protected IEnumerable<ISection> InitializeSections()
		{
			var sections = new List<ISection>()
			{
				new Section()
				{
					//new EntryElement("Element API", new Binding(this, "Text", "Value", null)) { Placeholder =  "Add stuff here" }
					new EntryElement("Element API") { Placeholder = "Add stuff here" }
				}
			};
			
			return sections;
		}

		public ElementAPIView(): base(new RectangleF(0,0,320, 80))
		{
			Text = "This";
			Text2 = "That";
		}
	}
}

