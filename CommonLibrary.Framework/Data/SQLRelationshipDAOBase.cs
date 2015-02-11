using CommonLibrary.Framework.Caching;
using System.Data;

namespace CommonLibrary.Framework.Data {
	public abstract class SQLRelationshipDAOBase<BusObjType> : SQLDAOBase<BusObjType> {

		#region Constructors
		public SQLRelationshipDAOBase(string key, string primaryKeyName, DbType primaryKeyType, ICache cache, bool isCachingEnabled) : base(key, primaryKeyName, primaryKeyType, cache, isCachingEnabled) { }
		#endregion

		#region Methods
		public void DeleteAll() {
			this.RunProcNoResult(string.Format("dbo.SP_{0}_DELETE_ALL", this.Key), true);
		}
		#endregion

	}
}