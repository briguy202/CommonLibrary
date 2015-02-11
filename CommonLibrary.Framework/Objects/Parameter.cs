using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	public class Parameter : NameValue<string> {
		public override string ToString() {
			return base.Name + "=" + base.Value;
		}

		public Parameter(string name, string value) : base(name, value) { }
	}
}