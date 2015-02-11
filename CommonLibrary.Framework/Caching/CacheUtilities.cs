using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Text;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;

namespace CommonLibrary.Framework.Caching {
	public class CacheUtilities {
		private static Hashtable _caches;

		#region Constructors
		static CacheUtilities() {
			_caches = new Hashtable();
		}
		#endregion

		#region Methods
		/// <summary>
		/// This returns a handle to every cache currently running in the system.
		/// </summary>
		/// <returns></returns>
		public static List<ICache> GetAllCaches() {
			List<ICache> coll = new List<ICache>();
			lock (_caches.SyncRoot) {
				foreach (ICache cache in _caches.Values) {
					coll.Add(cache);
				}
			}
			return coll;
		}

		/// <summary>
		/// This returns a handle to every cache currently running in the system in the given physical cache.
		/// </summary>
		/// <returns></returns>
		public static ICache GetCache(string cacheID) {
			lock (_caches.SyncRoot) {
				foreach (ICache cache in _caches.Values) {
					if (cache.ID == cacheID) {
						return cache;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// This registers a new cache with the system so that it can be tracked and monitored.
		/// </summary>
		/// <param name="cacheName"></param>
		/// <param name="cache"></param>
		internal static void Register(ICache cache) {
			lock (_caches.SyncRoot) {
				if (!_caches.Contains(cache.ID)) { _caches.Add(cache.ID, cache); }
			}
		}

		/// <summary>
		/// This removes the given cache from the system.
		/// </summary>
		/// <param name="cacheID"></param>
		internal static void Remove(string cacheID) {
			lock (_caches) {
				if (_caches.ContainsKey(cacheID)) {
					_caches.Remove(cacheID);
				}
			}
		}

		/// <summary>
		/// This will clear out the specified cache.
		/// </summary>
		/// <param name="cacheID"></param>
		/// <returns>The number of items cleared from the cache.</returns>
		public static int ClearCache(string cacheID) {
			int count = 0;
			lock (_caches.SyncRoot) {
				if (_caches.ContainsKey(cacheID)) {
					ICache cache = (ICache)_caches[cacheID];
					lock (cache) {
						count = cache.Count;
						cache.Clear();
					}
				}
			}

			return count;
		}

		/// <summary>
		/// This will clear all caches currently registered in the system.
		/// </summary>
		/// <returns>The number of items cleared from the cache.</returns>
		public static int ClearCaches() {
			int count = 0;
			lock (_caches) {
				foreach (ICache cache in _caches.Values) {
					lock (cache) {
						count += cache.Count;
						cache.Clear();
					}
				}
			}

			return count;
		}

		#endregion

	}
}
