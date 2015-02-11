using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Data {
	public static class DataFactory {
		public enum eDataAccessType {
			MSSQL
		}

		public static IDataAccess GetDataAccess() {
			string type = ConfigurationBase.GetConfigString("DefaultDataAccess", "mssql");

			switch (type.ToUpper()) {
				default:
					return DataFactory.GetDataAccess(eDataAccessType.MSSQL);
			}
		}

		public static IDataAccess GetDataAccess(eDataAccessType type) {
			switch (type) {
				case eDataAccessType.MSSQL:
					return new ApplicationBlock50();
				default:
					return new ApplicationBlock50();
			}
		}
	}
}