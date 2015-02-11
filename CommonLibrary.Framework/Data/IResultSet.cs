using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace CommonLibrary.Framework.Data {
	public interface IResultSet : IDisposable {
		bool MoveNext();
		bool HasData { get; }
		List<string> GetPropertyNames();
		object GetValue(string propertyName);
		object GetValue(string propertyName, object nullValue);
		object GetValue(int index, object nullValue);
		int GetInteger(string propertyName);
		int GetInteger(string propertyName, int nullValue);
		long GetLong(string propertyName);
		long GetLong(string propertyName, long nullValue);
		short GetShort(string propertyName);
		short GetShort(string propertyName, short nullValue);
		decimal GetDecimal(string propertyName);
		decimal GetDecimal(string propertyName, decimal nullValue);
		double GetDouble(string propertyName);
		double GetDouble(string propertyName, double nullValue);
		System.Guid GetGuid(string propertyName);
		System.Guid GetGuid(string propertyName, System.Guid nullValue);
		string GetString(string propertyName);
		string GetString(string propertyName, string nullValue);
		DateTime GetDateTime(string propertyName);
		DateTime GetDateTime(string propertyName, DateTime nullValue);
		bool GetBoolean(string propertyName);
		bool GetBoolean(string propertyName, bool nullValue);
		object GetObject(string propertyName);
		object GetObject(string propertyName, object nullValue);
		System.Data.DataTable GetSchema();
	}
}