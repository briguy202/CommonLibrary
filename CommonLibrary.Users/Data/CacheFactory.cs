using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Caching;

namespace CommonLibrary.Users.Data {
	public static class CacheFactory {
		public static ICache GetRoleCache() {
			return CommonLibrary.Framework.Caching.CacheFactory.GetCache("roleCache");
		}
	}
}