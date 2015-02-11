using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CommonLibrary.Framework.Data {
	public class SQLParameter {
		public DbType Type { get; private set; }
		public string Name { get; private set; }
		public object Value { get; private set; }

		public SQLParameter(string name, object value, DbType type) {
			this.Name = name;
			this.Value = value;
			this.Type = type;
		}
	}
}