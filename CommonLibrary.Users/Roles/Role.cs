using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Data;
using CommonLibrary.Framework.Objects;
using System.Collections;

namespace CommonLibrary.Users.Roles {
	public class Role {

		#region Data Members
		private static SystemDataDAOBase<Role> _dao;
		private eSystemRole _enum;
		private string _description;
		private string _systemName;
		private string _name;
		private int _id;
		#endregion

		#region Enumerators
		public enum eSystemRole {
			Guest,
			Admin,

			// Other must be last
			Other
		}
		#endregion

		#region Constructors
		public Role(int roleID) {
			this.ID = roleID;
			_enum = Role.GetEnum(roleID);
		}

		static Role() {
			_dao = Data.Factory.GetRolesDAO();
		}
		#endregion

		#region Properties
		public int ID {
			get { return _id; }
			private set { _id = value; }
		}

		public string Name {
			get { return _name; }
			set { _name = value; }
		}
	
		public string SystemName {
			get { return _systemName; }
			set { _systemName = value; }
		}
	
		public eSystemRole Enum {
			get { return _enum; }
		}

		public string Description {
			get { return _description; }
			set { _description = value; }
		}
		#endregion

		#region Methods
		public static Role GetByValue(int value) {
			return _dao.LoadByValue(value.ToString());
		}

		public static Role GetByName(string name) {
			return _dao.LoadByName(name);
		}

		public static Role GetByEnum(eSystemRole enumValue) {
			if (enumValue == eSystemRole.Other) {
				throw new ArgumentException("Cannot use 'Other' enum value because it is ambiguous.  Please use a defined role type for GetByEnum", "enumValue");
			}

			return Role.GetByValue((int)enumValue);
		}

		public static List<Role> GetAll() {
			//return _dao.LoadAll();
			return null;
		}

		public static eSystemRole GetEnum(int id) {
			// System role IDs go from 0 - 999
			// All other roles go from 1000 and up

			if (id < 0) {
				throw new ArgumentOutOfRangeException("id", "Allowable ID ranges are from 0-999 for system Role IDs, and 1000 and up for all other Role IDs");
			} else if (id <= 999) {
				// These are system IDs
				switch (id) {
					case 0:
						return eSystemRole.Guest;
					case 1:
						return eSystemRole.Admin;
					default:
						return eSystemRole.Other;
				}
			} else {
				return eSystemRole.Other;
			}
		}
		#endregion

	}
}