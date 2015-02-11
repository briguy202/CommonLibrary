using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace CommonLibrary.Framework.Caching {
	public interface ICache {
		object this[string index] { get; }
		void Add(string key, object value);
		void Add(string key, object value, long expirationMinutes);
		void Add(string key, object value, long expirationMinutes, bool useAbsoluteExpiration);
		void Remove(string key);
		bool Contains(string key);
		void Clear();
		int Count { get; }
		object GetData(string key);
		bool IsPopulated();
		Collection<string> Keys { get; }
		string ID { get; }
		string Manager { get; }
	}
}