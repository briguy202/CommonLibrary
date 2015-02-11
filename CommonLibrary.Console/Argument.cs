using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommonLibrary.Console {
	public class Argument {

		#region Enums
		public enum ConfigSourceType {
			NotSet,
			Default,
			ConfigFile,
			CommandLine
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the argument name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the name of the required group this argument belongs to.  A required group means that at least
		/// one of the arguments in the group must be set.
		/// </summary>
		public string RequiredGroupName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this argument is required.
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		/// Gets or sets the list of values that this parameter accepts.
		/// </summary>
		public List<string> AllowedValues { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a value for this argument is required.
		/// </summary>
		public bool IsValueRequired { get; set; }
		public bool IsSet { get; set; }
		public bool EnforceChecks { get; set; }

		/// <summary>
		/// Gets or sets a list of arguments that have to be set if this argument is set.
		/// </summary>
		public Collection<string> RequiredIfSetList { get; set; }

		/// <summary>
		/// Gets or sets the default value that will be used for this argument if none is specified.
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets the value assigned to this argument.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the description shown to the user that explains what this argument is for.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value that shows where the argument was assigned from at runtime.
		/// </summary>
		public ConfigSourceType ConfigSource { get; set; }
		#endregion

		#region Constructors
		public Argument(string name, string description, bool isRequired, bool isValueRequired) {
			if (string.IsNullOrEmpty(name)) { throw new ArgumentException("name"); }
			this.EnforceChecks = true;
			this.DefaultValue = string.Empty;
			this.AllowedValues = new List<string>();
			this.RequiredIfSetList = new Collection<string>();

			this.Name = name;
			this.IsRequired = isRequired;
			this.IsValueRequired = isValueRequired;
			this.Description = description;
			this.ConfigSource = ConfigSourceType.NotSet;
		}
		#endregion

	}
}