using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Framework.Interfaces {
	public interface INameValue<T> {
		string Name { get; }
		string Value { get; }
	}
}
