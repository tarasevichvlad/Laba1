using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using App1.Contracts;
using App1.Repositories;

namespace App1.Services
{
	public class NoteService : INotifyCollectionChanged
	{
		private const string DatabaseName = "nodes2.db";
		private static NodeRepository _database;
		public static NodeRepository Database
		{
			get
			{
				if (_database == null)
				{
					_database = new NodeRepository(
						Path.Combine(
							Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseName));
				}
				return _database;
			}
		}

		public NoteService()
		{
			CollectionChanged = OnCollectionChanged;
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
		}

		public IEnumerable<Note> GetNotes()
		{
			return Database.GetNodes();
		}

		public IEnumerable<Note> Find(string query)
		{
			var notes = Database.GetNodes();
			if (query.Equals("*"))
			{
				return notes;
			}

			return notes.Where(x => x.Body.Contains(query) || x.Tags.Contains(query) || x.color.Equals(query)).ToList();
		}

		public void Save(Note note)
		{
			var item = Database.GetNode(note.Id);

			if (item == null)
			{
				Database.SaveItem(note);
				Notify(NotifyCollectionChangedAction.Add);
			}
			else
			{
				item.Update(note);
				Database.SaveItem(item);
				Notify(NotifyCollectionChangedAction.Replace);
			}
		}

		public void Notify(NotifyCollectionChangedAction action)
		{
			if (CollectionChanged != null) CollectionChanged(this, null);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}