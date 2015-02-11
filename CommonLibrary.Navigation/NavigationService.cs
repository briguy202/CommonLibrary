using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Users;
using CommonLibrary.Framework.Objects;

namespace CommonLibrary.Navigation {
	public static class NavigationService {
		public static Node GetRootNode(Node node, UserContext context) {
			Node _return = node;
			while (_return.ParentNode != null) {
				_return = _return.ParentNode;
			}
			return _return;
		}

		public static Node GetNodeByID(int id, UserContext context) {
			return new Node(id, context);
		}

		public static Node GetNodeByURL(string URL, UserContext context) {
			Data.INavigationDAO _dao = Data.Factory.GetNavigationDAO();
			if (_dao != null) {
				return _dao.FindNodeByURL(URL, context);
			} else {
				return null;
			}
		}

		internal static URLDetail GetMostMatchedURL(URLDetail url, List<URLDetail> haystack) {
			if (url == null) { throw new ArgumentNullException("url"); }
			if (haystack == null || haystack.Count == 0) { throw new ArgumentNullException("haystack", "Value cannot be null or have a count of zero"); }

			int matchCount = 0;
			URLDetail _return = haystack[0];

			foreach (URLDetail item in haystack) {
				if (url.Parameters.Count > 0) {
					int resultMatchedParams = 0;
					foreach (Parameter parameter in url.Parameters) {
						if (item.Parameters.Contains(parameter)) {
							resultMatchedParams++;
						}
					}

					if (resultMatchedParams > matchCount) {
						_return = item;
					}
				} else {
					if (item.Parameters.Count == 0) {
						return item;	// just return the first one that fits here.
					}
				}
			}

			return _return;
		}
	}
}