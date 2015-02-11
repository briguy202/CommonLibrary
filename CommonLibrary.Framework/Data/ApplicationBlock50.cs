using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using CommonLibrary.Framework.Exceptions;
using Microsoft.Practices.EnterpriseLibrary.Data;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Framework.Data {
	public class ApplicationBlock50 : IDataAccess {

		#region IDataAccess Members

		#region ExecuteResultSet
		public IResultSet ExecuteResultSet(string query) {
			throw new Exception("The method or operation is not implemented.");
		}

		public IResultSet ExecuteResultSet(string query, Database database) {
			throw new Exception("The method or operation is not implemented.");
		}

		public IResultSet ExecuteResultSet(DataOperation operation) {
			return this.ExecuteResultSet(operation, "Default Connection String", 1);
		}

		private IResultSet ExecuteResultSet(DataOperation operation, string connectionConfigurationName, int skipFrames) {
			return this.ExecuteResultSet(operation, DatabaseFactory.CreateDatabase(connectionConfigurationName), skipFrames + 1);
		}

		public IResultSet ExecuteResultSet(DataOperation operation, Database database) {
			return this.ExecuteResultSet(operation, database, null, 1);
		}

		private IResultSet ExecuteResultSet(DataOperation operation, Database database, int skipFrames) {
			return this.ExecuteResultSet(operation, database, null, skipFrames + 1);
		}

		private IResultSet ExecuteResultSet(DataOperation operation, Database database, DbTransaction transaction, int skipFrames) {
			try {
				DbCommand command = this.GetCommand(operation, database, skipFrames + 1);
				IDataReader reader = null;
				if (transaction != null) {
					reader = database.ExecuteReader(command, transaction);
				} else {
					reader = database.ExecuteReader(command);
				}
				return new DataResultSet(reader);
			} catch (Exception ex) {
				throw new Exception(string.Format("Error executing procedure {0}", operation.StoredProcedure), ex);
			}
		}
		#endregion

		#region ExecuteTransactionalNonQuery
		public void ExecuteTransactionalNonQuery(List<DataOperation> operations) {
			this.ExecuteTransactionalNonQuery(operations, "Default Connection String", 1);
		}

		public void ExecuteTransactionalNonQuery(List<DataOperation> operations, string connectionConfigurationName) {
			Database database = DatabaseFactory.CreateDatabase(connectionConfigurationName);
			this.ExecuteTransactionalNonQuery(operations, database, 1);
		}

		private void ExecuteTransactionalNonQuery(List<DataOperation> operations, string connectionConfigurationName, int skipFrames) {
			Database database = DatabaseFactory.CreateDatabase(connectionConfigurationName);
			this.ExecuteTransactionalNonQuery(operations, database, skipFrames + 1);
		}

		public void ExecuteTransactionalNonQuery(List<DataOperation> operations, Database database) {
			this.ExecuteTransactionalNonQuery(operations, database, 1);
		}

		public void ExecuteTransactionalNonQuery(List<DataOperation> operations, Database database, int skipFrames) {
			skipFrames++;
			TraceManager.Trace("Beginning database transaction", FrameworkTraceTypes.Default);
			//Logging.Logger.Log("Beginning database transaction", CommonLibrary.Framework.Logging.Logger.LogCategory.General, skipFrames);

			DbConnection connection = database.CreateConnection();
			using (connection) {
				connection.Open();
				DbTransaction transaction = connection.BeginTransaction();

				try {
					foreach (DataOperation operation in operations) {
						this.ExecuteNonQuery(operation, database, transaction, skipFrames);
					}

					TraceManager.Trace("Committing database transaction", FrameworkTraceTypes.DataAccess);
					//Logging.Logger.Log("Committing database transaction", CommonLibrary.Framework.Logging.Logger.LogCategory.DataAccess, skipFrames);
					transaction.Commit();
				} catch (Exception ex) {
					ExceptionHandler.Handle(ex);
					TraceManager.Trace(string.Format("Rolling back database transaction due to error: {0}", ex.Message), TraceVerbosities.Minimal, FrameworkTraceTypes.Exception, ex);
					//Logging.Logger.Log(string.Format("Rolling back database transaction due to error: {0}", ex.Message), CommonLibrary.Framework.Logging.Logger.LogCategory.DataAccess, skipFrames);
					transaction.Rollback();
				}
			}
		}
		#endregion

		#region ExecuteNonQuery
		public void ExecuteNonQuery(DataOperation operation) {
			this.ExecuteNonQuery(operation, "Default Connection String");
		}

		private void ExecuteNonQuery(DataOperation operation, string connectionConfigurationName) {
			this.ExecuteNonQuery(operation, DatabaseFactory.CreateDatabase(connectionConfigurationName), 1);
		}

		public void ExecuteNonQuery(DataOperation operation, Database database) {
			this.ExecuteNonQuery(operation, database, null, 1);
		}

		private void ExecuteNonQuery(DataOperation operation, Database database, int skipFrames) {
			this.ExecuteNonQuery(operation, database, null, skipFrames + 1);
		}

		private void ExecuteNonQuery(DataOperation operation, Database database, DbTransaction transaction, int skipFrames) {
			DbCommand command = this.GetCommand(operation, database, skipFrames + 1);
			try {
				if (transaction != null) {
					database.ExecuteNonQuery(command, transaction);
				} else {
					database.ExecuteNonQuery(command);
				}
			} catch (Exception ex) {
				throw new Exception(string.Format("An error occurred executing procedure {0}", operation.StoredProcedure), ex);
			}
		}
		#endregion
		
		#endregion

		#region Methods
		private DbCommand GetCommand(DataOperation operation, Database database, int skipFrames) {
			skipFrames++;
			TraceManager.Trace(string.Format("Executing stored procedure: {0}", operation.StoredProcedure), FrameworkTraceTypes.DataAccess);
			//Logging.Logger.Log(string.Format("Executing stored procedure: {0}", operation.StoredProcedure), CommonLibrary.Framework.Logging.Logger.LogCategory.DataAccess, skipFrames);
			DbCommand command = database.GetStoredProcCommand(operation.StoredProcedure);

			if (operation.Parameters.Count > 0) {
				foreach (DataParameter parameter in operation.Parameters) {
					string parameterValue = (parameter.Value == null) ? "NULL" : parameter.Value.ToString();

					switch (parameter.Direction) {
						case ParameterDirection.Input:
							TraceManager.TraceFormat("Adding input parameter {0}, value: {1}", parameter.Name, parameterValue);
							//Logging.Logger.Log(string.Format("Adding input parameter {0}, value: {1}", parameter.Name, parameterValue), CommonLibrary.Framework.Logging.Logger.LogCategory.General, skipFrames);
							database.AddInParameter(command, parameter.Name, parameter.Type, parameter.Value);
							break;
						case ParameterDirection.Output:
							TraceManager.TraceFormat("Adding output parameter {0}, value: {1}", parameter.Name, parameterValue);
							//Logging.Logger.Log(string.Format("Adding output parameter {0}, value: {1}", parameter.Name, parameterValue), CommonLibrary.Framework.Logging.Logger.LogCategory.General, skipFrames);
							database.AddOutParameter(command, parameter.Name, parameter.Type, 255);
							break;
						default:
							throw new Exception("Improper parameter direction supplied.");
					}
				}
			}

			return command;
		}
		#endregion

	}
}