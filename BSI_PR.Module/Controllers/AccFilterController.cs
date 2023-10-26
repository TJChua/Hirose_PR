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
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;

#region update log
// TJC - 20230510 - New enhancement ver 0.1

#endregion

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class AccFilterController : ViewController
    {
        public AccFilterController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            if (string.IsNullOrEmpty(user.CurrDept))
                user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;

            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            {
                if (View.Id == "PurchaseRequests_ListView")
                {
                    PermissionPolicyRole PRRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('PRUserRole')"));

                    if (PRRole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                }
            }

            //if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            //{
            //    if (View.Id == "PurchaseRequests_ListView_SAPStatus")
            //    {
            //        PermissionPolicyRole PRRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('PRUserRole')"));

            //        if (PRRole != null)
            //        {

            //           // ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[CreateUser] = ?", user.Oid);
            //           ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[CreateUser] = ? AND [IsPosted]=?", user.Oid,true);
            //        }
            //    }
            //}
            ///////////////////////////////////////////////////

            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            {
                if (View.Id == "PurchaseRequests_ListView_Approved")
                {
                    PermissionPolicyRole AdminRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));
                    PermissionPolicyRole PRSuperUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('PRSuperUserRole')"));

                    //if (AdminRole == null)
                    //{
                    //    //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[PurchaseRequestStatus] in (?,?,?)", "Accepted", "Posted", "Rejected");
                    //    ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [Department.BoCode] = ?", "Approved", user.DefaultDept.BoCode);
                    //}

                    if (PRSuperUserRole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [Department.BoCode] = ?", "Approved", user.DefaultDept.BoCode);
                    }
                    else
                    {
                        PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                        if (AppRole != null)
                        {
                            //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[PurchaseRequestStatus] in (?,?,?)", "Accepted", "Posted", "Rejected");
                            //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ?", "Approved");
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("Contains([WhoApprove],?)", user.UserName);

                        }
                    }
                }
            }

            //if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            //{
            //    if (View.Id == "PurchaseRequests_ListView_Correction")
            //    {
            //        PermissionPolicyRole AdminRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));
            //        PermissionPolicyRole PRSuperUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('PRSuperUserRole')"));

            //        if (AdminRole == null)
            //        {

            //            //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ?", "Not_Applicable");
            //              ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [ApprovalUser] = ?", user.Oid);
            //            // ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[CreateUser] = ?", user.Oid);

            //        }
            //    }
            //}

            //if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            //{
            //    if (View.Id == "PurchaseRequests_ListView_SAPStatus")
            //    {
            //        PermissionPolicyRole AdminRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));

            //        if (AdminRole != null)
            //        {                  
            //            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[IsPosted]=?", true);
            //        }
            //    }
            //}

            ///////////////////////////////////////////////////

            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            {
                if (View.Id == "PurchaseRequests_ListView")
                {
                    PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                    if (AppRole != null)
                    {


                        //  ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[PurchaseRequestStatus] in (?,?) and [ApprovalStatus] = ?", "Passed" , "Correction_Pass", "Required_Approval");

                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                     

                    }
                }
            }
            //if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            //{
            //    if (View.Id == "PurchaseRequests_ListView_Approved")
            //    {
            //        PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

            //        if (AppRole != null)
            //        {
            //            //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[PurchaseRequestStatus] in (?,?,?)", "Accepted", "Posted", "Rejected");
            //            //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ?", "Approved");
            //            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and Contains([WhoApprove],?)", "Approved", user.FName);

            //        }
            //    }
            //}

            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            {
                if (View.Id == "PurchaseRequests_ListView_Correction")
                {
                    PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                    if (AppRole != null)
                    {
                        //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [ApprovalUser] = ?", 2, user.Oid);
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and Contains([AppUser],?)", 2, user.UserName);
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                if (View.Id == "PurchaseOrder_ListView")
                {
                    PermissionPolicyRole porole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('POUserRole')"));
                    PermissionPolicyRole ViewRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ReadOnlyUser')"));

                    if (porole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                    else if (ViewRole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);

                    }


                }


            }

            if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                if (View.Id == "PurchaseOrder_ListView_Approved")
                {
                    PermissionPolicyRole PRSuperUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('PRSuperUserRole')"));

                    if (PRSuperUserRole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [Department.BoCode] = ?", "Approved", user.DefaultDept.BoCode);
                    }
                    else
                    {
                        PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                        if (AppRole != null)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("Contains([WhoApprove],?)", user.UserName);

                        }
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                if (View.Id == "PurchaseOrder_ListView_PendingApp")
                {
                    PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                    if (AppRole != null)
                    {
                        //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [ApprovalUser] = ?", 2, user.Oid);
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and Contains([AppUser],?)", 2, user.UserName);
                    }
                }
            }

            // Start ver 0.1
            if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderJapan))
            {
                if (View.Id == "PurchaseOrderJapan_ListView")
                {
                    PermissionPolicyRole porole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('POUserRole')"));
                    PermissionPolicyRole ViewRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ReadOnlyUser')"));

                    if (porole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                    else if (ViewRole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);

                    }


                }


            }

            if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderJapan))
            {
                if (View.Id == "PurchaseOrderJapan_ListView_Approved")
                {
                    PermissionPolicyRole PRSuperUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('PRSuperUserRole')"));

                    if (PRSuperUserRole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [Department.BoCode] = ?", "Approved", user.DefaultDept.BoCode);
                    }
                    else
                    {
                        PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                        if (AppRole != null)
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("Contains([WhoApprove],?)", user.UserName);

                        }
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderJapan))
            {
                if (View.Id == "PurchaseOrderJapan_ListView_PendingApp")
                {
                    PermissionPolicyRole AppRole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ApprovalUserRole')"));

                    if (AppRole != null)
                    {
                        //((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and [ApprovalUser] = ?", 2, user.Oid);
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [ApprovalStatus] = ? and Contains([AppUser],?)", 2, user.UserName);
                    }
                }
            }
            // End ver 0.1

            if (View.ObjectTypeInfo.Type == typeof(GoodReceipt))
            {
                if (View.Id == "GoodReceipt_ListView")
                {
                    PermissionPolicyRole grnrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('GRNUserRole')"));

                    if (grnrole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(GoodsReturn))
            {
                if (View.Id == "GoodsReturn_ListView")
                {
                    PermissionPolicyRole grrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('GRUserRole')"));

                    if (grrole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(APInvoice))
            {
                if (View.Id == "APInvoice_ListView")
                {
                    PermissionPolicyRole invrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('InvoiceUserRole')"));
                    PermissionPolicyRole invReadOnlyrole = ObjectSpace.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('ReadOnlyUser')"));

                    if (invrole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                    else if (invReadOnlyrole != null)
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse(" [Department.BoCode]=?", user.DefaultDept.BoCode);
                    }
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
