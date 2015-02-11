using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	public class Address : ReadOnlyBase {

		#region Data Members
		private string _line1;
		private string _line2;
		private string _city;
		private State _state;
		private int _zipCode;
		#endregion

		#region Properties
		public int ZipCode {
			get { return _zipCode; }
			set { _zipCode = value; }
		}

		public State State {
			get { return _state; }
			set { _state = value; }
		}

		public string City {
			get { return _city; }
			set { _city = value; }
		}

		public string Line2 {
			get { return _line2; }
			set { _line2 = value; }
		}

		public string Line1 {
			get { return _line1; }
			set { _line1 = value; }
		}
		#endregion

		#region Constructors
		public Address() : base() { }
		#endregion

		#region Methods
		#endregion
					
	}
}