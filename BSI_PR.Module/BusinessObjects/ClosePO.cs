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
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Editors;
using System.Globalization;
using System.Web.UI.WebControls;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;


namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Closed PO Report")]
    [NavigationItem("Closed PO Report")]
    [Appearance("HideSaveButton_ClosePO", AppearanceItemType = "Action", TargetItems = "Save", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideDeleteButton_ClosePO", AppearanceItemType = "Action", TargetItems = "Delete", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideSaveAndCloseButton_ClosePO", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideSaveAndNewButton_ClosePO", AppearanceItemType = "Action", TargetItems = "SaveAndNew", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEdit_ClosePO", AppearanceItemType = "Action", TargetItems = "Edit", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideNew_ClosePO", AppearanceItemType = "Action", TargetItems = "New", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideValidate_ClosePO", AppearanceItemType = "Action", TargetItems = "Validation", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]


    public class ClosePO : XPObject//, IStateMachineProvider
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClosePO(Session session)
            : base(session)
        {
        }



        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).


            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            if (string.IsNullOrEmpty(user.CurrDept))
                user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;

            PermissionPolicyRole ReportSuperUserRole = Session.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ReportSuperUserRole')"));
            if (ReportSuperUserRole != null)
            {
                IsReportUser = true;

            }
            else
            {
                IsReportUser = false;
            }

            UserName = user.FName;
            Department = Session.FindObject<vw_ReportDepartments>(new BinaryOperator("BoCode", user.CurrDept, BinaryOperatorType.Equal));
            //YesNo = YesNo.Y;
            DocDate2 = DateTime.Today;
            DocDate3 = DateTime.Today;
            showAll = false;
        }

        


        private DateTime _DocDate2;
        [NonPersistent, RuleRequiredField()]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("From Date:"), ToolTip("Enter Text")]
        ////////////
        [Appearance("DocDate", Enabled = true)]
        ////////////////
        public DateTime DocDate2
        {
            get { return _DocDate2; }
            set
            {
                SetPropertyValue("DocDate2", ref _DocDate2, value);
            }
        }

        private DateTime _DocDate3;
        [NonPersistent, RuleRequiredField()]
        [Index(12), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("To Date:"), ToolTip("Enter Text")]
        /////////////
        [Appearance("DocDate3", Enabled = true)]
        /////////////////s
        public DateTime DocDate3
        {
            get { return _DocDate3; }
            set
            {
                SetPropertyValue("DocDate3", ref _DocDate3, value);
            }
        }

        private vw_ReportDepartments _Department;
        [ImmediatePostData]
        [NoForeignKey]
        [NonPersistent]
        [XafDisplayName("Department"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[DataSourceProperty("user.Department")]
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Department", Enabled = false, Criteria = "not IsReportUser")]    
        [DataSourceCriteria("username = '@this.UserName'")]
        //[RuleRequiredField(DefaultContexts.Save)]
        public vw_ReportDepartments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
                if (!IsLoading && value != null)
                {
                    Creator = Session.FindObject<vw_DepartmentUser>(CriteriaOperator.Parse("DepartmentCde=?", Department.BoCode.ToString()));
                }
            }
        }


        private vw_DepartmentUser _Creator;
        [ImmediatePostData]
        [NoForeignKey]
        [NonPersistent]
        [XafDisplayName("Creator"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[DataSourceProperty("user.Department")]
        [Index(24), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("Department", Enabled = false, Criteria = "not IsReportUser")]
        [DataSourceCriteria("DepartmentCde = '@this.Department.BoCode'")]
        //[RuleRequiredField(DefaultContexts.Save)]
        public vw_DepartmentUser Creator
        {
            get { return _Creator; }
            set
            {
                SetPropertyValue("Creator", ref _Creator, value);
            }
        }


        private YesNo _YesNo;
        [NonPersistent]
        [XafDisplayName("Show Details"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[DataSourceProperty("user.Department")]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Show Details", Enabled = true)]
        public YesNo YesNo
        {
            get { return _YesNo; }
            set
            {
                SetPropertyValue("YesNo", ref _YesNo, value);
            }
        }

        private bool _showAll;
        [XafDisplayName("ShowAll")]
        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Show All", Enabled = false, Criteria = "not IsReportUser")]
        public bool showAll
        {
            get { return _showAll; }
            set
            {
                SetPropertyValue("ShowAll", ref _showAll, value);
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
        [ImmediatePostData]
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