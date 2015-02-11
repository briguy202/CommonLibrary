using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Data;
using CommonLibrary.Users.Roles;
using System.Data;

namespace CommonLibrary.Users.Data {
	public class RoleSQLDAO : IRoleDAO {
		#region IRoleDAO Members

		public void Load(User user, UserContext context, params object[] parameters) {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00105_USER_ROLE_LIST_GET");
			operation.Parameters.Add(new DataParameter("@userID", user.SystemID, DbType.Int32));

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				while (result.HasData) {
					Role role = new Role(result.GetInteger("role_id"));
					role.SystemName = result.GetString("system_name");
					user.Roles.Add(role);

					result.MoveNext();
				}
			}
		}

		#endregion
	}
}