using BSI_PR.Module;
using BSI_PR.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static System.Threading.Mutex mutex = null;
        [STAThread]
        static void Main()
        {
            DevExpress.ExpressApp.FrameworkSettings.DefaultSettingsCompatibilityMode = DevExpress.ExpressApp.FrameworkSettingsCompatibilityMode.v20_1;
            const string appName = "Integration";
            bool createdNew;

            mutex = new System.Threading.Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Application.Exit();
                return;
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            RegisterEntities();
            AuthenticationStandard authentication = new AuthenticationStandard();
            SecurityStrategyComplex security = new SecurityStrategyComplex(typeof(PermissionPolicyUser), typeof(PermissionPolicyRole), authentication);
            //security.RegisterXPOAdapterProviders();
            security.AllowAnonymousAccess = true;
            string connectionString = ConfigurationManager.ConnectionStrings["DataSourceConnectionString"].ConnectionString;
            IObjectSpaceProvider objectSpaceProvider = new SecuredObjectSpaceProvider(security, connectionString, null);

            #region Allow Store Proc
            ((SecuredObjectSpaceProvider)objectSpaceProvider).AllowICommandChannelDoWithSecurityContext = true;
            #endregion

            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Integration mainForm = new Integration(security, objectSpaceProvider);

            //mainForm.defuserid = ConfigurationManager.AppSettings["DataSourceUserID"].ToString();
            //mainForm.defpassword = ConfigurationManager.AppSettings["DataSourcePassword"].ToString();
            string temp = ConfigurationManager.AppSettings["AutoPostAfterLogin"].ToString().ToUpper();
            if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                mainForm.autopostafterlogin = true;
            else
                mainForm.autopostafterlogin = false;

            temp = "";
            temp = ConfigurationManager.AppSettings["AutoLogin"].ToString().ToUpper();
            if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                mainForm.autologin = true;
            else
                mainForm.autologin = false;

            Application.Run(mainForm);
        }

        private static void RegisterEntities()
        {
            XpoTypesInfoHelper.GetXpoTypeInfoSource();

            XafTypesInfo.Instance.RegisterEntity(typeof(APInvoice));
            XafTypesInfo.Instance.RegisterEntity(typeof(APInvoiceDetails));
            XafTypesInfo.Instance.RegisterEntity(typeof(APInvoiceAttachment));
            XafTypesInfo.Instance.RegisterEntity(typeof(APInvoiceDocStatues));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrder));
            XafTypesInfo.Instance.RegisterEntity(typeof(GeneralSettings));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_costcenter));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_Currency));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_DepartmentUser));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_GRN));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_GRNInv));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_InvoiceBudget));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_items));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_ItemSeries));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_PaymentTerms));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_PO));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_POBudget));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_POInv));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_PR));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_RecvPO));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_ReportDepartments));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_SAPPostingPeriod));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_taxes));
            XafTypesInfo.Instance.RegisterEntity(typeof(vw_vendors));

            XafTypesInfo.Instance.RegisterEntity(typeof(SystemUsers));
            XafTypesInfo.Instance.RegisterEntity(typeof(PermissionPolicyUser));
            XafTypesInfo.Instance.RegisterEntity(typeof(PermissionPolicyRole));
        }
    }
}
