using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Web.UI.ResourceBundler;
using System.Globalization;
using System.Web;

namespace CommonLibrary.Web.UI.YUI {
	public static class YUIHelper {

		#region Methods
		private static class Includes {
			internal static void Animation() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/animation{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void Button() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/button{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void Calendar() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/calendar{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void Container() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/container{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void DataSource() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/datasource{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void DataTable() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/datatable{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void DOMEvent() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/yahoo-dom-event.js", false), typeof(YUIHelper).Assembly, true); }
			internal static void DragAndDrop() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/dragdrop{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void Element() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/element-beta{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void Menu() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/menu{0}.js"), typeof(YUIHelper).Assembly, true); }
			internal static void RichTextEditor() { Bundler.RegisterFile(YUIHelper.GetFilePath("/YUI/YUIElements/editor{0}.js"), typeof(YUIHelper).Assembly, true); }
		}

		private static string GetFilePath(string formattedFilePath) {
			return YUIHelper.GetFilePath(formattedFilePath, true);
		}

		private static string GetFilePath(string formattedFilePath, bool canMinify) {
			string filepath = formattedFilePath;

			bool doMinification = canMinify;
			if (HttpContext.Current.Handler is CommonLibrary.Web.UI.Page) {
				CommonLibrary.Web.UI.Page page = (CommonLibrary.Web.UI.Page)HttpContext.Current.Handler;
				if (page.IsAdministrator) {
					doMinification = false;
				}
			}

			if (canMinify) {
				if (doMinification) {
					filepath = String.Format(CultureInfo.InvariantCulture, filepath, "-min");
				} else {
					filepath = String.Format(CultureInfo.InvariantCulture, filepath, string.Empty);
				}
			}

			return filepath;
		}
		#endregion

		#region Component Includes
		public static class Components {
			public static void RichTextEditor(bool includeDefaultCss) {
				YUIHelper.Includes.DOMEvent();
				YUIHelper.Includes.Element();
				YUIHelper.Includes.Container();
				YUIHelper.Includes.Menu();
				YUIHelper.Includes.Button();
				YUIHelper.Includes.RichTextEditor();
				if (includeDefaultCss) {
					if (HttpContext.Current.Handler is CommonLibrary.Web.UI.Page) {
						CommonLibrary.Web.UI.Page page = (CommonLibrary.Web.UI.Page)HttpContext.Current.Handler;
						page.AddCSS("/Assets/YUI/skin.css", "all");
					}
				}
			}

			public static void DataTable(bool enableCalendarEditor, bool enableColumnResizeReorder, bool includeDefaultCss) {
				YUIHelper.Includes.DOMEvent();
				YUIHelper.Includes.Element();
				YUIHelper.Includes.DataSource();
				YUIHelper.Includes.DataTable();
				if (enableCalendarEditor) { YUIHelper.Includes.Calendar(); }
				if (enableColumnResizeReorder) { YUIHelper.Includes.DragAndDrop(); }
				if (includeDefaultCss) {
					if (HttpContext.Current.Handler is CommonLibrary.Web.UI.Page) {
						CommonLibrary.Web.UI.Page page = (CommonLibrary.Web.UI.Page)HttpContext.Current.Handler;
						page.AddCSS("/Assets/YUI/datatable.css", "all");
					}
				}
			}

			public static void Animation() {
				YUIHelper.Includes.DOMEvent();
				YUIHelper.Includes.Animation();
			}

			public static void Panel() {
				YUIHelper.Includes.DOMEvent();
				YUIHelper.Includes.Animation();
				YUIHelper.Includes.DragAndDrop();
				YUIHelper.Includes.Container();
			}

			public static void Button(bool isMenuOrSplitButton, bool includeDefaultCss) {
				YUIHelper.Includes.DOMEvent();
				YUIHelper.Includes.Element();
				YUIHelper.Includes.Button();

				if (isMenuOrSplitButton) {
					YUIHelper.Includes.Container();
					YUIHelper.Includes.Menu();
				}

				if (includeDefaultCss) {
					if (HttpContext.Current.Handler is CommonLibrary.Web.UI.Page) {
						CommonLibrary.Web.UI.Page page = (CommonLibrary.Web.UI.Page)HttpContext.Current.Handler;
						Bundler.RegisterFile("/YUI/CSS/button.css", typeof(YUIHelper).Assembly);
						if (isMenuOrSplitButton) {
							Bundler.RegisterFile("/YUI/CSS/menu.css", typeof(YUIHelper).Assembly);
						}
					}
				}
			}
		}
		#endregion

	}
}