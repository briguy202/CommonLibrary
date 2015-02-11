using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Data;

namespace CommonLibrary.Framework.Objects.Data {
	internal static class DAOFactory {
		internal static SystemDataDAOBase<State> GetStateDAO() {
			return new StateSQLDAO();
		}
	}
}