using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BSI_PR.Module.BusinessObjects;
using DevExpress.ExpressApp.Web;

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class NavigationController : WindowController
    {
        public NavigationController()
        {
            InitializeComponent();
            // Target required Windows (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target Window.
            ShowNavigationItemController showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            showNavigationItemController.CustomShowNavigationItem += showNavigationItemController_CustomShowNavigationItem;
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        void showNavigationItemController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Id == "BudgetReport_DetailView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(BudgetReport));
                BudgetReport BudgetReports = objectSpace.CreateObject<BudgetReport>();
                DetailView detailView = Application.CreateDetailView(objectSpace, BudgetReports);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "OpenPO_DetailView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(OpenPO));
                OpenPO OpenPO= objectSpace.CreateObject<OpenPO>();
                DetailView detailView = Application.CreateDetailView(objectSpace, OpenPO);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "ClosePO_DetailView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(ClosePO));
                ClosePO ClosePO = objectSpace.CreateObject<ClosePO>();
                DetailView detailView = Application.CreateDetailView(objectSpace, ClosePO);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "VendorTransaction_DetailView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(VendorTransaction));
                VendorTransaction VendorTransactions = objectSpace.CreateObject<VendorTransaction>();
                DetailView detailView = Application.CreateDetailView(objectSpace, VendorTransactions);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "BudgetCategoryReport_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(BudgetCategoryReport));
                BudgetCategoryReport budgetcategory = objectSpace.CreateObject<BudgetCategoryReport>();
                DetailView detailView = Application.CreateDetailView(objectSpace, budgetcategory);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }
        }
    }
}