using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary.Framework.Data;
using CommonLibrary.Users;
using CommonLibrary.Users.Roles;
using System.Xml;
using System.Collections;
using CommonLibrary.Framework.Objects;

namespace CommonLibrary.Navigation.Data {
	public class NavigationSQLDAO : INavigationDAO {

		#region INavigationDAO Members
		public void LoadNode(Node node, UserContext context, params object[] parameters) {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00001_PAGE_GET");
			operation.Parameters.Add(new DataParameter("@pageID", node.ID, DbType.Int32));

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				if (result.HasData) {
					node.ID = result.GetInteger("page_id");
					node.Name = result.GetString("name");
					node.LinkName = result.GetString("link_name");
					node.URL = new URLDetail(result.GetString("url"));
					node.DisplayLeftNav = result.GetBoolean("display_left_nav");
					node.DisplaySequence = result.GetInteger("display_sequence", 999);
					node.Visible = result.GetBoolean("visible", true);
				}
			}
		}

		public void LoadNodeRoles(Node node, UserContext context, params object[] parameters) {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00002_PAGE_ROLE_LIST_GET");
			operation.Parameters.Add(new DataParameter("@pageID", node.ID, DbType.Int32));

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				while (result.HasData) {
					node.Roles.Add(new Role(result.GetInteger("role_id")));

					result.MoveNext();
				}
			}
		}

		public void LoadChildNodes(Node node, UserContext context, params object[] parameters) {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00001_PAGE_CHILD_GET");
			operation.Parameters.Add(new DataParameter("@pageID", node.ID, DbType.Int32));

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				while (result.HasData) {
					Node child = new Node(result.GetInteger("page_id"), context);
					node.ChildNodes.Add(child);

					result.MoveNext();
				}
			}
		}

		public void LoadParentNode(Node node, UserContext context, params object[] parameters) {
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00001_PAGE_PARNT_GET");
			operation.Parameters.Add(new DataParameter("pageID", node.ID, DbType.Int32));

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				if (result.HasData) {
					Node parent = new Node(result.GetInteger("page_id"), context);
					node.ParentNode = parent;
				}
			}
		}

		public XmlDocument GenerateXMLNodeTree(string savePath, UserContext context) {
			XmlDocument _return = new XmlDocument();
			Node rootNode = null;

			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00001_PAGE_ROOT_GET");

			IResultSet result = access.ExecuteResultSet(operation);
			using (result) {
				if (result.HasData) {
					rootNode = new Node(result.GetInteger("page_id"), context);
					XmlTextWriter writer = new XmlTextWriter(savePath, Encoding.UTF8);

					writer.WriteStartDocument();
					this.RecursivelyWriteNodes(rootNode, writer);
					writer.Close();

					_return.Load(savePath);
					return _return;
				}
			}

			throw new Exception("Could not find root navigation node");
		}

		public Node FindNodeByURL(string url, UserContext context) {
			Node _return = null;
			IDataAccess access = DataFactory.GetDataAccess();
			DataOperation operation = new DataOperation("dbo.SP_00001_PAGE_ID_GET");
			URLDetail _url = new URLDetail(url);
			operation.Parameters.Add(new DataParameter("@url", _url.FullFilePath, DbType.String));

			IResultSet result = access.ExecuteResultSet(operation);
			Hashtable hash = new Hashtable();
			List<URLDetail> urls = new List<URLDetail>();

			using (result) {
				while (result.HasData) {
					string tmpURL = result.GetString("url");
					if (!hash.ContainsKey(tmpURL)) {
						urls.Add(new URLDetail(tmpURL));
						hash.Add(tmpURL, result.GetInteger("page_id"));
					}

					result.MoveNext();
				}
			}

			if (urls.Count > 0) {
				URLDetail mostMatched = NavigationService.GetMostMatchedURL(_url, urls);
				int pageID = (int)hash[mostMatched.FullFilePathWithParameters];

				_return = new Node(pageID, context);
			}

			return _return;
		}
		#endregion

		#region Private Methods
		private void RecursivelyWriteNodes(List<Node> pageNodes, XmlTextWriter writer) {
			foreach (Node node in pageNodes) {
				this.RecursivelyWriteNodes(node, writer);
			}
		}

		private void RecursivelyWriteNodes(Node pageNode, XmlTextWriter writer) {
			pageNode.DAO = new Data.NavigationSQLDAO();

			// The "urlLower" attribute gets around the case sensitivity of XPath queries by duplicating the URL, but making all
			// characters lowercase.  We use this value to find the node, but use the "url" attribute to display or use the value.
			// This gives us the flexibility of being able to use different casing on the value, while being able to find the node
			// with case insensitivity.
			writer.WriteStartElement("page");
			writer.WriteAttributeString("id", pageNode.ID.ToString());
			writer.WriteAttributeString("url", pageNode.URL.FullFilePath);
			writer.WriteAttributeString("urlLower", pageNode.URL.FullFilePath.ToLower());
			writer.WriteAttributeString("parameters", pageNode.URL.Parameters.ToString());
			writer.WriteAttributeString("name", pageNode.Name);
			writer.WriteAttributeString("linkName", pageNode.LinkName);
			writer.WriteAttributeString("displayLeftNav", pageNode.DisplayLeftNav.ToString());
			writer.WriteAttributeString("displaySequence", pageNode.DisplaySequence.ToString());
			writer.WriteAttributeString("visible", pageNode.Visible.ToString());

			if (pageNode.Roles.Count > 0) {
				string[] roleIDs = new string[pageNode.Roles.Count];
				for (int i = 0; i < pageNode.Roles.Count; i++) {
					roleIDs[i] = pageNode.Roles[i].ID.ToString();
				}
				writer.WriteAttributeString("roles", String.Join(",", roleIDs));
			} else {
				writer.WriteAttributeString("roles", string.Empty);
			}

			if (pageNode.ChildNodes.Count > 0) {
				this.RecursivelyWriteNodes(pageNode.ChildNodes, writer);
			}

			writer.WriteEndElement();
		}
		#endregion

	}
}