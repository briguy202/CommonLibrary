using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework;
using CommonLibrary.Framework.Objects;
using System.Reflection;
using CommonLibrary.Users.Roles;
using CommonLibrary.Framework.Data;

namespace CommonLibrary.Users {
	public class User {

		#region Data Members
		private UserContext _context;
		private string _userID;
		private string _firstName;
		private string _lastName;
		private EmailAddress _email;
		private RoleCollection _roleCollection;
		private int _systemID;
		private string _password;
		private ContextLazyLoader<User> _loader;
		private Data.IUserDAO _userDAO;
		private Data.IRoleDAO _roleDAO;
		#endregion

		#region Properties
		public UserContext Context {
			get { return _context; }
			private set { _context = value; }
		}

		public string UserID {
			get { return _userID; }
		}

		public string Password {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _password;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_password = value;
			}
		}

		public int SystemID {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _systemID;
			}
			internal set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_systemID = value;
			}
		}

		public EmailAddress EmailAddress {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _email;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_email = value;
			}
		}

		public string LastName {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _lastName;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_lastName = value;
			}
		}

		public string FirstName {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _firstName;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_firstName = value;
			}
		}

		public RoleCollection Roles {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context, new ContextLoadDelegate<User>(_roleDAO.Load));
				return _roleCollection;
			}
			internal set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_roleCollection = value;
			}
		}
		#endregion

		#region Constructors
		public User(string userID, UserContext context) {
			if (context == null) { throw new ArgumentNullException("context"); }
			if (string.IsNullOrEmpty(userID)) { throw new ArgumentNullException("userID"); }

			this.Context = context;
			_userID = userID;

			_userDAO = Data.Factory.GetUserDAO();
			_roleDAO = Data.Factory.GetRoleDAO();

			_loader = new ContextLazyLoader<User>(new ContextLoadDelegate<User>(_userDAO.Load));
			_roleCollection = new RoleCollection();
			
			Role role = new Role(0);
			role.SystemName = "GUEST";
			this.Roles.Add(role);
		}
		#endregion

		#region Methods
		public bool HasRole(Role role) {
			if (role == null) { return false; }

			foreach (Role find in this.Roles) {
				if (role.ID == find.ID) {
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}