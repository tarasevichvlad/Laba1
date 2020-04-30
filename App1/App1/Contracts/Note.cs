using System;
using System.Drawing;
using SQLite;

namespace App1.Contracts
{
	[Table("Nodes")]
	public class Note
	{
		[PrimaryKey, AutoIncrement, Column("_id")]
		public int Id { get; set; }
		public string Body { get; set; } = string.Empty;
		public string Tags { get; set; }
		public string color { get; set; }

		[Ignore]
		public Color Color
		{
			get => Color.FromName(color ?? Color.Black.Name);
			set => color = value.Name;
		}

		public DateTime Date { get; set; } = DateTime.Now;

		public void Update(Note note)
		{
			Body = note.Body;
			Tags = note.Tags;
			Color = note.Color;
		}
	}
}