using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonLibrary.Console.Tracing {
	public static class Trace {

		#region Declarations
		private static StreamWriter _writer;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the full file path to the trace file.  This path allows a string that contains a '{0}' placeholder for the
		/// current environment variable value of the COMPUTERNAME setting.
		/// </summary>
		public static string FullFilePath { get; set; }

		/// <summary>
		/// Gets or sets a value that specifies whether the trace file should simply append to the end, or whether it should be
		/// overwritten each time the trace file is created.
		/// </summary>
		public static bool Overwrite { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates whether tracing is enabled.  If this value is false, a buffer is created to hold all
		/// trace messages.  If the value is then set to true, on the first write of a message, the buffer will be cleared and written
		/// out.
		/// </summary>
		public static bool Enabled { get; set; }

		private static StringBuilder Buffer { get; set; }
		private static StreamWriter Writer {
			get {
				if (_writer == null) {
					try {
						if (string.IsNullOrEmpty(Trace.FullFilePath)) {
							throw new Exception("Trace file path not specified.");
						}
						string filePath = string.Format(Trace.FullFilePath, Environment.GetEnvironmentVariable("COMPUTERNAME")).Replace('/', '\\');
						if (filePath.Contains('\\')) {
							string directory = filePath.Substring(0, filePath.LastIndexOf('\\'));
							Directory.CreateDirectory(directory);
						}
						FileStream stream = File.Open(filePath, (Trace.Overwrite) ? FileMode.OpenOrCreate : FileMode.Append, FileAccess.Write, FileShare.Read);
						if (Trace.Overwrite) {
							stream.SetLength(0);
						}
						_writer = new StreamWriter(stream);
						_writer.AutoFlush = true;
					} catch (Exception ex) {
						_writer = null;
						throw ex;
					}
					
				}
				return _writer;
			}
		}
		#endregion

		#region Constructors
		static Trace() {
			Trace.Buffer = new StringBuilder();
		}
		#endregion

		#region Methods
		public static void WriteLine() {
			Trace.DoWriteLine(string.Empty);
		}

		public static void WriteLine(string message) {
			message = string.Concat(DateTime.Now, " - ", message);
			Trace.DoWriteLine(message);
		}

		public static void WriteLine(string format, params object[] args) {
			format = string.Concat(DateTime.Now, " - ", format);
			string message = string.Format(format, args);
			Trace.DoWriteLine(message);
		}

		public static void WriteSeparator() {
			Trace.DoWriteLine(new string('*', 100));
		}

		public static void Flush() {
			if (Trace.Enabled && Trace.Buffer != null && Trace.Buffer.Length > 0) {
				Trace.Writer.Write(Trace.Buffer);	// Write out buffer.
				Trace.Buffer = new StringBuilder();	// Clear it.
			}
		}

		private static void DoWriteLine(string message) {
			if (Trace.Enabled) {
				Trace.Flush();
				Trace.Writer.WriteLine(message);
			} else {
				Trace.Buffer.AppendLine(message);
			}
		}
		#endregion

	}
}
