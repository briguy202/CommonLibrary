using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Users.Roles;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Users {
	public class UserContext {

		#region Data Members
		private string _userID;
		private RoleCollection _roleCollection = new RoleCollection();
		#endregion

		#region Properties
		public RoleCollection Roles {
			get { return _roleCollection; }
			set { _roleCollection = value; }
		}

		public string UserID {
			get { return _userID; }
			private set { _userID = value; }
		}
		#endregion

		#region Constructors
		public UserContext(string userID) {
			this.UserID = userID;
		}
		#endregion

	}
}