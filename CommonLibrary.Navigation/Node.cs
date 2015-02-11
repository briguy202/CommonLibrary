using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CommonLibrary.Framework;
using CommonLibrary.Users;
using CommonLibrary.Users.Roles;
using CommonLibrary.Framework.Objects;

namespace CommonLibrary.Navigation {

	public class Node {

		#region Data Members
		private Node _parentNode;
		private ContextLazyLoader<Node> _loader;
		private List<Node> _childNodes = new List<Node>();
		private RoleCollection _roles = new RoleCollection();
		private URLDetail _url;
		private string _name;
		private string _linkName;
		private int _id;
		private bool _displayLeftNav;
		private Data.INavigationDAO _dao;
		private UserContext _context;
		private int _displaySequence;
		private bool _visible;
		#endregion

		#region Properties
		internal Data.INavigationDAO DAO {
			get { return _dao; }
			set { 
				_dao = value;
				// Change the loader as well since the DAO may change
				_loader = new ContextLazyLoader<Node>(new ContextLoadDelegate<Node>(_dao.LoadNode));
			}
		}

		public ContextLazyLoader<Node> Loader {
			get { return _loader; }
		}

		public RoleCollection Roles {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context, new ContextLoadDelegate<Node>(_dao.LoadNodeRoles));
				return _roles;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_roles = value;
			}
		}

		public UserContext Context {
			get { return _context; }
		}

		public int DisplaySequence {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _displaySequence;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_displaySequence = value;
			}
		}

		public bool DisplayLeftNav {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _displayLeftNav;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_displayLeftNav = value;
			}
		}

		public int ID {
			get {
				// Do NOT call Load here - not needed as it is part of the constructor
				return _id;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_id = value;
			}
		}

		public string Name {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _name;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_name = value;
			}
		}

		public string LinkName {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return (!string.IsNullOrEmpty(_linkName)) ? _linkName : this.Name;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_linkName = value;
			}
		}

		public Node ParentNode {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context, new ContextLoadDelegate<Node>(_dao.LoadParentNode));
				return _parentNode;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_parentNode = value;
			}
		}

		public List<Node> VisibleChildNodes {
			get {
				List<Node> _return = new List<Node>();
				foreach (Node node in this.ChildNodes) {
					if (node.Visible) {
						_return.Add(node);
					}
				}
				return _return;
			}
		}

		public List<Node> ChildNodes {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context, new ContextLoadDelegate<Node>(_dao.LoadChildNodes));
				return _childNodes;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_childNodes = value;
			}
		}

		public URLDetail URL {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _url;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_url = value;
			}
		}

		public bool Visible {
			get {
				_loader.Load(MethodBase.GetCurrentMethod().Name, this, this.Context);
				return _visible;
			}
			set {
				_loader.SetLoaded(MethodBase.GetCurrentMethod().Name);
				_visible = value;
			}
		}
		#endregion

		#region Constructors
		internal Node(int id, UserContext context) {
			if (id < 0) {
				throw new ArgumentOutOfRangeException("id");
			}

			_id = id;
			_context = context;
			_dao = Data.Factory.GetNavigationDAO();
			_loader = new ContextLazyLoader<Node>(new ContextLoadDelegate<Node>(_dao.LoadNode));
		}
		#endregion

		#region Methods
		public bool IsViewPermitted(RoleCollection userRoles) {
			if (this.Roles.Count > 0 && !userRoles.HasRole(this.Roles) && !userRoles.HasRole(Role.GetByEnum(Role.eSystemRole.Admin))) {
				return false;
			}

			return true;
		}
		#endregion

	}
}