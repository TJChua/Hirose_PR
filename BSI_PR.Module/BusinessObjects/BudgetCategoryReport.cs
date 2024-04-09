using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("PO Budget Report")]
    [NavigationItem("Reports")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSave", AppearanceItemType = "Action", TargetItems = "Save", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    public class BudgetCategoryReport : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public BudgetCategoryReport(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            PermissionPolicyRole ReportSuperUserRole = Session.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ReportSuperUserRole')"));
            if (ReportSuperUserRole != null)
            {
                IsReportUser = true;
                UserName = "All";

            }
            else
            {
                IsReportUser = false;
                UserName = user.FName;
            }

            Department = Session.FindObject<vw_ReportDepartments>(new BinaryOperator("BoCode", user.CurrDept, BinaryOperatorType.Equal));
            BudgetCategory = Session.FindObject<vw_BudgetCategory>(CriteriaOperator.Parse("BoCode = ?", "All"));
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;
        }

        private vw_ReportDepartments _Department;
        [ImmediatePostData]
        [NoForeignKey]
        [NonPersistent]
        [XafDisplayName("Department"), ToolTip("Enter Text")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("Department", Enabled = false, Criteria = "not IsReportUser")]
        [DataSourceCriteria("username = '@this.UserName'")]
        //[RuleRequiredField(DefaultContexts.Save)]
        public vw_ReportDepartments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private DateTime _FromDate;
        [NonPersistent, RuleRequiredField()]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("From Date:"), ToolTip("Enter Text")]
        public DateTime FromDate
        {
            get { return _FromDate; }
            set
            {
                SetPropertyValue("FromDate", ref _FromDate, value);
            }
        }

        private DateTime _ToDate;
        [NonPersistent, RuleRequiredField()]
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("To Date:"), ToolTip("Enter Text")]
        public DateTime ToDate
        {
            get { return _ToDate; }
            set
            {
                SetPropertyValue("ToDate", ref _ToDate, value);
            }
        }

        private vw_BudgetCategory _BudgetCategory;
        [NoForeignKey]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vw_BudgetCategory BudgetCategory
        {
            get { return _BudgetCategory; }
            set
            {
                SetPropertyValue("BudgetCategory", ref _BudgetCategory, value);
            }
        }

        private bool _SummaryDetails;
        [XafDisplayName("SummaryDetails")]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public bool SummaryDetails
        {
            get { return _SummaryDetails; }
            set
            {
                SetPropertyValue("SummaryDetails", ref _SummaryDetails, value);
            }
        }

        private bool _IsReportUser;
        [ImmediatePostData]
        [Browsable(false), NonPersistent]
        [XafDisplayName("IsReportUser")]
        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("IsReportUser", Enabled = true)]
        public bool IsReportUser
        {
            get { return _IsReportUser; }
            set
            {
                SetPropertyValue("IsReportUser", ref _IsReportUser, value);
            }
        }

        private string _UserName;
        [NonPersistent]
        [XafDisplayName("UserName")]
        [Index(28), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("UserName", Enabled = true)]
        public string UserName
        {
            get { return _UserName; }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }
    }
}