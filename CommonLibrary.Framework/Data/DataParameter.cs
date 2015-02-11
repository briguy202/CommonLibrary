using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace CommonLibrary.Framework.Data {
	public class DataParameter {

		#region Data Members
		#endregion

		#region Properties
		private string _name;
		private object _value;
		private DbType _type;
		private ParameterDirection _direction;

		public ParameterDirection Direction {
			get { return _direction; }
			set { _direction = value; }
		}
	
		public DbType Type {
			get { return _type; }
			set { _type = value; }
		}
	
		public object Value {
			get { return _value; }
			set { _value = value; }
		}
	
		public string Name {
			get { return _name; }
			set { _name = value; }
		}
	
		#endregion

		#region Constructors
		public DataParameter(string name, object value, DbType type) : this(name, value, type, ParameterDirection.Input) { }

		public DataParameter(string name, object value, DbType type, ParameterDirection direction) {
			_name = name;
			_value = value;
			_type = type;
			_direction = direction;
		}
		#endregion

		#region Methods
		public static SqlParameter ConvertDataParameter(DataParameter parameter) {
			SqlParameter _return = new SqlParameter();
			_return.Direction = parameter.Direction;
			_return.DbType = parameter.Type;
			_return.ParameterName = parameter.Name;
			_return.Value = parameter.Value;

			return _return;
		}

		public static SqlParameter[] ConvertDataParameter(List<DataParameter> parameters) {
			SqlParameter[] _return = new SqlParameter[parameters.Count];

			for (int i = 0; i < parameters.Count; i++) {
				_return[i] = DataParameter.ConvertDataParameter(parameters[i]);
			}

			return _return;
		}
		#endregion
	}				
}