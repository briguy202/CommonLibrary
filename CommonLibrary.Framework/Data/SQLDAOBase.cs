using System.Collections.ObjectModel;
using CommonLibrary.Framework.Caching;
using System.Data;
using System.Collections.Generic;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Framework.Data {
	public abstract class SQLDAOBase<BusObjType> {

		#region Declarations
		protected delegate void HandleOperationDelegate(DataOperation operation);
		protected delegate Collection<BusObjType> HandleResultsDelegate(IResultSet reader);
		#endregion

		#region Constructors
		protected SQLDAOBase(string key, string primaryKeyName, DbType primaryKeyType, ICache cache, bool isCachingEnabled) {
			this.Key = key;
			this.PrimaryKeyName = primaryKeyName;
			this.PrimaryKeyType = primaryKeyType;
			this.Cache = cache;
			this.IsCachingEnabled = isCachingEnabled;
		}
		#endregion

		#region Properties
		private string _key;
		private ICache _cache;
		private bool _isCachingEnabled;
		private string _primaryKeyName;
		private DbType _primaryKeyType;

		public string PrimaryKeyName {
			get { return _primaryKeyName; }
			private set { _primaryKeyName = value; }
		}

		public DbType PrimaryKeyType {
			get { return _primaryKeyType; }
			private set { _primaryKeyType = value; }
		}
	
		public bool IsCachingEnabled {
			get { return _isCachingEnabled; }
			private set { _isCachingEnabled = value; }
		}
	
		public ICache Cache {
			get { return _cache; }
			private set { _cache = value; }
		}

		public string Key {
			get { return _key; }
			private set { _key = value; }
		}
		#endregion

		#region Methods
		public abstract BusObjType MapResultToBusinessObject(IResultSet reader);

		public abstract DataOperation MapBusinessObjectToDataOperation(BusObjType businessObject);

		private Collection<BusObjType> Get(string cacheKey, string storedProcedureName, Collection<SQLParameter> parameters) {
			Collection<BusObjType> _return = null;

			if (this.IsCachingEnabled) {
				object cachedValue = this.Cache[cacheKey];
				if (cachedValue != null) {
					if (cachedValue is Collection<BusObjType>) {
						_return = (Collection<BusObjType>)cachedValue;
					} else {
						_return = new Collection<BusObjType>() { (BusObjType)cachedValue };
					}
				}
			}

			if (_return == null) {
				TraceManager.Trace(string.Format("Unable to find cache entry for key {0}.", cacheKey), FrameworkTraceTypes.Caching);
				DataOperation operation = new DataOperation(storedProcedureName);
				IDataAccess access = DataFactory.GetDataAccess();
				if (parameters != null) {
					foreach (SQLParameter parameter in parameters) {
						operation.Parameters.Add(new DataParameter(string.Format("@{0}", parameter.Name), parameter.Value, parameter.Type));
					}
				}
				IResultSet reader = access.ExecuteResultSet(operation);

				using (reader) {
					if (reader.HasData) {
						_return = new Collection<BusObjType>();

						while (reader.HasData) {
							_return.Add(this.MapResultToBusinessObject(reader));

							// Advance the reader.
							reader.MoveNext();
						}

						if (this.IsCachingEnabled) {
							this.Cache.Add(cacheKey, _return, ConfigurationBase.GetConfigInteger(string.Format("{0}CacheExpirationInMinutes", this.Key), 10));
						}
					}
				}
			} else {
				TraceManager.Trace(string.Format("Found cache entry for key {0}.", cacheKey), FrameworkTraceTypes.Caching);
			}

			return _return;
		}

		public BusObjType Get(object value) {
			SQLParameter parameter = new SQLParameter(this.PrimaryKeyName, value, this.PrimaryKeyType);
			string cacheKey = string.Format("{0}.{1}", this.Key.ToUpper(), parameter.Value);
			string storedProcedureName = string.Format("dbo.SP_{0}_GET", this.Key);
			Collection<BusObjType> values = this.Get(cacheKey, storedProcedureName, new Collection<SQLParameter>() { parameter });
			
			if (values != null && values.Count == 1) {
				return values[0];
			} else {
				return default(BusObjType);
			}
		}

		public Collection<BusObjType> GetAllByValue(SQLParameter parameter) {
			string cacheKey = string.Format("{0}_LIST_BY_{1}.{2}", this.Key.ToUpper(), parameter.Name.ToUpper(), parameter.Value);
			string storedProcedureName = string.Format("dbo.SP_{0}_LIST_GET_BY_{1}", this.Key, parameter.Name.ToUpper());
			Collection<BusObjType> _return = this.Get(cacheKey, storedProcedureName, new Collection<SQLParameter>() { parameter });
			if (_return != null && _return.Count == 0) { _return = null; }
			return _return;
		}

		public Collection<BusObjType> GetAll() {
			string cacheKey = string.Format("{0}_LIST", this.Key.ToUpper());
			string storedProcedureName = string.Format("dbo.SP_{0}_LIST_GET", this.Key);
			Collection<BusObjType> _return = this.Get(cacheKey, storedProcedureName, null);
			if (_return != null && _return.Count == 0) { _return = null; }
			return _return;
		}

		public void Create(BusObjType businessObject) {
			this.RunProcNoResult(businessObject, string.Format("dbo.SP_{0}_INSERT", this.Key), true);
		}

		public void Edit(BusObjType businessObject) {
			this.RunProcNoResult(businessObject, string.Format("dbo.SP_{0}_UPDATE", this.Key), true);
		}

		public void Delete(BusObjType businessObject) {
			this.RunProcNoResult(businessObject, string.Format("dbo.SP_{0}_DELETE", this.Key), true);
		}

		protected Collection<BusObjType> RunProcWithResult(string storedProc, string cacheId) {
			return this.RunProcWithResult(storedProc, cacheId, null);
		}

		protected Collection<BusObjType> RunProcWithResult(string storedProc, string cacheId, HandleResultsDelegate resultsCallback) {
			return this.RunProcWithResult(storedProc, cacheId, resultsCallback, null);
		}

		protected Collection<BusObjType> RunProcWithResult(string storedProc, string cacheId, HandleResultsDelegate resultsCallback, HandleOperationDelegate operationCallback) {
			Collection<BusObjType> _return = null;
			string cacheKey = string.Format("{0}.{1}", this.Key.ToUpper(), cacheId);

			if (this.IsCachingEnabled) {
				_return = (Collection<BusObjType>)this.Cache[cacheKey];
			}

			if (_return == null) {
				TraceManager.Trace(string.Format("Unable to find cache entry for key {0}.", cacheKey), FrameworkTraceTypes.Caching);
				DataOperation operation = new DataOperation(storedProc);
				if (operationCallback != null) {
					operationCallback(operation);
				}
				IDataAccess access = DataFactory.GetDataAccess();
				IResultSet reader = access.ExecuteResultSet(operation);

				using (reader) {
					if (reader.HasData) {
						if (resultsCallback != null) {
							_return = resultsCallback(reader);
						} else {
							_return = new Collection<BusObjType>();
							while (reader.HasData) {
								_return.Add(this.MapResultToBusinessObject(reader));

								// Advance the reader.
								reader.MoveNext();
							}
						}

						if (this.IsCachingEnabled) {
							this.Cache.Add(cacheKey, _return, ConfigurationBase.GetConfigInteger(string.Format("{0}CacheExpirationInMinutes", this.Key), 10));
						}
					}
				}
			} else {
				TraceManager.Trace(string.Format("Found cache entry for key {0}.", cacheKey), FrameworkTraceTypes.Caching);
			}

			return _return;
		}

		protected void RunProcNoResult(string storedProc, bool clearCache) {
			this.RunProcNoResult(null, storedProc, clearCache);
		}

		protected void RunProcNoResult(object businessObject, string storedProc, bool clearCache) {
			DataOperation operation = new DataOperation();
			if (businessObject != null) {
				operation = this.MapBusinessObjectToDataOperation((BusObjType)businessObject);
			}
			operation.StoredProcedure = storedProc;
			IDataAccess access = DataFactory.GetDataAccess();
			access.ExecuteNonQuery(operation);

			if (clearCache && this.IsCachingEnabled) {
				// Clear the cache
				CacheUtilities.ClearCache(_cache.ID);
			}
		}
		#endregion

	}
}