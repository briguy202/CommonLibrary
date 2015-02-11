using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Users.Exceptions {
	public class RoleNotAuthorizedException : Exception {

		#region Constructors
		public RoleNotAuthorizedException() : base() { }
		public RoleNotAuthorizedException(string message) : base(message) { }
		public RoleNotAuthorizedException(string message, Exception innerException) : base(message, innerException) { }
		#endregion

	}
}