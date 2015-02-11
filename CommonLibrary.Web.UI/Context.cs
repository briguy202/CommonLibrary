//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CommonLibrary.Web.UI {
//    public static class Context {

//        #region Data Members
//        #endregion

//        #region Properties
//        /// <summary>
//        /// Gets a value indicating if the current session is an administrator session which allows for certain privileged operations.
//        /// </summary>
//        public bool IsAdministrator {
//            get { return _isAdministrator; }
//            protected set { _isAdministrator = value; }
//        }

//        /// <summary>
//        /// Gets or sets a value indicating if metrics should be gathered on this page.
//        /// </summary>
//        public bool IsMetricsEnabled {
//            get { return (_isMetricsEnabled && this.Server.MachineName != ConfigurationBase.GetConfigString("LocalMachineName")); }
//            protected set { _isMetricsEnabled = value; }
//        }

//        /// <summary>
//        /// Gets a value indicating if tracing will be displayed on the bottom of the page.
//        /// </summary>
//        public bool IsTracingEnabled {
//            get { return _isTracingEnabled; }
//            protected set { _isTracingEnabled = value; }
//        }

//        /// <summary>
//        /// Gets or sets a value indicating if protective rendering should be used to mask the site.
//        /// </summary>
//        public bool IsProtectedRenderingMode {
//            get { return _isProtectedRenderingMode; }
//            protected set { _isProtectedRenderingMode = value; }
//        }

//        /// <summary>
//        /// Gets a value indicating if the current user is authenticated on the site.
//        /// </summary>
//        public bool IsAuthenticated {
//            get { return User.Identity.IsAuthenticated; }
//        }

//        /// <summary>
//        /// Gets or sets a value indicating if the page will halt before redirecting to a destination, giving the testing user
//        /// a chance to see where the redirect will go.
//        /// </summary>
//        public bool IsRedirectionEnabled {
//            get { return _isRedirectionEnabled; }
//            protected set { _isRedirectionEnabled = value; }
//        }
//        #endregion

//        #region Constructors
//        #endregion

//        #region Methods
//        #endregion
					
//    }
//}