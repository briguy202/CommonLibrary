using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Framework.Objects;
using CommonLibrary.Business.Data;

namespace CommonLibrary.Business {
	public class Content : EditableBase {

		#region Data Members
		private int _id;
		private string _text;
		private static IContentDAO _dao;
		#endregion

		#region Properties
		public string Text {
			get { return _text; }
			set { _text = value; }
		}

		public int ID {
			get { return _id; }
			private set { _id = value; }
		}
	
		#endregion

		#region Constructors
		internal Content(int id) : base() {
			this.ID = id;
		}

		static Content() {
			_dao = DAOFactory.GetContentDAO();
		}
		#endregion

		#region Methods
		public static Content GetContentByID(int id) {
			return _dao.GetContentByID(id);
		}

		protected override void OnSave() {
			base.Save();
			_dao.Edit(this);
		}
		#endregion
					
	}
}