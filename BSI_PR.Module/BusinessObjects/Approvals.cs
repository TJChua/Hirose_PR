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

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    [NavigationItem("Setup")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [RuleCriteria("ApprovalsDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [RuleCriteria("ApprovalsSaveRule", DefaultContexts.Save, "IsValid", "Cannot Save.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Approvals : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Approvals(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsWithinBudget = false;
            IsActive = false;
            ApprovalCnt = 1;
            ApprovalSQL = "";
            DocAmount = 0;
            AppType = ApprovalTypes.Document;
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue("PersistentProperty", ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}

        private string _BoCode;
        [XafDisplayName("Code"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleUniqueValue]
        [Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public string BoCode
        {
            get { return _BoCode; }
            set
            {
                SetPropertyValue("BoCode", ref _BoCode, value);
            }
        }

        private string _BoName;
        [XafDisplayName("Name"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string BoName
        {
            get { return _BoName; }
            set
            {
                SetPropertyValue("BoName", ref _BoName, value);
            }
        }

        [Index(2), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return BoCode + "-" + BoName; }
        }

        private DocTypes _DocType;
        [XafDisplayName("Document Type"), ToolTip("Select Document")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [Index(3)]
        public DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private ApprovalTypes _AppType;
        [ImmediatePostData]
        [XafDisplayName("Approval Type"), ToolTip("Select Type")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("AppType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(4)]
        public ApprovalTypes AppType
        {
            get { return _AppType; }
            set
            {
                if (SetPropertyValue("AppType", ref _AppType, value))
                {
                    if (!IsLoading)
                    {
                        SetPropertyValue("DocAmount", ref _DocAmount, 0);
                        SetPropertyValue("ApprovalSQL", ref _ApprovalSQL, "");
                    }
                }
            }
        }
        private int _ApprovalCnt;
        [XafDisplayName("Number of Approval"), ToolTip("Enter Number")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(5)]
        public int ApprovalCnt
        {
            get { return _ApprovalCnt; }
            set
            {
                SetPropertyValue("ApprovalCnt", ref _ApprovalCnt, value);
            }
        }

        private string _ApprovalLevel;
        [XafDisplayName("Approval Level"), ToolTip("Enter Number")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [RuleUniqueValue]
        [Index(6)]
        public string ApprovalLevel
        {
            get { return _ApprovalLevel; }
            set
            {
                SetPropertyValue("ApprovalLevel", ref _ApprovalLevel, value);
            }
        }
        private decimal _DocAmount;
        [XafDisplayName("Document Amount"), ToolTip("Enter Number")]
        [Appearance("DocAmount", Enabled = false, Criteria = "not IsAllowedDocAmount")]
        [Index(7)]
        public decimal DocAmount
        {
            get { return _DocAmount; }
            set
            {
                SetPropertyValue("DocAmount", ref _DocAmount, value);
            }
        }
        [Browsable(false)]
        public bool IsAllowedDocAmount
        {
            get
            {
                if (AppType == ApprovalTypes.Document)
                    return true;
                return false;
            }
        }

        private bool _IsActive;
        [XafDisplayName("Active")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        private bool _IsWithinBudget;
        [XafDisplayName("Within Budget?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(30)]
        public bool IsWithinBudget
        {
            get { return _IsWithinBudget; }
            set
            {
                SetPropertyValue("IsWithinBudget", ref _IsWithinBudget, value);
            }
        }

        private string _ApprovalSQL;
        [XafDisplayName("Approval SQL"), ToolTip("Enter Text")]
        [Appearance("ApprovalSQL", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsSQL")]
        [Index(40)]
        public string ApprovalSQL
        {
            get { return _ApprovalSQL; }
            set
            {
                SetPropertyValue("ApprovalSQL", ref _ApprovalSQL, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsBudget
        {
            get
            {
                if (AppType == ApprovalTypes.Budget)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsSQL
        {
            get
            {
                if (AppType == ApprovalTypes.SQL)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (AppType == ApprovalTypes.Budget && BudgetMaster != null && DocAmount <= 0)
                    return true;

                if (AppType == ApprovalTypes.Document && DocAmount > 0)
                    return true;

                if (AppType == ApprovalTypes.SQL && BudgetMaster != null && DocAmount <= 0 && !string.IsNullOrEmpty(ApprovalSQL))
                    return true;

                return false;
            }
        }
        [Association("ApprovalTriggers")]
        [XafDisplayName("Trigger User")]
        public XPCollection<SystemUsers> TriggerUser
        {
            get { return GetCollection<SystemUsers>("TriggerUser"); }
        }
        [Association("ApprovalUsers")]
        [XafDisplayName("Approve User")]
        public XPCollection<SystemUsers> ApprovalUser
        {
            get { return GetCollection<SystemUsers>("ApprovalUser"); }
        }
        [Association("ApprovalBudgetMasters")]
        [XafDisplayName("Budget Master")]
        [Appearance("BudgetMaster", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsBudget")]
        public XPCollection<BudgetMasters> BudgetMaster
        {
            get { return GetCollection<BudgetMasters>("BudgetMaster"); }
        }
    }
}