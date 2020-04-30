using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App1.Contracts
{
	public class Grouping<K, T> : ObservableCollection<T>
	{
		public K Name { get; private set; }
		public Grouping(K name, IEnumerable<T> items)
		{
			Name = name;
			foreach (T item in items)
				Items.Add(item);
		}
	}
}