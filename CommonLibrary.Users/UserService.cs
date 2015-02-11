using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Users {
	public static class UserService {

		#region Properties
		public static string GuestUserID {
			get { return "GUEST"; }
		}
		#endregion

		#region Methods
		public static User GetUser(string userID, UserContext context) {
			User user = new User(userID, context);
			return (user.SystemID > 0) ? user : null;
		}

		public static User GetGuestUser() {
			UserContext context = UserService.GetGuestUserContext();
			return UserService.GetGuestUser(context);
		}

		public static User GetGuestUser(UserContext context) {
			if (context == null) { throw new ArgumentNullException("context"); }

			User user = new User(UserService.GuestUserID, context);
			user.FirstName = "Guest";
			return user;
		}

		public static UserContext GetGuestUserContext() {
			return new UserContext(UserService.GuestUserID);
		}
		#endregion
		
	}
}