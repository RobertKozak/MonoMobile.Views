WHAT'S NEW
==========

Fixed INotifyCollectionChanged support.
 
Changed DataBinding to work with DataTemplates and no longer as partial classes 
to the elements.

Totally rewritten the MultilineElement to grow the Cell as you type (Lines = 0) or 
restricted to a set number of lines. (Still some redraw issues)

DateTimeElements are no longer working in this release. I am pushing this version up
so that Travis can get his code working. Don't get this one if you need Date or Time Elements

 
Travis Smith has  a blog post entitled "MonoMobile.MVVM, Round 1" which is a 
quick Hello World introduction to MonoMobile.MVVM. 

You can find it here: http://legomaster.net/2011/06/monomobile-mvvm-round-1/

Round 2 is now up:

http://legomaster.net/2011/06/monomobile-mvvm-round-2/

This looks like it can be a great resource.

	
Released Beta 0.9

It is almost done. I have a few more things to look into like: 
	
	Add a real sample
	Add more Documentation
	and finish up any other bugs I find.
	Add more bindable properties

This document has been updated.

NOTE:
=====

This is beta and not production ready. There are a number of bugs and is in active development.

There are many changes and fixes on the way.

Current Version
===============

Beta 0.9

MonoMobile.MVVM
================

MonoMobile.MVVM started as adding WPF/Silverlight style binding 
to MonoTouch.Dialog but in doing so it required more changes than 
expected and so it became this project rather than just a branch 
from MonoTouch.Dialog.

Most of the concepts of MonoTouch.Dialog are still there such as 
Cells, Elements, Roots, Sections. Refer to MonoTouch.Dialog for 
more information.

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

MonoMobile.MVVM is being released as Beta 0.5. I would like feedback and I have 
plans to take this to MonoDroid and possibly WP7. If this works we can have a common 
platform for all 3 major phone/tablet OSs

Take a look at the samples I provide to give you an idea what can be done.

If you want to help out on this project please send me a message.
    

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Application
===========

Creating an application in MonoMobile.MVVM is very easy. You don't need to create a 
app delegate. In the Main method just call Run passing in a Title, the type of the view 
you want to display and args.  

	namespace Samples
	{
		using MonoMobile.MVVM;
		
		public class Application : MonoMobileApplication
		{
			static void Main(string[] args)
			{
				Run("Sample View", typeof(TestView), args);
			}	
		}
	}

if you want to customize the run behavior there is an overload where you can pass in
the name of your AppDelegate.  

	namespace Samples
	{
		using MonoMobile.MVVM;
		
		public class Application : MonoMobileApplication
		{
			static void Main(string[] args)
			{
				Run("AppDelegate", args);
			}	
		}
	}

The main Properties of MonoMobileApp are:

		public static UIWindow Window { get; set; }
		public static UINavigationController NavigationController { get; set; }
		public static UIView MainView { get; set; }
		public static string Title { get; set; }

Data Binding
============

I've made a few changes to MonoTouch.Dialog to support data binding.

First, there is a new abstract Element type and a new property 
"DataContext".  DataContext holds the main value that will be translated to Control properties via IValueConverter

For example: The EntryElement has a UITextField and in this case the DataContext binds to the Text property of the UITextField.

[Bind] attribute takes a SourcePath and/or TargetPath. Source is assumed to be
the property it is attached to. Target is the property in the UIElement to bind to.

		[Entry]
		[Bind("DataContext")]
		public string MovieName 
		{
			get;
			set;
		}

In the example above, the string value of the property MovieName will be 
data bound to the DataContext property of EntryElement.

Note: Since DataContext is available by default in each Element type if there is
no [Bind] statement for it it will be done for you. So this is the same as
above:

		[Entry]
		public string MovieName 
		{
			get;
			set;
		}

Note: You can bind to any BindableProperty of Element and descendents. 

THERE ARE 45 BINDABLEPROPERTIES & MORE ON THE WAY.

You can see these in the source.

