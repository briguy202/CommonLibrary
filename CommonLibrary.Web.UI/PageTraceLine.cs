using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Web.UI {
	public class PageTraceLine {
		private string _message;
		private string _sourceFile;
		private int _sourceLine;
		private ICollection<string> _categories;

		public ICollection<string> Categories {
			get { return _categories; }
			set { _categories = value; }
		}

		public int SourceLine {
			get { return _sourceLine; }
			set { _sourceLine = value; }
		}
	
		public string SourceFile {
			get { return _sourceFile; }
			set { _sourceFile = value; }
		}
	
		public string Message {
			get { return _message; }
		}

		public PageTraceLine(string message) {
			_message = message;
		}

	}
}