using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using CommonLibrary.Framework.Caching;
using System.Collections.ObjectModel;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;
using System.Configuration;

namespace CommonLibrary.Framework.Caching {
	/// <summary>
	/// This class uses ICache to abstract the physical implementation of the cache used, in this case the
	/// 2.0 Enterprise Application Blocks for Caching.
	/// </summary>
	internal class Cache : ICache, IDisposable {

		#region Declarations
		protected ICacheManager _cache;	// The internal implementation of the cache.
		protected Collection<string> _keys;		// The list of cache keys added
		private const long _defaultExpirationInMinutes = 5;
		private string _ID;
		private string _manager;
		#endregion

		#region Constructors
		internal Cache(string ID) : this(ID, string.Empty) { }

		internal Cache(string ID, string manager) {
			if (string.IsNullOrEmpty(ID)) { throw new ArgumentNullException(ID); }

			if (string.IsNullOrEmpty(manager)) {
				CacheManagerSettings customSection = ConfigurationManager.GetSection(Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerSettings.SectionName) as CacheManagerSettings;
				manager = customSection.DefaultCacheManager;
			}

			this.ID = ID;
			this.Manager = manager;

			_keys = new Collection<string>();
			if (!string.IsNullOrEmpty(manager)) {
				_cache = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager(manager);
			} else {
				// Use default
				_cache = Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager();
			}
		}

		~Cache() {
			this.Clear();
		}
		#endregion

		#region ICache Members
		public string ID {
			get { return _ID; }
			private set { _ID = value; }
		}

		public string Manager {
			get { return _manager; }
			private set { _manager = value; }
		}

		public object this[string key] {
			get {
				if (key == null) { key = string.Empty; }
				return _cache.GetData(key.ToUpper().Trim());
			}
		}

		public void Add(string key, object value) {
			this.Add(key, value, _defaultExpirationInMinutes);
		}

		public void Add(string key, object value, long expirationMinutes) {
			this.Add(key, value, expirationMinutes, true);
		}

		public void Add(string key, object value, long expirationMinutes, bool useAbsoluteExpiration) {
			if (key == null) { key = string.Empty; }
			string newKey = key.ToUpper().Trim();

			if (useAbsoluteExpiration) {
				_cache.Add(newKey, value, CacheItemPriority.Normal, null, new AbsoluteTime(DateTime.Now.AddMinutes(expirationMinutes)));
			} else {
				_cache.Add(newKey, value, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(expirationMinutes)));
			}

			_keys.Add(newKey);
		}

		public void Remove(string key) {
			if (key == null) { key = string.Empty; }
			key = key.ToUpper().Trim();
			_cache.Remove(key);
		}

		public bool Contains(string key) {
			return (key != null && _cache[key.ToUpper().Trim()] != null);
		}

		public void Clear() {
			if (_cache != null) {
				_cache.Flush();
				_keys.Clear();
			}
		}

		public int Count {
			get { return _cache.Count; }
		}

		public object GetData(string key) {
			if (key == null) { key = string.Empty; }
			return _cache.GetData(key.ToUpper().Trim());
		}

		public bool IsPopulated() {
			return (_cache.Count > 0);
		}

		public Collection<string> Keys {
			get { return _keys; }
		}
		#endregion

		#region IDisposable Members
		public void Dispose() {
			this.Clear();
		}
		#endregion

		#region ICacheItemRefreshAction Members
		public virtual void Refresh(string removedKey, object expiredValue, CacheItemRemovedReason removalReason) {
			if (removedKey == null) { removedKey = string.Empty; }
			_keys.Remove(removedKey);

			//lock (_cacheKeys.SyncRoot) {
			//    if (_cacheKeys != null && _cacheKeys.Contains(removedKey)) {
			//        lock (_cache) {
			//            _cacheKeys.Remove(removedKey);
			//            DateTime now = DateTime.Now;
			//            Logging.Logger.Log(now.ToLongTimeString() + "." + now.Millisecond + " Removing key: '" + removedKey.Remove(0, _prefix.Length) + "' Cache Key Count: " + _cacheKeys.Count + ".");
			//        }
			//    }
			//}
		}
		#endregion

	}
}