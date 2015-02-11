using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	public class NameValue<T> {

		#region Data Members
		private string _name;
		private T _value;
		#endregion

		#region Constructors
		public NameValue(string name, T value) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}

			this.Name = name;
			this.Value = value;
		}
		#endregion

		#region Properties
		public T Value {
			get { return _value; }
			set { _value = value; }
		}

		public string Name {
			get { return _name; }
			set { _name = value; }
		}
		#endregion
	
	}
}