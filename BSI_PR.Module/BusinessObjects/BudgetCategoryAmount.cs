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
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    public class BudgetCategoryAmount : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public BudgetCategoryAmount(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private CategoryMonth _Month;
        [XafDisplayName("Month")]
        [Appearance("Month", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public CategoryMonth Month
        {
            get { return _Month; }
            set
            {
                SetPropertyValue("Month", ref _Month, value);
            }
        }

        private string _Year;
        [XafDisplayName("Year")]
        [Appearance("Year", Enabled = false, Criteria = "not IsNew")]
        [ModelDefault("EditMask", "0000")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string Year
        {
            get { return _Year; }
            set
            {
                SetPropertyValue("Year", ref _Year, value);
            }
        }

        private decimal _Budget;
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [ModelDefault("EditMask", "{0:n2}")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan, "0")]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Budget")]
        public decimal Budget
        {
            get { return _Budget; }
            set
            {
                SetPropertyValue("Budget", ref _Budget, value);
                if (!IsLoading)
                {
                    MonthlyBudgetBalance = Budget;
                }
            }
        }

        private decimal _MonthlyBudgetBalance;
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [ModelDefault("EditMask", "{0:n2}")]
        [Appearance("MonthlyBudgetBalance", Enabled = false)]
        [Index(8), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("Monthly Budget Balance")]
        public decimal MonthlyBudgetBalance
        {
            get { return _MonthlyBudgetBalance; }
            set
            {
                SetPropertyValue("MonthlyBudgetBalance", ref _MonthlyBudgetBalance, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private BudgetCategoryDetails _BudgetCategoryDetails;
        [Association("BudgetCategoryDetails-BudgetCategoryAmount")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("BudgetCategoryDetails", Enabled = false)]
        public BudgetCategoryDetails BudgetCategoryDetails
        {
            get { return _BudgetCategoryDetails; }
            set { SetPropertyValue("BudgetCategoryDetails", ref _BudgetCategoryDetails, value); }
        }
    }
}