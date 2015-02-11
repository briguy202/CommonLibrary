using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Console {
	public class ConsoleException : Exception {
		public ConsoleException(string format, params object[] args) : base(string.Format(format, args)) { }
	}
}