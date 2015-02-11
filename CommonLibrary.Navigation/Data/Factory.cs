using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework;

namespace CommonLibrary.Navigation.Data {
	public class Factory {
		public static INavigationDAO GetNavigationDAO() {
			switch (ConfigurationBase.GetConfigString("NavigationDAO")) {
				case "SQL":
					return new CommonLibrary.Navigation.Data.NavigationSQLDAO();
				case "XML":
					return new CommonLibrary.Navigation.Data.NavigationXMLDAO();
				default:
					return null;
			}
		}
	}
}