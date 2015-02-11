using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace CommonLibrary.Framework.Data {
	public class DataOperation {

		#region Data Members
		private string _storedProcedure;
		private List<DataParameter> _parameters;
		#endregion

		#region Properties
		public List<DataParameter> Parameters {
			get { return _parameters; }
			set { _parameters = value; }
		}

		public string StoredProcedure {
			get { return _storedProcedure; }
			set { _storedProcedure = value; }
		}
		#endregion

		#region Constructors
		public DataOperation() {
			_parameters = new List<DataParameter>();
		}

		public DataOperation(string storedProcedure)
			: this() {
			if (!string.IsNullOrEmpty(storedProcedure)) {
				_storedProcedure = storedProcedure;
			} else {
				throw new ArgumentNullException("storedProcedure");
			}
		}
		#endregion
	}
}