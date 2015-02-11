using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Business.Data {
	internal static class DAOFactory {
		internal static IContentDAO GetContentDAO() {
			return new ContentSQLDAO();
		}
	}
}