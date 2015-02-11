using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.ObjectModel;

namespace CommonLibrary.Console {
	public class ArgumentCollection : Dictionary<string, Argument> {

		#region Properties
		private bool EnforceChecks { get; set; }

		/// <summary>
		/// Gets or sets a value that indicates whether the help text should be shown if the program is run without any arguments.  Default
		/// value is true.
		/// </summary>
		public bool ShowHelpIfNoArguments { get; set; }

		/// <summary>
		/// Gets or sets a pointer to the argument that defines the help command.
		/// </summary>
		public Argument HelpArgument { get; set; }

		/// <summary>
		/// Gets or sets a pointer to the argument that defines where an optional configuration file exists that has arguments defined in it.
		/// </summary>
		public Argument ConfigurationFileArgument { get; set; }

		/// <summary>
		/// Gets or sets the argument prefix value which preceeds all arguments supplied to the program.
		/// </summary>
		public string ArgumentPrefix { get; set; }
		#endregion

		#region Constructors
		public ArgumentCollection(string argPrefix) : base() {
			if (string.IsNullOrEmpty(argPrefix)) { throw new ArgumentNullException("argPrefix"); }
			this.ArgumentPrefix = argPrefix;
			this.ShowHelpIfNoArguments = true;
		}
		#endregion

		#region Methods
		public void Add(Argument argument) {
			this.Add(argument.Name, argument);
		}

		public void ParseArguments(string[] args) {
			// Add in the special parameters to the "this" collection
			if (this.ConfigurationFileArgument != null) { this.Add(this.ConfigurationFileArgument); }
			if (this.HelpArgument != null) { this.Add(this.HelpArgument); }

			if (this.HelpArgument != null && (args.Length == 0 && this.ShowHelpIfNoArguments)) {
				this.HelpArgument.IsSet = true;
			}

			string key = string.Empty;
			this.EnforceChecks = true;
			Dictionary<string, string> argValues = new Dictionary<string, string>();

			// Parse out the entered arguments into their key/value pairs.
			foreach (var arg in args) {
				if (arg.StartsWith(this.ArgumentPrefix)) {
					// This is a new key
					if (!string.IsNullOrEmpty(key)) {
						// Store the last key
						argValues.Add(key, string.Empty);
					}
					key = arg.Substring(this.ArgumentPrefix.Length);	// Store the key without the prefix value
				} else {
					// This is a value
					argValues[key] = arg;
					key = string.Empty;
				}
			}

			// Write the last value
			if (!string.IsNullOrEmpty(key)) {
				argValues.Add(key, string.Empty);
			}

			// Check to see if a xml configuration file has been specified.  Note that this occurs BEFORE the values supplied to the program
			// are read in so that these values from the config file can be overridden.
			if (this.ConfigurationFileArgument != null && argValues.ContainsKey(this.ConfigurationFileArgument.Name) && !string.IsNullOrEmpty(argValues[this.ConfigurationFileArgument.Name])) {
				string configFilePath = argValues[this.ConfigurationFileArgument.Name];
				if (!File.Exists(configFilePath)) {
					throw new InputException(InputException.ErrorCode.FileDoesNotExist, configFilePath);
				} else {
					XmlDocument configXml = new XmlDocument();
					configXml.Load(configFilePath);

					// Loop through each of the nodes defined in the configuration file.
					foreach (XmlElement node in configXml.DocumentElement.ChildNodes) {
						this.ParseInput(node.Name, node.InnerText, Argument.ConfigSourceType.ConfigFile);
					}
				}
			}

			// Loop through each and validate each of the supplied arguments.  Note that this occurs AFTER the config file has been read in, if
			// it exists, so that these values override what is defined in the config file.
			foreach (KeyValuePair<string, string> pair in argValues) {
				this.ParseInput(pair.Key, pair.Value, Argument.ConfigSourceType.CommandLine);
			}

			// this is the list of required group names that have already been processed (avoids repeating it).
			List<string> processedRequiredGroups = new List<string>();

			if (this.HelpArgument == null || !this.HelpArgument.IsSet) {
				// Loop through each of the defined arguments.
				foreach (KeyValuePair<string, Argument> pair in this) {
					// Set the default values if no value was given
					if (string.IsNullOrEmpty(pair.Value.Value)) {
						if (!pair.Value.IsRequired && pair.Value.IsSet) {
							// This isn't a required value, but the argument was set - this is a boolean situation where
							// a parameter is set like "blah.exe -doSomething" where doSomething doesn't have a value, but
							// should be set to True.
							pair.Value.Value = "True";
						} else if (!string.IsNullOrEmpty(pair.Value.DefaultValue)) {
							pair.Value.Value = pair.Value.DefaultValue;
							pair.Value.ConfigSource = Argument.ConfigSourceType.Default;
						}
					}

					// Check the list of arguments that are required when this argument is set.
					if (pair.Value.IsSet && pair.Value.RequiredIfSetList != null && pair.Value.RequiredIfSetList.Count > 0) {
						foreach (string requiredIfSet in pair.Value.RequiredIfSetList) {
							if (!this.ContainsKey(requiredIfSet)) {
								// The required argument has not been defined, so throw an error.
								throw new InputException(InputException.ErrorCode.RequiredIfSetInvalid, requiredIfSet);
							} else if (!this[requiredIfSet].IsSet) {
								throw new InputException(InputException.ErrorCode.RequiredIfSetNotSet, new Collection<string>() { requiredIfSet, pair.Key });
							}
						}
					}

					// Check other arguments defined in the same required group.
					if (!string.IsNullOrEmpty(pair.Value.RequiredGroupName) && !processedRequiredGroups.Contains(pair.Value.RequiredGroupName)) {
						processedRequiredGroups.Add(pair.Value.RequiredGroupName);
						IEnumerable<KeyValuePair<string, Argument>> requiredPairs = this.Where((a) => (a.Value.RequiredGroupName == pair.Value.RequiredGroupName) && a.Value.IsSet);
						if (requiredPairs.Count() == 0) {
							IEnumerable<KeyValuePair<string, Argument>> requiredPairsFullList = this.Where((a) => a.Value.RequiredGroupName == pair.Value.RequiredGroupName);
							List<string> requiredArgumentsFullList = new List<string>();
							foreach (KeyValuePair<string, Argument> item in requiredPairsFullList)
							{
								requiredArgumentsFullList.Add(item.Value.Name);
							}
							throw new InputException(InputException.ErrorCode.ArgumentIsNotSet, string.Join(" or ", requiredArgumentsFullList.ToArray()));
						}
					}
				}

				// Skip enforcing values if we're going to be displaying help or if a parameter has stated that this should be skipped.
				if (this.EnforceChecks) {
					foreach (KeyValuePair<string, Argument> pair in this) {
						if (pair.Value.IsRequired && !pair.Value.IsSet && string.IsNullOrEmpty(pair.Value.DefaultValue)) {
							throw new InputException(InputException.ErrorCode.ArgumentIsNotSet, pair.Key);
						}
					}
				}
			}
		}

