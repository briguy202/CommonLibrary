using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Interfaces;

namespace CommonLibrary.Framework.Objects {
	public class Country : ReadOnlyBase, INameValue<Country> {

		#region Data Members
		private string _name;
		private string _value;
		#endregion

		#region Properties
		public string Value {
			get { return _value; }
		}
	
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		#endregion

		#region Constructors
		internal Country(string value) : base() {
			_value = Value;
		}
		#endregion

		#region Methods
		public Country GetByEnum() {
			return new Country("test");
		}

		#region INameValue<Country> Members
		public Country GetByName(string name) {
			throw new Exception("The method or operation is not implemented.");
		}

		public Country GetByValue(string value) {
			throw new Exception("The method or operation is not implemented.");
		}

		public List<Country> GetAll() {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
		#endregion
		
	}
}