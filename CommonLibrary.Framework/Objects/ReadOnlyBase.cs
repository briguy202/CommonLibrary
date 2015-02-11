using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	[Serializable]
	public abstract class ReadOnlyBase {

		#region Declarations
		private Hashtable _loadedHash;
		private string _objectID;
		private bool _isLoaded;
		private bool _isNew;
		#endregion

		#region Constructors
		public ReadOnlyBase() {
			_isLoaded = true;
			_isNew = true;
			_objectID = Guid.NewGuid().ToString();
		}
		#endregion

		#region Properties
		protected internal string ObjectID {
			get { return _objectID; }
			set { _objectID = value; }
		}

		protected bool IsLoaded {
			get { return _isLoaded; }
			set { _isLoaded = value; }
		}

		public bool IsNew {
			get { return _isNew; }
			set { _isNew = value; }
		}
		#endregion

		#region Methods
		public virtual string GetIdentificationString(string propertyName) {
			if ((propertyName == String.Empty)) {
				return ("ReadOnlyBase:" + (this.ObjectID.ToString() + (":" + "*")));
			} else {
				return ("ReadOnlyBase:" + (this.ObjectID.ToString() + (":" + propertyName)));
			}
		}

		public virtual string GetIdentificationString(int ID) {
			return ("ReadOnlyBase:" + (ID.ToString() + ":*"));
		}

		public virtual string GetGenericIdentificationString(string propertyName) {
			if ((propertyName == String.Empty)) {
				return ("ReadOnlyBase:*:" + "*");
			} else {
				return ("ReadOnlyBase:*:" + propertyName);
			}
		}

		protected bool IsPropertyLoaded(string property) {
			if (_loadedHash == null) {
				_loadedHash = new Hashtable();
			}
			return (_loadedHash.Contains(property));
		}

		protected void SetIsPropertyLoaded(string property) {
			if (!IsPropertyLoaded(property)) {
				_loadedHash.Add(property, property);
			}
		}

		protected virtual void SetIsLoaded() {
			this.IsLoaded = true;
			this.IsNew = false;
			if (_loadedHash != null) {
				_loadedHash.Clear();
			}
		}
		#endregion

	}
}