		protected void ParseInput(string name, string value, CommonLibrary.Console.Argument.ConfigSourceType configSource) {
			if (!this.ContainsKey(name)) {
				throw new InputException(InputException.ErrorCode.InvalidArgument, name);
			} else if (configSource == Argument.ConfigSourceType.ConfigFile && this[name].IsSet) {
				throw new InputException(InputException.ErrorCode.DuplicateKey, name);
			} else if (this[name].IsValueRequired && string.IsNullOrEmpty(value)) {
				throw new InputException(InputException.ErrorCode.ValueIsNotSet, name);
			} else if (this[name].AllowedValues != null && this[name].AllowedValues.Count > 0 && !this[name].AllowedValues.Contains(value)) {
				throw new InputException(InputException.ErrorCode.InvalidValue, new Collection<string>() { name, value } );
			} else {
				if (!this[name].EnforceChecks) { this.EnforceChecks = false; }

				this[name].IsSet = true;
				this[name].ConfigSource = configSource;
				if (!string.IsNullOrEmpty(value)) {
					this[name].Value = value;
				}
			}
		}

		public bool ContainsMultiple(params string[] arguments) {
			bool found = false;
			foreach (var argument in arguments) {
				if (this.ContainsKey(argument)) {
					if (found) {
						return true;
					} else {
						found = true;
					}
				}
			}

			return false;
		}

		public bool ContainsNone(params string[] arguments) {
			foreach (var argument in arguments) {
				if (this.ContainsKey(argument)) {
					return false;
				}
			}

			return true;
		}
		#endregion

	}
}