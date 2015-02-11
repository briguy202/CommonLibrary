using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Framework.Validation {
	public static class ValidationUtility {
		public static void ThrowIfNullOrEmpty<T>(T obj, string name) {
			if (obj == null) {
				throw new ArgumentNullException(name, "Parameter is null");
			} else if (obj is IEnumerable<T> && System.Linq.Enumerable.Count((IEnumerable<T>)obj) == 0) {
				throw new ArgumentNullException(name, "Parameter collection value is empty");
			} else if (obj is string && string.IsNullOrWhiteSpace(obj.ToString())) {
				throw new ArgumentNullException(name, "Parameter value is null, empty, or whitespace");
			}
		}

		public static void ThrowIfLessThan(int value, int upperValueExclusive, string name) {
			if (value < upperValueExclusive) {
				throw new ArgumentOutOfRangeException(name, value, string.Format("Parameter is less than the allowed value '{0}'", upperValueExclusive));
			}
		}
	}
}