Other properties of BindAttribute:

		public string SourcePath		
		public string TargetPath
		public BindingMode BindingMode
		public object TargetNullValue
		public object FallbackValue
		public Type ValueConverterType
		public object ConverterParameter
		public CultureInfo ConverterCulture

Value Converters
================

Sometimes, you want to bind an integer value to a Entry element which has a DataContext property
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

Included IValueConverters:
	
	EnumConverter
	EnumerableConverter
	FloatConverter
	IntConverter
	MoneyConverter
	PercentConverter
	UriConverter
	
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

ViewModels and Views in MonoMobile.MVVM all derive from ObservableObject which
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

Themes
======

MonoMobile.MVVM has a full theming engine. Check the Samples for implementation and ideas.

Reflection API
==============

The reflection api follows MonoTouch.Dialog closely but differs in many important ways.

Main UI Elements:
-----------------

	[Entry]			- creates an EntryElement
	[Password]		- creates an EntryElement with IsPassword set to true
	[Date]			- creates a RootElement that can input a date via popup 
	[DateTime]		- creats a RootElement that can input a DateTime via popup
	[Html]			- creates a RootElement that can navigate to a webpage 
	[Map]			- creates a RootElement that can show a location on a map
	[Multiline] 	- multiline string Element
	[Ownerdrawn]	- draw your own element
	[Radio]			- creates a RadioButtonElement
	[Range]			- creates a UISlider	
	[String]		- creates a element to display a readonly string
	[Time]			- creates a RootElement to edit time via popup

Modifier Attributes:
--------------------
these are attributes that modify the Elements or another Attribute

	[Bind]			- used in DataBinding. See above
	[Caption] 		- use this caption instead of default
	[Checkmark]		- creates a CheckboxElement rather than a RadioButtonElement
	[List]			- Takes an IEnumerable and renders it inline in a section rather than on another page
	[Order]			- makes the order the elements are rendered more explicit
	[PopOnSelection]- for Enumerables and Enums tells it to popback after selection
	[Root]			- puts Elements into a RootElement, oposite of [List]
	[Section]		- starts a new section to hold elements
	[Skip] 			- ignore this field or property
	[View]			- used with lists of ViewModels to tell it which view to use to render
	[Theme]			- specify a theme to apply to this element or at the Class level all elements in view
						Themes can be stacked.

Method Attributes:
------------------
Attributes that work on Methods.

	[Button]		- creates a button that will execute this method when touched.
	[LoadMore]		- creates a button that has an activity control that displays while method is executing
	[NavbarButton]	- creates a button on the NavBar that will execute this method when touched.
	[PullToRefresh]	- this method will be called when user pulls down to refresh
	[Searchbar]		- this method will be called during the search funtion of the Searchbar. 
						On a class it means show the search bar automatically when View is loaded
	[Toolbar]		- creates a button on the toolbar that will execute this method when touched 


When the View is passed to the BindingContext it is parsed and a new TableView is created. Each field, 
property and method is analyzed and depending on type, value and attributes UIElements are created. 
There are certain built in rules, such as, if property type is bool then create a BooleanElement which can
be tweaked or overriden by an attribute. A [Checkmark] on a boolean will change the Element from a UISwitch
to a checkmark. This is known as Implict Elements or Implicit UI.

You can also specify Elements directly:

	public EntryElement UserName { get; set; }

if the Element is just declared and is null the ViewParser will create one for you and assign it our
property or field. This is also known as Explicit Elements or Explicit UI.

Dynamic Nature of Reflection and Databinding
============================================

MonoTouch has a very agressive linker. This is because the entire .Net Framework is compiled into your app since
iOS does not allow for Dlls to be shared between Apps. This can be very big so it is suggested that you change the 
Link Behavior in your project options. This will do a static analysis of your code and determine which members are 
not being called and remove them. This is problematic in that many classes in MonoMobile.MVVM are created dynamically 
as needed. For these classes you need to decorate them with a [Preserve(AllMembers=true)] attribute. 