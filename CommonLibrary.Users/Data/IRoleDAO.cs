using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Users.Roles;

namespace CommonLibrary.Users.Data {
	public interface IRoleDAO {
		void Load(User user, UserContext context, params object[] parameters);
	}
}