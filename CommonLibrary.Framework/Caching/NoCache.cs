using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.Framework.Caching {
	internal class NoCache : ICache, IDisposable {
		private Hashtable _hash;
		private string _ID;
		private string _manager;

		#region Constructors
		internal NoCache() {
			_hash = new Hashtable();
			this.ID = System.Guid.NewGuid().ToString();
			this.Manager = "Default";
		}
		#endregion

		#region ICache Members
		public string ID {
			get { return _ID; }
			set { _ID = value; }
		}

		public string Manager {
			get { return _manager; }
			set { _manager = value; }
		}

		public object this[string index] {
			get {
				// BRIAN EDIT
				//return _hash[index.ToUpper().Trim()];
				return null;
			}
		}

		public void Add(string key, object value) {
			// BRIAN EDIT - do nothing here
			//if (!string.IsNullOrEmpty(key)) {
			//    string tempKey = key.ToUpper().Trim();
			//    lock (_hash.SyncRoot) {
			//        if (!_hash.Contains(tempKey)) {
			//            _hash.Add(tempKey, value);
			//        }
			//    }
			//}
		}

		public void Add(string key, object value, long expirationMinutes) {
			this.Add(key, value);
		}

		public void Add(string key, object value, long expirationMinutes, bool useAbsoluteExpiration) {
			this.Add(key, value);
		}

		public void Remove(string key) {
			if (!string.IsNullOrEmpty(key)) {
				lock (_hash.SyncRoot) {
					_hash.Remove(key.ToUpper().Trim());
				}
			}
		}

		public bool Contains(string key) {
			return (string.IsNullOrEmpty(key)) ? false : _hash.Contains(key.ToUpper().Trim());
		}

		public void Clear() {
			lock (_hash.SyncRoot) {
				_hash.Clear();
			}
		}

		public int Count {
			get {
				return _hash.Count;
			}
		}

		public object GetData(string key) {
			return _hash[key.ToUpper().Trim()];
		}

		/// <summary>
		/// This will always return false to force the cache to be repopulated.
		/// </summary>
		/// <returns></returns>
		public bool IsPopulated() {
			return false;
		}

		public Collection<string> Keys {
			get {
				Collection<string> keys = new Collection<string>();
				lock (_hash.SyncRoot) {
					foreach (string key in _hash.Keys) {
						keys.Add(key);
					}
				}
				return keys;
			}
		}

		#endregion

		#region IDisposable Members
		public void Dispose() {
			_hash.Clear();
		}
		#endregion

	}
}