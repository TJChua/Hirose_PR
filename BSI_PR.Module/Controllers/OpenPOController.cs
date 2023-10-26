using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSI_PR.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using CrystalDecisions.CrystalReports.Engine;
using System.Web;
using DevExpress.XtraReports.Configuration;
using System.Configuration;
using CrystalDecisions.Shared;
using System.IO;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Web;
using System.Web.UI;

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class OpenPOController : ViewController
    {
        GenControllers genCon;
        public OpenPOController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(OpenPO);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }

     
        private void OpenPOController_ViewEditModeChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenControllers>();
        }
        protected override void OnDeactivated()
        {
            if (View.GetType() == typeof(DetailView))
            {
                ((DetailView)View).ViewEditModeChanged -= OpenPOController_ViewEditModeChanged;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        protected string GetScript(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            //To View PDF
            return @"var newWindow = window.open('about:blank', '_blank');
            newWindow.document.write('<iframe src=""Download.aspx?filename=" + fileInfo.Name + @""" frameborder =""0"" allowfullscreen style=""width: 100%;height: 100%""></iframe>');
            ";
        }

        protected string GetScriptExcel(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            //To View PDF
            //return @"var newWindow = window.open('about:blank', '_blank');
            return @"var newWindow = window.open('');
            newWindow.document.write('<iframe src=""Download2.aspx?filename=" + fileInfo.Name + @""" frameborder =""0"" allowfullscreen style=""width: 100%;height: 100%""></iframe>');
            newWindow.Close();
            ";

        }

        public void MsgBox(string PopUpMsg)
        {
            try
            {
                MessageOptions options = new MessageOptions();
                options.Duration = 7000;
                options.Message = string.Format(PopUpMsg);
                options.Type = InformationType.Error;
                options.Web.Position = InformationPosition.Right;
                options.Win.Caption = "Error";
                options.Win.Type = WinMessageType.Flyout;
                Application.ShowViewStrategy.ShowMessage(options);
            }
            catch
            {
                throw new Exception(PopUpMsg);
            }
        }

        private void OpenPO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                OpenPO OpenPO = (OpenPO)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    if (OpenPO.showAll == true)
                    {
                        
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\OpenPOAll.rpt"));
                        strServer = GeneralSettings.B1Server;
                        strDatabase = GeneralSettings.B1CompanyDB;
                        strUserID = GeneralSettings.B1DbUserName;
                        strPwd = GeneralSettings.B1DbPassword;
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.SetParameterValue("Date", OpenPO.DocDate2.ToString("yyyyMMdd"));
                        doc.SetParameterValue("status", OpenPO.POSTATUS.ToString());
                    }
                    else
                    {
                      
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\OpenPO.rpt"));
                        strServer = GeneralSettings.B1Server;
                        strDatabase = GeneralSettings.B1CompanyDB;
                        strUserID = GeneralSettings.B1DbUserName;
                        strPwd = GeneralSettings.B1DbPassword;
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.SetParameterValue("Date", OpenPO.DocDate2.ToString("yyyyMMdd"));
                        doc.SetParameterValue("Department", OpenPO.Department.BoCode.ToString());
                        doc.SetParameterValue("status", OpenPO.POSTATUS.ToString());


                    }
                   
                    //doc.SetParameterValue("Detail", Budget.YesNo.ToString());

                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_OpenPO_" + user.UserName + "_"
                       + DateTime.Parse(OpenPO.DocDate2.ToString()).ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + ".pdf";



                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();
                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", GetScript(filename));
                }
                catch (Exception ex)
                {
                    MsgBox(ex.Message);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No Open PO Available", InformationType.Error);
            }
        }

        private void ExportExcelOpenPO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                OpenPO OpenPO = (OpenPO)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    if (OpenPO.showAll == true)
                    {

                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\OpenPOAll.rpt"));
                        strServer = GeneralSettings.B1Server;
                        strDatabase = GeneralSettings.B1CompanyDB;
                        strUserID = GeneralSettings.B1DbUserName;
                        strPwd = GeneralSettings.B1DbPassword;
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.SetParameterValue("Date", OpenPO.DocDate2.ToString("yyyyMMdd"));
                        doc.SetParameterValue("status", OpenPO.POSTATUS.ToString());
                    }
                    else
                    {

                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\OpenPO.rpt"));
                        strServer = GeneralSettings.B1Server;
                        strDatabase = GeneralSettings.B1CompanyDB;
                        strUserID = GeneralSettings.B1DbUserName;
                        strPwd = GeneralSettings.B1DbPassword;
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.SetParameterValue("Date", OpenPO.DocDate2.ToString("yyyyMMdd"));
                        doc.SetParameterValue("Department", OpenPO.Department.BoCode.ToString());
                        doc.SetParameterValue("status", OpenPO.POSTATUS.ToString());


                    }

                    //doc.SetParameterValue("Detail", Budget.YesNo.ToString());

                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_OpenPO_" + user.UserName + "_"
                       + DateTime.Parse(OpenPO.DocDate2.ToString()).ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + ".xls";



                    doc.ExportToDisk(ExportFormatType.Excel, filename);
                    doc.Close();
                    doc.Dispose();
                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", GetScriptExcel(filename));
                }
                catch (Exception ex)
                {
                    MsgBox(ex.Message);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No Open PO Available", InformationType.Error);
            }
        }
    }
}
