using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	public class FileDetail {

		#region Enumerations
		public enum eSlashType {
			Forward,
			Backward
		}
		#endregion

		#region Data Members
		private string _fileName;
		private string _filePath;
		private eSlashType _slashType;
		#endregion

		#region Properties
		public string SlashValue {
			get { return (this.SlashType == eSlashType.Forward) ? "/" : "\\"; }
		}

		public eSlashType SlashType {
			get { return _slashType; }
			private set { _slashType = value; }
		}

		public string FileName {
			get { return _fileName; }
			set { _fileName = value; }
		}

		public string FilePath {
			get { return _filePath; }
			set { _filePath = this.ConvertSlashes(value); }
		}

		public string FullFilePath {
			get {
				if (!string.IsNullOrEmpty(this.FileName) || !string.IsNullOrEmpty(this.FilePath)) {
					return string.Concat(this.FilePath, this.SlashValue, this.FileName);
				} else {
					return string.Empty;
				}
			}
			set {
				if (!string.IsNullOrEmpty(value)) {
					string tmp = this.ConvertSlashes(value);
					int lastIndex = tmp.LastIndexOf(this.SlashValue);

					if (lastIndex > 0) {
						_fileName = tmp.Substring(lastIndex + 1);
						_filePath = tmp.Substring(0, lastIndex);
					} else {
						_fileName = tmp;
						_filePath = string.Empty;
					}
				}
			}
		}
		#endregion

		#region Constructors
		public FileDetail() : this(eSlashType.Forward) { }

		public FileDetail(eSlashType slashType) {
			this.SlashType = slashType;
		}

		public FileDetail(string fullFilePath) : this(fullFilePath, eSlashType.Forward) { }

		public FileDetail(string fullFilePath, eSlashType slashType) {
			this.SlashType = slashType;
			this.FullFilePath = this.ConvertSlashes(fullFilePath);
		}
		#endregion

		#region Methods
		private string ConvertSlashes(string text) {
			switch (this.SlashType) {
				case eSlashType.Backward:
					text = text.Replace('/', '\\');
					break;
				default:
					text = text.Replace('\\', '/');
					break;
			}

			return text;
		}

		public static string GetSlashValue(eSlashType slashType) {
			return (slashType == eSlashType.Forward) ? "/" : "\\";
		}

		public static string ConcatenatePaths(string path1, string path2, eSlashType slashType) {
			if (string.IsNullOrEmpty(path1) && string.IsNullOrEmpty(path2)) {
				throw new ArgumentNullException("path1 and path2", "Both paths cannot be null or empty");
			}

			if (string.IsNullOrEmpty(path1)) { return path2; }
			if (string.IsNullOrEmpty(path2)) { return path1; }

			string slashValue = FileDetail.GetSlashValue(slashType);
			path1 = path1.Trim();
			path2 = path2.Trim();
			string joiner = string.Empty;
			if (!(path1.EndsWith("/") || path1.EndsWith("\\")) && !(path2.StartsWith("/") || path2.StartsWith("\\"))) {
				joiner = slashValue;
			}
			string _return = string.Concat(path1, joiner, path2);
			return _return;
		}
		#endregion

	}
}