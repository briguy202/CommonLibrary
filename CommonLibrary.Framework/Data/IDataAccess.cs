using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace CommonLibrary.Framework.Data {
	public interface IDataAccess {
		IResultSet ExecuteResultSet(string query);
		IResultSet ExecuteResultSet(string query, Database database);
		IResultSet ExecuteResultSet(DataOperation operation);
		IResultSet ExecuteResultSet(DataOperation operation, Database database);
		void ExecuteTransactionalNonQuery(List<DataOperation> operations);
		void ExecuteTransactionalNonQuery(List<DataOperation> operations, Database database);
		void ExecuteNonQuery(DataOperation operation);
		void ExecuteNonQuery(DataOperation operation, Database database);
	}
}