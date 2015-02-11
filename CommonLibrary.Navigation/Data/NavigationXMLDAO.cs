using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using CommonLibrary.Framework;
using CommonLibrary.Users.Roles;
using CommonLibrary.Users;
using CommonLibrary.Framework.Objects;
using System.Collections;
using CommonLibrary.Framework.Tracing;

namespace CommonLibrary.Navigation.Data {
	public class NavigationXMLDAO : INavigationDAO {
		static XmlDocument _document;

		static NavigationXMLDAO() {
			_document = new XmlDocument();
			_document.Load(ConfigurationBase.GetConfigString("XMLNavigationFilePath"));
		}

		#region INavigationDAO Members

		public XmlDocument GenerateXMLNodeTree(string savePath, UserContext context) {
			lock (_document) {
				// Regenerate the XML document and reload it
				Data.NavigationSQLDAO dao = new NavigationSQLDAO();
				_document = dao.GenerateXMLNodeTree(savePath, context);
				return _document;
			}
		}

		public void LoadNode(Node node, CommonLibrary.Users.UserContext context, params object[] parameters) {
			XmlNode page = _document.SelectSingleNode(string.Format("//page[@id='{0}']", node.ID));
			if (page != null) {
				string _parameters = page.Attributes["parameters"].Value;

				node.ID = int.Parse(page.Attributes["id"].Value);
				node.Name = page.Attributes["name"].Value;
				node.LinkName = page.Attributes["linkName"].Value;
				node.URL = new URLDetail(page.Attributes["url"].Value);
				if (!string.IsNullOrEmpty(_parameters)) { node.URL.Parameters = ParametersCollection.Parse(_parameters); }
				node.DisplayLeftNav = bool.Parse(page.Attributes["displayLeftNav"].Value);
				node.DisplaySequence = int.Parse(page.Attributes["displaySequence"].Value);
				node.Visible = bool.Parse(page.Attributes["visible"].Value);
			} else {
				throw new Exception(string.Format("Unable to find node: {0}", node.ID));
			}
		}

		public void LoadNodeRoles(Node node, CommonLibrary.Users.UserContext context, params object[] parameters) {
			XmlNode page = _document.SelectSingleNode(string.Format("//page[@id='{0}']", node.ID));
			if (page != null) {
				string roles = page.Attributes["roles"].Value;
				if (!string.IsNullOrEmpty(roles)) {
					string[] rolesArr = roles.Split(',');
					foreach (string roleID in rolesArr) {
						node.Roles.Add(new Role(int.Parse(roleID)));
					}
				}
			} else {
				throw new Exception(string.Format("Unable to find node: {0}", node.ID));
			}
		}

		public void LoadChildNodes(Node node, CommonLibrary.Users.UserContext context, params object[] parameters) {
			XmlNode _node = _document.SelectSingleNode(string.Format("//page[@id='{0}']", node.ID));
			if (_node != null) {
				XmlNodeList _children = _node.ChildNodes;
				foreach (XmlNode _child in _children) {
					Node child = new Node(int.Parse(_child.Attributes["id"].Value), context);
					node.ChildNodes.Add(child);
				}
			} else {
				throw new Exception(string.Format("Unable to find node: {0}", node.ID));
			}
		}

		public void LoadParentNode(Node node, CommonLibrary.Users.UserContext context, params object[] parameters) {
			XmlNode _node = _document.SelectSingleNode(string.Format("//page[@id='{0}']", node.ID));
			if (_node != null) {
				XmlNode _parent = _node.ParentNode;
				if (_parent != null && _parent is XmlElement) {
					Node parent = new Node(int.Parse(_parent.Attributes["id"].Value), context);
					node.ParentNode = parent;
				}
			} else {
				throw new Exception(string.Format("Unable to find node: {0}", node.ID));
			}
		}

		public Node FindNodeByURL(string url, UserContext context) {
			Node _return = null;
			Hashtable hash = new Hashtable();
			URLDetail _url = new URLDetail(url);
			List<URLDetail> urls = new List<URLDetail>();

			// Find the node using the "urlLower" attribute ... this gets around the case sensitivity limitation.
			XmlNodeList pages = _document.SelectNodes(string.Format("//page[@urlLower='{0}']", _url.FullFilePath.ToLower()));
			
			if (pages.Count > 0) {
				foreach (XmlNode node in pages) {
					URLDetail tmpURL = new URLDetail(node.Attributes["url"].Value);
					string _parameters = node.Attributes["parameters"].Value;
					if (!string.IsNullOrEmpty(_parameters)) { tmpURL.Parameters = ParametersCollection.Parse(_parameters); }

					TraceManager.TraceFormat("Considering URL {0}", tmpURL.FullFilePathWithParameters);
					if (!hash.ContainsKey(tmpURL.FullFilePathWithParameters)) {
						urls.Add(tmpURL);
						hash.Add(tmpURL.FullFilePathWithParameters, node.Attributes["id"].Value);
					}
				}
			}

			if (urls.Count > 0) {
				URLDetail mostMatched = NavigationService.GetMostMatchedURL(_url, urls);
				int pageID = int.Parse(hash[mostMatched.FullFilePathWithParameters].ToString());

				_return = new Node(pageID, context);
			}

			return _return;
		}
		#endregion
	}
}