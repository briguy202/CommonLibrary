using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Users;
using System.Xml;

namespace CommonLibrary.Navigation.Data {
	public interface INavigationDAO {
		XmlDocument GenerateXMLNodeTree(string savePath, UserContext context);
		void LoadNode(Node node, UserContext context, params object[] parameters);
		void LoadNodeRoles(Node node, UserContext context, params object[] parameters);
		void LoadChildNodes(Node node, UserContext context, params object[] parameters);
		void LoadParentNode(Node node, UserContext context, params object[] parameters);
		Node FindNodeByURL(string url, UserContext context);
	}
}