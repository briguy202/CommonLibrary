using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Exceptions {
	public class NotInitializedException : Exception {
		public NotInitializedException(string className) : base(string.Format("Class '{0}' was not initialized", className)) { }
	}
}