using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Business.Data {
	internal interface IContentDAO {
		void Edit(Content content);
		Content GetContentByID(int id);
	}
}