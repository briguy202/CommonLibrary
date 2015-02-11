using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommonLibrary.Framework.Objects {
	public class GenericReadOnlyCollection<T> : CollectionBase {

		#region Declarations
		private ArrayList _removed;
		private ArrayList _added;
		private bool _isCollectionDirty;
		#endregion

		#region Constructors
		public GenericReadOnlyCollection() {
			_removed = new ArrayList();
			_added = new ArrayList();
			this.IsCollectionDirty = false;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Provides access to items in this collection by index
		/// </summary>
		public T this[int index] {
			get {
				return (T)this.List[index];
			}
			set {
				if (index < this.Count)
					ItemRemoved(this[index]);

				this.List[index] = (T)value;
				ItemAdded(this[index]);
			}
		}

		public bool IsCollectionDirty {
			get { return _isCollectionDirty; }
			private set { _isCollectionDirty = value; }
		}

		public T[] AddedItems {
			get {
				T[] _array = new T[_added.Count];
				for (int i = 0; i < _added.Count; i++)
					_array[i] = (T)_added[i];

				return _array;
			}
		}

		public T[] RemovedItems {
			get {
				T[] _array = new T[_removed.Count];
				for (int i = 0; i < _removed.Count; i++)
					_array[i] = (T)_removed[i];

				return _array;
			}
		}

		#endregion

		#region Methods
		/// <summary>
		/// Add an item to the collection
		/// </summary>
		/// <param name="value"> object to add to the collection</param>
		/// <returns>The index of the object in the collection</returns>
		public int Add(T value) {
			this.ItemAdded(value);
			return (this.List.Add(value));
		}

		/// <summary>
		/// Checks if the object is contained in this collection
		/// </summary>
		/// <param name="value">The object to check for existence in this collection</param>
		/// <returns>True if the object exists in this collection, false otherwise</returns>
		public bool Contains(T value) {
			return (this.List.Contains(value));
		}

		/// <summary>
		/// Finds the index of the object in this collection
		/// </summary>
		/// <param name="value">The object to find the index of</param>
		/// <returns>Index of the object in this collection</returns>
		public int IndexOf(T value) {
			return (this.List.IndexOf(value));
		}

		/// <summary>
		/// Inserts an object into the collection at the specified index
		/// </summary>
		/// <param name="index">The index to insert the object at</param>
		/// <param name="value">The object to insert</param>
		public void Insert(int index, T value) {
			this.ItemAdded(value);
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Removes the first occurance of an object from the collection
		/// </summary>
		/// <param name="value">The object to remove from the collection</param>
		public void Remove(T value) {
			ItemRemoved(value);
			if (this.List.Contains(value)) { this.List.Remove(value); }
		}

		/// <summary>
		/// Stores a reference to removed items.
		/// </summary>
		protected void ItemRemoved(T removedObject) {
			int pos;
			if (removedObject != null) {
				pos = _added.IndexOf(removedObject);
				if (pos >= 0) {
					_added.Remove(removedObject);
				} else {
					_removed.Add(removedObject);
				}
				this.IsCollectionDirty = true;
			}
		}

		/// <summary>
		/// Stores a reference to added items.
		/// </summary>
		protected void ItemAdded(T addedObject) {
			int pos;
			if (addedObject != null) {
				pos = _removed.IndexOf(addedObject);
				if (pos >= 0) {
					_removed.Remove(addedObject);
				} else {
					_added.Add(addedObject);
				}
				this.IsCollectionDirty = true;
			}
		}

		/// <summary>
		/// Clears the removed/added item collections.
		/// </summary>
		public void ClearAddRemove() {
			_removed.Clear();
			_added.Clear();
			this.IsCollectionDirty = false;
		}

		public ArrayList GetAddedItems() {
			return ((ArrayList)(_added.Clone()));
		}

		public ArrayList GetRemovedItems() {
			return ((ArrayList)(_removed.Clone()));
		}

		/// <summary>
		/// Removes the item at the specified index
		/// </summary>
		public new void RemoveAt(int index) {
			if (index < this.List.Count) {
				this.ItemRemoved((T)this.List[index]);
			}

			base.RemoveAt(index);
		}

		/// <summary>
		/// Removes all items in the collection.
		/// </summary>
		public new void Clear() {
			while ((this.List.Count > 0)) {
				this.RemoveAt(0);
			}
		}

		/// <summary>
		/// Sorts the elements in the entire collection using the IComparable implementation of each element.
		/// </summary>
		public void Sort() {
			this.InnerList.Sort();
		}

		/// <summary>
		/// Sorts the elements in a section of the collection using the specified comparer.
		/// </summary>
		/// Index of the item on which to start sorting
		/// Number of items from Index to sort
		/// Comparer class to use for sorting
		public void Sort(int index, int count, IComparer comparer) {
			this.InnerList.Sort(index, count, comparer);
		}

		/// <summary>
		/// Sorts the elements in the entire collection using the specified comparer.
		/// </summary>
		/// Comparer class to use for sorting
		public void Sort(IComparer comparer) {
			this.InnerList.Sort(comparer);
		}

		/// <summary>
		/// This method will restore the object to its previous state by replacing/removing the
		/// objects in the added/removed collections and resetting the dirty flag.
		/// </summary>
		public void Restore() {
			foreach (T bo in this.RemovedItems) {
				this.List.Add(bo);
			}
			foreach (T bo in this.AddedItems) {
				if (this.List.Contains(bo)) {
					this.List.Remove(bo);
				}
			}

			// Reset internal state
			this.ClearAddRemove();
		}
		#endregion

	}
}