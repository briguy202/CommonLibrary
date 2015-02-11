using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Interfaces;
using CommonLibrary.Framework.Objects.Data;
using CommonLibrary.Framework.Data;

namespace CommonLibrary.Framework.Objects {
	public class State : ReadOnlyBase, INameValue<State> {

		#region Data Members
		private string _value;
		private string _name;
		private static SystemDataDAOBase<State> _dao;
		#endregion

		#region Properties
		public string Name {
			get { return _name; }
		}

		public string Value {
			get { return _value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor for web service calls
		/// </summary>
		internal State() { }

		internal State(string name, string value) : this() {
			_name = name;
			_value = value;
		}

		static State() {
			_dao = DAOFactory.GetStateDAO();
		}
		#endregion

		#region Methods
		public static State GetByName(string name) {
			return _dao.LoadByName(name);
		}

		public static State GetByValue(string value) {
			return _dao.LoadByValue(value);
		}
		#endregion
					
	}
}