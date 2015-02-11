using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary.Framework.Data;

namespace CommonLibrary.Users.Data {
	public class UserSQLDAO : IUserDAO {
		#region IUserDAO Members
		public void Load(User user, UserContext context, params object[] parameters) {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00100_USER_GET_BY_USERNAME");
			operation.Parameters.Add(new DataParameter("@username", user.UserID, DbType.String));

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				if (result.HasData) {
					user.SystemID = result.GetInteger("user_id");
					user.Password = result.GetString("password");
					user.FirstName = result.GetString("first_name");
					user.LastName = result.GetString("last_name");
					user.EmailAddress = new CommonLibrary.Framework.Objects.EmailAddress(result.GetString("email"));
				}
			}
		}
		#endregion
	}
}