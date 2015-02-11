using System.Configuration;

namespace CommonLibrary.Framework.Tracing {
	public class TraceConfigurationElement : ConfigurationElement {
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name {
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("value", IsRequired = true)]
		public int Value {
			get { return (int)this["value"]; }
			set { this["value"] = value; }
		}
	}
}