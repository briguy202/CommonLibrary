using CommonLibrary.Framework.Validation;
namespace CommonLibrary.Framework.Tracing {
	public class TraceManager {

		#region Declarations
		private static int _systemVerbosity;
		private static object _syncRoot;
		#endregion

		#region Events
		public class TraceWrittenEventArgs {
			public string Message { get; set; }
			public int Verbosity { get; set; }
			public string Type { get; set; }
			public object Data { get; set; }
		}
		public delegate void TraceWrittenEventHandler(object sender, TraceWrittenEventArgs e);
		public static event TraceWrittenEventHandler TraceWritten;
		#endregion

		#region Constructors
		static TraceManager() {
			_systemVerbosity = TraceVerbosities.Default;
			_syncRoot = new object();
		}
		#endregion

		#region Methods
		public static void SetVerbosity(int level) {
			lock (_syncRoot) {
				_systemVerbosity = level;
			}
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="format">The string formatter to use to create the trace message.</param>
		/// <param name="args">The formatter values.</param>
		public static void TraceFormat(string format, params object[] args) {
			ValidationUtility.ThrowIfNullOrEmpty(format, "format");
			TraceManager.Trace(string.Format(format, args), TraceVerbosities.Default);
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="message">The trace message.</param>
		public static void Trace(string message) {
			TraceManager.Trace(message, TraceVerbosities.Default, FrameworkTraceTypes.Default, null);
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="message">The trace message.</param>
		/// <param name="verbosityLevel">The verbosity level of this trace message.  If the verbosity of the
		/// trace message is higher than the system's verbosity, the message will not be sent to the trace
		/// listeners.
		/// </param>
		public static void Trace(string message, int verbosityLevel) {
			TraceManager.Trace(message, verbosityLevel, FrameworkTraceTypes.Default, null);
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="message">The trace message.</param>
		/// <param name="traceType">The classification of the trace message.</param>
		public static void Trace(string message, string traceType) {
			TraceManager.Trace(message, TraceVerbosities.Default, traceType, null);
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="message">The trace message.</param>
		/// <param name="verbosityLevel">The verbosity level of this trace message.  If the verbosity of the
		/// trace message is higher than the system's verbosity, the message will not be sent to the trace
		/// listeners.
		/// </param>
		/// <param name="traceType">The classification of the trace message.</param>
		public static void Trace(string message, int verbosityLevel, string traceType) {
			TraceManager.Trace(message, verbosityLevel, traceType, null);
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="message">The trace message.</param>
		/// <param name="verbosityLevel">The verbosity level of this trace message.  If the verbosity of the
		/// trace message is higher than the system's verbosity, the message will not be sent to the trace
		/// listeners.
		/// </param>
		/// <param name="traceType">The classification of the trace message.</param>
		/// <param name="data"></param>
		public static void Trace(string message, int verbosityLevel, string traceType, object data) {
			TraceManager.Trace(new TraceWrittenEventArgs() {
				Message = message,
				Verbosity = verbosityLevel,
				Type = traceType,
				Data = data
			});
		}

		/// <summary>
		/// Writes a trace message to all trace listeners.
		/// </summary>
		/// <param name="traceData"></param>
		public static void Trace(TraceWrittenEventArgs traceData) {
			if (traceData.Verbosity <= _systemVerbosity && TraceManager.TraceWritten != null) {
				TraceManager.TraceWritten(null, traceData);
			}
		}
		#endregion
		
	}
}