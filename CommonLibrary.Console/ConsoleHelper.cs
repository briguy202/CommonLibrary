using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.Console {
	public class ConsoleHelper {

		#region Enums
		public enum WriteMode {
			Console,
			Log,
			Both
		}

		public enum VerbosityLevel {
			Quiet,
			Normal,
			Verbose
		}
		#endregion

		#region Properties
		public string UsageTitle { get; set; }
		public int UsageLeftIndentationSpaces { get; set; }
		public int UsageNameRightPaddingSpaces { get; set; }
		public int UsageDescriptionIndentationSpaces { get; set; }
		public VerbosityLevel RuntimeVerbosity { get; set; }
		public ArgumentCollection Arguments { get; protected set; }
		public enum LogType {
			Default,
			Info,
			Error,
			Warning,
			Success
		}
		#endregion

		#region Constructors
		public ConsoleHelper(ArgumentCollection arguments) {
			this.Arguments = arguments;
			this.UsageLeftIndentationSpaces = 4;
			this.UsageNameRightPaddingSpaces = 1;
			this.UsageDescriptionIndentationSpaces = 2;
			this.RuntimeVerbosity = VerbosityLevel.Normal;
		}
		#endregion

		#region Methods
		public bool ParseArguments(string[] inputArgs) {
			this.Log("Beginning parsing arguments ...", WriteMode.Log);
			try {
				this.Arguments.ParseArguments(inputArgs);

				StringBuilder sb = new StringBuilder();
				foreach (KeyValuePair<string, Argument> pair in this.Arguments) {
					WriteMode mode = (pair.Value.ConfigSource == Argument.ConfigSourceType.ConfigFile) ? WriteMode.Both : WriteMode.Log;
					this.LogFormat(mode, "Argument {0}, Value: '{1}', Source: {2}", pair.Key, pair.Value.Value, pair.Value.ConfigSource.ToString());

					if (pair.Value.ConfigSource == Argument.ConfigSourceType.CommandLine) {
						sb.AppendFormat(" {0}{1} \"{2}\"", this.Arguments.ArgumentPrefix, pair.Key, pair.Value.Value);
					}
				}

				this.Log(string.Concat("Command Line:", sb.ToString()));
			} catch (InputException ex) {
				switch (ex.Code) {
					case InputException.ErrorCode.InvalidArgument:
						this.LogFormat(LogType.Error, "Invalid parameter '{0}'.", ex.Arguments[0]);
						break;
					case InputException.ErrorCode.InvalidValue:
						this.LogFormat(LogType.Error, "Invalid value '{0}' given to parameter '{1}'.", ex.Arguments[1], ex.Arguments[0]);
						break;
					case InputException.ErrorCode.ValueIsNotSet:
						this.LogFormat(LogType.Error, "No value given for parameter '{0}'.", ex.Arguments[0]);
						break;
					case InputException.ErrorCode.DuplicateKey:
						this.LogFormat(LogType.Error, "Parameter '{0}' can only be used once.", ex.Arguments[0]);
						break;
					case InputException.ErrorCode.ArgumentIsNotSet:
						this.LogFormat(LogType.Error, "Required parameter '{0}' was not set.", ex.Arguments[0]);
						break;
					case InputException.ErrorCode.RequiredIfSetInvalid:
						this.LogFormat(LogType.Error, "Dependent parameter '{0}' is invalid.", ex.Arguments[0]);
						break;
					case InputException.ErrorCode.RequiredIfSetNotSet:
						this.LogFormat(LogType.Error, "Parameter '{0}' was not set and needs to be if '{1}' is set.", ex.Arguments[0], ex.Arguments[1]);
						break;
					case InputException.ErrorCode.FileDoesNotExist:
						this.LogFormat(LogType.Error, "File '{0}' does not exist.", ex.Arguments[0]);
						break;
					default:
						break;
				}
				return false;
			}

			// Check if the help command was set above, or if we didn't get any arguments processed.
			if (this.Arguments.HelpArgument != null && this.Arguments.HelpArgument.IsSet) {
				this.Log("Help argument was set, printing out the help message.", WriteMode.Log);
				this.PrintUsage();
				return false;
			}

			this.Log("Arguments parsed succesfully.", WriteMode.Log);
			return true;
		}

		public bool GetBoolArgument(string argument) {
			return this.GetBoolArgument(argument, false);
		}

		public bool GetBoolArgument(string argument, bool nullValue) {
			if (this.IsBadArgument(argument)) {
				return nullValue;
			} else {
			    return bool.Parse(this.Arguments[argument].Value);
			}
		}

		public int GetIntArgument(string argument) {
			return this.GetIntArgument(argument, int.MinValue);
		}

		public int GetIntArgument(string argument, int nullValue) {
			if (this.IsBadArgument(argument)) {
				return nullValue;
			} else {
				return int.Parse(this.Arguments[argument].Value);
			}
		}

		public string GetStringArgument(string argument) {
			return this.GetStringArgument(argument, string.Empty);
		}

		public string GetStringArgument(string argument, string nullValue) {
			if (this.IsBadArgument(argument)) {
				return nullValue;
			} else {
				return this.Arguments[argument].Value;
			}
		}

		private bool IsBadArgument(string argument) {
			return (string.IsNullOrEmpty(argument) || this.Arguments == null || this.Arguments.Count == 0
					|| !this.Arguments.ContainsKey(argument) || string.IsNullOrEmpty(this.Arguments[argument].Value));
		}

		public void PrintUsage() {
			System.Console.WriteLine();
			System.Console.WriteLine(" " + this.UsageTitle);
			System.Console.WriteLine();

			int consoleWidth = (System.Console.LargestWindowWidth > 0) ? System.Console.WindowWidth : 150;	// If running from Visual Studio, System.Console.WindowWidth isn't set.
			int maxNameSize = this.Arguments.Max<KeyValuePair<string, Argument>>(v => v.Value.Name.Length);
			int leftRailPosition = this.UsageLeftIndentationSpaces + this.Arguments.ArgumentPrefix.Length + maxNameSize + this.UsageNameRightPaddingSpaces;
			int rightContentWidth = consoleWidth - 1 - leftRailPosition;

			// Sort the argument list.
			IOrderedEnumerable<KeyValuePair<string, Argument>> sortedArguments = this.Arguments.OrderBy(a => a.Value.Name);
			foreach (var pair in sortedArguments) {
				StringBuilder builder = new StringBuilder(pair.Value.Description);
				string leftBlock = string.Empty;
				string rightBlock = string.Empty;
				bool isFirst = true;

				if (pair.Value.AllowedValues != null && pair.Value.AllowedValues.Count > 0) {
					builder.AppendFormat(" Allowed values are: {0}.", string.Join(", ", pair.Value.AllowedValues.ToArray()));
				}

				if (!string.IsNullOrEmpty(pair.Value.DefaultValue)) {
					builder.AppendFormat(" Default value is {0}.", pair.Value.DefaultValue);
				}

				if (pair.Value.RequiredIfSetList != null && pair.Value.RequiredIfSetList.Count > 0) {
					string paramList = "-" + string.Join(" and -", pair.Value.RequiredIfSetList.ToArray());
					builder.AppendFormat(" This parameter requires that {1} also be set.", paramList);
				}

				string description = builder.ToString().Trim();
				while (description.Length > 0 || isFirst) {
					if (isFirst) {
						leftBlock = string.Concat(new String(' ', this.UsageLeftIndentationSpaces), this.Arguments.ArgumentPrefix, pair.Value.Name, new String(' ', (maxNameSize + this.UsageNameRightPaddingSpaces - pair.Value.Name.Length)));
					} else {
						leftBlock = new String(' ', leftRailPosition);
					}

					if (!isFirst) {
						description = string.Concat(new String(' ', this.UsageDescriptionIndentationSpaces), description);
					} else {
						if (string.IsNullOrEmpty(description)) {
							description = "No description.";
						}
					}

					if (description.Length > rightContentWidth) {
						rightBlock = description.Substring(0, rightContentWidth);
						// Check to see if there is a good breakpoint.  If not, just split at the max length.  We have to Trim() the value
						// so that we don't find the leading padding spaces as breakpoints.
						if (rightBlock.Trim().LastIndexOfAny(new char[] { ' ', '-' }) > -1) {
							rightBlock = rightBlock.Substring(0, rightBlock.LastIndexOfAny(new char[] { ' ', '-' }));	// find a good break point.
						}
					} else {
						rightBlock = description;
					}

					description = description.Substring(rightBlock.Length).Trim();
					System.Console.WriteLine(string.Concat(leftBlock, rightBlock));
					isFirst = false;
				}
			}
		}

		public void Log(string message) {
			this.Log(message, WriteMode.Both, VerbosityLevel.Normal);
		}

		public void Log(string message, VerbosityLevel verbosity) {
			this.Log(message, WriteMode.Both, verbosity);
		}

		public void Log(string message, WriteMode mode) {
			this.Log(message, mode, VerbosityLevel.Normal);
		}

		public void Log(string message, WriteMode mode, VerbosityLevel verbosity) {
			this.Log(message, mode, LogType.Default, verbosity);
		}

		public void Log(string message, WriteMode mode, LogType type) {
			this.Log(message, mode, type, VerbosityLevel.Normal);
		}

		public void Log(string message, WriteMode mode, LogType type, VerbosityLevel verbosity) {
			this.LogFormat(mode, type, message, verbosity, null);
		}

		public void LogFormat(string message, params object[] args) {
			this.LogFormat(message, VerbosityLevel.Normal, args);
		}

		public void LogFormat(string message, VerbosityLevel verbosity, params object[] args) {
			this.LogFormat(WriteMode.Both, message, verbosity, args);
		}

		public void LogFormat(WriteMode mode, string message, params object[] args) {
			this.LogFormat(mode, message, VerbosityLevel.Normal, args);
		}

		public void LogFormat(WriteMode mode, string message, VerbosityLevel verbosity, params object[] args) {
			this.LogFormat(mode, LogType.Default, message, verbosity, args);
		}

		public void LogFormat(LogType type, string message, params object[] args) {
			this.LogFormat(type, message, VerbosityLevel.Normal, args);
		}

		public void LogFormat(LogType type, string message, VerbosityLevel verbosity, params object[] args) {
			this.LogFormat(WriteMode.Both, type, message, verbosity, args);
		}

		public void LogFormat(WriteMode mode, LogType type, string message, VerbosityLevel verbosity, params object[] args) {
			// Skip tracing out the line if the runtime verbosity is less than the message's verbosity level.
			if (this.RuntimeVerbosity < verbosity) { return; }

			ConsoleColor color = ConsoleColor.Gray;
			switch (type) {
				case LogType.Error:
					message = string.Concat("ERROR: ", message);
					color = ConsoleColor.Red;
					break;
				case LogType.Warning:
					message = string.Concat("WARNING: ", message);
					color = ConsoleColor.Yellow;
					break;
				case LogType.Success:
					color = ConsoleColor.Green;
					break;
				case LogType.Info:
					color = ConsoleColor.White;
					break;
			}

			if (args != null) {
				message = string.Format(message, args);
			}

			if (type == LogType.Error) {
				mode = WriteMode.Both;
				this.PrintUsage();
			}

			if (mode == WriteMode.Console || mode == WriteMode.Both) {
				System.Console.ForegroundColor = color;
				System.Console.WriteLine(message);
				System.Console.ResetColor();
			}

			if (mode == WriteMode.Log || mode == WriteMode.Both) {
				CommonLibrary.Console.Tracing.Trace.WriteLine(message);
			}
		}

		public void ExecuteCommand(string command) {
			this.ExecuteCommand(command, string.Empty);
		}

		public void ExecuteCommand(string command, string arguments) {
			string _return = string.Empty;

			this.LogFormat(WriteMode.Log, "Executing command: \"{0}\" {1}", command, arguments);
			ProcessStartInfo pInfo = new ProcessStartInfo();
			pInfo.RedirectStandardOutput = true;
			pInfo.UseShellExecute = false;
			pInfo.CreateNoWindow = false;
			pInfo.FileName = string.Format("\"{0}\"", command);
			if (!string.IsNullOrEmpty(arguments)) {
				pInfo.Arguments = arguments;
			}
			Process process = new Process();
			process.StartInfo = pInfo;
			process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
			process.Start();
			process.BeginOutputReadLine();

			process.WaitForExit();
			this.Log("Command completed.", WriteMode.Log);
		}

		private void process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			if (!string.IsNullOrEmpty(e.Data)) {
				this.Log(e.Data, WriteMode.Log);
			}
		}
		#endregion
		
	}
}