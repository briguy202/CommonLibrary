using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Caching;
using CommonLibrary.Framework.Data;

namespace CommonLibrary.Framework.Objects.Data {
	internal class StateSQLDAO : SystemDataDAOBase<State> {

		#region Constructors
		static StateSQLDAO() {
			StateSQLDAO.Cache = CacheFactory.GetCache("StateCache");
		}
		#endregion

		protected override void Populate() {
			DataOperation operation = new DataOperation("dbo.SP_STATES_GET_ALL");
			IDataAccess access = DataFactory.GetDataAccess();
			IResultSet reader = access.ExecuteResultSet(operation);

			using (reader) {
				while (reader.HasData) {
					string name = reader.GetString("name");
					string value = reader.GetString("id");
					State state = new State(name, value);
					this.AddToCache(name, value, state);

					reader.MoveNext();
				}
			}
		}
	}
}