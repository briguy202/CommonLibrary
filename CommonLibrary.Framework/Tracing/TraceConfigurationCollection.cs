using System.Configuration;

namespace CommonLibrary.Framework.Tracing {
	[ConfigurationCollection(typeof(TraceConfigurationElement), AddItemName = "level")]
	public class TraceConfigurationCollection : ConfigurationElementCollection {
		public TraceConfigurationCollection() { }

		protected override ConfigurationElement CreateNewElement() {
			return new TraceConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((TraceConfigurationElement)element).Name;
		}
	}
}
