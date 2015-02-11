using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Users.Data {
	public class RoleNullDAO : IRoleDAO {
		#region IRoleDAO Members

		public void Load(User user, UserContext context, params object[] parameters) {
			// Do nothing
		}

		#endregion
	}
}