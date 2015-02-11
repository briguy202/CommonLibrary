using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Users.Roles {
	public class RoleCollection : List<Role> {
		#region Methods
		public bool HasRole(Role role) {
			if (role == null) { return false; }

			foreach (Role find in this) {
				if (role.ID == find.ID) {
					return true;
				}
			}

			return false;
		}

		public bool HasRole(RoleCollection roles) {
			if (roles == null) { return false; }

			foreach (Role find in roles) {
				foreach (Role aRole in this) {
					if (find.ID == aRole.ID) {
						return true;
					}
				}
			}

			return false;
		}
		#endregion
	}
}