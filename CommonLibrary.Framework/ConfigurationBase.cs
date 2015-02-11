using System;
using System.Collections.Specialized;

namespace CommonLibrary.Framework {
	public class ConfigurationBase {
		private static NameValueCollection _settings = null;
		private static string _defaultConnectionString;
		public enum EnvironmentType {
			Development,
			Production
		}

		#region Constructors
		static ConfigurationBase() {
			_settings = System.Configuration.ConfigurationManager.AppSettings;
			if (_settings == null) {
				throw new Exception("Unable to obtain configuration");
			}
			_defaultConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseDefaultConnectionString"].ConnectionString;
		}
		#endregion

		#region Properties
		public static string DefaultConnectionString {
			get { return _defaultConnectionString; }
		}

		public static EnvironmentType Environment {
			get { return (EnvironmentType)Enum.Parse(typeof(EnvironmentType), ConfigurationBase.GetConfigString("Environment", "Development")); }
		}
		#endregion

		#region Methods
		public static string GetConfigString(string key) {
			return ConfigurationBase.GetConfigString(key, string.Empty);
		}

		public static string GetConfigString(string key, string defaultValue) {
			object value = _settings[key];
			return (value != null) ? value.ToString() : defaultValue;
		}

		public static int GetConfigInteger(string key) {
			return ConfigurationBase.GetConfigInteger(key, int.MinValue);
		}

		public static int GetConfigInteger(string key, int defaultValue) {
			int value;
			return (_settings[key] != null && int.TryParse(_settings[key].ToString(), out value)) ? value : defaultValue;
		}

		public static double GetConfigDouble(string key) {
			return ConfigurationBase.GetConfigDouble(key, double.MinValue);
		}

		public static double GetConfigDouble(string key, double defaultValue) {
			double value;
			return (_settings[key] != null && double.TryParse(_settings[key].ToString(), out value)) ? value : defaultValue;
		}

		public static bool GetConfigBoolean(string key) {
			return ConfigurationBase.GetConfigBoolean(key, false);
		}

		public static bool GetConfigBoolean(string key, bool defaultValue) {
			bool value;
			return (_settings[key] != null && bool.TryParse(_settings[key].ToString(), out value)) ? value : defaultValue;
		}
		#endregion

	}
}