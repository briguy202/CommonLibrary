using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CommonLibrary.Framework.Data {
	public class DataResultSet : IResultSet {
		#region Data Members
		IDataReader _reader;
		bool _hasData;
		#endregion

		#region Constructors
		public DataResultSet(IDataReader reader) {
			_reader = reader;
			_hasData = false;
			MoveNext();
		}
		#endregion

		#region IResultSet Properties
		public bool HasData {
			get { return _hasData; }
		}
		#endregion

		#region IResultSet Methods
		public bool MoveNext() {
			if (_reader != null) {
				_hasData = _reader.Read();

				if (!_hasData) {
					while (_reader.NextResult()) ;
				}
			}

			return _hasData;
		}

		public void Close() {
			if (_reader != null && !_reader.IsClosed) {
				try {
					if (_hasData) {
						while (_reader.NextResult()) { }
					}
				} catch {
					// Ignore
				} finally {
					try {
						_reader.Close();
					} catch { }
				}
			}
		}

		public List<string> GetPropertyNames() {
			DataTable dt = _reader.GetSchemaTable();
			List<string> _return = new List<string>();

			if (dt != null) {
				foreach (DataRow row in dt.Rows) {
					_return.Add(row["ColumnName"].ToString());
				}
			}

			return _return;
		}

		public object GetValue(string propertyName) {
			return this.GetValue(propertyName, null);
		}

		public object GetValue(string propertyName, object nullValue) {
			if (_reader != null) {
				object obj = null;
				try {
					obj = _reader[propertyName];
				} catch (IndexOutOfRangeException ex) {
					throw new IndexOutOfRangeException(string.Format("Could not find property '{0}' in stored procedure", propertyName), ex);
				}

				if (obj == DBNull.Value) {
					return nullValue;
				} else {
					return obj;
				}
			} else {
				return null;
			}
		}

		public object GetValue(int index) {
			return this.GetValue(index, null);
		}

		public object GetValue(int index, object nullValue) {
			if (_reader != null) {
				object obj = _reader[index];
				if (obj == DBNull.Value) {
					return nullValue;
				} else {
					return obj;
				}
			} else {
				return null;
			}
		}

		public int GetInteger(string propertyName) {
			return this.GetInteger(propertyName, Int32.MinValue);
		}

		public int GetInteger(string propertyName, int nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return Int32.Parse(val.ToString());
			}
		}

		public long GetLong(string propertyName) {
			return this.GetLong(propertyName, long.MinValue);
		}

		public long GetLong(string propertyName, long nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return long.Parse(val.ToString());
			}
		}

		public short GetShort(string propertyName) {
			return this.GetShort(propertyName, short.MinValue);
		}

		public short GetShort(string propertyName, short nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return short.Parse(val.ToString());
			}
		}

		public decimal GetDecimal(string propertyName) {
			return this.GetDecimal(propertyName, decimal.MinValue);
		}

		public decimal GetDecimal(string propertyName, decimal nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return decimal.Parse(val.ToString());
			}
		}

		public double GetDouble(string propertyName) {
			return this.GetDouble(propertyName, double.MinValue);
		}

		public double GetDouble(string propertyName, double nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return double.Parse(val.ToString());
			}
		}

		public Guid GetGuid(string propertyName) {
			return this.GetGuid(propertyName, Guid.Empty);
		}

		public Guid GetGuid(string propertyName, Guid nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return new Guid(val.ToString());
			}
		}

		public string GetString(string propertyName) {
			return this.GetString(propertyName, string.Empty);
		}

		public string GetString(string propertyName, string nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return val.ToString();
			}
		}

		public DateTime GetDateTime(string propertyName) {
			return this.GetDateTime(propertyName, DateTime.MinValue);
		}

		public DateTime GetDateTime(string propertyName, DateTime nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return DateTime.Parse(val.ToString());
			}
		}

		public bool GetBoolean(string propertyName) {
			return this.GetBoolean(propertyName, false);
		}

		public bool GetBoolean(string propertyName, bool nullValue) {
			object val = GetValue(propertyName);
			if (val == null) {
				return nullValue;
			} else {
				return bool.Parse(val.ToString());
			}
		}

		public object GetObject(string propertyName) {
			return GetValue(propertyName);
		}

		public object GetObject(string propertyName, object nullValue) {
			return GetValue(propertyName, nullValue);
		}

		public System.Data.DataTable GetSchema() {
			return _reader.GetSchemaTable();
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			this.Close();
		}

		#endregion
	}
}
