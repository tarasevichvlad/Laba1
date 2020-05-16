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
			var groupLabel = new Label { Text = "Is group by color"};
			var groupSwitch = new Switch();
			var groupLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { groupSwitch, groupLabel }
			};

			var groupLabel1 = new Label { Text = "Is group by tags"};
			var groupSwitch1 = new Switch();
			var groupLayout1 = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { groupSwitch1, groupLabel1 }
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

			var listNotesView = UpdateView(groupSwitch, groupSwitch1, header, notes);

			listNotesView.ItemTapped += OnItemTapped;
			newNoteButton.Clicked += NewNote;
			groupSwitch.Toggled += (sender, args) =>
			{
				if ((sender as Switch).IsToggled)
				{
					groupSwitch1.IsToggled = false;
				}
				
				Find(sender, args);
			};
			groupSwitch1.Toggled += (sender, args) =>
			{
				if ((sender as Switch).IsToggled)
				{
					groupSwitch.IsToggled = false;
				}
				
				Find(sender, args);
			};

			findInput.TextChanged += Find;

			void Find(object sender, EventArgs e)
			{
				var findItems = _noteService.Find(findInput.Text);
				var view = UpdateView(groupSwitch, groupSwitch1, header, findItems);

				listNotesView.IsGroupingEnabled = view.IsGroupingEnabled;
				listNotesView.ItemsSource = view.ItemsSource;
				listNotesView.GroupDisplayBinding = view.GroupDisplayBinding;
			}

			Padding = new Thickness(10, 0, 10, 0);
			Content = new StackLayout { Children = { findLayer, groupLayout, groupLayout1, listNotesView } };
		}

		public ListView UpdateView(Switch groupSwitch1, Switch groupSwitch2, Label header, IEnumerable<Note> notes)
		{
			if (groupSwitch1.IsToggled)
			{
				groupSwitch2.IsToggled = false;
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

						var title = new Label { FontSize = 15 };
						title.SetBinding(Label.TextProperty, "Title");

						var body = new Label();
						body.SetBinding(Label.TextProperty, "Body");

						var date = new Label();
						date.SetBinding(Label.TextProperty, "Date");

						var layout = new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							//Padding = new Thickness(0, 5),
							Children = { title, body, date }
						};

						return new ViewCell
						{
							View = new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Padding = new Thickness(0, 5),
								Children = { image, layout }
							}
						};

					})
				};
			}

			if (groupSwitch2.IsToggled)
			{
				groupSwitch1.IsToggled = false;
				var items = notes
					.GroupBy(x => x.Tags)
					.SelectMany(x => x.Key.Select(y => new Grouping<string, Note>(y, x.Select(i => i))))
					.GroupBy(x => x.Name)
					.Select(x => new Grouping<string, Note>(x.Key, x.SelectMany(i => i.Select(p => p))));

					return new ListView
				{
					IsGroupingEnabled = true,
					GroupDisplayBinding = new Binding("Name"),
					Header = header,
					HasUnevenRows = true,
					ItemsSource = items,
					ItemTemplate = new DataTemplate(() =>
					{
						var image = new BoxView();
						image.SetBinding(BackgroundColorProperty, "Color");

						var title = new Label { FontSize = 15 };
						title.SetBinding(Label.TextProperty, "Title");

						var body = new Label();
						body.SetBinding(Label.TextProperty, "Body");

						var date = new Label();
						date.SetBinding(Label.TextProperty, "Date");

						var layout = new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							//Padding = new Thickness(0, 5),
							Children = { title, body, date }
						};

						return new ViewCell
						{
							View = new StackLayout
							{
								Orientation = StackOrientation.Horizontal,
								Padding = new Thickness(0, 5),
								Children = { image, layout }
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

					var title = new Label { FontSize = 15 };
					title.SetBinding(Label.TextProperty, "Title");

					var body = new Label();
					body.SetBinding(Label.TextProperty, "Body");

					var date = new Label();
					date.SetBinding(Label.TextProperty, "Date");

					var layout = new StackLayout
					{
						Orientation = StackOrientation.Vertical,
						//Padding = new Thickness(0, 5),
						Children = { title, body, date }
					};

					return new ViewCell
					{
						View = new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Padding = new Thickness(0, 5),
							Children = { image, layout }
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