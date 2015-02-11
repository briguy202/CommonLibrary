using System.Configuration;

namespace CommonLibrary.Framework.Tracing {
	public class TraceConfigurationSection : ConfigurationSection {
		private static TraceConfigurationSection _settings = ConfigurationManager.GetSection("commonLibrary/tracing") as TraceConfigurationSection;

		public static TraceConfigurationSection Settings {
			get { return _settings; }
		}

		[ConfigurationProperty("verbosityLevels")]
		public TraceConfigurationCollection VerbosityLevels {
			get { return (TraceConfigurationCollection)this["verbosityLevels"]; }
		}
	}
}