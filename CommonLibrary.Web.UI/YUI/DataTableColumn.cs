using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Web.UI.YUI {
	public class DataTableColumn {

		#region Data Members
		public enum DataType {
			String,
			Number,
			Date
		}
		#endregion

		#region Properties
		private string _key;
		private string _label;
		private bool _sortable;
		private DataType _dataTypeValue;

		public DataType DataTypeValue {
			get { return _dataTypeValue; }
			set { _dataTypeValue = value; }
		}

		public bool Sortable {
			get { return _sortable; }
			set { _sortable = value; }
		}

		public string Label {
			get { return _label; }
			protected set { _label = value; }
		}

		public string Key {
			get { return _key; }
			protected set { _key = value; }
		}
		#endregion

		#region Constructors
		public DataTableColumn(string key, string label) : this(key, label, false) { }

		public DataTableColumn(string key, string label, bool sortable) : this(key, label, sortable, DataType.String) { }

		public DataTableColumn(string key, string label, bool sortable, DataType dataType) {
			this.Key = key;
			this.Label = label;
			this.Sortable = sortable;
			this.DataTypeValue = dataType;
		}
		#endregion

		#region Methods
		#endregion
					
	}
}
