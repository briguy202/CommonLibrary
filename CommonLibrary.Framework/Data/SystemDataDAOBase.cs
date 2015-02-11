using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Framework.Data {
	public abstract class SystemDataDAOBase<T> {

		#region Data Members
		private static Caching.ICache _cache = null;
		#endregion

		#region Constructors
		#endregion

		#region Properties
		protected static Caching.ICache Cache {
			get { return _cache; }
			set { _cache = value; }
		}
		#endregion

		#region Methods
		public T LoadByName(string name) {
			string key = this.GetKey(name);
			T cacheItem = (T)_cache[key];

			if (cacheItem == null) {
				TraceManager.Trace(string.Format("Unable to find cache entry for key {0}.", key), FrameworkTraceTypes.Caching);
				this.Populate();
			} else {
				TraceManager.Trace(string.Format("Found cache entry for key {0}.", key), FrameworkTraceTypes.Caching);
			}

			return (T)_cache[key];
		}

		public T LoadByValue(string value) {
			return LoadByName(value);	// this is just a wrapper ... items are cached by name and value, so there is no difference.
		}

		protected virtual void AddToCache(string name, string value, object obj) {
			this.AddToCache(name, value, obj, ConfigurationBase.GetConfigInteger("SystemDAODefaultExpirationInMinutes", 5));
		}

		protected virtual void AddToCache(string name, string value, object obj, long expiration) {
			SystemDataDAOBase<T>.Cache.Add(this.GetKey(name), obj, expiration);
			SystemDataDAOBase<T>.Cache.Add(this.GetKey(value), obj, expiration);
		}

		protected string GetKey(string keyName) {
			return string.Format("{0}.{1}", typeof(T).Name, keyName.Trim().ToUpper());
		}
		#endregion

		#region Abstract Methods
		protected abstract void Populate();
		#endregion

	}
}