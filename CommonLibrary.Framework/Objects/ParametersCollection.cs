using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CommonLibrary.Framework.Objects {
	public class ParametersCollection : List<Parameter> {
		public new bool Contains(Parameter item) {
			foreach (Parameter param in this) {
				if ((item.Name == param.Name) && (item.Value == param.Value)) {
					return true;
				}
			}

			return false;
		}

		public override string ToString() {
			string[] arr = new string[this.Count];

			for (int i = 0; i < this.Count; i++) {
				arr[i] = this[i].ToString();
			}

			return String.Join("&", arr);
		}

		/// <summary>
		/// Parses either a full URL that contains parameters, or simply the parameter QueryString value portion of the URL.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static ParametersCollection Parse(string value) {
			if (string.IsNullOrEmpty(value)) {
				throw new ArgumentNullException("value");
			}

			ParametersCollection _return = new ParametersCollection();
			string paramString = string.Empty;

			if (value.Contains("?")) {
				string[] parts = value.Split('?');
				if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1])) {
					paramString = parts[1];
				}
			} else if (value.Contains("=")) {
				paramString = value;
			} else {
				return _return;
			}

			string[] chunks = paramString.Split('&');
			foreach (string chunk in chunks) {
				string[] bits = chunk.Split('=');
				_return.Add(new Parameter(bits[0], bits[1]));
			}

			return _return;
		}
	}
}