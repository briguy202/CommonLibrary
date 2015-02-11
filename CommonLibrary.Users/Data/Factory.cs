using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Users.Data;
using CommonLibrary.Framework.Data;
using CommonLibrary.Framework;

namespace CommonLibrary.Users.Data {
	public static class Factory {
		public static IRoleDAO GetRoleDAO() {
			if (ConfigurationBase.GetConfigBoolean("UseSiteUsers", true)) {
				return new RoleSQLDAO();
			} else {
				return new RoleNullDAO();
			}
		}

		public static SystemDataDAOBase<Roles.Role> GetRolesDAO() {
			if (ConfigurationBase.GetConfigBoolean("UseSiteUsers", true)) {
				return new RolesSQLDAO();
			} else {
				return new RolesNullDAO();
			}
		}

		public static IUserDAO GetUserDAO() {
			if (ConfigurationBase.GetConfigBoolean("UseSiteUsers", true)) {
				return new UserSQLDAO();
			} else {
				return new UserNullDAO();
			}
		}
	}					
}