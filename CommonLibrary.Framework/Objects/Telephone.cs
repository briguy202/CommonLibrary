using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Objects {
	public class Telephone {
		private int _areaCode;
		private int _firstThree;
		private int _lastFour;

		public int LastFour {
			get { return _lastFour; }
			set { _lastFour = value; }
		}

		public int FirstThree {
			get { return _firstThree; }
			set { _firstThree = value; }
		}
	
		public int AreaCode {
			get { return _areaCode; }
			set { _areaCode = value; }
		}

		/// <summary>
		/// Constructor for web service calls
		/// </summary>
		public Telephone() { }

		public Telephone(int areaCode, int firstThree, int lastFour) : this() {
			this.AreaCode = areaCode;
			this.FirstThree = firstThree;
			this.LastFour = lastFour;
		}
	}
}