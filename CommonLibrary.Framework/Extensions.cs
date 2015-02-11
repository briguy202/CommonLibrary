using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using CommonLibrary.Framework.Validation;
using System;

namespace CommonLibrary.Framework {
	public static class Extensions {
		private static Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

		public static bool IsGuid(this string guid) {
			if (!string.IsNullOrEmpty(guid)) {
				if (isGuid.IsMatch(guid)) {
					return true;
				}
			}

			return false;
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {
			foreach (var item in collection) {
				action(item);
			}
		}

		public static string Join(this IEnumerable<string> collection, string separator) {
			string _return = string.Join(separator, collection);
			return _return;
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) {
			if (collection != null) {
				using (var enumerator = collection.GetEnumerator()) {
					return !enumerator.MoveNext();
				}
			}
			return true;
		}

		public static string ReplaceInvalidPathCharacters(this string value, string replacement) {
			string directoryRegexString = @"[\[<>\|""^?*\]]";
			string filenamesRegexString = @"[\[<>\|""^?*\]:\\]";
			string directory = Path.GetDirectoryName(value);
			string directoryCleaned = Regex.Replace(directory, directoryRegexString, replacement);

			// If there is a directory value then we need to find the filename part.  If no directory value, then the whole
			// thing is assumed to be a filename.
			string filename = (!string.IsNullOrEmpty(directory)) ? Path.GetFileName(value) : value;
			string filenameCleaned = Regex.Replace(filename, filenamesRegexString, replacement);

			// If there is no directory, then all we were given was a filename so don't add the separator if only a file.
			string joiner = (!string.IsNullOrEmpty(directory)) ? @"\" : string.Empty;

			var finalValue = string.Concat(directoryCleaned, joiner, filenameCleaned);
			return finalValue;
		}

		public static FileInfo[] GetFilesPattern(this DirectoryInfo directory, string searchPattern) {
			string[] searchPatterns = searchPattern.Split('|');
			List<FileInfo> files = new List<FileInfo>();
			foreach (string pattern in searchPatterns) {
				files.AddRange(directory.GetFiles(pattern));
			}
			files.Sort((a, b) => a.Name.CompareTo(b.Name));
			return files.ToArray();
		}

		#region XML Extensions
		public static bool IsNullOrEmpty(this XmlNode node) {
			return (node == null || string.IsNullOrEmpty(node.InnerText));
		}

		public static void AddAttribute(this XmlNode node, string name, string value) {
			XmlDocument doc = node.OwnerDocument;
			XmlAttribute attribute = doc.CreateAttribute(name);
			attribute.Value = value;
			node.Attributes.Append(attribute);
		}

		public static XmlElement AddNode(this XmlNode parent, string nodeName) {
			ValidationUtility.ThrowIfNullOrEmpty(parent, "parent");
			ValidationUtility.ThrowIfNullOrEmpty(nodeName, "nodeName");

			return parent.AddNode(nodeName, string.Empty);
		}

		public static XmlElement AddNode(this XmlNode parent, string nodeName, string text) {
			ValidationUtility.ThrowIfNullOrEmpty(parent, "parent");
			ValidationUtility.ThrowIfNullOrEmpty(nodeName, "nodeName");

			XmlElement node = null;
			XmlDocument doc = (parent is XmlDocument) ? (XmlDocument)parent : parent.OwnerDocument;
			node = doc.CreateElement(nodeName);

			if (!string.IsNullOrEmpty(text)) {
				node.InnerText = text;
			}
			parent.AppendChild(node);
			return node;
		}

		public static T GetNodeValue<T>(this XmlNode parent, string name) {
			T value = parent.GetNodeValue<T>(name, default(T));
			return value;
		}

		public static T GetNodeValue<T>(this XmlNode parent, string name, T defaultValue) {
			ValidationUtility.ThrowIfNullOrEmpty(parent, "parent");

			T value = defaultValue;
			if (!parent.SelectSingleNode(name).IsNullOrEmpty()) {
				string nodeValue = parent.SelectSingleNode(name).InnerText;
				if (!string.IsNullOrWhiteSpace(nodeValue)) {
					value = nodeValue.CastString<T>(defaultValue);
				}
			}

			return value;
		}

		public static T GetAttributeValue<T>(this XmlNode parent, string name) {
			T value = parent.GetAttributeValue<T>(name, default(T));
			return value;
		}

		public static T GetAttributeValue<T>(this XmlNode parent, string name, T defaultValue) {
			ValidationUtility.ThrowIfNullOrEmpty(parent, "parent");

			T value = defaultValue;
			XmlAttribute attribute = parent.Attributes[name];
			if (attribute != null) {
				string nodeValue = attribute.Value;
				if (!string.IsNullOrWhiteSpace(nodeValue)) {
					value = nodeValue.CastString<T>(defaultValue);
				}
			}

			return value;
		}

		public static T CastString<T>(this string stringValue, T defaultValue) {
			T value = defaultValue;
			if (!string.IsNullOrWhiteSpace(stringValue)) {
				if (defaultValue is int) {
					int parseData;
					if (int.TryParse(stringValue, out parseData)) {
						value = (T)(object)parseData;
					}
				} else if (defaultValue is DateTime) {
					DateTime parseData;
					if (DateTime.TryParse(stringValue, out parseData)) {
						value = (T)(object)parseData;
					}
				} else if (defaultValue is double) {
					double parseData;
					if (double.TryParse(stringValue, out parseData)) {
						value = (T)(object)parseData;
					}
				} else if (defaultValue is bool) {
					bool parseData;
					if (bool.TryParse(stringValue, out parseData)) {
						value = (T)(object)parseData;
					}
				} else {
					value = (T)(object)stringValue;
				}
			}
			return value;
		}
		#endregion

		public static T GetData<T>(this Dictionary<string, object> collection, string key) {
			ValidationUtility.ThrowIfNullOrEmpty(collection, "collection");
			ValidationUtility.ThrowIfNullOrEmpty(key, "key");

			T value = collection.GetData<T>(key, default(T));
			return value;
		}

		public static T GetData<T>(this Dictionary<string, object> collection, string key, T defaultValue) {
			ValidationUtility.ThrowIfNullOrEmpty(collection, "collection");
			ValidationUtility.ThrowIfNullOrEmpty(key, "key");

			T value = defaultValue;
			if (collection.ContainsKey(key)) {
				object data = collection[key];
				if (data is string && defaultValue is int) {
					int parseData;
					if (int.TryParse((string)data, out parseData)) {
						value = (T)(object)parseData;
					}
				} else if (data is string && defaultValue is DateTime) {
					DateTime parseData;
					if (DateTime.TryParse((string)data, out parseData)) {
						value = (T)(object)parseData;
					}
				} else {
					value = (T)collection[key];
				}
			}
			return value;
		}
	}
}