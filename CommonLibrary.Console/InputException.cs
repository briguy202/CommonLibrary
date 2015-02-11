using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.Console {
	public class InputException : Exception {

		#region Declarations
		public enum ErrorCode {
			InvalidArgument,
			InvalidValue,
			ValueIsNotSet,
			ArgumentIsNotSet,
			DuplicateKey,
			RequiredIfSetInvalid,
			RequiredIfSetNotSet,
			FileDoesNotExist
		}
		#endregion

		#region Properties
		public Collection<string> Arguments { get; set; }
		public ErrorCode Code { get; set; }
		#endregion

		#region Constructors
		public InputException(ErrorCode code, string argument) : this(code, new Collection<string>() { argument }) { }

		public InputException(ErrorCode code, Collection<string> arguments) : base() {
			this.Code = code;
			this.Arguments = arguments;
		}
		#endregion

		#region Methods
		#endregion

	}
}