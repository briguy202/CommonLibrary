using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.Framework.Objects {
	public class GenericEditableCollection<T> : GenericReadOnlyCollection<T> {

		#region Constructors
		public GenericEditableCollection() : base() { }

		public GenericEditableCollection(Collection<T> collection) : base() {
			if (collection != null) {
				foreach (T item in collection) {
					this.Add(item);
				}
				this.ClearAddRemove();
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Saves all adds, deletes, and edits of the items in the collection to the persistence layer.
		/// </summary>
		public void Save() {
			this.SaveCollection();
			this.SaveItems();
		}

		public void SaveCollection() {
			this.OnPreSaveCollection();
			this.OnSaveCollection();
		}

		public void SaveItems() {
			this.OnPreSaveItems();
			this.OnSaveItems();
		}

		/// <summary>
		/// Method that is called before the saving process to handle custom saving logic.
		/// </summary>
		protected virtual void OnPreSaveCollection() { }

		/// <summary>
		/// Method that is called during the saving process to handle custom saving logic.
		/// </summary>
		protected virtual void OnSaveCollection() { }

		/// <summary>
		/// Method that is called before the saving process to handle custom saving logic.
		/// </summary>
		protected virtual void OnPreSaveItems() { }

		/// <summary>
		/// Method that is called during the saving process to handle custom saving logic.
		/// </summary>
		protected virtual void OnSaveItems() { }

		/// <summary>
		/// Deletes the business object from the persistence layer.
		/// </summary>
		public void Delete() {
			this.DeleteCollection();
			this.DeleteItems();
		}

		public void DeleteCollection() {
			this.OnPreDeleteCollection();
			this.OnDeleteCollection();
		}

		public void DeleteItems() {
			this.OnPreDeleteItems();
			this.OnDeleteItems();
		}

		/// <summary>
		/// Method that is called before the deletion process to handle custom deletion logic.
		/// </summary>
		protected virtual void OnPreDeleteCollection() { }

		/// <summary>
		/// Method that is called during the deletion process to handle custom deletion logic.
		/// </summary>
		protected virtual void OnDeleteCollection() { }

		/// <summary>
		/// Method that is called before the deletion process to handle custom deletion logic.
		/// </summary>
		protected virtual void OnPreDeleteItems() { }

		/// <summary>
		/// Method that is called during the deletion process to handle custom deletion logic.
		/// </summary>
		protected virtual void OnDeleteItems() { }

		/// <summary>
		/// Sets all items in the collection to non-dirty.
		/// </summary>
		internal void SetNotDirty() {
			foreach (EditableBase item in this) {
				item.SetNotDirty();
			}
		}

		/// <summary>
		/// Checks to see if any items are dirty
		/// </summary>
		/// <returns>True if any items are dirty, false otherwise.</returns>
		public bool HasDirtyItem() {
			foreach (EditableBase item in this) {
				if (item.IsDirty()) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Sets all objects in this collection as new
		/// </summary>
		internal void SetNew() {
			foreach (EditableBase obj in this) {
				obj.SetNew();
			}
		}
		#endregion

	}
}