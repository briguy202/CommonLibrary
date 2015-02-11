using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Users.Roles;
using CommonLibrary.Framework.Data;

namespace CommonLibrary.Users.Data {
	public class RolesSQLDAO : SystemDataDAOBase<Role> {
		static RolesSQLDAO() {
			SystemDataDAOBase<Role>.Cache = CacheFactory.GetRoleCache();
		}
		
		protected override void Populate() {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00120_ROLE_LIST_GET");

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				while (result.HasData) {
					Role role = new Role(result.GetInteger("role_id"));
					role.Name = result.GetString("name");
					role.SystemName = result.GetString("system_name").ToUpper().Trim();
					role.Description = result.GetString("description");
					this.AddToCache(role.SystemName, role.ID.ToString(), role);

					result.MoveNext();
				}
			}
		}
	}
}