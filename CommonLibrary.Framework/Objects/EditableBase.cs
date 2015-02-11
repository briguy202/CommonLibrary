using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace CommonLibrary.Framework.Objects {
	[Serializable]
	public abstract class EditableBase : ReadOnlyBase {

		#region Declarations
		private Hashtable _dirtyHash = new Hashtable();
		private bool _isDeleted = false;
		private bool _wasDirty = false;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new instance of this object.
		/// </summary>
		public EditableBase() : base() { }
		#endregion

		#region Properties
		public bool IsDeleted {
			get { return _isDeleted; }
			set { _isDeleted = value; }
		}

		public bool WasDirty {
			get { return _wasDirty; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Saves the business object to the persistence layer.
		/// </summary>
		public void Save() {
			if (this.IsDirty()) {
				this.OnPreSave();
				this.OnSave();
			}
		}

		/// <summary>
		/// Method that is called before the saving process to handle custom saving logic.
		/// </summary>
		protected virtual void OnPreSave() { }

		/// <summary>
		/// Method that is called during the saving process to handle custom saving logic.
		/// </summary>
		protected virtual void OnSave() { }

		/// <summary>
		/// Deletes the business object from the persistence layer.
		/// </summary>
		public void Delete() {
			if (!this.IsNew) {
				this.OnPreDelete();
				this.OnDelete();
			}
		}

		/// <summary>
		/// Method that is called before the deletion process to handle custom deletion logic.
		/// </summary>
		protected virtual void OnPreDelete() { }

		/// <summary>
		/// Method that is called during the deletion process to handle custom deletion logic.
		/// </summary>
		protected virtual void OnDelete() { }

		public void SetNotDirty() {
			_dirtyHash.Clear();
			this.IsNew = false;
		}

		public void SetNotDirty(string propertyName) {
			_dirtyHash[propertyName] = false;
		}

		public bool IsDirty() {
			if (this.IsNew) {
				return true;
			} else {
				foreach (string key in _dirtyHash.Keys) {
					if (this.IsPropertyDirty(key)) {
						return true;
					}
				}

				return false;
			}
		}

		public bool IsPropertyDirty(string propertyName) {
			if (_dirtyHash.ContainsKey(propertyName)) {
				return (bool)_dirtyHash[propertyName];
			} else {
				return false;
			}
		}

		public StringCollection GetAllDirtyProperties() {
			StringCollection sc = new StringCollection();
			foreach (string key in _dirtyHash.Keys) {
				if ((bool)_dirtyHash[key]) { sc.Add(key); }
			}
			return sc;
		}

		protected void SetDirty<T>(string propertyName, T oldValue, T newValue) where T : IComparable {
			if ((IComparable)oldValue != (IComparable)newValue) {
				this.SetDirty(propertyName);
			}
		}

		protected void SetDirty(string propertyName) {
			_dirtyHash[propertyName] = true;
			_wasDirty = true;
		}

		internal void SetDeleted() {
			_isDeleted = true;
		}

		internal void SetNew() {
			this.IsLoaded = true;
			_isDeleted = false;
			this.IsNew = true;
		}

		protected override void SetIsLoaded() {
			base.SetIsLoaded();
			this.SetNotDirty();
			_isDeleted = false;
		}
		#endregion

	}
}