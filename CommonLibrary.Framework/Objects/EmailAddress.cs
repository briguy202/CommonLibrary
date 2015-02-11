using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonLibrary.Framework.Objects {
	public class EmailAddress {

		#region Data Members
		private string _prefix;
		private string _domain;
		private string _suffix;
		#endregion

		#region Properties
		public string Suffix {
			get { return _suffix; }
			set { _suffix = value; }
		}
	
		public string Domain {
			get { return _domain; }
			set { _domain = value; }
		}
	
		public string Prefix {
			get { return _prefix; }
			set { _prefix = value; }
		}

		public string SuffixAndDomain {
			get { return this.Suffix + '.' + this.Domain; }
		}
		#endregion

		#region Constructors
		public EmailAddress() { }

		public EmailAddress(string address) {
			if (!EmailAddress.IsValidAddress(address)) {
				throw new InvalidEmailAddressException(address);
			}
			
			Regex regex = new Regex("(^.*)@(.*)\\.(.*)$");
			MatchCollection matches = regex.Matches(address);
			Match match = matches[0];

			this.Prefix = match.Groups[1].Value;
			this.Suffix = match.Groups[2].Value;
			this.Domain = match.Groups[3].Value;
		}
		#endregion

		#region Methods
		public static bool IsValidAddress(string address) {
			if (string.IsNullOrEmpty(address)) {
				return false;
			}

			Regex regex = new Regex("(^.*)@(.*)\\.(.*)$");
			return regex.IsMatch(address);
		}

		public override string ToString() {
			return string.Format("{0}@{1}", this.Prefix, this.SuffixAndDomain);
		}
		#endregion

		#region Classes
		public class InvalidEmailAddressException : Exception {
			private string _address;

			public string Address {
				get { return _address; }
			}

			public InvalidEmailAddressException(string address) : base() {
				_address = address;
			}

			public InvalidEmailAddressException(string address, string message)
				: base(message) {
				_address = address;
			}
		}
		#endregion

	}
}