namespace CommonLibrary.Framework.Objects {
	public class ImageDetail : FileDetail {
		#region Properties
		private int _height;
		private int _width;
		private string _linkURL;

		public int Height {
			get { return _height; }
			set { _height = value; }
		}

		public int Width {
			get { return _width; }
			set { _width = value; }
		}

		public string LinkURL {
			get { return _linkURL; }
			set { _linkURL = value; }
		}
		#endregion

		#region Constructors
		public ImageDetail() { }

		public ImageDetail(string fullFilePath) : base(fullFilePath) {
			this.FullFilePath = fullFilePath;
		}
		#endregion
	}
}