using System;
using System.Linq;
using App1.Contracts;
using App1.Services;
using Xamarin.Forms;
using Color = System.Drawing.Color;

namespace App1.Pages
{
	public class NotePage : ContentPage
	{
		private readonly NoteService _noteService;
		private Note currentNote;

		public NotePage(Note note, NoteService noteService)
		{
			_noteService = noteService;
			currentNote = note;

			Initialize(currentNote);
		}

		private void Initialize(Note note)
		{
			var tagsLabel = new Label { Text = "Tags" };
			var noteTitle = new Label { Text = "Title" };
			var noteLabel = new Label { Text = "Note" };
			var noteColor = new Label { Text = "Color" };
			var title = new Editor { Text = note.Body, Placeholder = "Enter note title", AutoSize = EditorAutoSizeOption.TextChanges};
			var body = new Editor { Text = note.Body, Placeholder = "Enter your note", AutoSize = EditorAutoSizeOption.TextChanges};
			var tags = new Entry{ Text = note.tags ?? string.Empty };
			var colors = new Picker();
			var existTags = new Picker();

			var tagsList = _noteService.GetTags();
			tagsList = tagsList.Where(t => !note.Tags.Contains(t));
			foreach (var tag in tagsList)
			{
				existTags.Items.Add(tag);
			}

			existTags.SelectedIndexChanged += (sender, args) =>
			{
				var obj = sender as Picker;
				var tag = obj.SelectedItem as string;
				tags.Text = $"{tags.Text} {tag}";
				existTags.Items.Clear();
				var list = tagsList.Where(x => !x.Equals(tag));
				foreach (var tagn in list)
				{
					existTags.Items.Add(tagn);
				}
			};

			colors.Items.Add(Color.Red.Name);
			colors.Items.Add(Color.Yellow.Name);
			colors.Items.Add(Color.Green.Name);
			colors.Items.Add(Color.Blue.Name);
			colors.SelectedItem = note.Color.Name;

			var saveButton = new Button { Text = "Save" };
			
			saveButton.Clicked += async (sender, e) =>
			{
				currentNote.Color = new ColorConvertor().Convert(colors.SelectedItem);
				currentNote.Body = body.Text;
				currentNote.tags = tags.Text;
				currentNote.Title = title.Text;
				currentNote.Date = DateTime.Now;
				Save(currentNote);
				await Navigation.PopModalAsync();
			};

			Padding = new Thickness(
				10,
				Device.RuntimePlatform == Device.iOS ? 40 : 0,
				10,
				0);
			Content = new StackLayout
			{
				Children = { tagsLabel, tags, existTags, noteTitle, title, noteLabel, body, noteColor, colors, saveButton }
			};
		}

		private void Save(Note note)
		{
			_noteService.Save(note);
		}
	}
}