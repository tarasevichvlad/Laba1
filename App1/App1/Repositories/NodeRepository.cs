using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using App1.Contracts;
using SQLite;

namespace App1.Repositories
{
	public class NodeRepository 
	{
		private SQLiteConnection database;

		public NodeRepository(string databasePath)
		{
			database = new SQLiteConnection(databasePath);
			database.CreateTable<Note>();
		}

		public IEnumerable<Note> GetNodes()
		{
			return database.Table<Note>();
		}

		public IEnumerable<string> GetTags()
		{
			var notes =  database.Table<Note>();
			return notes.SelectMany(s => s.Tags);
		}
		public Note GetNode(int id)
		{
			try
			{
				return database.Get<Note>(id);
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public int DeleteItem(int id)
		{
			return database.Delete<Note>(id);
		}

		public void SaveItem(Note item)
		{
			
			if (item.Id != 0)
			{
				database.Update(item);
			}
			else
			{
				database.Insert(item);
			}
		}

		
	}
}