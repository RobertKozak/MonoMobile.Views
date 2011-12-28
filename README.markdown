WHAT'S NEW
==========

This document has been updated but it is not at all complete. The basic concepts are outlined below
but I will get back to this soon and make a real tutorial. MonoMobile.Views is simple to use yet very complex
internally to do all of the magic that you see on screen.

Totally redesigned and redeveloped the codebase.
	
Released Beta 1.0

It is almost done. I have a few more things to look into like: 
	
	Add a real sample
	Add more Documentation
	and finish up any other bugs I find.

Robert Kozak (rkozak@gmail.com)
@robertkozak - Twitter

Current Version
===============

Beta 1.0

MonoMobile.Views
================

MonoMobile.Views started as adding WPF/Silverlight style binding 
to MonoTouch.Dialog but in doing so it required more changes than 
expected and so it became this project rather than just a branch 
from MonoTouch.Dialog.

Originally, I had retained most of the concepts of MonoTouch.Dialog
but after a redesign it is no longer like MonoTouch.Dialog except maybe
in spirit

MVVM, or the Model-View-ViewModel pattern is a pattern that promotes
the separation between Data, Business Logic and User Interface. The 
Model describes the Data, the View describes the UI and the ViewModel 
sits between the Model and View and contains all of the business logic.

The key to the MVVM pattern is the communication between the layers.
 
Data binding is one way to accomplish this communication in generic manner.

MonoMobile.Views is a framework which makes it very easy to create UI on the iPhone/iPad/iDevice
that is represented via Cells and UITableView. It removes the need to worry about
UITableViewControllers, UINavigationControllers, and DataSources. Developers only need 
to create Views. (See samples below)

Project Status
==============

MonoMobile.Views is being released as Beta 1.0. I would like feedback and I have 
plans to take this to MonoDroid and possibly WP7. If this works we can have a common 
platform for all 3 major phone/tablet OSs. If anyone wants to take on one of these
projects go ahead I would love to see it.

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

Creating an application in MonoMobile.Views is very easy. You don't need to create a 
app delegate. In the Main method just call Run passing in a Title, the type of the view 
you want to display and any optional args.  

	namespace Samples
	{
		using MonoMobile.Views;
		
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
		using MonoMobile.Views;
		
		public class Application : MonoMobileApplication
		{
			static void Main(string[] args)
			{
				Run("AppDelegate", args);
			}	
		}
	}

The main Properties and Methods of MonoMobileApp are:

		public static Type[] ViewTypes { get; private set;}
		public static UIWindow Window { get; set; }
		public static UINavigationController NavigationController { get; set; }
		public static List<object> Views { get; set; }
		public static List<DialogViewController> DialogViewControllers { get; private set;}

		public static DialogViewController CurrentDialogViewController { get; }
		public static UIViewController CurrentViewController { get; } 

		public static string Title { get; set; }
		public static Action ResumeFromBackgroundAction { get; set; }

		public override bool NetworkActivityIndicatorVisible  { get; set;}


		public static bool IsSearchbarVisible();
		public static void ToggleSearchbar();


Views and ViewModels
====================

MonoMobile.Views is an MVVM framework but doesn't have to be used as a "pure" MVVM framework. MVVM separates out the Model
ViewModel and View. Model gets and sets data, ViewModel packages that data and performs all of the business logic on the data
and the View renders the data. This framework concerns itself mostly with the View portion. 

Take a look at this HelloWorld app:

		namespace HelloWorld
		{
		  using MonoMobile.Views;
		  
		  public class HelloView: View
		  {
		  [Section]
		    [Entry]
		    public string Name { get; set; }
		
		  [Section]
		    [Button]
		    public void Hello()
		    {
		      Alert.Show("Hello World", string.Format("Hello {0}!", Name));
		    } 
		  }
			
		  public class Application : MonoMobileApplication
		  {
			public new static void Main(string[] args)
			{
				Run("HelloWorld", typeof(HelloView), args);
			}
		  }
		}

Here we have created a View with a few CellViewTemplates to tell the framework how to render the cells. We have two Sections
one has an entry type cell and the other has a Button.

In this particular case, there is no ViewModel and there is no Model. They are both optional but will probably be needed for more advanced apps.

The framework contains a ViewModel base class which implements INotifyPropertyChanged and will fire off notifications that the UI will respond to. So if you 
change a property of a ViewModel in code it will be reflected on the screen immediately.

The framework is smart enough to keep both the View and ViewModel in sync. So for example, if you have a PersonViewModel with 
a FirstName property set to "Robert" and a PersonView with the same property name FirstName the framework will make sure that
both of these properties have the same value.

