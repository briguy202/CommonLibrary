using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Data;
using CommonLibrary.Framework.Caching;
using CommonLibrary.Framework;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Business.Data {
	internal class ContentSQLDAO : IContentDAO {

		#region Declarations
		private static ICache _cache = null;
		#endregion

		#region Constructors
		static ContentSQLDAO() {
			_cache = CacheFactory.GetCache("ContentCache", "Content Cache Manager");
		}
		#endregion

		#region IContentDAO Members
		public Content GetContentByID(int id) {
			string key = string.Format("CONTENT.{0}", id.ToString());
			Content _return = null;
			IResultSet reader = null;
			string cacheValue = (string)_cache[key];

			if (cacheValue == null) {
				TraceManager.Trace(string.Format("Unable to find cache entry for key {0}.", key), FrameworkTraceTypes.Caching);
				DataOperation operation = new DataOperation("dbo.SP_CONTENT_GET");
				IDataAccess access = DataFactory.GetDataAccess();
				operation.Parameters.Add(new DataParameter("@content_id", id, System.Data.DbType.Int32));
				reader = access.ExecuteResultSet(operation);

				if (reader != null) {
					using (reader) {
						if (reader.HasData) {
							cacheValue = reader.GetString("text");
						}
					}
				}

				_cache.Add(key, cacheValue, ConfigurationBase.GetConfigInteger("ContentCacheExpirationInMinutes", 10));
			} else {
				TraceManager.Trace(string.Format("Found cache entry for key {0}.", key), FrameworkTraceTypes.Caching);
			}

			if (cacheValue != null) {
				_return = new Content(id);
				_return.Text = cacheValue;
			}

			return _return;
		}

		public void Edit(Content content) {
			DataOperation operation = new DataOperation("dbo.SP_CONTENT_UPDATE");
			IDataAccess access = DataFactory.GetDataAccess();
			operation.Parameters.Add(new DataParameter("@content_id", content.ID, System.Data.DbType.Int32));
			operation.Parameters.Add(new DataParameter("@value", content.Text, System.Data.DbType.String));
			access.ExecuteNonQuery(operation);
			
			// Reset the cache to show the updated content.
			_cache.Clear();
		}
		#endregion
	}
}