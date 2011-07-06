using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoMobile.MVVM;
using System.Collections.ObjectModel;

namespace MVVMExample
{

	public class NotesView : View
	{
		protected new NotesViewModel DataContext
		{
			get { return (NotesViewModel)GetDataContext(); }
		}

		public NotesView()
		{
		}

		[List(ViewType = typeof(NoteItemView))]
		public ObservableCollection<NoteItem> Notes
		{
			get { return DataContext.Notes; }
			set { DataContext.Notes = value; }
		}

		[NavbarButton("+")]
		public void AddNote()
		{
			Notes.Add(new NoteItem { Name = "Default" });
		}

		public override void Initialize()
		{
			base.DataContext = new NotesViewModel();
		}

		[ToolbarButton]
		public void ChangeNote()
		{
			Notes[0].Name = "Two!";
		}
	}

	public class NotesViewModel : ViewModel
	{
		public NotesViewModel()
		{
			var dataModel = new NotesDataModel();
			Notes = new ObservableCollection<NoteItem>();
			foreach (var item in dataModel.Load())
				Notes.Add(item);
		}

		public ObservableCollection<NoteItem> Notes
		{
			get { return Get(() => Notes); }
			set { Set(() => Notes, value); }
		}
	}

	public class NoteItem : ViewModel
	{
		public string Name
		{
			get { return Get(() => Name); }
			set { Set(() => Name, value); }
		}
		public string Text
		{
			get { return Get(() => Text); }
			set { Set(() => Text, value); }
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class NoteItemView : View
	{
		[Section]
		[Entry]
		public string Name
		{
			get;
			set;
		}
		
		[Section("Note")]
		//[Entry]
		[Multiline(5)]
		[Caption("")]
		public string Text
		{
			get;
			set;
		}
	}

	public class NotesDataModel
	{
		public IEnumerable<NoteItem> Load()
		{
				/*					new NoteViewModel { Name = "Eight" }, 
					new NoteViewModel { Name = "Nine" }, 
					new NoteViewModel { Name = "Ten" },
					new NoteViewModel { Name = "Eleven" }, 
					new NoteViewModel { Name = "Dozen" } */				
			return new List<NoteItem> { new NoteItem { Name = "One" }, new NoteItem { Name = "Two" }, new NoteItem { Name = "Three" }, new NoteItem { Name = "Four" }, new NoteItem { Name = "Five" }, new NoteItem { Name = "Six" }, new NoteItem { Name = "Seven" } }			;
		}
	}
}