On a Get MonoMobile.Views will check first to see if you have a ViewModel (found in the View's DataContext property) and return 
that and if not then it returns the value from the View.

On a Set MonoMobile.Views will set it both in the ViewModel (if exists) and in the View, and also in the UI (via IHandleNotifyPropertyChanged)
if you implemented it.

Data Binding
============

I've made a few changes to MonoTouch.Dialog to support data binding.

First, there is a no longer an Element type. When a View is parsed lightweight MemberData objects are created to 
represent the MemberInfo and is Value(s). There is a reference to the View and the ViewModel values.

Sections are created to hold a reference to these MemberData and CellViewTemplates. Cells are created for each MemberData
and Cells can have multiple CellViewTemplate Types associated with them. CellViewTemplates have a DataContext (IDataContext<MemberData>)
that allows access to the Values of the View and ViewModel.  
  
For example: The EntryAttribute has a UITextField and in this case the DataContext binds to the Text property of the UITextField.

		[Entry]
		public string MovieName 
		{
			get;
			set;
		}


CellViewTemplates
=================

CellViewTemplates are a combination of an Attribute, UIView, IValueConverter and Theme. This is a very extensible part of 
MonoMobile.Views framework. With a CellViewTemplate you can update, take action on a cell, bind data, change its appearance
and many more things. 

An associated UIView works like a controller and gives you access to Update, Selected of the cell. These features are represented
via interfaces so that you can create only what you need and no more. 

There are 22 interfaces you can apply to your cell's UIView:

	IAccessoryView
	IActivation
	ICaption
	ICellContent
	ICellViewTemplate
	ICommandButton
	ICustomDraw
	IDataContext
	IFocusable
	IHandleNotifyCollectionChanged
	IHandleNotifyPropertyChanged
	IInitializable
	IInitializeCell
	INavigatable
	IRequestImage
	ISearchable
	ISearchBar
	ISelectable
	ISizeable
	ITableViewStyle
	IThemeable
	IUpdateable

There is a base UIView to use with cells called CellView which has some of these features coded by default.

public class CellView<T> : UIView, 
	IDataContext<MemberData>, 
	IInitializable, 
	IInitializeCell, 
	ICellViewTemplate, 
	IUpdateable, 
	ICaption, 
	IThemeable, 
	IHandleNotifyPropertyChanged

You can use this base class and override the methods and properties as you need.

Themes and IValueConverters are described below.
 
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
	
	DataTimeConverter
	DataTimeToStringConverter
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

ViewModels in MonoMobile.Views all derive from ObservableObject which
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

MonoMobile.Views has a full theming engine. Check the Samples for implementation and ideas.

Reflection API
==============

The reflection api of MonoTouch.Dialog is similar to the model used in MonoMobile.Views but 
differs in many important ways. The most important is that the attributes are much more than they are 
in MonoTouch.Dialog. In this framework an attribute is usually a CellViewTemplate. CellViewTemplates derive
from Attribute and add in:

	CellViewType: 	This is the type of the view that will be used to render the cell
	Theme:			This is the Theme that will be applied to the cell
	ValueConverter:	This is a ValueConverter that will be used when assigning data to and from the cell and view/viewmodel
	View:			This is the View that will be used if you Navigate from the cell to a new View

Having all of this packaged together makes it very easy to create new functionality and cell types to MonoMobile.Views 

You can check out a github project called MonoMobile.Views.Templates to look at other templates and add your own.

 
Main UI Elements:
-----------------

	[Entry]			- creates an cell that can take an input
	[Password]		- creates an cell with IsPassword set to true
	[Date]			- creates a cell that can input a date via popup 
	[DateTime]		- creats a cell that can input a DateTime via popup
	[Html]			- creates a cell that can navigate to a webpage 
	[Map]			- creates a cell that can show a location on a map
	[Multiline] 	- multiline string cell
	[Range]			- creates a UISlider	
	[String]		- creates a cell to display a readonly string
	[Time]			- creates a cell to edit time via popup

Modifier Attributes:
--------------------
these are attributes that modify the cells or another Attribute

	[Caption] 		- use this caption instead of default
	[Checkmark]		- creates a CheckboxElement rather than a RadioButtonElement
	[List]			- Takes an IEnumerable and renders it inline in a section rather than on another page
	[Order]			- makes the order the elements are rendered more explicit
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


When creating a DialogViewController the View is parsed and a new TableView is created. Each field, 
property and method is analyzed and depending on type, value and attributes MemberData objects are created. 
There are certain built in rules, such as, if property type is bool then create a BooleanElement which can
be tweaked or overriden by an CellViewTemplate. A [Checkmark] on a boolean will change the Cell's UIView from a UISwitch
to a checkmark. 

[List], ListCellView and Enums, IEnumerable
==============================================

The list attribute is very powerful. When used on an Enum or IEnumerable type field or property it allows you to do Multiselection 
or single Selection without having to write any code.

The DisplayMode property allows you to tell the framework that you want to display your property as a List of individual cells
or as a Root Cell that when clicked navigates to a page of Cells of the items. There is even a Collapsable display mode (experimental)
that Displays the Root Cell and when clicked expands open in place to display the list of individual cells.

The SelectedAction property tells the framwork what do do when the cell is selected:

		NavigateToView,
		PopOnSelection,
		Selection,
		Multiselection,
		Custom,
		None

You can Navigate to a new view, return back once a selection is pressed, single selection, multiselection, or custom which allows you 
to handle the selection in a Selected method. None does nothing.

The SelectedItemMemberName is the name of a field or property that you want to hold the Selected item from the list.
The SelectedItemsMemberName does the same but it holds a lit of all the selected items when you SelectedAction is Multiselection.
You can specify SelectedAccessoryViews and UnselectedAccessoryViews is you want to display a custom checkmark.

There is an UnselectionBehavior property that tells the framework how you want to handle the Selected Item property when in MultiSelection 
and you are unselecting a value. 

		SetSelectedToCurrentValue,
		SetSelectedToPreviousValueOrNull,
		SetSelectedToNull

When in Multiselection the SelectedItems property will hold all of the selected items. But the SelectedItem property can hold on of three 
values depending on the UnselectionBehavior. If you set it to SetSelectionToCurrentValue (default) it will set it to the current item no 
matter if it was selected or unselected. If you choose SetSelectionToPrevious when you unselect an item it will set the SelectedItem property 
to the last item that was selected. And SetSelectedToNull will set the SelectedItem property to Null if an item was unselected.

Here is a list of the ListAtrtribute properties:
    
		public string SelectedItemMemberName { get; set; }
		public string SelectedItemsMemberName { get; set; }
		public Type SelectedAccessoryViewType { get; set; }		
		public Type UnselectedAccessoryViewType { get; set; }
		public DisplayMode DisplayMode { get; set; }
		public SelectionAction SelectionAction { get; set; }
		public UnselectionBehavior UnselectionBehavior { get; set; }
		public bool ReplaceCaptionWithSelection { get; set; }

Dynamic Nature of Reflection and Databinding
============================================

MonoTouch has a very agressive linker. This is because the entire .Net Framework is compiled into your app since
iOS does not allow for Dlls to be shared between Apps. This can be very big so it is suggested that you change the 
Link Behavior in your project options. This will do a static analysis of your code and determine which members are 
not being called and remove them. This is problematic in that many classes in MonoMobile.Views are created dynamically 
as needed. For these classes you need to decorate them with a [Preserve(AllMembers=true)] attribute. 

MonoMobile.View.Themes and MonoMobile.Views.Templates are two more GitHub projects for you to place new Themes and CellViewTemplates 
that you want to share with the world and enhance the MonoMobile.Views framework ecosystem.

Some simple examples
====================

Simple Helow World:

		namespace HelloWorld
		{
		  using MonoMobile.Views;
		  
		  public class HelloView: View
		  {
		  [Section]
		    [Entry]
		    public string Name { get; set; }
		
		  [Section]
		    [Button]
		    public void Hello()
		    {
		      Alert.Show("Hello World", string.Format("Hello {0}!", Name));
		    } 
		  }
			
		  public class Application : MonoMobileApplication
		  {
			public new static void Main(string[] args)
			{
				Run("HelloWorld", typeof(HelloView), args);
			}
		  }
		}



Simple Enum test:


		namespace EnumTest
		{
		  using MonoMobile.Views;
		  
		  public enum Numbers {One, Two, Three, Four, Five, Six, Seven, Eight, Nine }
		  
          public class EnumTestView: View
		  {
	        private Number SelectedNumber;

		  [Section]
		    [List(SelectedItemMemberName = "SelectedNumber")]
		    public Numbers Number { get; set; }
		
		  [Section]
		    [Button]
		    public void ClickMe()
		    {
		      Alert.Show("Enum Test", string.Format("Selected {0}!", SelectedNumber.ToString()));
		    } 
		  }
			
		  public class Application : MonoMobileApplication
		  {
			public new static void Main(string[] args)
			{
				Run("Enum Test", typeof(EnumTestView), args);
			}
		  }
		}



Simple MultiSelect test:


		namespace MultiselectTest
		{
		  using MonoMobile.Views;
		  
		  public string[] Numbers {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" }
		  
          public class MultiselectTestView: View
		  {
	        private string SelectedNumber;
	        private string[] SelectedNumbers;

		  [Section]
		    [List(SelectedItemMemberName = "SelectedNumber", SelectedItemsMemberName = "SelectedNumbers")]
		    public string Number { get; set; }
		
		  [Section]
		    [Button]
		    public void ClickMe()
		    {
		      Alert.Show("Multiselect Test", string.Format("Last selected {0}!  Number Selected", SelectedNumber.ToString(), SelectedNumbers.Length));
		    } 
		  }
			
		  public class Application : MonoMobileApplication
		  {
			public new static void Main(string[] args)
			{
				Run("Multiselect Test", typeof(MultiselectTestView), args);
			}
		  }
		}