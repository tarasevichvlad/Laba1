using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using App1.Contracts;
using App1.Pages;
using App1.Services;
using Xamarin.Forms;

namespace App1
{
	public partial class MainPage : ContentPage
	{
		private readonly NoteService _noteService;
		public MainPage()
		{
			Title = "Laboratornaya rabota 1";
			_noteService = new NoteService();

			Init();

			_noteService.CollectionChanged += (sender, args) =>
			{
				Init();
			};
		}

		public void Init()
		{
			var groupLabel = new Label { Text = "Is group"};
			var groupSwitch = new Switch();
			groupSwitch.IsToggled = true;
			var groupLayout = new StackLayout
			{
				Children = { groupLabel, groupSwitch}
			};

			var findInput =new Entry
			{
				Text = "",
				Margin = new Thickness(
					0,
					10,
					0,
					0)
			};

			var newNoteButton = new Button { Text = "New note" };
			var findLayer = new StackLayout
			{
				Children = { findInput, newNoteButton }
			};

			var header = new Label
			{
				Text = "Notes",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center,
				Padding = new Thickness(0, 10)
			};

			var notes = _noteService.GetNotes();

			var listNotesView = UpdateView(groupSwitch, header, notes);

			listNotesView.ItemTapped += OnItemTapped;
			newNoteButton.Clicked += NewNote;
			groupSwitch.Toggled += Find;
			findInput.TextChanged += Find;

			void Find(object sender, EventArgs e)
			{
				var findItems = _noteService.Find(findInput.Text);
				var view = UpdateView(groupSwitch, header, findItems);

				listNotesView.IsGroupingEnabled = view.IsGroupingEnabled;
				listNotesView.ItemsSource = view.ItemsSource;
				listNotesView.GroupDisplayBinding = view.GroupDisplayBinding;
			}

			Padding = new Thickness(10, 0, 10, 0);
			Content = new StackLayout { Children = { findLayer, groupLayout, listNotesView } };
		}

		public ListView UpdateView(Switch groupSwitch, Label header, IEnumerable<Note> notes)
		{
			if (groupSwitch.IsToggled)
			{
				return new ListView
				{
					IsGroupingEnabled = true,
					GroupDisplayBinding = new Binding("Name"),
					Header = header,
					HasUnevenRows = true,
					ItemsSource = notes.GroupBy(x => x.color)
						.Select(x => new Grouping<string, Note>(x.Key, x)),
					ItemTemplate = new DataTemplate(() =>
					{
						var image = new BoxView();
						image.SetBinding(BackgroundColorProperty, "Color");

						var body = new Label();
						body.SetBinding(Label.TextProperty, "Body");

						var date = new Label();
						date.SetBinding(Label.TextProperty, "Date");

						return new ViewCell
						{
							View = new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Padding = new Thickness(0, 5),
								Children = {image, body, date}
							}
						};
					})
				};
			}

			return new ListView
			{
				Header = header,
				HasUnevenRows = true,
				ItemsSource = notes,
				ItemTemplate = new DataTemplate(() =>
				{
					var image = new BoxView();
					image.SetBinding(BackgroundColorProperty, "Color");

					var body = new Label();
					body.SetBinding(Label.TextProperty, "Body");

					var date = new Label();
					date.SetBinding(Label.TextProperty, "Date");

					return new ViewCell
					{
						View = new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Padding = new Thickness(0, 5),
							Children = { image, body, date}
						}
					};
				})
			};
		}

		public async void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			var selectedNote = e.Item as Note;
			if (selectedNote != null)
				await Navigation.PushModalAsync(new NotePage(selectedNote, _noteService));
		}

		public async void NewNote(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NotePage(new Note(), _noteService));
		}
	}
}