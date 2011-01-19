IMPORTANT NOTE:
===============

This version works fine in the Simulator but there are issues with 
the linker that gives subtle errors when deployed to the iPhone.

I'll have a solution within the next few days now that I kinda see 
whats going on.

1. Because MonoTouch has a very aggressive linker if you bind to 
something like "Entry.TextAlignment" it can fail if TextAlignment 
is not used directly. I plan on exposing the most common properties
to link to in each element in a Region called BindableProperties.

2. Because of the way I am doing Generics in Root<T> (and possibly 
Element<T>) I need to investigate another way of doing this. One of 
the Errors I am seeing is after a RootElement is created it can fail 
with an "Argument is out of range. Parameter name: index". This one
is particularly interesting since I dont have an index property.

But don't worry. I'm sure I'll have an answer in a few days :-)

MonoTouch.MVVM
================

MonoTouch.MVVM started as adding WPF/Silverlight style binding 
to MonoTouch.Dialog but in doing so it required more changes than 
expected and so it became this project rather than just a branch 
from MonoTouch.Dialog.

Most of the concepts of MonoTouch.Dialog are still there (in fact, 
at this point, the changes aren't that major but will be in the 
future) such as Cells, Elements, Roots, Sections. Refer to 
MonoTouch.Dialog for more information.

MVVM, or the Model-View-ViewModel pattern is a pattern that promotes
the separation between Data, Business Logic and User Interface. The 
Model describes the Data, the View describes the UI and the ViewModel 
sits between the Model and View and contains all of the business logic.

The key to the MVVM pattern is the communication between the layers.
 
Data binding is one way to accomplish this communication in generic manner.

Robert Kozak (rkozak@gmail.com)
@robertkozak - Twitter

Project Status
==============

MonoTouch.MVVM is being released as Alpha 0.1. I would like feedback and I have 
plans to take this to MonoDroid and possibly WM7. If this works we can have a common 
platform for all 3 major phone/tablet OSs

Consider this to be highly experimental POC release. I have a lot more refactoring to do
and I have a lot of ideas I want to incorporate but it is a good start. 

Take a look at the sample I provided to give you an idea what can be done.

If you want to help out on this project please send me a message.
    

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


Data Binding
===========

I've made a few changes to MonoTouch.Dialog to support data binding.

First, there is a never Element type that is generic and a new property 
"Value" to hold the value of the generic type. Element<string> has a 
property Value of type string and Element<int> has a Value of type int.

The other concrete Element types derive from Element<T> and so they all
have a Value property. This is the default binding property:

		[Entry]
		[Bind("Value")]
		public string MovieName 
		{
			get;
			set;
		}

In the example above, the string value of the property MovieName will be 
data bound to the Value property of EntryElement.

Note: Since Value is available by default in each Element type if there is
no [Bind] statement for it it will be done for you. So this is the same as
above:

		[Entry]
		public string MovieName 
		{
			get;
			set;
		}

Note: Any public property of Element or derived classes can be data bound, except for Cell.
There is no real programatic limitation of binding to Cell but the cell of an element is 
virtualized by iOS and if you have more cells than what can be seen on the screen it will 
be reused. And your binding may affect another unrelated cell. For this reason I suggest 
you don't bind to any property of Cell and only to public properties of an Element.

Value Converters
================

Sometimes, you want to bind an integer value to a Entry element which has a Value property
of type string or you want to bind a float and you want to display it as $10 rather than 10.0.

You do this with an IValueConverter. IValueConverter has Convert and Convertback methods that
is part of the binding update process to automatically take care of conversions for you.

Here is a simple example of converter that takes a float and returns a string formatted as 
a Percentage:

	public class PercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return value;
			
			return string.Format("{0}%", value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var result = ((string)value).Replace("%", "");
			return result;
		}
	}

and he is how it used:

	[Bind(ValueConverterType = typeof(PercentConverter)]
	public float CriticsRating 
	{ 
		get { return Get(()=>CriticsRating); } 
		set { Set(()=>CriticsRating, value); }
	} 

INotifyPropertyChanged 
======================

In order for data binding to work the framework needs to know then a property changes.
We do that using an interface called INotifiyPropertyChanged or INPC for short. It
is typically implemented like this:

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(String info)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

and in the setter of your properties you call NotifyPropertyChanged().

ViewModels and Views in MonoTouch.MVVM all derive from PropertyNotifier which
has two methods to help with this: Set() and Get().

	public string FirstName 
	{ 
		get { return Get(()=>FirstName); } 
		set { Set(()=>FirstName, value); }
	} 

The Set() method knows how to call the PropertyChanged event and 
since it is a function rather than a string it is compile time safe. The 
standard way of calling PropertyChanged uses a string for the name of the 
property that changed. This can lead to errors if you change the Property 
declaration and forget to update the INPC call. Using Set() eliminates this
problem. 

It also uses a Dictionary to hold the property values so you don't need a backing field.

