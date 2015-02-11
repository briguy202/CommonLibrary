using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Framework.Exceptions {
	public class ExceptionHandler {
		public enum eExceptionPolicy {
			DefaultPolicy,
			UnhandledExceptionPolicy
		}

		public static void Handle(Exception ex) {
			ExceptionHandler.Handle(ex, eExceptionPolicy.DefaultPolicy, 1);
		}

		public static void Handle(Exception ex, eExceptionPolicy policy) {
			ExceptionHandler.Handle(ex, policy, 1);
		}

		private static void Handle(Exception ex, eExceptionPolicy policy, int skipFrames) {
			Exception rethrowEx = null;
			TraceManager.Trace(ex.Message);
			//Logger.Log(ex.Message, Logger.LogCategory.General, skipFrames + 1);
			if (ExceptionPolicy.HandleException(ex, ExceptionHandler.GetPolicyValue(policy), out rethrowEx)) {
				// Returned a rethrow instruction
				if (rethrowEx != null) {
					throw rethrowEx;
				} else {
					throw ex;
				}
			}
		}

		private static string GetPolicyValue(eExceptionPolicy policy) {
			switch (policy) {
				case eExceptionPolicy.DefaultPolicy:
					return "Default Policy";
				case eExceptionPolicy.UnhandledExceptionPolicy:
					return "Unhandled Exception Policy";
				default:
					return "Default Policy";
			}
		}
	}
}