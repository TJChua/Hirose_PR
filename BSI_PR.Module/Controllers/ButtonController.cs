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
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Web;
using System.IO;
using DevExpress.ExpressApp.Web;
using System.Configuration;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;


#region update log
// TJC - 20210503 - Fixed "Closed" correct PO ver 0.1
// TJC - 20210730 - Fixed "Closed" correct PO ver 0.2
// TJC - 20211102 - Change batch posting ver 0.3
// TJC - 20211115 - Block approve multiple times ver 0.4
// TJC - 20211214 - Add Attachment Compare button ver 0.5
// TJC - 20230725 - New service req ver 0.6
// TJC - 20230607 - add Japan PO ver 0.8
// TJC - 20230628 - Cancel/Reject PO add back the budget ver 0.9
// TJC - 20230926 - not allow submit if over budget ver 0.10

#endregion

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ButtonController : ViewController
    {
        GenControllers genCon;
        public ButtonController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.Pass.Active.SetItemValue("Enabled", false);
            this.Accept_PO.Active.SetItemValue("Enabled", false);
            this.Pass_GRN.Active.SetItemValue("Enabled", false);
            this.Pass_GoodsReturn.Active.SetItemValue("Enabled", false);
            this.CopyFromPR.Active.SetItemValue("Enabled", false);
            this.CopyFromPO.Active.SetItemValue("Enabled", false);
            this.CopyFromGRN.Active.SetItemValue("Enabled", false);
            this.Approval_PO.Active.SetItemValue("Enabled", false);
            this.INV_CopyFromPO.Active.SetItemValue("Enabled", false);
            this.INV_CopyFromGRN.Active.SetItemValue("Enabled", false);
            this.Pass_Invoice.Active.SetItemValue("Enabled", false);
            this.EscalateUser.Active.SetItemValue("Enabled", false);
            this.EscalateUser_PO.Active.SetItemValue("Enabled", false);
            this.Closed_PO.Active.SetItemValue("Enabled", false);
            this.Cancel_GRN.Active.SetItemValue("Enabled", false);
            this.Cancel_Invoice.Active.SetItemValue("Enabled", false);
            this.BudgetInvoice.Active.SetItemValue("Enabled", false);
            this.Escalate_SystemUser.Active.SetItemValue("Enabled", false);
            this.BudgetPO.Active.SetItemValue("Enabled", false);
            this.Cancel_PO.Active.SetItemValue("Enabled", false);
            this.Post_Invoice.Active.SetItemValue("Enabled", false);
            this.DepartmantFilter.Active.SetItemValue("Enabled", false);
           
            this.PrintPO1.Active.SetItemValue("Enabled", false);
            this.Duplicate_PO.Active.SetItemValue("Enabled", false);
            // Start ver 0.5
            this.CompareAtt.Active.SetItemValue("Enabled", false);
            // End ver 0.5
            // Start ver 0.6
            this.CancelLeave.Active.SetItemValue("Enabled", false);
            this.PrintDepartmentBudget.Active.SetItemValue("Enabled", false);
            // End ver 0.6

            // Start ver 0.7
            this.LevelOfApp.Active.SetItemValue("Enabled", false);
            this.Pass_JapanPO.Active.SetItemValue("Enabled", false);
            this.Duplicate_POJapan.Active.SetItemValue("Enabled", false);
            this.Closed_POJapan.Active.SetItemValue("Enabled", false);
            this.PrintPOJapan.Active.SetItemValue("Enabled", false);
            this.EscalateUserJapan.Active.SetItemValue("Enabled", false);
            this.Cancel_POJapan.Active.SetItemValue("Enabled", false);
            this.BudgetPOJapan.Active.SetItemValue("Enabled", false);
            this.Approval_POJapan.Active.SetItemValue("Enabled", false);
            this.CompareAttJapan.Active.SetItemValue("Enabled", false);
            this.DuplicateBudget.Active.SetItemValue("Enabled", false);
            this.DuplicateBudgetData.Active.SetItemValue("Enabled", false);
            // End ver 0.7


            ListViewController controller = Frame.GetController<ListViewController>();
            if (controller != null)
            {
                if (View.Id == "PurchaseRequests_ListView" || View.Id == "PurchaseRequests_ListView_Approved" ||
                    View.Id == "PurchaseRequests_ListView_Correction" || View.Id == "PurchaseRequests_ListView_Approved" ||
                    View.Id == "PurchaseRequests_ListView_Approved" || View.Id == "PurchaseOrder_ListView" ||
                    View.Id == "PurchaseOrder_ListView_Approved" || View.Id == "PurchaseOrder_ListView_PendingApp" ||
                    View.Id == "GoodReceipt_ListView" || View.Id == "GoodsReturn_ListView" ||
                    View.Id == "APInvoice_ListView" || 
                    // Start ver 0.7
                    View.Id == "PurchaseOrderJapan_ListView" || View.Id == "PurchaseOrderJapan_ListView_Approved" ||
                    View.Id == "PurchaseOrderJapan_ListView_PendingApp"
                    // End ver 0.7
                    )
                {
                    controller.EditAction.Active["123"] = false;
                }
            }

            if (typeof(PurchaseOrder).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
                {
                    SystemUsers curruser = (SystemUsers)SecuritySystem.CurrentUser;
                    PermissionPolicyRole approle = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));
                    PermissionPolicyRole adminrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));

                    this.Pass.Active.SetItemValue("Enabled", true);
                    if (approle == null)
                    {
                        this.Duplicate_PO.Active.SetItemValue("Enabled", true);
                    }
                    //this.CopyFromPR.Active.SetItemValue("Enabled", true);
                    this.Closed_PO.Active.SetItemValue("Enabled", true);

                    /////
                    this.PrintPO1.Active.SetItemValue("Enabled", true);
                    ///

                    if (View.Id == "PurchaseOrder_ListView" || View.Id == "PurchaseOrder_ListView_Approved" || View.Id == "PurchaseOrder_ListView_PendingApp")
                    {
                        // Start ver 0.6
                        //if (adminrole != null || approle != null)
                        if (adminrole != null)
                        // End ver 0.6
                        {
                            Escalate_SystemUser.Items.Clear();
                            foreach (SystemUsers user in View.ObjectSpace.CreateCollection(typeof(SystemUsers), null))
                            {
                                PermissionPolicyUser permissionuser = ObjectSpace.FindObject<PermissionPolicyUser>(CriteriaOperator.Parse("Oid = ?", user.Oid));

                                foreach (PermissionPolicyRole permissionrole in permissionuser.Roles)
                                {
                                    if (permissionrole.Name == "ApprovalUserRole")
                                    {
                                        Escalate_SystemUser.Items.Add(new ChoiceActionItem(user.FName, user.FName));
                                    }
                                }
                            }

                            this.EscalateUser_PO.Active.SetItemValue("Enabled", true);
                            this.Escalate_SystemUser.Active.SetItemValue("Enabled", true);
                           
                        }
                    }
                    if (approle != null)
                    {
                        this.Approval_PO.Active.SetItemValue("Enabled", true);
                    }

                    if (View.Id == "PurchaseOrder_DetailView")
                    {
                        this.BudgetPO.Active.SetItemValue("Enabled", true);
                        this.Cancel_PO.Active.SetItemValue("Enabled", true);

                        
                        this.PrintPO1.Active.SetItemValue("Enabled", true);
                    }

                    if ((View.Id == "PurchaseOrder_ListView")  && approle != null)
                    {
                        DepartmantFilter.Items.Clear();
                        // Start ver 0.6
                        //if (adminrole != null)
                        //{
                            DepartmantFilter.Items.Add(new ChoiceActionItem("All", "All"));
                            DepartmantFilter.Items.Add(new ChoiceActionItem(curruser.DefaultDept.BoCode, curruser.DefaultDept.BoCode));
                        //}
                        // End ver 0.6
                        foreach (Departments dtl in curruser.Department)
                        {
                            if (dtl.BoCode != curruser.DefaultDept.BoCode)
                            {
                                DepartmantFilter.Items.Add(new ChoiceActionItem(dtl.BoCode, dtl.BoCode));
                            }
                        }

                        DepartmantFilter.SelectedIndex = 1;

                        // Start ver 0.6
                        if (DepartmantFilter.SelectedItem.Id != "All")
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] = ?",
                                DepartmantFilter.SelectedItem.Id);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] != ?",
                                "All");
                        }
                        // End ver 0.6

                        this.DepartmantFilter.Active.SetItemValue("Enabled", true);
                     
                    }

                    // Start ver 0.5
                    //if ((View.Id == "PurchaseOrder_ListView" || View.Id == "PurchaseOrder_ListView_Approved" || View.Id == "PurchaseOrder_ListView_PendingApp"))
                    //{
                        this.CompareAtt.Active.SetItemValue("Enabled", true);
                    //}
                    // End ver 0.5
                }
            }

            if (typeof(GoodReceipt).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(GoodReceipt))
                {
                    this.Pass_GRN.Active.SetItemValue("Enabled", true);
                    this.CopyFromPO.Active.SetItemValue("Enabled", true);
                    if (View.Id == "GoodReceipt_DetailView")
                    {
                        this.Cancel_GRN.Active.SetItemValue("Enabled", true);
                    }
                }
            }

            if (typeof(GoodsReturn).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(GoodsReturn))
                {
                    this.Pass_GoodsReturn.Active.SetItemValue("Enabled", true);
                    this.CopyFromGRN.Active.SetItemValue("Enabled", true);
                }
            }

            if (typeof(APInvoice).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(APInvoice))
                {
                    SystemUsers curruser = (SystemUsers)SecuritySystem.CurrentUser;
                    PermissionPolicyRole approle = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));
                    // Start ver 0.6
                    PermissionPolicyRole adminrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));
                    // End ver 0.6

                    this.Pass_Invoice.Active.SetItemValue("Enabled", true);
                    this.INV_CopyFromPO.Active.SetItemValue("Enabled", true);
                    //this.INV_CopyFromGRN.Active.SetItemValue("Enabled", true);
                    if (View.Id == "APInvoice_DetailView")
                    {
                        this.Cancel_Invoice.Active.SetItemValue("Enabled", true);
                        this.BudgetInvoice.Active.SetItemValue("Enabled", true);
                    }
                    this.Post_Invoice.Active.SetItemValue("Enabled", false);

                    if ((View.Id == "APInvoice_ListView") && approle != null)
                    {
                        DepartmantFilter.Items.Clear();
                        // Start ver 0.6
                        //if (adminrole != null)
                        //{
                            DepartmantFilter.Items.Add(new ChoiceActionItem("All", "All"));
                            DepartmantFilter.Items.Add(new ChoiceActionItem(curruser.DefaultDept.BoCode, curruser.DefaultDept.BoCode));
                        //}
                        // End ver 0.6
                        foreach (Departments dtl in curruser.Department)
                        {
                            if (dtl.BoCode != curruser.DefaultDept.BoCode)
                            {
                                DepartmantFilter.Items.Add(new ChoiceActionItem(dtl.BoCode, dtl.BoCode));
                            }
                        }

                        DepartmantFilter.SelectedIndex = 1;

                        // Start ver 0.6
                        if (DepartmantFilter.SelectedItem.Id != "All")
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] = ?",
                                DepartmantFilter.SelectedItem.Id);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] != ?",
                                "All");
                        }
                        // End ver 0.6

                        this.DepartmantFilter.Active.SetItemValue("Enabled", true);
                    }
                }
            }

            if (typeof(PurchaseRequests).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
                {
                    PermissionPolicyRole approle = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));
                    PermissionPolicyRole adminrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));

                    if (View.Id == "PurchaseRequests_ListView" || View.Id == "PurchaseRequests_ListView_Approved" || View.Id == "PurchaseRequests_ListView_Correction")
                    {
                        // Start ver 0.6
                        //if (adminrole != null || approle != null)
                        if (adminrole != null)
                        // End ver 0.6
                        {
                            Escalate_SystemUser.Items.Clear();
                            foreach (SystemUsers user in View.ObjectSpace.CreateCollection(typeof(SystemUsers), null))
                            {
                                PermissionPolicyUser permissionuser = ObjectSpace.FindObject<PermissionPolicyUser>(CriteriaOperator.Parse("Oid = ?", user.Oid));

                                foreach (PermissionPolicyRole permissionrole in permissionuser.Roles)
                                {
                                    if (permissionrole.Name == "ApprovalUserRole")
                                    {
                                        Escalate_SystemUser.Items.Add(new ChoiceActionItem(user.FName, user.FName));
                                    }
                                }
                            }

                            this.EscalateUser.Active.SetItemValue("Enabled", true);
                            this.Escalate_SystemUser.Active.SetItemValue("Enabled", true);
                        }
                    }
                }
            }

            // Start ver 0.6
            if (typeof(LeaveApplication).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(LeaveApplication))
                {
                    this.CancelLeave.Active.SetItemValue("Enabled", true);
                }
            }

            if (typeof(BudgetCategoryReport).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(BudgetCategoryReport))
                {
                    if (View.Id == "BudgetCategoryReport_DetailView")
                    {
                        this.PrintDepartmentBudget.Active.SetItemValue("Enabled", true);
                    }
                }
            }
            // End ver 0.6

            // Start ver 0.7
            if (typeof(PurchaseOrderJapan).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderJapan))
                {
                    SystemUsers curruser = (SystemUsers)SecuritySystem.CurrentUser;
                    PermissionPolicyRole approle = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));
                    PermissionPolicyRole adminrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));

                    this.Pass_JapanPO.Active.SetItemValue("Enabled", true);

                    if (approle == null)
                    {
                        this.Duplicate_POJapan.Active.SetItemValue("Enabled", true);
                    }

                    this.Closed_POJapan.Active.SetItemValue("Enabled", true);
                    this.PrintPOJapan.Active.SetItemValue("Enabled", true);

                    if (View.Id == "PurchaseOrderJapan_ListView" || View.Id == "PurchaseOrderJapan_ListView_Approved" || View.Id == "PurchaseOrderJapan_ListView_PendingApp")
                    {
                        if (adminrole != null)
                        {
                            Escalate_SystemUser.Items.Clear();
                            foreach (SystemUsers user in View.ObjectSpace.CreateCollection(typeof(SystemUsers), null))
                            {
                                PermissionPolicyUser permissionuser = ObjectSpace.FindObject<PermissionPolicyUser>(CriteriaOperator.Parse("Oid = ?", user.Oid));

                                foreach (PermissionPolicyRole permissionrole in permissionuser.Roles)
                                {
                                    if (permissionrole.Name == "ApprovalUserRole")
                                    {
                                        Escalate_SystemUser.Items.Add(new ChoiceActionItem(user.FName, user.FName));
                                    }
                                }
                            }

                            this.EscalateUserJapan.Active.SetItemValue("Enabled", true);
                            this.Escalate_SystemUser.Active.SetItemValue("Enabled", true);

                        }
                    }

                    if (approle != null)
                    {
                        this.Approval_POJapan.Active.SetItemValue("Enabled", true);
                    }

                    if (View.Id == "PurchaseOrderJapan_DetailView")
                    {
                        this.BudgetPOJapan.Active.SetItemValue("Enabled", true);
                        this.Cancel_POJapan.Active.SetItemValue("Enabled", true);


                        this.PrintPOJapan.Active.SetItemValue("Enabled", true);
                    }

                    if ((View.Id == "PurchaseOrderJapan_ListView") && approle != null)
                    {
                        DepartmantFilter.Items.Clear();
                        //if (adminrole != null)
                        //{
                            DepartmantFilter.Items.Add(new ChoiceActionItem("All", "All"));
                            DepartmantFilter.Items.Add(new ChoiceActionItem(curruser.DefaultDept.BoCode, curruser.DefaultDept.BoCode));
                        //}
                        foreach (Departments dtl in curruser.Department)
                        {
                            if (dtl.BoCode != curruser.DefaultDept.BoCode)
                            {
                                DepartmantFilter.Items.Add(new ChoiceActionItem(dtl.BoCode, dtl.BoCode));
                            }
                        }

                        DepartmantFilter.SelectedIndex = 1;

                        if (DepartmantFilter.SelectedItem.Id != "All")
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] = ?",
                                DepartmantFilter.SelectedItem.Id);
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] != ?",
                                "All");
                        }

                        this.DepartmantFilter.Active.SetItemValue("Enabled", true);
                    }

                    this.CompareAttJapan.Active.SetItemValue("Enabled", true);

                    if ((View.Id == "PurchaseOrderJapan_ListView"))
                    {
                        LevelOfApp.Items.Clear();
                        LevelOfApp.Items.Add(new ChoiceActionItem("", ""));
                        LevelOfApp.Items.Add(new ChoiceActionItem("Level 1 - Factory GM", "Level 1 - Factory GM"));
                        LevelOfApp.Items.Add(new ChoiceActionItem("Level 2 - Honbuchou", "Level 2 - Honbuchou"));
                        LevelOfApp.Items.Add(new ChoiceActionItem("Level 3 - President", "Level 3 - President"));

                        this.LevelOfApp.Active.SetItemValue("Enabled", true);
                    }
                }
            }

            if (typeof(BudgetCategory).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(BudgetCategory))
                {
                    this.DuplicateBudget.Active.SetItemValue("Enabled", true);
                }
            }

            if (typeof(BudgetCategoryData).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(BudgetCategoryData))
                {
                    this.DuplicateBudgetData.Active.SetItemValue("Enabled", true);
                }
            }
            // End ver 0.7
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenControllers>();

            if (typeof(PurchaseRequests).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                // Start ver 0.6
                PermissionPolicyRole adminrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));
                // End ver 0.6

                if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
                {
                    if (View.Id == "PurchaseRequests_DetailView")
                    {
                        if (((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit)
                        {
                            // Start ver 0.6
                            if (adminrole != null)
                            {
                            // End ver 0.6
                                this.EscalateUser.Active.SetItemValue("Enabled", true);
                            // Start ver 0.6
                            }
                            // End ver 0.6
                        }
                        else
                        {
                            this.EscalateUser.Active.SetItemValue("Enabled", false);
                        }
                    }
                }
            }

            if (typeof(PurchaseOrder).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                // Start ver 0.6
                PermissionPolicyRole adminrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));
                // End ver 0.6

                if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
                {
                    if (View.Id == "PurchaseOrder_DetailView")
                    {
                        if (((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit)
                        {
                            // Start ver 0.6
                            if (adminrole != null)
                            {
                            // End ver 0.6
                                this.EscalateUser_PO.Active.SetItemValue("Enabled", true);
                            // Start ver 0.6
                            }
                            // End ver 0.6
                            this.BudgetPO.Active.SetItemValue("Enabled", false);

                            this.PrintPO1.Active.SetItemValue("Enabled", false);
                            this.CompareAtt.Active.SetItemValue("Enabled", false);
                        }
                        else
                        {
                            this.EscalateUser_PO.Active.SetItemValue("Enabled", false);
                            this.BudgetPO.Active.SetItemValue("Enabled", true);
                            ////Added by YJ 2022-5-19 User request to print PO in detail view
                            this.PrintPO1.Active.SetItemValue("Enabled", true);
                        }
                    }
                }
            }

            if (typeof(APInvoice).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(APInvoice))
                {
                    if (View.Id == "APInvoice_DetailView")
                    {
                        if (((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit)
                        {
                            this.BudgetInvoice.Active.SetItemValue("Enabled", false);                      
                        }
                        else
                        {
                            this.BudgetInvoice.Active.SetItemValue("Enabled", true);
                        }
                    }
                }
            }

            // Start ver 0.7
            if (typeof(PurchaseOrderJapan).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderJapan))
                {
                    if (View.Id == "PurchaseOrderJapan_DetailView")
                    {
                        if (((DetailView)View).ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit)
                        {
                            this.EscalateUserJapan.Active.SetItemValue("Enabled", true);
                            this.BudgetPOJapan.Active.SetItemValue("Enabled", false);

                            this.PrintPOJapan.Active.SetItemValue("Enabled", false);
                            this.CompareAttJapan.Active.SetItemValue("Enabled", false);
                        }
                        else
                        {
                            this.EscalateUserJapan.Active.SetItemValue("Enabled", false);
                            this.BudgetPOJapan.Active.SetItemValue("Enabled", true);
                            ////Added by YJ 2022-5-19 User request to print PO in detail view
                            this.PrintPOJapan.Active.SetItemValue("Enabled", true);
                        }
                    }
                }
            }
            // End ver 0.7
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void Pass_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;

                if (p.IsErr) return;

                // Start ver 0.10
                if (selectedObject.BudgetCategoryData != null)
                {
                    if (selectedObject.MonthlyBudgetBalance < (double)selectedObject.Amount)
                    {
                        genCon.showMsg("Error", "Over budget.", InformationType.Error);
                        return;
                    }
                }
                // End ver 0.10

                selectedObject.IsPassed = true;
                selectedObject.IsRejected = false;
                selectedObject.ApprovalStatus = ApprovalStatuses.Not_Applicable;
                selectedObject.ApprovalStatus = ApprovalStatuses.Not_Applicable;
                PurchaseOrderDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderDocStatuses>();
                ds.DocStatus = DocumentStatus.DocPassed;
                ds.DocRemarks = p.ParamString;
                selectedObject.PurchaseOrderDocStatuses.Add(ds);
                //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

                if (!selectedObject.DocType.IsPassAccept)
                {
                    //ModificationsController controller = Frame.GetController<ModificationsController>();
                    //if (controller != null)
                    //{
                    //    controller.SaveAction.DoExecute();
                    //}
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
                if (selectedObject.DocType.IsPassAccept)
                {
                    Accept_PO_Execute(sender, e);
                }
                if (!selectedObject.DocType.IsPassAccept)
                {
                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));
                    genCon.openNewView(os, prtrx, ViewEditMode.View);
                    genCon.showMsg("Successful", "Submit Done.", InformationType.Success);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected/More Than One PO Selected.", InformationType.Error);
            }
        }
        private void Pass_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Accept_PO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsAccepted = true;
                selectedObject.IsRejected = false;
                PurchaseOrderDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderDocStatuses>();
                ds.DocStatus = DocumentStatus.Accepted;
                ds.DocRemarks = p.ParamString;
                selectedObject.PurchaseOrderDocStatuses.Add(ds);

                PurchaseOrderAppStatuses dsApp = ObjectSpace.CreateObject<PurchaseOrderAppStatuses>();
                dsApp.AppStatus = ApprovalStatuses.Not_Applicable;
                dsApp.AppRemarks = "";
                selectedObject.PurchaseOrderAppStatuses.Add(dsApp);

                //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

                //ModificationsController controller = Frame.GetController<ModificationsController>();
                //if (controller != null)
                //{
                //    controller.SaveAction.DoExecute();
                //}
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                #region approval
                XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                List<string> ToEmails = new List<string>();
                string emailbody = "";
                string emailsubject = "";
                string emailaddress = "";
                Guid emailuser;
                DateTime emailtime = DateTime.Now;
                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetApproval", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("PurchaseOrders"));

                IObjectSpace emailos = Application.CreateObjectSpace();
                EmailSents emailobj = null;

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        string receiver = null;
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            if (receiver == null)
                            {
                                receiver = row.Values[4].ToString();
                            }
                            else
                            {
                                receiver = receiver + "/" + row.Values[4];
                            }
                        }
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            emailbody = "Dear " + receiver + "," + System.Environment.NewLine + System.Environment.NewLine +
                                row.Values[3] + System.Environment.NewLine + GeneralSettings.appurl + row.Values[2].ToString() +
                                System.Environment.NewLine + System.Environment.NewLine + "Regards," + System.Environment.NewLine +
                                    //selectedObject.CreateUser.FName.ToString();
                                   "E-PO System";

                            emailsubject = "Purchase Order Approval";
                            emailaddress = row.Values[1].ToString();
                            emailuser = (Guid)row.Values[0];

                            ToEmails.Add(emailaddress);

                            emailobj = emailos.CreateObject<EmailSents>();
                            emailobj.CreateDate = (DateTime?)emailtime;
                            emailobj.EmailUser = emailobj.Session.GetObjectByKey<SystemUsers>(emailuser);
                            emailobj.EmailAddress = emailaddress;
                            //assign body will get error???
                            //emailobj.EmailBody = emailbody;
                            emailobj.Remarks = emailsubject;
                            emailobj.PurchaseOrder = emailobj.Session.GetObjectByKey<PurchaseOrder>(selectedObject.Oid);

                            emailobj.Save();
                        }
                    }
                }

                if (ToEmails.Count > 0)
                {
                    if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                    {
                        if (emailos.IsModified)
                            emailos.CommitChanges();
                    }
                }

                #endregion

                // Start ver 0.9
                IObjectSpace pos = Application.CreateObjectSpace();
                PurchaseOrder trx = pos.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));
                if (trx.BudgetCategoryData != null)
                {
                    genCon.UpdateBudget(trx.Department.Oid, trx.BudgetCategoryData.BudgetCategoryName,
                        trx.DocDate.Month, trx.DocDate.Year.ToString(), trx.Amount, pos, "Add");
                }
                // End ver 0.9

                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));
                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Submit to approver.", InformationType.Success);
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected/More Than One PO Selected.", InformationType.Error);
            }
        }

        private void Accept_PO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Pass_GRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Pass_GRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                GoodReceipt selectedObject = (GoodReceipt)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsSubmit = true;
                GoodReceiptDocStatus ds = ObjectSpace.CreateObject<GoodReceiptDocStatus>();
                ds.DocStatus = "Submit";
                ds.DocRemarks = p.ParamString;
                selectedObject.GoodReceiptDocStatus.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                IObjectSpace os = Application.CreateObjectSpace();
                GoodReceipt prtrx = os.FindObject<GoodReceipt>(new BinaryOperator("Oid", selectedObject.Oid));

                foreach (GoodReceiptDetails dtl in prtrx.GoodReceiptDetails)
                {
                    IObjectSpace POos = Application.CreateObjectSpace();
                    PurchaseOrder po = POos.FindObject<PurchaseOrder>(new BinaryOperator("DocNum", dtl.DocNum));

                    foreach (PurchaseOrderDetails dtl2 in po.PurchaseOrderDetails)
                    {
                        if (dtl2.Oid == dtl.POOid && dtl2.PODocNum == dtl.DocNum)
                        {
                            dtl2.OpenQty -= dtl.Quantity;
                        }
                    }

                    POos.CommitChanges();
                }

                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Submit Done.", InformationType.Success);
            }
            else
            {
                genCon.showMsg("Fail", "No GRN Selected/More Than One GRN Selected.", InformationType.Error);
            }
        }

        private void Pass_GoodsReturn_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                GoodsReturn selectedObject = (GoodsReturn)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsSubmit = true;
                GoodsReturnDocStatus ds = ObjectSpace.CreateObject<GoodsReturnDocStatus>();
                ds.DocStatus = "Submit";
                ds.DocRemarks = p.ParamString;
                selectedObject.GoodsReturnDocStatus.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                IObjectSpace os = Application.CreateObjectSpace();
                GoodsReturn prtrx = os.FindObject<GoodsReturn>(new BinaryOperator("Oid", selectedObject.Oid));

                foreach (GoodsReturnDetails dtl in prtrx.GoodsReturnDetails)
                {
                    IObjectSpace POos = Application.CreateObjectSpace();
                    PurchaseOrder po = POos.FindObject<PurchaseOrder>(new BinaryOperator("DocNum", dtl.PODocNum));

                    foreach (PurchaseOrderDetails dtl2 in po.PurchaseOrderDetails)
                    {
                        if (dtl2.Oid == dtl.POOid && dtl2.PODocNum == dtl.PODocNum)
                        {
                            dtl2.OpenQty += dtl.Quantity;
                        }
                    }

                    POos.CommitChanges();

                    IObjectSpace GRNos = Application.CreateObjectSpace();
                    GoodReceipt grn = GRNos.FindObject<GoodReceipt>(new BinaryOperator("DocNum", dtl.GRND));

                    foreach (GoodReceiptDetails dtl3 in grn.GoodReceiptDetails)
                    {
                        if (dtl3.Oid == dtl.BaseLine && dtl3.GRNDocNum == dtl.GRND)
                        {
                            dtl3.OpenQty -= dtl.Quantity;
                        }
                    }

                    GRNos.CommitChanges();
                }

                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Submit Done.", InformationType.Success);
            }
            else
            {
                genCon.showMsg("Fail", "No GR Selected/More Than One GR Selected.", InformationType.Error);
            }
        }

        private void Pass_GoodsReturn_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CopyFromPR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrder newPO = os.CreateObject<PurchaseOrder>();

                    foreach (vw_PR dtl in e.PopupWindowViewSelectedObjects)
                    {
                        newPO.Vendor = newPO.Session.GetObjectByKey<vw_vendors>(dtl.BoCode);

                        PurchaseOrderDetails newPOItem = os.CreateObject<PurchaseOrderDetails>();

                        newPOItem.Item = newPOItem.Session.GetObjectByKey<vw_items>(dtl.ItemCode);
                        newPOItem.ItemDesc = dtl.ItemDescrip;
                        newPOItem.UOM = dtl.UOM;
                        newPOItem.OpenQty = dtl.Quantity;
                        newPOItem.Quantity = dtl.Quantity;
                        newPOItem.Tax = newPOItem.Session.GetObjectByKey<vw_taxes>(dtl.Tax);
                        newPOItem.Amount = dtl.UnitPrice;
                        newPOItem.DocNum = dtl.SAPDocNum;
                        newPOItem.PROid = dtl.PRLine;

                        newPO.PurchaseOrderDetails.Add(newPOItem);
                    }
                      
                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newPO);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    genCon.showMsg("Success", "Copy Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PR Selected.", InformationType.Error);
            }
        }

        private void CopyFromPR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            PurchaseOrder purchaseorder = (PurchaseOrder)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_PR));
            var cs = Application.CreateCollectionSource(os, typeof(vw_PR), viewId);
            if (purchaseorder.Vendor != null)
            {
                cs.Criteria["BoCode"] = new BinaryOperator("BoCode", purchaseorder.Vendor.BoCode);
            }
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void CopyFromPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    IObjectSpace os = Application.CreateObjectSpace();
                    GoodReceipt newGRN = os.CreateObject<GoodReceipt>();

                    foreach (vw_RecvPO dtl in e.PopupWindowViewSelectedObjects)
                    {
                        newGRN.Vendor = newGRN.Session.GetObjectByKey<vw_vendors>(dtl.BoCode);

                        GoodReceiptDetails newGRNItem = os.CreateObject<GoodReceiptDetails>();

                        newGRNItem.ItemCode = newGRNItem.Session.GetObjectByKey<vw_items>(dtl.ItemCode);
                        newGRNItem.ItemDescrip = dtl.ItemDescrip;
                        newGRNItem.UOM = dtl.UOM;
                        newGRNItem.Quantity = dtl.OpenQty;
                        newGRNItem.Tax = newGRNItem.Session.GetObjectByKey<vw_taxes>(dtl.Tax);
                        newGRNItem.UnitPrice = dtl.UnitPrice;
                        newGRNItem.DocNum = dtl.SAPDocNum;
                        newGRNItem.POOid = dtl.POLine;

                        newGRN.GoodReceiptDetails.Add(newGRNItem);
                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newGRN);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    genCon.showMsg("Success", "Copy Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected.", InformationType.Error);
            }
        }

        private void CopyFromPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            GoodReceipt goodreceipt = (GoodReceipt)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_RecvPO));
            var cs = Application.CreateCollectionSource(os, typeof(vw_RecvPO), viewId);
            if (goodreceipt.Vendor != null)
            {
                cs.Criteria["BoCode"] = new BinaryOperator("BoCode", goodreceipt.Vendor.BoCode);
            }
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void CopyFromGRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    IObjectSpace os = Application.CreateObjectSpace();
                    GoodsReturn newGRN = os.CreateObject<GoodsReturn>();

                    foreach (vw_GRN dtl in e.PopupWindowViewSelectedObjects)
                    {
                        newGRN.Vendor = newGRN.Session.GetObjectByKey<vw_vendors>(dtl.BoCode);

                        GoodsReturnDetails newGRNItem = os.CreateObject<GoodsReturnDetails>();

                        newGRNItem.ItemCode = newGRNItem.Session.GetObjectByKey<vw_items>(dtl.ItemCode);
                        newGRNItem.ItemDescrip = dtl.ItemDescrip;
                        newGRNItem.UOM = dtl.UOM;
                        newGRNItem.Quantity = dtl.Quantity;
                        newGRNItem.Tax = newGRNItem.Session.GetObjectByKey<vw_taxes>(dtl.Tax);
                        newGRNItem.UnitPrice = dtl.UnitPrice;
                        newGRNItem.GRND = dtl.SAPDocNum;
                        newGRNItem.BaseLine = dtl.GRNLine;
                        newGRNItem.PODocNum = dtl.PODocNum;
                        newGRNItem.POOid = dtl.POLine;

                        newGRN.GoodsReturnDetails.Add(newGRNItem);
                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newGRN);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    genCon.showMsg("Success", "Copy Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No GRN Selected.", InformationType.Error);
            }
        }

        private void CopyFromGRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            GoodsReturn goodsreturn = (GoodsReturn)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_GRN));
            var cs = Application.CreateCollectionSource(os, typeof(vw_GRN), viewId);
            if (goodsreturn.Vendor != null)
            {
                cs.Criteria["BoCode"] = new BinaryOperator("BoCode", goodsreturn.Vendor.BoCode);
            }
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void Approval_PO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                int totaldoc = 0;

                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                foreach (PurchaseOrder dtl in e.SelectedObjects)
                {
                    IObjectSpace pos = Application.CreateObjectSpace();
                    PurchaseOrder po = pos.FindObject<PurchaseOrder>(new BinaryOperator("Oid", dtl.Oid));
               
                    // Start ver 0.4
                    if (po.NextApprover != null)
                    {
                    // End ver 0.4
                        po.Escalate = null;
                        po.WhoApprove = po.WhoApprove + user.UserName + ", ";
                        ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;

                        //if (selectedObject.PurchaseOrderAppStatuses.Count() > 0)
                        //    appstatus = selectedObject.PurchaseOrderAppStatuses.OrderBy(c => c.Oid).Last().AppStatus;

                        //if (appstatus != ApprovalStatuses.Not_Applicable)
                        //    if (po.PurchaseOrderAppStatuses.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                        //        appstatus = po.PurchaseOrderAppStatuses.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

                        if (appstatus == ApprovalStatuses.Not_Applicable)
                            appstatus = ApprovalStatuses.Required_Approval;


                        if (p.IsErr) return;
                        if (appstatus == ApprovalStatuses.Required_Approval && p.AppStatus == ApprovalActions.NA)
                        {
                            genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatuses.Approved && p.AppStatus == ApprovalActions.Yes)
                        {
                            genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatuses.Rejected && p.AppStatus == ApprovalActions.No)
                        {
                            genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        if (p.AppStatus == ApprovalActions.NA)
                        {
                            appstatus = ApprovalStatuses.Required_Approval;
                        }
                        if (p.AppStatus == ApprovalActions.Yes)
                        {
                            appstatus = ApprovalStatuses.Approved;
                        }
                        if (p.AppStatus == ApprovalActions.No)
                        {
                            appstatus = ApprovalStatuses.Rejected;
                        }

                        PurchaseOrderAppStatuses ds = pos.CreateObject<PurchaseOrderAppStatuses>();
                        ds.PurchaseOrder = pos.GetObjectByKey<PurchaseOrder>(po.Oid);
                        ds.AppStatus = appstatus;
                        if (appstatus == ApprovalStatuses.Rejected)
                        {
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Reject User: " + user.FName + ")" +
                                System.Environment.NewLine + "(Reason: Approval Rejected)";
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse(" 02868725-036A-483B-A123-2932180D4A03"));

                            // Start ver 0.9
                            if (po.BudgetCategoryData != null)
                            {
                                genCon.UpdateBudget(po.Department.Oid, po.BudgetCategoryData.BudgetCategoryName,
                                    po.DocDate.Month, po.DocDate.Year.ToString(), po.Amount, pos, "Cancel");
                            }
                            // End ver 0.9
                        }
                        else
                        {
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Approved User: " + user.FName + ")";
                        }
                        po.PurchaseOrderAppStatuses.Add(ds);

                        pos.CommitChanges();
                        pos.Refresh();

                        totaldoc++;

                        #region approval
                        XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

                        List<string> ToEmails = new List<string>();
                        string emailbody = "";
                        string emailsubject = "";
                        string emailaddress = "";
                        Guid emailuser;
                        DateTime emailtime = DateTime.Now;
                        SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_Approval", new OperandValue(user.UserName), new OperandValue(po.Oid), new OperandValue("PurchaseOrder"), new OperandValue((int)appstatus));

                        IObjectSpace emailos = Application.CreateObjectSpace();
                        EmailSents emailobj = null;

                        if (sprocData.ResultSet.Count() > 0)
                        {
                            if (sprocData.ResultSet[0].Rows.Count() > 0)
                            {
                                string receiver = null;
                                foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                                {
                                    if (receiver == null)
                                    {
                                        receiver = row.Values[4].ToString();
                                    }
                                    else
                                    {
                                        receiver = receiver + "/" + row.Values[4];
                                    }
                                }

                                foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                                {
                                    emailbody = "Dear " + receiver + "," + System.Environment.NewLine + System.Environment.NewLine +
                                        row.Values[3] + System.Environment.NewLine + GeneralSettings.appurl + row.Values[2].ToString() +
                                        System.Environment.NewLine + System.Environment.NewLine + "Regards," + System.Environment.NewLine +
                                        //po.CreateUser.FName.ToString();
                                        "E-PO System";

                                    if (appstatus == ApprovalStatuses.Approved)
                                        emailsubject = "Purchase Order Approval";
                                    else if (appstatus == ApprovalStatuses.Rejected)
                                        emailsubject = "Purchase Order Approval Rejected";

                                    emailaddress = row.Values[1].ToString();
                                    emailuser = (Guid)row.Values[0];

                                    ToEmails.Add(emailaddress);

                                    emailobj = emailos.CreateObject<EmailSents>();
                                    emailobj.CreateDate = (DateTime?)emailtime;
                                    emailobj.EmailUser = emailobj.Session.GetObjectByKey<SystemUsers>(emailuser);
                                    emailobj.EmailAddress = emailaddress;
                                    //assign body will get error???
                                    //emailobj.EmailBody = emailbody;
                                    emailobj.Remarks = emailsubject;
                                    emailobj.PurchaseOrder = emailobj.Session.GetObjectByKey<PurchaseOrder>(po.Oid);

                                    emailobj.Save();
                                }
                            }
                        }

                        if (ToEmails.Count > 0)
                        {
                            if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                            {
                                if (emailos.IsModified)
                                    emailos.CommitChanges();
                            }
                        }
                        #endregion
                    }

                    //IObjectSpace os = Application.CreateObjectSpace();
                    //PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));
                    //genCon.openNewView(os, prtrx, ViewEditMode.View);
                    //genCon.showMsg("Successful", "Approval Done.", InformationType.Success);

                    ObjectSpace.CommitChanges(); //This line persists created object(s).
                    ObjectSpace.Refresh();

                    genCon.showMsg("Info", "Total Document : " + totaldoc + " Approval Done.", InformationType.Info);
                // Start ver 0.4
                }
                // End ver 0.4
            }
            else
            {
                genCon.showMsg("Fail", "No PO selected.", InformationType.Error);
            }
        }

        private void Approval_PO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;

            //if (((DetailView)View).ViewEditMode == ViewEditMode.View)
            //{
            //    genCon.showMsg("Failed", "Viewing Document cannot proceed.", InformationType.Error);
            //    err = true;
            //}
            //else
            //{
            //if (!selectedObject.IsAccepted)
            //{
            //    genCon.showMsg("Failed", "Document is not Accepted.", InformationType.Error);
            //    err = true;
            //}
            //if (!selectedObject.IsApprovalUserCheck)
            //{
            //    genCon.showMsg("Failed", "This is not a Approval User process.", InformationType.Error);
            //    err = true;
            //}
            //if (selectedObject.IsClosed)
            //{
            //    genCon.showMsg("Failed", "Document is Closed.", InformationType.Error);
            //    err = true;
            //}
            //if (selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable)
            //{
            //    genCon.showMsg("Failed", "Document is not Approval Required.", InformationType.Error);
            //    err = true;
            //}
            //}
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;

            //if (selectedObject.PurchaseOrderAppStatuses.Where(p => p.CreateUser.Oid == user.Oid).Count() > 0)
            //    appstatus = selectedObject.PurchaseOrderAppStatuses.Where(p => p.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            switch (appstatus)
            {
                case ApprovalStatuses.Required_Approval:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                    break;
                case ApprovalStatuses.Approved:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    break;
                case ApprovalStatuses.Rejected:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                    break;
            }
            ((ApprovalParameters)dv.CurrentObject).IsErr = err;
            ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void INV_CopyFromPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    int count = 0;
                    IObjectSpace os = Application.CreateObjectSpace();
                    APInvoice newinv = os.CreateObject<APInvoice>();


                    decimal Discount = 0;

                    foreach (vw_POInv dtl in e.PopupWindowViewSelectedObjects)
                    {
                        count++;
                        if (newinv.Vendor == null)
                        {
                            newinv.Vendor = newinv.Session.GetObjectByKey<vw_vendors>(dtl.BoCode);
                        }
                        if (count == 1)
                        {
                            newinv.CurrRate = 0;
                        }
                        if (newinv.CurrRate == 0)
                        {
                            newinv.CurrRate = dtl.CurrRate;
                        }

                        APInvoiceDetails newINVItem = os.CreateObject<APInvoiceDetails>();

                        newINVItem.ItemCode = newINVItem.Session.GetObjectByKey<vw_items>(dtl.ItemCode);
                        newINVItem.ItemDescrip = dtl.ItemDescrip;
                        newINVItem.UOM = dtl.UOM;
                        newINVItem.Tax = newINVItem.Session.GetObjectByKey<vw_taxes>(dtl.Tax);
                        newINVItem.UnitPrice = dtl.UnitPrice;
                        newINVItem.DocNum = dtl.SAPDocNum;
                        newINVItem.LinkOid = dtl.POLine;
                        newINVItem.Quantity = dtl.OpenQty;
                        newINVItem.Series = newINVItem.Session.GetObjectByKey<vw_ItemSeries>(dtl.Series);

                        //New added
                        //-------------------------------------------------------------------------------
                       
                        Discount += dtl.Quantity * dtl.DocDiscUnitAmount;
                        newINVItem.DocDiscUnitAmount = dtl.DocDiscUnitAmount;
                        newINVItem.DocDiscAmount = dtl.DocDiscAmount;
                        newINVItem.DocDiscAmountAfter = dtl.DocDiscUnitAmount * dtl.Quantity;
                        newINVItem.TaxAmount =Math.Round((dtl.TaxUnitAmount*dtl.Quantity),2);
                        newINVItem.TaxUnitAmount = dtl.TaxUnitAmount;

                        //newINVItem.DiscP = dtl.DiscP;

                        //New added
                        //-------------------------------------------------------------------------------


                        newinv.APInvoiceDetails.Add(newINVItem);
                    }
                    //New added
                    //-------------------------------------------------------------------------------
                    newinv.DocDiscAmount = Discount;
                    //New added
                    //-------------------------------------------------------------------------------

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newinv);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    genCon.showMsg("Success", "Copy Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected.", InformationType.Error);
            }
        }

        private void INV_CopyFromPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            APInvoice apinvoice = (APInvoice)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_POInv));
            var cs = Application.CreateCollectionSource(os, typeof(vw_POInv), viewId);
            if (apinvoice.Vendor != null)
            {
                cs.Criteria["BoCode"] = new BinaryOperator("BoCode", apinvoice.Vendor.BoCode);
            }
            cs.Criteria["Depertment"] = new BinaryOperator("Depertment", apinvoice.Department.Oid);
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void INV_CopyFromGRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    IObjectSpace os = Application.CreateObjectSpace();
                    APInvoice newinv = os.CreateObject<APInvoice>();

                    foreach (vw_GRNInv dtl in e.PopupWindowViewSelectedObjects)
                    {
                        newinv.Vendor = newinv.Session.GetObjectByKey<vw_vendors>(dtl.BoCode);

                        APInvoiceDetails newINVItem = os.CreateObject<APInvoiceDetails>();

                        newINVItem.ItemCode = newINVItem.Session.GetObjectByKey<vw_items>(dtl.ItemCode);
                        newINVItem.ItemDescrip = dtl.ItemDescrip;
                        newINVItem.UOM = dtl.UOM;
                        newINVItem.Quantity = dtl.OpenQty;
                        newINVItem.Tax = newINVItem.Session.GetObjectByKey<vw_taxes>(dtl.Tax);
                        newINVItem.UnitPrice = dtl.UnitPrice;
                        newINVItem.DocNum = dtl.SAPDocNum;
                        newINVItem.LinkOid = dtl.GRNLine;

                        newinv.APInvoiceDetails.Add(newINVItem);
                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newinv);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    genCon.showMsg("Success", "Copy Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected.", InformationType.Error);
            }
        }

        private void INV_CopyFromGRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            APInvoice apinvoice = (APInvoice)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_GRNInv));
            var cs = Application.CreateCollectionSource(os, typeof(vw_GRNInv), viewId);
            if (apinvoice.Vendor != null)
            {
                cs.Criteria["BoCode"] = new BinaryOperator("BoCode", apinvoice.Vendor.BoCode);
            }
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void Pass_Invoice_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Pass_Invoice_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                APInvoice selectedObject = (APInvoice)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                if (string.IsNullOrEmpty(user.CurrDept))
                    user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;

                #region post invoice
                //if (genCon.ConnectSAP(user.CurrDept))
                //{
                //    IObjectSpace ios = Application.CreateObjectSpace();
                //    APInvoice iobj = ios.GetObjectByKey<APInvoice>(selectedObject.Oid);

                //    if (!GeneralSettings.oCompany.InTransaction) GeneralSettings.oCompany.StartTransaction();
                //    int temp = 0;

                //    this.genCon.Active.SetItemValue("Enabled", false);
                //    temp = genCon.PostInvoicetoSAP(iobj);
                //    if (temp == 1)
                //    {
                //        if (GeneralSettings.oCompany.InTransaction)
                //            GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                //        ios.CommitChanges();

                selectedObject.IsSubmit = true;
                APInvoiceDocStatues ds = ObjectSpace.CreateObject<APInvoiceDocStatues>();
                ds.DocStatus = "Submit";
                ds.DocRemarks = p.ParamString;
                selectedObject.APInvoiceDocStatues.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                IObjectSpace os = Application.CreateObjectSpace();
                APInvoice prtrx = os.FindObject<APInvoice>(new BinaryOperator("Oid", selectedObject.Oid));

                foreach (APInvoiceDetails dtl in prtrx.APInvoiceDetails)
                {
                    bool closed = true;
                    IObjectSpace POos = Application.CreateObjectSpace();
                    // start ver 0.1
                    //PurchaseOrder po = POos.FindObject<PurchaseOrder>(new BinaryOperator("DocNum", dtl.DocNum));

                    IObjectSpace dos = Application.CreateObjectSpace();
                    PurchaseOrderDetails detail = dos.FindObject<PurchaseOrderDetails>(new BinaryOperator("Oid", dtl.LinkOid));

                    PurchaseOrder po = POos.FindObject<PurchaseOrder>(new BinaryOperator("Oid", detail.PurchaseOrder.Oid));
                    // end ver 0.1

                    foreach (PurchaseOrderDetails dtl2 in po.PurchaseOrderDetails)
                    {
                        if (dtl2.Oid == dtl.LinkOid && dtl2.PODocNum == dtl.DocNum)
                        {
                            dtl2.OpenQty -= dtl.Quantity;

                            // start ver 0.1
                            if (dtl2.OpenQty > 0)
                            {
                                closed = false;
                            }
                            // end ver 0.1
                        }
                        // start ver 0.2
                        else
                        {
                            if (dtl2.OpenQty > 0)
                            {
                                closed = false;
                            }
                        }
                        // end ver 0.2

                        // start ver 0.1
                        //if (dtl2.OpenQty > 0)
                        //{
                        //    closed = false;
                        //}
                        // end ver 0.1
                    }

                    if (closed == true)
                    {
                        po.IsClosed = true;

                        PurchaseOrderDocStatuses pods = POos.CreateObject<PurchaseOrderDocStatuses>();
                        pods.DocStatus = DocumentStatus.Closed;
                        pods.DocRemarks = p.ParamString;
                        po.PurchaseOrderDocStatuses.Add(pods);
                    }
                    POos.CommitChanges();
                }

                //    }
                //    else if (temp <= 0)
                //    {
                //        if (GeneralSettings.oCompany.InTransaction)
                //            GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                //        genCon.showMsg("Fails", "Post Fail.", InformationType.Error);
                //    }

                //}
                #endregion

                //IObjectSpace os = Application.CreateObjectSpace();
                //APInvoice prtrx = os.FindObject<APInvoice>(new BinaryOperator("Oid", selectedObject.Oid));
                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Submit Done.", InformationType.Success);

                // Start ver 0.3
                //ost_Invoice_Execute(sender, e);
                // End ver 0.3
            }
            else
            {
                genCon.showMsg("Fail", "No Invoice Selected/More Than One Invoice Selected.", InformationType.Error);
            }
        }

        private void EscalateUser_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                if (Escalate_SystemUser.SelectedItem != null)
                {
                    foreach (PurchaseRequests dtl in e.SelectedObjects)
                    {
                        IObjectSpace pos = Application.CreateObjectSpace();
                        PurchaseRequests CurrObject = pos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));

                        if (Escalate_SystemUser.SelectedItem.Id != null && CurrObject.ApprovalStatus == ApprovalStatuses.Required_Approval)
                        {
                            PurchaseRequestAppStatuses ds = pos.CreateObject<PurchaseRequestAppStatuses>();
                            ds.PurchaseRequest = pos.GetObjectByKey<PurchaseRequests>(CurrObject.Oid);
                            ds.AppStatus = ApprovalStatuses.Required_Approval;
                            ds.AppRemarks = "(System Escalated Info " + "- " + user.FName + " )";
                            //foreach (PurchaseRequestAppStages dtl2 in CurrObject.PurchaseRequestAppStage)
                            //{
                            //    count++;
                            //    if (count == 1)
                            //    {
                            //        ds.CreateUser = ObjectSpace.GetObjectByKey<SystemUsers>(dtl2.Approval.ApprovalUser.Where(p => p.UserApproval != null).First().Oid);
                            //    }
                            //}
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse("02868725-036A-483B-A123-2932180D4A03"));
                            CurrObject.PurchaseRequestAppStatus.Add(ds);

                            CurrObject.Escalate = pos.FindObject<SystemUsers>(CriteriaOperator.Parse("FName = ?", Escalate_SystemUser.SelectedItem.Id));
                            CurrObject.AppUser = CurrObject.Escalate.UserName;

                            pos.CommitChanges();
                            pos.Refresh();

                            //IObjectSpace os = Application.CreateObjectSpace();
                            //PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", CurrObject.Oid));
                            //genCon.openNewView(os, prtrx, ViewEditMode.View);
                            genCon.showMsg("Successful", "Escalate Done.", InformationType.Success);

                            IObjectSpace eos = Application.CreateObjectSpace();
                            PurchaseRequests po = eos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));

                            #region Alert User
                            List<string> ToEmails = new List<string>();
                            string emailsubject = "";
                            string emailbody = "";
                            string emailaddress = "";
                            Guid emailuser;
                            DateTime emailtime = DateTime.Now;

                            IObjectSpace emailos = Application.CreateObjectSpace();

                            emailbody = "Dear " + po.Escalate.FName + "," + System.Environment.NewLine + System.Environment.NewLine +
                                "Please click following link to approve the Purchase order. (" + po.DocNum + ")" + System.Environment.NewLine +
                                GeneralSettings.appurl + "#ViewID=PurchaseRequests_DetailView&ObjectKey=" + po.Oid.ToString() + "&ObjectClassName=PurchaseRequests.Module.BusinessObjects.PurchaseRequests&mode=View" +
                                System.Environment.NewLine + System.Environment.NewLine +
                                "Escalate To: " + po.Escalate.FName + System.Environment.NewLine + System.Environment.NewLine +
                                "Regards," + System.Environment.NewLine +
                                po.CreateUser.FName;

                            emailsubject = "Purchase Order Approval";
                            emailaddress = po.Escalate.UserEmail;
                            emailuser = po.Escalate.Oid;

                            ToEmails.Add(emailaddress);

                            if (ToEmails.Count > 0)
                            {
                                if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                                {
                                    if (emailos.IsModified)
                                        emailos.CommitChanges();
                                }
                            }
                            #endregion
                        }
                        //else
                        //{
                        //    genCon.showMsg("Fail", "No Escalate User Selected.", InformationType.Error);
                        //}
                    }
                }

                ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
            }
            else
            {
                genCon.showMsg("Fail", "No PO selected.", InformationType.Error);
            }
        }

        private void EscalateUser_PO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                if (Escalate_SystemUser.SelectedItem != null)
                {
                    foreach (PurchaseOrder dtl in e.SelectedObjects)
                    {
                        IObjectSpace pos = Application.CreateObjectSpace();
                        PurchaseOrder CurrObject = pos.FindObject<PurchaseOrder>(new BinaryOperator("Oid", dtl.Oid));

                        if (Escalate_SystemUser.SelectedItem != null && CurrObject.ApprovalStatus == ApprovalStatuses.Required_Approval)
                        {
                            PurchaseOrderAppStatuses ds = pos.CreateObject<PurchaseOrderAppStatuses>();
                            ds.PurchaseOrder = pos.GetObjectByKey<PurchaseOrder>(CurrObject.Oid); ;
                            ds.AppStatus = ApprovalStatuses.Required_Approval;
                            ds.AppRemarks = "(System Escalated Info " + "- " + user.FName + " )";
                            //foreach (PurchaseRequestAppStages dtl2 in CurrObject.PurchaseRequestAppStage)
                            //{
                            //    count++;
                            //    if (count == 1)
                            //    {
                            //        ds.CreateUser = ObjectSpace.GetObjectByKey<SystemUsers>(dtl2.Approval.ApprovalUser.Where(p => p.UserApproval != null).First().Oid);
                            //    }
                            //}
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse("02868725-036A-483B-A123-2932180D4A03"));
                            CurrObject.PurchaseOrderAppStatuses.Add(ds);

                            CurrObject.Escalate = pos.FindObject<SystemUsers>(CriteriaOperator.Parse("FName = ?", Escalate_SystemUser.SelectedItem.Id));
                            CurrObject.AppUser = CurrObject.Escalate.UserName;

                            pos.CommitChanges();
                            pos.Refresh();

                            //IObjectSpace os = Application.CreateObjectSpace();
                            //PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", CurrObject.Oid));
                            //genCon.openNewView(os, prtrx, ViewEditMode.View);
                            genCon.showMsg("Successful", "Escalate Done.", InformationType.Success);

                            IObjectSpace eos = Application.CreateObjectSpace();
                            PurchaseOrder po = eos.FindObject<PurchaseOrder>(new BinaryOperator("Oid", dtl.Oid));

                            #region Alert User
                            List<string> ToEmails = new List<string>();
                            string emailsubject = "";
                            string emailbody = "";
                            string emailaddress = "";
                            Guid emailuser;
                            DateTime emailtime = DateTime.Now;

                            IObjectSpace emailos = Application.CreateObjectSpace();

                            emailbody = "Dear " + po.Escalate.FName + "," + System.Environment.NewLine + System.Environment.NewLine +
                                "Please click following link to approve the Purchase Order. (" + po.DocNum + ")" + System.Environment.NewLine +
                                GeneralSettings.appurl + "#ViewID=PurchaseOrder_DetailView&ObjectKey=" + po.Oid.ToString() + "&ObjectClassName=PurchaseRequests.Module.BusinessObjects.PurchaseOrder&mode=View" +
                                System.Environment.NewLine + System.Environment.NewLine +
                                "Escalate To: " + po.Escalate.FName + System.Environment.NewLine + System.Environment.NewLine +
                                "Regards," + System.Environment.NewLine +
                                po.CreateUser.FName;

                            emailsubject = "Purchase Order Approval";
                            emailaddress = po.Escalate.UserEmail;
                            emailuser = po.Escalate.Oid;

                            ToEmails.Add(emailaddress);

                            if (ToEmails.Count > 0)
                            {
                                if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                                {
                                    if (emailos.IsModified)
                                        emailos.CommitChanges();
                                }
                            }
                            #endregion
                            //else
                            //{
                            //    genCon.showMsg("Fail", "No Escalate User Selected.", InformationType.Error);
                            //}
                        }
                    }
                }
                else
                {
                    genCon.showMsg("Fail", "Please select escalate user.", InformationType.Error);
                }

                ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
            }
            else
            {
                genCon.showMsg("Fail", "No PO selected.", InformationType.Error);
            }
        }

        private void Closed_PO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.IsClosed = true;
            selectedObject.IsRejected = false;
            PurchaseOrderDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderDocStatuses>();
            ds.DocStatus = DocumentStatus.Closed;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseOrderDocStatuses.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));
            genCon.openNewView(os, prtrx, ViewEditMode.View);
            genCon.showMsg("Successful", "Closed Done.", InformationType.Success);
        }

        private void Closed_PO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Cancel_GRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                GoodReceipt selectedObject = (GoodReceipt)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsClosed = true;
                GoodReceiptDocStatus ds = ObjectSpace.CreateObject<GoodReceiptDocStatus>();
                ds.DocStatus = "Cancel";
                ds.DocRemarks = p.ParamString;
                selectedObject.GoodReceiptDocStatus.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                IObjectSpace gos = Application.CreateObjectSpace();
                GoodReceipt grntrx = gos.FindObject<GoodReceipt>(new BinaryOperator("Oid", selectedObject.Oid));

                foreach (GoodReceiptDetails dtl in grntrx.GoodReceiptDetails)
                {
                    IObjectSpace POos = Application.CreateObjectSpace();
                    PurchaseOrder po = POos.FindObject<PurchaseOrder>(new BinaryOperator("DocNum", dtl.DocNum));

                    foreach (PurchaseOrderDetails dtl2 in po.PurchaseOrderDetails)
                    {
                        if (dtl2.Oid == dtl.POOid && dtl2.PODocNum == dtl.DocNum)
                        {
                            dtl2.OpenQty += dtl.Quantity;
                        }
                    }

                    POos.CommitChanges();
                }

                IObjectSpace os = Application.CreateObjectSpace();
                GoodReceipt prtrx = os.FindObject<GoodReceipt>(new BinaryOperator("Oid", selectedObject.Oid));
                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Document Cancelled.", InformationType.Success);

            }
            catch (Exception)
            {
                genCon.showMsg("Fail", "Cancellation Fail.", InformationType.Error);
            }
        }

        private void Cancel_GRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Cancel_Invoice_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                try
                {
                    APInvoice selectedObject = (APInvoice)e.CurrentObject;
                    StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                    if (p.IsErr) return;

                    selectedObject.IsCancel = true;
                    APInvoiceDocStatues ds = ObjectSpace.CreateObject<APInvoiceDocStatues>();
                    ds.DocStatus = "Cancel";
                    ds.DocRemarks = p.ParamString;
                    selectedObject.APInvoiceDocStatues.Add(ds);

                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();

                    //IObjectSpace gos = Application.CreateObjectSpace();
                    //APInvoice invtrx = gos.FindObject<APInvoice>(new BinaryOperator("Oid", selectedObject.Oid));

                    //foreach (APInvoiceDetails dtl in invtrx.APInvoiceDetails)
                    //{
                    //    bool closed = true;
                    //    IObjectSpace POos = Application.CreateObjectSpace();
                    //    PurchaseOrder po = POos.FindObject<PurchaseOrder>(new BinaryOperator("DocNum", dtl.DocNum));

                    //    foreach (PurchaseOrderDetails dtl2 in po.PurchaseOrderDetails)
                    //    {
                    //        if (dtl2.Oid == dtl.LinkOid && dtl2.PODocNum == dtl.DocNum)
                    //        {
                    //            dtl2.OpenQty += dtl.Quantity;
                    //        }

                    //        if (dtl2.OpenQty > 0)
                    //        {
                    //            closed = false;
                    //        }
                    //    }

                    //    if (closed == false)
                    //    {
                    //        po.IsClosed = false;
                    //    }

                    //    POos.CommitChanges();
                    //}

                    IObjectSpace os = Application.CreateObjectSpace();
                    APInvoice prtrx = os.FindObject<APInvoice>(new BinaryOperator("Oid", selectedObject.Oid));
                    genCon.openNewView(os, prtrx, ViewEditMode.View);
                    genCon.showMsg("Successful", "Document Cancelled.", InformationType.Success);

                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Cancellation Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No Invoice Selected/More Than One Invoice Selected.", InformationType.Error);
            }
        }

        private void Cancel_Invoice_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void BudgetInvoice_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void Escalate_SystemUser_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {

        }

        private void BudgetPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void BudgetPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            PurchaseOrder po = (PurchaseOrder)View.CurrentObject;

            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetBudget", new OperandValue(po.DocDate), new OperandValue(user.DefaultDept.BoCode));

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_POBudget));
            var cs = Application.CreateCollectionSource(os, typeof(vw_POBudget), viewId);
            cs.Criteria["PO"] = new BinaryOperator("PO", po.Oid, BinaryOperatorType.Equal);
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void BudgetInvoice_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            APInvoice invoice = (APInvoice)View.CurrentObject;

            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetBudget", new OperandValue(invoice.DocDate), new OperandValue(user.DefaultDept.BoCode));

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_InvoiceBudget));
            var cs = Application.CreateCollectionSource(os, typeof(vw_InvoiceBudget), viewId);
            cs.Criteria["ApInv"] = new BinaryOperator("ApInv", invoice.Oid, BinaryOperatorType.Equal);
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void Cancel_PO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                try
                {
                    PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
                    StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                    if (p.IsErr) return;

                    selectedObject.IsCancelled = true;
                    selectedObject.IsRejected = false;
                    PurchaseOrderDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderDocStatuses>();
                    ds.DocStatus = DocumentStatus.Cancelled;
                    ds.DocRemarks = p.ParamString;
                    selectedObject.PurchaseOrderDocStatuses.Add(ds);
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();

                    // Start ver 0.9
                    IObjectSpace pos = Application.CreateObjectSpace();
                    PurchaseOrder trx = pos.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));

                    if (trx.IsCancelled == true)
                    {
                        if (trx.BudgetCategoryData != null)
                        {
                            genCon.UpdateBudget(trx.Department.Oid, trx.BudgetCategoryData.BudgetCategoryName,
                                trx.DocDate.Month, trx.DocDate.Year.ToString(), trx.Amount, pos, "Cancel");
                        }
                    }
                    // End ver 0.9

                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", selectedObject.Oid));
                    genCon.openNewView(os, prtrx, ViewEditMode.View);
                    genCon.showMsg("Successful", "Document Cancelled.", InformationType.Success);

                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Cancellation Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected/More Than One PO Selected.", InformationType.Error);
            }
        }

        private void Cancel_PO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Post_Invoice_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                //int totaldoc = 0;
                //int succdoc = 0;
                //int faildoc = 0;

                //StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                //if (p.IsErr) return;

                //SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                //if (string.IsNullOrEmpty(user.CurrDept))
                //    user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;

                //foreach (APInvoice dtl in e.SelectedObjects)
                //{
                //    IObjectSpace aos = Application.CreateObjectSpace();
                //    APInvoice selectedObject = aos.FindObject<APInvoice>(new BinaryOperator("Oid", dtl.Oid));

                //    #region post invoice
                //    if (genCon.ConnectSAP(user.CurrDept))
                //    {
                //        IObjectSpace ios = Application.CreateObjectSpace();
                //        APInvoice iobj = ios.GetObjectByKey<APInvoice>(selectedObject.Oid);

                //        if (!GeneralSettings.oCompany.InTransaction) GeneralSettings.oCompany.StartTransaction();
                //        int temp = 0;

                //        this.genCon.Active.SetItemValue("Enabled", false);
                //        temp = genCon.PostInvoicetoSAP(iobj);
                //        if (temp == 1)
                //        {
                //            if (GeneralSettings.oCompany.InTransaction)
                //                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                //            ios.CommitChanges();

                //            selectedObject.IsClosed = true;
                //            APInvoiceDocStatues ds = aos.CreateObject<APInvoiceDocStatues>();
                //            ds.DocStatus = "Closed";
                //            ds.DocRemarks = p.ParamString;
                //            selectedObject.APInvoiceDocStatues.Add(ds);

                //            aos.CommitChanges();
                //            aos.Refresh();

                //            succdoc++;
                //        }
                //        else if (temp <= 0)
                //        {
                //            if (GeneralSettings.oCompany.InTransaction)
                //                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                //            faildoc++;
                //            //genCon.showMsg("Fails", "Post Fail.", InformationType.Error);
                //        }

                //    }
                //    #endregion

                //    totaldoc++;
                //}

                ////IObjectSpace os = Application.CreateObjectSpace();
                ////APInvoice prtrx = os.FindObject<APInvoice>(new BinaryOperator("Oid", selectedObject.Oid));
                ////genCon.openNewView(os, prtrx, ViewEditMode.View);
                ////genCon.showMsg("Successful", "Submit Done.", InformationType.Success);
                //ObjectSpace.CommitChanges(); //This line persists created object(s).
                //ObjectSpace.Refresh();

                //genCon.showMsg("Info", "Total Document : " + totaldoc + Environment.NewLine +
                //               "Successful Posted : " + succdoc + Environment.NewLine +
                //               "Failed Posted : " + faildoc, InformationType.Info);
            
                APInvoice selectedObject = (APInvoice)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;


                IObjectSpace ios = Application.CreateObjectSpace();
                APInvoice iobj = ios.GetObjectByKey<APInvoice>(selectedObject.Oid);

                if (iobj.APInvoiceStatus != "Posted")
                {
                    #region post PO

                    SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                    if (string.IsNullOrEmpty(user.CurrDept))
                        user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;
                    ///////////////HERE
                    if (genCon.ConnectSAP(user.CurrDept))
                    {
                        ///////////////


                        //APInvoice selectedSI = (APInvoice)View.CurrentObject;
                        //if (!GeneralSettings.oCompany.InTransaction) GeneralSettings.oCompany.StartTransaction();

                        int temp = 0;

                        //ShowViewParameters svp = new ShowViewParameters();
                        //DetailView dv = Application.CreateDetailView(ios, iobj);
                        //dv.ViewEditMode = ViewEditMode.View;
                        //svp.CreatedView = dv;


                        if (iobj.APInvoiceStatus != "Posted")
                        {
                            this.genCon.Active.SetItemValue("Enabled", false);
                            temp = genCon.PostAPIVtoSAP2(iobj);
                            if (temp == 1)
                            {
                                if (GeneralSettings.oCompany.InTransaction)
                                    GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                             
                                ios.CommitChanges();

                                string DocEntry = "";
                                DocEntry = GeneralSettings.oCompany.GetNewObjectKey();
                                iobj.IsPosted = true;
                                iobj.SAPINVNum = DocEntry;
                                DocEntry = null;

                               //IObjectSpace aos = Application.CreateObjectSpace();
                                APInvoiceDocStatues ds = ios.CreateObject<APInvoiceDocStatues>();
                                ds.DocStatus = "Posted";
                                ds.DocRemarks = p.ParamString;
                                iobj.APInvoiceDocStatues.Add(ds);
                                ios.CommitChanges();
                                //ios.Refresh();

                                IObjectSpace os = Application.CreateObjectSpace();
                                genCon.showMsg("Successful", "Posting Done.", InformationType.Success);
                            }
                            else if (temp <= 0)
                            {
                                if (GeneralSettings.oCompany.InTransaction)
                                    GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                                IObjectSpace os = Application.CreateObjectSpace();
                                APInvoice prtrx = os.FindObject<APInvoice>(new BinaryOperator("Oid", selectedObject.Oid));
                                genCon.openNewView(os, prtrx, ViewEditMode.View);
                                genCon.showMsg("Failed", "Posting Failed."+GeneralSettings.oCompany.GetLastErrorDescription(), InformationType.Error);
                            }
                            GeneralSettings.oCompany.Disconnect();
                        }
                    }
                    #endregion

                    //ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
            }
            else
            {
                genCon.showMsg("Fail", "No Invoice Selected/More Than One Invoice Selected.", InformationType.Error);
            }
        }

        private void Post_Invoice_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void DepartmantFilter_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            // Start ver 0.6
            //if (e.SelectedChoiceActionItem.Id != null)
            //{
            //    ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] = ?",
            //        e.SelectedChoiceActionItem.Id);
            //}
            if (e.SelectedChoiceActionItem.Id != "All")
            {
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] = ?",
                    e.SelectedChoiceActionItem.Id);
            }
            else
            {
                //if (View.Id == "PurchaseOrder_ListView")
                //{
                    //IObjectSpace os = Application.CreateObjectSpace();
                    //CollectionSourceBase cs = Application.CreateCollectionSource(os, typeof(PurchaseOrder), "PurchaseOrder_ListView");
                    //IModelListView listViewModel = (IModelListView)Application.Model.Views["PurchaseOrder_ListView"];
                    //ListView lv = Application.CreateListView(listViewModel, cs, true);
                    //Application.MainWindow.SetView(lv);

                    ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode] != ?", "All");
                //}
                //else if (View.Id == "APInvoice_ListView")
                //{
                //    IObjectSpace os = Application.CreateObjectSpace();
                //    CollectionSourceBase cs = Application.CreateCollectionSource(os, typeof(APInvoice), "APInvoice_ListView");
                //    IModelListView listViewModel = (IModelListView)Application.Model.Views["APInvoice_ListView"];
                //    ListView lv = Application.CreateListView(listViewModel, cs, true);
                //    Application.MainWindow.SetView(lv);
                //}
                //// Start ver 0.7
                //else if (View.Id == "PurchaseOrderJapan_ListView")
                //{
                //    IObjectSpace os = Application.CreateObjectSpace();
                //    CollectionSourceBase cs = Application.CreateCollectionSource(os, typeof(PurchaseOrderJapan), "PurchaseOrderJapan_ListView");
                //    IModelListView listViewModel = (IModelListView)Application.Model.Views["PurchaseOrderJapan_ListView"];
                //    ListView lv = Application.CreateListView(listViewModel, cs, true);
                //    Application.MainWindow.SetView(lv);
                //}
                //// end ver 0.7
            }
            // End ver 0.6
        }


        protected string GetScript(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            return @"var newWindow = window.open('about:blank', '_blank');
            newWindow.document.write('<iframe src=""Download.aspx?filename=" + fileInfo.Name + "&embedded=true" + @""" frameborder =""0"" allowfullscreen style=""width: 100%;height: 100%"" type=""application/pdf""></iframe>');
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


        private void PrintPO1_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;

            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                PurchaseOrder PO = (PurchaseOrder)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\PO.rpt"));
                    strServer = GeneralSettings.B1Server;
                    //strDatabase = GeneralSettings.B1CompanyDB;
                    strDatabase = "HRS_PR";
                    strUserID = GeneralSettings.B1DbUserName;
                    strPwd = GeneralSettings.B1DbPassword;
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);

                    doc.SetParameterValue("DocNum", PO.Oid);


                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_PO_" + PO.DocNum + "_" + user.UserName + "_"
                       + DateTime.Parse(PO.DocDate.ToString()).ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + ".pdf";

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
                genCon.showMsg("Fail", "No PO Selected.", InformationType.Error);
            }
        }

        private void Duplicate_PO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                try
                {
                    PurchaseOrder PurchaseOrder = (PurchaseOrder)View.CurrentObject;
                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrder newPO = os.CreateObject<PurchaseOrder>();

                    newPO.DocDate = DateTime.Today;
                    newPO.Vendor = newPO.Session.GetObjectByKey<vw_vendors>(PurchaseOrder.Vendor.BoCode);
                    newPO.Department = newPO.Session.GetObjectByKey<Departments>(PurchaseOrder.Department.Oid);
                    newPO.RefNo = PurchaseOrder.RefNo;
                    // newPR.ApprovalStatus = PurchaseRequest.ApprovalStatus;
                    newPO.DocType = newPO.Session.GetObjectByKey<DocTypes>(PurchaseOrder.DocType.Oid);
                    newPO.IsBudget = PurchaseOrder.IsBudget;
                    newPO.Remarks = PurchaseOrder.Remarks;
                    newPO.DocDisc = PurchaseOrder.DocDisc;

                    foreach (PurchaseOrderDetails dtl in PurchaseOrder.PurchaseOrderDetails)
                    {
                        PurchaseOrderDetails newPOdetails = os.CreateObject<PurchaseOrderDetails>();

                        newPOdetails.Item = newPOdetails.Session.GetObjectByKey<vw_items>(dtl.Item.BoCode);
                        newPOdetails.ItemDesc = dtl.ItemDesc;
                        newPOdetails.Series = newPOdetails.Session.GetObjectByKey<vw_ItemSeries>(dtl.Series.BoCode);
                        newPOdetails.DocDate = dtl.DocDate;
                        newPOdetails.DelDate = dtl.DelDate;
                        newPOdetails.Quantity = dtl.Quantity;
                        newPOdetails.UOM = dtl.UOM;
                        newPOdetails.DiscP = dtl.DiscP;
                        newPOdetails.LineTotal = dtl.LineTotal;
                        newPOdetails.RefNo = dtl.RefNo;
                        newPOdetails.Remarks = dtl.Remarks;
                        newPOdetails.Amount = dtl.Amount;
                        newPOdetails.TaxAmount = dtl.TaxAmount;
                        if (dtl.Tax != null)
                        {
                            newPOdetails.Tax = newPOdetails.Session.GetObjectByKey<vw_taxes>(dtl.Tax.BoCode);
                        }
                        newPO.PurchaseOrderDetails.Add(newPOdetails);

                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newPO);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //genCon.openNewView(os, newPR, ViewEditMode.Edit);
                    genCon.showMsg("Success", "Duplicate Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Duplicate Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No item selected/More than one item selected.", InformationType.Error);
            }
        }

        // Start ver 0.5
        protected string GetAttactment(string DocNum)
        {
            return @"var newWindow = window.open();
            newWindow.document.write('<iframe src=""Attachments.aspx?DocNum=" + DocNum + @""" frameborder =""0"" allowfullscreen style=""width: 100%;height: 100%;""></iframe>');
            ";

        }

        private void CompareAtt_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
                //// add po - Y.J
                ///

                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;
                var reportFolderPath = HttpContext.Current.Server.MapPath("~/Attachments/" + selectedObject.DocNum); //change the "~/Reports" to your report folder name

                ReportDocument doc = new ReportDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\PO.rpt"));
                strServer = GeneralSettings.B1Server;
                strDatabase = GeneralSettings.ReportDB; //"HRS_PR"; 
                strUserID = GeneralSettings.B1DbUserName;
                strPwd = GeneralSettings.B1DbPassword;
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);

                doc.SetParameterValue("DocNum", selectedObject.Oid);


                filename = "PO_" + selectedObject.DocNum.ToString() + ".pdf";

                if (!System.IO.Directory.Exists(reportFolderPath))
                {
                    System.IO.Directory.CreateDirectory(reportFolderPath);
                }


                if (File.Exists(reportFolderPath + "\\" + filename))
                {

                    File.Delete(reportFolderPath + "\\" + filename);
                }
                doc.ExportToDisk(ExportFormatType.PortableDocFormat, reportFolderPath + "\\" + filename);
                doc.Close();
                doc.Dispose();
                ///

                WebWindow.CurrentRequestWindow.RegisterStartupScript("ViewAtt", GetAttactment(selectedObject.DocNum));
            }
            else
            {
                genCon.showMsg("Fail", "No item selected/More than one item selected.", InformationType.Error);
            }

            //foreach (PurchaseOrder newPO in View.ObjectSpace.CreateCollection(typeof(PurchaseOrder), null))
            //{
            //    foreach (PurchaseOrderAttachments dtl2 in newPO.PurchaseOrderAttachments)
            //    {
            //        string tempFileLocation = string.Concat(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\", "\\", dtl2.File.FileName);
            //        try
            //        {
            //            FileData fd = ObjectSpace.FindObject<FileData>(CriteriaOperator.Parse("Oid = ?", dtl2.File)); // Use any other IObjectSpace APIs to query required data.  

            //            Stream sourceStream = new MemoryStream();
            //            ((IFileData)fd).SaveToStream(sourceStream);
            //            sourceStream.Position = 0;

            //            FileInfo fileInfo = new FileInfo(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
            //            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
            //            if (!dirInfo.Exists) dirInfo.Create();

            //            using (var fileStream = new FileStream(tempFileLocation, FileMode.Create, FileAccess.Write))
            //            {
            //                sourceStream.CopyTo(fileStream);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //        }
            //    }
            //}
        }
        // End ver 0.5

        // Start 0.6
        private void LACancel_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                try
                {
                    SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                    ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                    int cnt = 0;
                    foreach (LeaveApplication dtl in e.SelectedObjects)
                    {
                        if (dtl.Status != LeaveStatus.Open)
                        {
                            IObjectSpace los = Application.CreateObjectSpace();
                            LeaveApplication leave = los.FindObject<LeaveApplication>(new BinaryOperator("Oid", dtl.Oid));

                            leave.Status = LeaveStatus.Cancel;

                            los.CommitChanges(); //This line persists created object(s).
                            los.Refresh();

                            cnt++;
                        }
                    }

                    ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
                    genCon.showMsg("Info", "Total Leave application : " + cnt + " Cancel Done.", InformationType.Info);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Cancellation Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No leave application Selected.", InformationType.Error);
            }
        }

        private void LACancel_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void PrintDepartmentBudget_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            BudgetCategoryReport selectedObject = (BudgetCategoryReport)e.CurrentObject;

            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                BudgetCategoryReport budget = (BudgetCategoryReport)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    if (budget.SummaryDetails == false)
                    {
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\BudgetCategory.rpt"));
                    }
                    else
                    {
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\BudgetCategoryDetails.rpt"));
                    }
                    strServer = GeneralSettings.B1Server;
                    //strDatabase = GeneralSettings.B1CompanyDB;
                    strDatabase = "HRS_PR";
                    strUserID = GeneralSettings.B1DbUserName;
                    strPwd = GeneralSettings.B1DbPassword;
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);

                    if (budget.Department != null)
                    {
                        doc.SetParameterValue("Department@", budget.Department.BoCode);
                    }
                    doc.SetParameterValue("FromDate@", budget.FromDate.Date);
                    doc.SetParameterValue("ToDate@", budget.ToDate.Date);
                    if (budget.BudgetCategory != null)
                    {
                        doc.SetParameterValue("BudgetCategoryOid@", budget.BudgetCategory.Oid);
                    }

                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_DepBudget_" + budget.Department + "_" + user.UserName + "_"
                       + DateTime.Now.ToString("hhmmss") + ".pdf";

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
                genCon.showMsg("Fail", "No Department Selected.", InformationType.Error);
            }
        }
        // End ver 0.6

        // Start ver 0.7
        private void LevelOfApp_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {

        }

        private void Pass_JapanPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                PurchaseOrderJapan selectedObject = (PurchaseOrderJapan)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsPassed = true;
                selectedObject.IsAccepted = true;
                selectedObject.IsRejected = false;
                PurchaseOrderJapanDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderJapanDocStatuses>();
                ds.DocStatus = DocumentStatus.Accepted;
                ds.DocRemarks = p.ParamString;
                selectedObject.PurchaseOrderJapanDocStatuses.Add(ds);

                PurchaseOrderJapanAppStatuses dsApp = ObjectSpace.CreateObject<PurchaseOrderJapanAppStatuses>();
                dsApp.AppStatus = ApprovalStatuses.Not_Applicable;
                dsApp.AppRemarks = "";
                selectedObject.PurchaseOrderJapanAppStatuses.Add(dsApp);

                //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

                //ModificationsController controller = Frame.GetController<ModificationsController>();
                //if (controller != null)
                //{
                //    controller.SaveAction.DoExecute();
                //}
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                #region approval
                XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                List<string> ToEmails = new List<string>();
                string emailbody = "";
                string emailsubject = "";
                string emailaddress = "";
                Guid emailuser;
                DateTime emailtime = DateTime.Now;
                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetApproval", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("PurchaseOrdersJapan"));

                IObjectSpace emailos = Application.CreateObjectSpace();
                //EmailSents emailobj = null;

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        string receiver = null;
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            if (receiver == null)
                            {
                                receiver = row.Values[4].ToString();
                            }
                            else
                            {
                                receiver = receiver + "/" + row.Values[4];
                            }
                        }
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            emailbody = "Dear " + receiver + "," + System.Environment.NewLine + System.Environment.NewLine +
                                row.Values[3] + System.Environment.NewLine + GeneralSettings.appurl + row.Values[2].ToString() +
                                System.Environment.NewLine + System.Environment.NewLine + "Regards," + System.Environment.NewLine +
                                   //selectedObject.CreateUser.FName.ToString();
                                   "E-PO System";

                            emailsubject = "Purchase Order Japan Approval";
                            emailaddress = row.Values[1].ToString();
                            emailuser = (Guid)row.Values[0];

                            ToEmails.Add(emailaddress);

                            //emailobj = emailos.CreateObject<EmailSents>();
                            //emailobj.CreateDate = (DateTime?)emailtime;
                            //emailobj.EmailUser = emailobj.Session.GetObjectByKey<SystemUsers>(emailuser);
                            //emailobj.EmailAddress = emailaddress;
                            ////assign body will get error???
                            ////emailobj.EmailBody = emailbody;
                            //emailobj.Remarks = emailsubject;
                            //emailobj.PurchaseOrder = emailobj.Session.GetObjectByKey<PurchaseOrder>(selectedObject.Oid);

                            //emailobj.Save();
                        }
                    }
                }

                if (ToEmails.Count > 0)
                {
                    if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                    {
                        if (emailos.IsModified)
                            emailos.CommitChanges();
                    }
                }

                #endregion

                IObjectSpace pos = Application.CreateObjectSpace();
                PurchaseOrderJapan trx = pos.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", selectedObject.Oid));

                //if (trx.ApprovalStatus == ApprovalStatuses.Not_Applicable && trx.IsAccepted == true)
                //{
                //    if (trx.BudgetCategoryData != null)
                //    {
                //        genCon.UpdateBudget(trx.Department.Oid, trx.BudgetCategoryData.BudgetCategoryName,
                //            trx.DocDate.Month, trx.DocDate.Year.ToString(), trx.Amount, pos);
                //    }
                //}

                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseOrderJapan prtrx = os.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", selectedObject.Oid));
                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Submit to approver.", InformationType.Success);
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected/More Than One PO Selected.", InformationType.Error);
            }
        }

        private void Pass_JapanPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Duplicate_POJapan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                try
                {
                    PurchaseOrderJapan PurchaseOrder = (PurchaseOrderJapan)View.CurrentObject;
                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrderJapan newPO = os.CreateObject<PurchaseOrderJapan>();

                    newPO.DocDate = DateTime.Today;
                    newPO.Vendor = newPO.Session.GetObjectByKey<vw_vendors>(PurchaseOrder.Vendor.BoCode);
                    newPO.Department = newPO.Session.GetObjectByKey<Departments>(PurchaseOrder.Department.Oid);
                    newPO.RefNo = PurchaseOrder.RefNo;
                    // newPR.ApprovalStatus = PurchaseRequest.ApprovalStatus;
                    newPO.DocType = newPO.Session.GetObjectByKey<DocTypes>(PurchaseOrder.DocType.Oid);
                    newPO.IsBudget = PurchaseOrder.IsBudget;
                    newPO.Remarks = PurchaseOrder.Remarks;
                    newPO.DocDisc = PurchaseOrder.DocDisc;

                    foreach (PurchaseOrderJapanDetails dtl in PurchaseOrder.PurchaseOrderJapanDetails)
                    {
                        PurchaseOrderJapanDetails newPOdetails = os.CreateObject<PurchaseOrderJapanDetails>();

                        newPOdetails.Item = newPOdetails.Session.GetObjectByKey<vw_items>(dtl.Item.BoCode);
                        newPOdetails.ItemDesc = dtl.ItemDesc;
                        newPOdetails.Series = newPOdetails.Session.GetObjectByKey<vw_ItemSeries>(dtl.Series.BoCode);
                        newPOdetails.DocDate = dtl.DocDate;
                        newPOdetails.DelDate = dtl.DelDate;
                        newPOdetails.Quantity = dtl.Quantity;
                        newPOdetails.UOM = dtl.UOM;
                        newPOdetails.DiscP = dtl.DiscP;
                        newPOdetails.LineTotal = dtl.LineTotal;
                        newPOdetails.RefNo = dtl.RefNo;
                        newPOdetails.Remarks = dtl.Remarks;
                        newPOdetails.Amount = dtl.Amount;
                        newPOdetails.TaxAmount = dtl.TaxAmount;
                        if (dtl.Tax != null)
                        {
                            newPOdetails.Tax = newPOdetails.Session.GetObjectByKey<vw_taxes>(dtl.Tax.BoCode);
                        }
                        newPO.PurchaseOrderJapanDetails.Add(newPOdetails);

                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newPO);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //genCon.openNewView(os, newPR, ViewEditMode.Edit);
                    genCon.showMsg("Success", "Duplicate Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Duplicate Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No item selected/More than one item selected.", InformationType.Error);
            }
        }

        private void Closed_POJapan_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrderJapan selectedObject = (PurchaseOrderJapan)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.IsClosed = true;
            selectedObject.IsRejected = false;
            PurchaseOrderJapanDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderJapanDocStatuses>();
            ds.DocStatus = DocumentStatus.Closed;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseOrderJapanDocStatuses.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            PurchaseOrderJapan prtrx = os.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", selectedObject.Oid));
            genCon.openNewView(os, prtrx, ViewEditMode.View);
            genCon.showMsg("Successful", "Closed Done.", InformationType.Success);
        }

        private void Closed_POJapan_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void PrintPOJapan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PurchaseOrderJapan selectedObject = (PurchaseOrderJapan)e.CurrentObject;

            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                PurchaseOrderJapan PO = (PurchaseOrderJapan)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\POJapan.rpt"));
                    strServer = GeneralSettings.B1Server;
                    //strDatabase = GeneralSettings.B1CompanyDB;
                    strDatabase = "HRS_PR";
                    strUserID = GeneralSettings.B1DbUserName;
                    strPwd = GeneralSettings.B1DbPassword;
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);

                    doc.SetParameterValue("DocNum", PO.Oid);


                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_POJapan_" + PO.DocNum + "_" + user.UserName + "_"
                       + DateTime.Parse(PO.DocDate.ToString()).ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + ".pdf";

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
                genCon.showMsg("Fail", "No PO Selected.", InformationType.Error);
            }
        }

        private void EscalateUserJapan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                if (Escalate_SystemUser.SelectedItem != null)
                {
                    foreach (PurchaseOrderJapan dtl in e.SelectedObjects)
                    {
                        IObjectSpace pos = Application.CreateObjectSpace();
                        PurchaseOrderJapan CurrObject = pos.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", dtl.Oid));

                        if (Escalate_SystemUser.SelectedItem != null && CurrObject.ApprovalStatus == ApprovalStatuses.Required_Approval)
                        {
                            PurchaseOrderJapanAppStatuses ds = pos.CreateObject<PurchaseOrderJapanAppStatuses>();
                            ds.PurchaseOrderJapan = pos.GetObjectByKey<PurchaseOrderJapan>(CurrObject.Oid); ;
                            ds.AppStatus = ApprovalStatuses.Required_Approval;
                            ds.AppRemarks = "(System Escalated Info " + "- " + user.FName + " )";
                            //foreach (PurchaseRequestAppStages dtl2 in CurrObject.PurchaseRequestAppStage)
                            //{
                            //    count++;
                            //    if (count == 1)
                            //    {
                            //        ds.CreateUser = ObjectSpace.GetObjectByKey<SystemUsers>(dtl2.Approval.ApprovalUser.Where(p => p.UserApproval != null).First().Oid);
                            //    }
                            //}
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse("02868725-036A-483B-A123-2932180D4A03"));
                            CurrObject.PurchaseOrderJapanAppStatuses.Add(ds);

                            CurrObject.Escalate = pos.FindObject<SystemUsers>(CriteriaOperator.Parse("FName = ?", Escalate_SystemUser.SelectedItem.Id));
                            CurrObject.AppUser = CurrObject.Escalate.UserName;

                            pos.CommitChanges();
                            pos.Refresh();

                            //IObjectSpace os = Application.CreateObjectSpace();
                            //PurchaseOrder prtrx = os.FindObject<PurchaseOrder>(new BinaryOperator("Oid", CurrObject.Oid));
                            //genCon.openNewView(os, prtrx, ViewEditMode.View);
                            genCon.showMsg("Successful", "Escalate Done.", InformationType.Success);

                            IObjectSpace eos = Application.CreateObjectSpace();
                            PurchaseOrderJapan po = eos.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", dtl.Oid));

                            #region Alert User
                            List<string> ToEmails = new List<string>();
                            string emailsubject = "";
                            string emailbody = "";
                            string emailaddress = "";
                            Guid emailuser;
                            DateTime emailtime = DateTime.Now;

                            IObjectSpace emailos = Application.CreateObjectSpace();

                            emailbody = "Dear " + po.Escalate.FName + "," + System.Environment.NewLine + System.Environment.NewLine +
                                "Please click following link to approve the Purchase Order Japan. (" + po.DocNum + ")" + System.Environment.NewLine +
                                GeneralSettings.appurl + "#ViewID=PurchaseOrderJapan_DetailView&ObjectKey=" + po.Oid.ToString() + "&ObjectClassName=PurchaseRequests.Module.BusinessObjects.PurchaseOrderJapan&mode=View" +
                                System.Environment.NewLine + System.Environment.NewLine +
                                "Escalate To: " + po.Escalate.FName + System.Environment.NewLine + System.Environment.NewLine +
                                "Regards," + System.Environment.NewLine +
                                po.CreateUser.FName;

                            emailsubject = "Purchase Order Japan Approval";
                            emailaddress = po.Escalate.UserEmail;
                            emailuser = po.Escalate.Oid;

                            ToEmails.Add(emailaddress);

                            if (ToEmails.Count > 0)
                            {
                                if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                                {
                                    if (emailos.IsModified)
                                        emailos.CommitChanges();
                                }
                            }
                            #endregion
                            //else
                            //{
                            //    genCon.showMsg("Fail", "No Escalate User Selected.", InformationType.Error);
                            //}
                        }
                    }
                }
                else
                {
                    genCon.showMsg("Fail", "Please select escalate user.", InformationType.Error);
                }

               ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
            }
            else
            {
                genCon.showMsg("Fail", "No PO selected.", InformationType.Error);
            }
        }

        private void Cancel_POJapan_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                try
                {
                    PurchaseOrderJapan selectedObject = (PurchaseOrderJapan)e.CurrentObject;
                    StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                    if (p.IsErr) return;

                    selectedObject.IsCancelled = true;
                    selectedObject.IsRejected = false;
                    PurchaseOrderJapanDocStatuses ds = ObjectSpace.CreateObject<PurchaseOrderJapanDocStatuses>();
                    ds.DocStatus = DocumentStatus.Cancelled;
                    ds.DocRemarks = p.ParamString;
                    selectedObject.PurchaseOrderJapanDocStatuses.Add(ds);
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();

                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrderJapan prtrx = os.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", selectedObject.Oid));
                    genCon.openNewView(os, prtrx, ViewEditMode.View);
                    genCon.showMsg("Successful", "Document Cancelled.", InformationType.Success);

                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Cancellation Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected/More Than One PO Selected.", InformationType.Error);
            }
        }

        private void Cancel_POJapan_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void BudgetPOJapan_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void BudgetPOJapan_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            PurchaseOrderJapan po = (PurchaseOrderJapan)View.CurrentObject;

            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetBudget", new OperandValue(po.DocDate), new OperandValue(user.DefaultDept.BoCode));

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vw_POBudget));
            var cs = Application.CreateCollectionSource(os, typeof(vw_POBudget), viewId);
            cs.Criteria["PO"] = new BinaryOperator("PO", po.Oid, BinaryOperatorType.Equal);
            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void Approval_POJapan_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                int totaldoc = 0;

                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                foreach (PurchaseOrderJapan dtl in e.SelectedObjects)
                {
                    IObjectSpace pos = Application.CreateObjectSpace();
                    PurchaseOrderJapan po = pos.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", dtl.Oid));

                    // Start ver 0.4
                    if (po.NextApprover != null)
                    {
                        // End ver 0.4
                        po.Escalate = null;
                        po.WhoApprove = po.WhoApprove + user.UserName + ", ";
                        ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;

                        //if (selectedObject.PurchaseOrderAppStatuses.Count() > 0)
                        //    appstatus = selectedObject.PurchaseOrderAppStatuses.OrderBy(c => c.Oid).Last().AppStatus;

                        //if (appstatus != ApprovalStatuses.Not_Applicable)
                        //    if (po.PurchaseOrderAppStatuses.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                        //        appstatus = po.PurchaseOrderAppStatuses.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

                        if (appstatus == ApprovalStatuses.Not_Applicable)
                            appstatus = ApprovalStatuses.Required_Approval;


                        if (p.IsErr) return;
                        if (appstatus == ApprovalStatuses.Required_Approval && p.AppStatus == ApprovalActions.NA)
                        {
                            genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatuses.Approved && p.AppStatus == ApprovalActions.Yes)
                        {
                            genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatuses.Rejected && p.AppStatus == ApprovalActions.No)
                        {
                            genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        if (p.AppStatus == ApprovalActions.NA)
                        {
                            appstatus = ApprovalStatuses.Required_Approval;
                        }
                        if (p.AppStatus == ApprovalActions.Yes)
                        {
                            appstatus = ApprovalStatuses.Approved;
                        }
                        if (p.AppStatus == ApprovalActions.No)
                        {
                            appstatus = ApprovalStatuses.Rejected;
                        }

                        PurchaseOrderJapanAppStatuses ds = pos.CreateObject<PurchaseOrderJapanAppStatuses>();
                        ds.PurchaseOrderJapan = pos.GetObjectByKey<PurchaseOrderJapan>(po.Oid);
                        ds.AppStatus = appstatus;
                        if (appstatus == ApprovalStatuses.Rejected)
                        {
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Reject User: " + user.FName + ")" +
                                System.Environment.NewLine + "(Reason: Approval Rejected)";
                            ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse(" 02868725-036A-483B-A123-2932180D4A03"));
                        }
                        else
                        {
                            ds.AppRemarks = p.ParamString +
                                System.Environment.NewLine + "(Approved User: " + user.FName + ")";
                        }
                        po.PurchaseOrderJapanAppStatuses.Add(ds);

                        pos.CommitChanges();
                        pos.Refresh();

                        totaldoc++;

                        #region approval
                        XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

                        List<string> ToEmails = new List<string>();
                        string emailbody = "";
                        string emailsubject = "";
                        string emailaddress = "";
                        Guid emailuser;
                        DateTime emailtime = DateTime.Now;
                        SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_Approval", new OperandValue(user.UserName), new OperandValue(po.Oid), new OperandValue("PurchaseOrderJapan"), new OperandValue((int)appstatus));

                        IObjectSpace emailos = Application.CreateObjectSpace();
                        EmailSents emailobj = null;

                        if (sprocData.ResultSet.Count() > 0)
                        {
                            if (sprocData.ResultSet[0].Rows.Count() > 0)
                            {
                                string receiver = null;
                                foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                                {
                                    if (receiver == null)
                                    {
                                        receiver = row.Values[4].ToString();
                                    }
                                    else
                                    {
                                        receiver = receiver + "/" + row.Values[4];
                                    }
                                }

                                foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                                {
                                    emailbody = "Dear " + receiver + "," + System.Environment.NewLine + System.Environment.NewLine +
                                        row.Values[3] + System.Environment.NewLine + GeneralSettings.appurl + row.Values[2].ToString() +
                                        System.Environment.NewLine + System.Environment.NewLine + "Regards," + System.Environment.NewLine +
                                        //po.CreateUser.FName.ToString();
                                        "E-PO System";

                                    if (appstatus == ApprovalStatuses.Approved)
                                        emailsubject = "Purchase Order Japan Approval";
                                    else if (appstatus == ApprovalStatuses.Rejected)
                                        emailsubject = "Purchase Order Japan Approval Rejected";

                                    emailaddress = row.Values[1].ToString();
                                    emailuser = (Guid)row.Values[0];

                                    ToEmails.Add(emailaddress);

                                    //emailobj = emailos.CreateObject<EmailSents>();
                                    //emailobj.CreateDate = (DateTime?)emailtime;
                                    //emailobj.EmailUser = emailobj.Session.GetObjectByKey<SystemUsers>(emailuser);
                                    //emailobj.EmailAddress = emailaddress;
                                    ////assign body will get error???
                                    ////emailobj.EmailBody = emailbody;
                                    //emailobj.Remarks = emailsubject;
                                    //emailobj.PurchaseOrder = emailobj.Session.GetObjectByKey<PurchaseOrder>(po.Oid);

                                    //emailobj.Save();
                                }
                            }
                        }

                        if (ToEmails.Count > 0)
                        {
                            if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                            {
                                if (emailos.IsModified)
                                    emailos.CommitChanges();
                            }
                        }
                        #endregion
                    }

                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseOrderJapan prtrx = os.FindObject<PurchaseOrderJapan>(new BinaryOperator("Oid", dtl.Oid));

                    //if (prtrx.ApprovalStatus == ApprovalStatuses.Approved)
                    //{
                    //    if (prtrx.BudgetCategoryData != null)
                    //    {
                    //        genCon.UpdateBudget(prtrx.Department.Oid, prtrx.BudgetCategoryData.BudgetCategoryName,
                    //            prtrx.DocDate.Month, prtrx.DocDate.Year.ToString(), prtrx.Amount, os);
                    //    }
                    //}

                    //genCon.openNewView(os, prtrx, ViewEditMode.View);
                    //genCon.showMsg("Successful", "Approval Done.", InformationType.Success);

                    ObjectSpace.CommitChanges(); //This line persists created object(s).
                    ObjectSpace.Refresh();

                    genCon.showMsg("Info", "Total Document : " + totaldoc + " Approval Done.", InformationType.Info);
                    // Start ver 0.4
                }
                // End ver 0.4
            }
            else
            {
                genCon.showMsg("Fail", "No PO selected.", InformationType.Error);
            }
        }

        private void Approval_POJapan_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            PurchaseOrderJapan selectedObject = (PurchaseOrderJapan)View.CurrentObject;
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;

            //if (selectedObject.PurchaseOrderAppStatuses.Where(p => p.CreateUser.Oid == user.Oid).Count() > 0)
            //    appstatus = selectedObject.PurchaseOrderAppStatuses.Where(p => p.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            switch (appstatus)
            {
                case ApprovalStatuses.Required_Approval:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                    break;
                case ApprovalStatuses.Approved:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    break;
                case ApprovalStatuses.Rejected:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                    break;
            }
            ((ApprovalParameters)dv.CurrentObject).IsErr = err;
            ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CompareAttJapan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                PurchaseOrderJapan selectedObject = (PurchaseOrderJapan)e.CurrentObject;
                //// add po - Y.J
                ///

                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;
                var reportFolderPath = HttpContext.Current.Server.MapPath("~/Attachments/" + selectedObject.DocNum); //change the "~/Reports" to your report folder name

                ReportDocument doc = new ReportDocument();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\POJapan.rpt"));
                strServer = GeneralSettings.B1Server;
                strDatabase = GeneralSettings.ReportDB; //"HRS_PR"; 
                strUserID = GeneralSettings.B1DbUserName;
                strPwd = GeneralSettings.B1DbPassword;
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);

                doc.SetParameterValue("DocNum", selectedObject.Oid);


                filename = "POJapan_" + selectedObject.DocNum.ToString() + ".pdf";

                if (!System.IO.Directory.Exists(reportFolderPath))
                {
                    System.IO.Directory.CreateDirectory(reportFolderPath);
                }


                if (File.Exists(reportFolderPath + "\\" + filename))
                {

                    File.Delete(reportFolderPath + "\\" + filename);
                }
                doc.ExportToDisk(ExportFormatType.PortableDocFormat, reportFolderPath + "\\" + filename);
                doc.Close();
                doc.Dispose();
                ///

                WebWindow.CurrentRequestWindow.RegisterStartupScript("ViewAtt", GetAttactment(selectedObject.DocNum));
            }
            else
            {
                genCon.showMsg("Fail", "No item selected/More than one item selected.", InformationType.Error);
            }

            //foreach (PurchaseOrder newPO in View.ObjectSpace.CreateCollection(typeof(PurchaseOrder), null))
            //{
            //    foreach (PurchaseOrderAttachments dtl2 in newPO.PurchaseOrderAttachments)
            //    {
            //        string tempFileLocation = string.Concat(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\", "\\", dtl2.File.FileName);
            //        try
            //        {
            //            FileData fd = ObjectSpace.FindObject<FileData>(CriteriaOperator.Parse("Oid = ?", dtl2.File)); // Use any other IObjectSpace APIs to query required data.  

            //            Stream sourceStream = new MemoryStream();
            //            ((IFileData)fd).SaveToStream(sourceStream);
            //            sourceStream.Position = 0;

            //            FileInfo fileInfo = new FileInfo(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
            //            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
            //            if (!dirInfo.Exists) dirInfo.Create();

            //            using (var fileStream = new FileStream(tempFileLocation, FileMode.Create, FileAccess.Write))
            //            {
            //                sourceStream.CopyTo(fileStream);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //        }
            //    }
            //}
        }

        private void DuplicateBudget_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                try
                {
                    BudgetCategory budget = (BudgetCategory)View.CurrentObject;
                    IObjectSpace os = Application.CreateObjectSpace();
                    BudgetCategory newbudget = os.CreateObject<BudgetCategory>();

                    foreach (BudgetCategoryDetails dtl in budget.BudgetCategoryDetails)
                    {
                        BudgetCategoryDetails newbudgetdetail = os.CreateObject<BudgetCategoryDetails>();

                        newbudgetdetail.BudgetCategory = newbudgetdetail.Session.GetObjectByKey<BudgetCategoryData>(dtl.BudgetCategory.Oid);
                        newbudgetdetail.YearlyBudgetBalance = dtl.YearlyBudgetBalance;

                        foreach (BudgetCategoryAmount dtl2 in dtl.BudgetCategoryAmount)
                        {
                            BudgetCategoryAmount newbudgetamount = os.CreateObject<BudgetCategoryAmount>();

                            newbudgetamount.Month = dtl2.Month;
                            newbudgetamount.Year = dtl2.Year;
                            newbudgetamount.Budget = dtl2.Budget;
                            newbudgetamount.MonthlyBudgetBalance = dtl2.Budget;

                            newbudgetdetail.BudgetCategoryAmount.Add(newbudgetamount);
                        }

                        newbudget.BudgetCategoryDetails.Add(newbudgetdetail);

                    }

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newbudget);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //genCon.openNewView(os, newPR, ViewEditMode.Edit);
                    genCon.showMsg("Success", "Duplicate Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Duplicate Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No item selected/More than one item selected.", InformationType.Error);
            }
        }

        private void DuplicateBudgetData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                try
                {
                    BudgetCategoryData budgetdata = (BudgetCategoryData)View.CurrentObject;
                    IObjectSpace os = Application.CreateObjectSpace();
                    BudgetCategoryData newbudgetdata = os.CreateObject<BudgetCategoryData>();

                    newbudgetdata.BudgetCategoryName = budgetdata.BudgetCategoryName;

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(os, newbudgetdata);
                    dv.ViewEditMode = ViewEditMode.Edit;
                    dv.IsRoot = true;
                    svp.CreatedView = dv;

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //genCon.openNewView(os, newPR, ViewEditMode.Edit);
                    genCon.showMsg("Success", "Duplicate Success.", InformationType.Success);
                }
                catch (Exception)
                {
                    genCon.showMsg("Fail", "Duplicate Fail.", InformationType.Error);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No item selected/More than one item selected.", InformationType.Error);
            }
        }
        // End ver 0.7
    }
}
