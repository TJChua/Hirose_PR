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
    [RuleCriteria("BudgetMonthValidation", DefaultContexts.Save, "IsValid = 0", "Not allow same month in same year.")]
    public class BudgetCategoryDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public BudgetCategoryDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsActive = true;
        }

        private BudgetCategoryData _BudgetCategory;
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("BudgetCategory", Enabled = false, Criteria = "not IsNew")]
        public BudgetCategoryData BudgetCategory
        {
            get { return _BudgetCategory; }
            set
            {
                SetPropertyValue("BudgetCategory", ref _BudgetCategory, value);
            }
        }

        private decimal _YearlyBudgetBalance;
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [ModelDefault("EditMask", "{0:n2}")]
        [XafDisplayName("Yearly Budget Balance")]
        [Index(5), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("YearlyBudgetBalance", Enabled = false)]
        public decimal YearlyBudgetBalance
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (BudgetCategoryAmount != null)
                        rtn += BudgetCategoryAmount.Sum(p => p.MonthlyBudgetBalance);

                    return rtn;
                }
                else
                {
                    return _YearlyBudgetBalance;
                }
            }
            set
            {
                SetPropertyValue("YearlyBudgetBalance", ref _YearlyBudgetBalance, value);
            }
        }

        private bool _IsActive;
        [XafDisplayName("Active")]
        [Index(10)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                var year = this.BudgetCategoryAmount.GroupBy(x => x.Year).ToList();

                foreach (var dtl in year)
                {
                    foreach (BudgetCategoryAmount dtl2 in this.BudgetCategoryAmount)
                    {
                        if (dtl2.Year == dtl.Key.ToString())
                        {
                            if (this.BudgetCategoryAmount.Where(x => x.Year.ToString() == dtl2.Year && x.Month == dtl2.Month).Count() > 1)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        private BudgetCategory _BudgetCategoryData;
        [Association("BudgetCategoryData-BudgetCategoryDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("BudgetCategoryData", Enabled = false)]
        public BudgetCategory BudgetCategoryData
        {
            get { return _BudgetCategoryData; }
            set { SetPropertyValue("BudgetCategoryData", ref _BudgetCategoryData, value); }
        }

        [Association("BudgetCategoryDetails-BudgetCategoryAmount")]
        [XafDisplayName("Budget")]
        public XPCollection<BudgetCategoryAmount> BudgetCategoryAmount
        {
            get { return GetCollection<BudgetCategoryAmount>("BudgetCategoryAmount"); }
        }
    }
}