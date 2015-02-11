using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework;

namespace CommonLibrary.Users {
	public delegate void ContextLoadDelegate<T>(T loadedObject, UserContext context, params object[] parameters);

	public class ContextLazyLoader<T> {

		#region Data Members
		private bool _isLoaded;
		private Hashtable _loadHash;
		private ContextLoadDelegate<T> _loader;
		#endregion

		#region Constructors
		public ContextLazyLoader(ContextLoadDelegate<T> loader) {
			_isLoaded = false;
			_loader = loader;
			_loadHash = new Hashtable();
		}
		#endregion

		#region Methods
		public bool IsLoaded() {
			return _isLoaded;
		}

		public bool IsLoaded(string key) {
			key = ContextLazyLoader<T>.ParseKey(key);
			return _loadHash.Contains(key);
		}

		public void SetLoaded(string key) {
			key = ContextLazyLoader<T>.ParseKey(key);
			if (!string.IsNullOrEmpty(key) && !this.IsLoaded(key)) {
				_loadHash.Add(key, true);
			}
		}

		public void SetUnLoaded(string key) {
			key = ContextLazyLoader<T>.ParseKey(key);
			if (!string.IsNullOrEmpty(key) && this.IsLoaded(key)) {
				_loadHash.Remove(key);
			}
		}

		public void Load(string key, T objectToLoad, UserContext context, ContextLoadDelegate<T> loader, params object[] parameters) {
			if (!this.IsLoaded(key)) {
				this.SetLoaded(key);
				loader(objectToLoad, context, parameters);
			}
		}

		public void Load(string key, T objectToLoad, UserContext context, params object[] parameters) {
			this.Load(key, objectToLoad, context, _loader, parameters);
		}		
		
		public void Load(T objectToLoad, UserContext context, params object[] parameters) {
			this.Load(objectToLoad, context, _loader, parameters);
		}

		public void Load(T objectToLoad, UserContext context, ContextLoadDelegate<T> loader, params object[] parameters) {
			if (!this.IsLoaded()) {
				_isLoaded = true;
				loader(objectToLoad, context, parameters);
			}
		}

		protected static string ParseKey(string key) {
			if (string.IsNullOrEmpty(key)) { return string.Empty; }
			key = key.Trim();
			if (key.StartsWith("get_") || key.StartsWith("set_")) {
				key = key.Substring(4);
			}
			return key;
		}
		#endregion

	}
}