using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MonoTouch.Dialog
{
	[Preserve(AllMembers = true)]
	public class ListBoxElement : Section
	{
		public ListBoxElement() : base("")
		{
		}

		public ListBoxElement(string caption) : base(caption)
		{
		}

		public ListBoxElement(string caption, string footer) : base(caption, footer)
		{
		}

		public ListBoxElement(UIView header) : base(header)
		{
		}

		public ListBoxElement(UIView header, UIView footer) : base(header, footer)
		{
		}
	}
}


