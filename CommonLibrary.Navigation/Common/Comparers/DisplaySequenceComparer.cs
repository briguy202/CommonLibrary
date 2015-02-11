using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Navigation.Common.Comparers {
	public class DisplaySequenceComparer : IComparer<Node> {

		#region Data Members
		private eSortDirection _sortDirection;
		#endregion

		#region Enumerations
		public enum eSortDirection {
			Ascending,
			Descending
		}
		#endregion

		#region Properties
		public eSortDirection SortDirection {
			get { return _sortDirection; }
			set { _sortDirection = value; }
		}
		#endregion

		#region IComparer<Node> Members
		public int Compare(Node x, Node y) {
			switch (this.SortDirection) {
				case eSortDirection.Descending:
					return y.DisplaySequence.CompareTo(x.DisplaySequence);
				default:
					return x.DisplaySequence.CompareTo(y.DisplaySequence);
			}
		}
		#endregion

		#region Constructors
		public DisplaySequenceComparer() : this(eSortDirection.Ascending) { }

		public DisplaySequenceComparer(eSortDirection sortDirection) {
			this.SortDirection = sortDirection;
		}
		#endregion

	}
}