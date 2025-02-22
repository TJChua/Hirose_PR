﻿using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;

namespace BSI_PR.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            SecurityAdapterHelper.Enable();
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if EASYTEST
            DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            Tracing.Initialize();
            WebApplication.SetInstance(Session, new BSI_PRAspNetApplication());
            DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContentNew.ClearSizeLimit();
            WebApplication.Instance.SwitchToNewStyle();

            #region GeneralSettings
            string temp = "";

            temp = ConfigurationManager.AppSettings["EmailSend"].ToString();
            BSI_PR.Module.GeneralSettings.EmailSend = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                BSI_PR.Module.GeneralSettings.EmailSend = true;

            BSI_PR.Module.GeneralSettings.EmailHost = ConfigurationManager.AppSettings["EmailHost"].ToString();
            BSI_PR.Module.GeneralSettings.EmailHostDomain = ConfigurationManager.AppSettings["EmailHostDomain"].ToString();
            BSI_PR.Module.GeneralSettings.EmailPort = ConfigurationManager.AppSettings["EmailPort"].ToString();
            BSI_PR.Module.GeneralSettings.Email = ConfigurationManager.AppSettings["Email"].ToString();
            BSI_PR.Module.GeneralSettings.EmailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
            BSI_PR.Module.GeneralSettings.EmailName = ConfigurationManager.AppSettings["EmailName"].ToString();

            temp = ConfigurationManager.AppSettings["EmailSSL"].ToString();
            BSI_PR.Module.GeneralSettings.EmailSSL = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                BSI_PR.Module.GeneralSettings.EmailSSL = true;

            temp = ConfigurationManager.AppSettings["EmailUseDefaultCredential"].ToString();
            BSI_PR.Module.GeneralSettings.EmailUseDefaultCredential = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                BSI_PR.Module.GeneralSettings.EmailUseDefaultCredential = true;

            BSI_PR.Module.GeneralSettings.DeliveryMethod = ConfigurationManager.AppSettings["DeliveryMethod"].ToString();

            temp = ConfigurationManager.AppSettings["B1Post"].ToString();
            BSI_PR.Module.GeneralSettings.B1Post = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                BSI_PR.Module.GeneralSettings.B1Post = true;

            BSI_PR.Module.GeneralSettings.B1UserName = ConfigurationManager.AppSettings["B1UserName"].ToString();
            BSI_PR.Module.GeneralSettings.B1Password = ConfigurationManager.AppSettings["B1Password"].ToString();
            BSI_PR.Module.GeneralSettings.B1Server = ConfigurationManager.AppSettings["B1Server"].ToString();
            BSI_PR.Module.GeneralSettings.B1CompanyDB = ConfigurationManager.AppSettings["B1CompanyDB"].ToString();
            BSI_PR.Module.GeneralSettings.B1License = ConfigurationManager.AppSettings["B1License"].ToString();
            BSI_PR.Module.GeneralSettings.B1DbServerType = ConfigurationManager.AppSettings["B1DbServerType"].ToString();
            BSI_PR.Module.GeneralSettings.B1Language = ConfigurationManager.AppSettings["B1Language"].ToString();
            BSI_PR.Module.GeneralSettings.B1DbUserName = ConfigurationManager.AppSettings["B1DbUserName"].ToString();
            BSI_PR.Module.GeneralSettings.B1DbPassword = ConfigurationManager.AppSettings["B1DbPassword"].ToString();
            BSI_PR.Module.GeneralSettings.B1AttachmentPath = ConfigurationManager.AppSettings["B1AttachmentPath"].ToString();
            BSI_PR.Module.GeneralSettings.ReportPath = ConfigurationManager.AppSettings["ReportPath"].ToString();
            BSI_PR.Module.GeneralSettings.ReportDB = ConfigurationManager.AppSettings["ReportDB"].ToString();






            BSI_PR.Module.GeneralSettings.B1APPRseries = int.Parse(ConfigurationManager.AppSettings["B1APPRseries"].ToString());

            //if (System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Substring(7, 9) == "localhost")
            //{
            //    //local
            //    //BSI_PR.Module.GeneralSettings.appurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, 46);
            //    BSI_PR.Module.GeneralSettings.appurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, 33); // + requestManager.GetQueryString(shortcut)
            //}
            //else
            //{
            //    //their server
            //    BSI_PR.Module.GeneralSettings.appurl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, 74); // + requestManager.GetQueryString(shortcut)
            //}

            BSI_PR.Module.GeneralSettings.appurl = System.Web.HttpContext.Current.Request.Url.Scheme + "://" +
                System.Web.HttpContext.Current.Request.Url.Authority + System.Web.HttpContext.Current.Request.Url.Segments[0] +
                System.Web.HttpContext.Current.Request.Url.Segments[1];

            BSI_PR.Module.GeneralSettings.defdept = ConfigurationManager.AppSettings["DefDept"].ToString();
            #endregion
            WebApplication.Instance.CustomizeFormattingCulture += Instance_CustomizeFormattingCulture;
            WebApplication.Instance.LoggedOn += Instance_LoggedOn;

            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached && WebApplication.Instance.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                WebApplication.Instance.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
#endif
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }
        private void Instance_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e)
        {

            e.FormattingCulture.NumberFormat.CurrencySymbol = "RM";
            e.FormattingCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        }

        private void Instance_LoggedOn(object sender, LogonEventArgs e)
        {
            try
            {
                BSI_PR.Module.BusinessObjects.SystemUsers user = (BSI_PR.Module.BusinessObjects.SystemUsers)SecuritySystem.CurrentUser;
                user.CurrDept = "";
                user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;

                if (string.IsNullOrEmpty(user.CurrDept))
                {
                    WebApplication.LogOff(Session);
                    WebApplication.DisposeInstance(Session);
                }
            }
            catch (Exception ex)
            {
                WebApplication.LogOff(Session);
                WebApplication.DisposeInstance(Session);
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e) {
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
