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
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Budgets : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Budgets(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        protected override void OnSaving()
        {
            base.OnSaving();
            BudgetYear = DateFrom.Year;
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

        private int _BudgetYear;
        [Browsable(false)]
        public int BudgetYear
        {
            get { return _BudgetYear; }
            set
            {
                SetPropertyValue("BudgetYear", ref _BudgetYear, value);
            }
        }

        [RuleUniqueValue]
        [XafDisplayName("DepDate From")]
        public string BudgetDate
        {
            get
            {
                string rtn = "";
                if (BudgetMaster != null)
                {
                    rtn = BudgetMaster.BoCode + DateFrom.ToShortDateString();
                }
                return rtn;
            }
        }

        [RuleUniqueValue]
        [XafDisplayName("DepDate To")]
        public string BudgetDateTo
        {
            get
            {
                string rtn = "";
                if (BudgetMaster != null)
                {
                    rtn = BudgetMaster.BoCode + DateTo.ToShortDateString();
                }
                return rtn;
            }
        }

        private DateTime _DateFrom;
        [XafDisplayName("Date From"), ToolTip("Enter Date")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("DateFrom", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set
            {
                SetPropertyValue("DateFrom", ref _DateFrom, value);
            }
        }

        private DateTime _DateTo;
        [XafDisplayName("Date To"), ToolTip("Enter Date")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleUniqueValue]
        [Appearance("DateTo", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public DateTime DateTo
        {
            get { return _DateTo; }
            set
            {
                SetPropertyValue("DateTo", ref _DateTo, value);
            }
        }

        private decimal _Amount;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Amount")]
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }

        private decimal _AllocatedAmount;
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Allocated Amount")]
        [Appearance("AllocatedAmount", Enabled = false)]
        public decimal AllocatedAmount
        {
            get { return _AllocatedAmount; }
            set
            {
                SetPropertyValue("AllocatedAmount", ref _AllocatedAmount, value);
            }
        }

        private BudgetMasters _BudgetMaster;
        [Association("BudgetMasters-Budgets")]
        [Index(101), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("BudgetMaster", Enabled = false)]
        public BudgetMasters BudgetMaster
        {
            get { return _BudgetMaster; }
            set { SetPropertyValue("BudgetMaster", ref _BudgetMaster, value); }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

    }
}