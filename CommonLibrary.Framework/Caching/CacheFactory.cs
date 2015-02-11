namespace CommonLibrary.Framework.Caching {
	public class CacheFactory {

		#region Methods
		public static ICache GetCache(string ID) {
			return CacheFactory.CreateCache(ID, string.Empty);
		}

		public static ICache GetCache(string ID, string manager) {
			return CacheFactory.CreateCache(ID, manager);
		}

		/// <summary>
		/// Create and return the appropriate cache for a given physical cache.
		/// </summary>
		/// <param name="cacheName"></param>
		/// <param name="cachePrefix"></param>
		/// <returns></returns>
		private static ICache CreateCache(string ID, string manager) {
			ICache cache = null;

			// First check if component caching is on.
			if (!ConfigurationBase.GetConfigBoolean("CachingEnabled")) {
				// return a cache with caching disabled
				cache = new NoCache();
			} else {
				cache = CacheUtilities.GetCache(ID);
				if (cache == null) {
					cache = new CommonLibrary.Framework.Caching.Cache(ID, manager);
					CacheUtilities.Register(cache);
				}
			}

			return cache;
		}
		#endregion

	}
}