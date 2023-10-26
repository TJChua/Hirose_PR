using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using BSI_PR.Module.BusinessObjects;

namespace BSI_PR.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppUpdatingModuleUpdatertopic.aspx
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FindObject<DomainObject1>(CriteriaOperator.Parse("Name=?", name));
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}

            string temp = "";
            string tempname = "";

            //ObjectSpace.CommitChanges();

            temp = GeneralSettings.defdept;
            tempname = "NA";
            Departments department = ObjectSpace.FindObject<Departments>(new BinaryOperator("BoCode", temp));
            if (department == null)
            {
                department = ObjectSpace.CreateObject<Departments>();
                department.BoCode = temp;
                department.BoName = tempname;
            }

            SystemUsers sampleUser = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "User"));
            if (sampleUser == null)
            {
                sampleUser = ObjectSpace.CreateObject<SystemUsers>();
                sampleUser.UserName = "User";
                sampleUser.DefaultDept = department;
                sampleUser.SetPassword("");
            }
            PermissionPolicyRole defaultRole = CreateDefaultRole();
            sampleUser.Roles.Add(defaultRole);

            SystemUsers userAdmin = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "Admin"));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<SystemUsers>();
                userAdmin.UserName = "Admin";
                userAdmin.DefaultDept = department;
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
            }
            // If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null)
            {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;
            userAdmin.Roles.Add(adminRole);

            temp = GeneralSettings.hq;
            tempname = "HQ";
            Companies company = ObjectSpace.FindObject<Companies>(new BinaryOperator("BoCode", temp));
            if (company == null)
            {
                company = ObjectSpace.CreateObject<Companies>();
                company.BoCode = temp;
                company.BoName = tempname;
                company.ESAPDoc = ESAPDocs.VendorPO;
            }

            temp = GeneralSettings.POsuperuserrole;
            PermissionPolicyRole role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = temp;
                role.Save();
            }

            temp = GeneralSettings.PRsuperuserrole;
            role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = temp;
                role.Save();
            }

            temp = GeneralSettings.PRuserrole;
            role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = temp;
                role.Save();
            }

            temp = GeneralSettings.verifyrole;
            role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = temp;
                role.Save();
            }

            temp = GeneralSettings.Acceptancerole;
            role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = temp;
                role.Save();
            }

            temp = GeneralSettings.postrole;
            role = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", temp));
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = temp;
                role.Save();
            }

            PermissionPolicyRole PORole = CreatePOUserRole();
            PermissionPolicyRole GRNRole = CreateGRNUserRole();
            PermissionPolicyRole GRRole = CreateGRUserRole();
            PermissionPolicyRole INVRole = CreateINVUserRole();
            PermissionPolicyRole AppRole = CreateAppUserRole();
            ObjectSpace.CommitChanges(); //This line persists created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
        private PermissionPolicyRole CreateDefaultRole()
        {
            PermissionPolicyRole defaultRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Default"));
            if (defaultRole == null)
            {
                defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                defaultRole.Name = "Default";

                defaultRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //defaultRole.AddTypePermissionsRecursively<OpenPO>(SecurityOperations.Create, SecurityPermissionState.Allow);
                //defaultRole.AddTypePermissionsRecursively<ClosePO>(SecurityOperations.Create, SecurityPermissionState.Allow);
                //defaultRole.AddTypePermissionsRecursively<BudgetReport>(SecurityOperations.Create, SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Reports/Items/OpenPOReport_DetailView", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Reports/Items/ClosePOReport_DetailView", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Reports/Items/BudgetReport_DetailView", SecurityPermissionState.Allow);
            }
            return defaultRole;
        }

        private PermissionPolicyRole CreatePOUserRole()
        {
            PermissionPolicyRole POUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "POUserRole"));
            if (POUserRole == null)
            {
                POUserRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                POUserRole.Name = "POUserRole";

                POUserRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                POUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                POUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                POUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                POUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order", SecurityPermissionState.Allow);
                POUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order/Items/PurchaseOrder_ListView", SecurityPermissionState.Allow);
                POUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order/Items/Purchase Order Approved", SecurityPermissionState.Allow);
                POUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order/Items/Purchase Order Pending Approval", SecurityPermissionState.Allow);

                //PO
                POUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //SAP
                POUserRole.AddTypePermissionsRecursively<vw_costcenter>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_GRN>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_items>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_ItemSeries>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_PO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_RecvPO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_PR>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_taxes>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_vendors>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_Currency>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_GRNInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_InvoiceBudget>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<vw_POInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //SystemUsers
                POUserRole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //Doctype
                POUserRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //Department
                POUserRole.AddTypePermissionsRecursively<Departments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<DepartmentDocs>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //File data
                POUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Create, SecurityPermissionState.Allow);
                POUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Delete, SecurityPermissionState.Allow);
            }
            return POUserRole;
        }

        private PermissionPolicyRole CreateGRNUserRole()
        {
            PermissionPolicyRole GRNUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "GRNUserRole"));
            if (GRNUserRole == null)
            {
                GRNUserRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                GRNUserRole.Name = "GRNUserRole";

                GRNUserRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                GRNUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                GRNUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                GRNUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                GRNUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Goods Receipt PO", SecurityPermissionState.Allow);
                GRNUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Goods Receipt PO/Items/GoodReceipt_ListView", SecurityPermissionState.Allow);

                //GRN
                GRNUserRole.AddTypePermissionsRecursively<GoodReceipt>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceipt>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceiptDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceiptDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceiptAttachment>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceiptAttachment>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceiptDocStatus>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<GoodReceiptDocStatus>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //PO
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //SAP
                GRNUserRole.AddTypePermissionsRecursively<vw_costcenter>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_GRN>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_items>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_ItemSeries>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_PO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_RecvPO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_PR>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_taxes>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_vendors>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_Currency>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_GRNInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_InvoiceBudget>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<vw_POInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //SystemUsers
                GRNUserRole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //Doctype
                GRNUserRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //Department
                GRNUserRole.AddTypePermissionsRecursively<Departments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<DepartmentDocs>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //File data
                GRNUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRNUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Delete, SecurityPermissionState.Allow);
            }
            return GRNUserRole;
        }

        private PermissionPolicyRole CreateGRUserRole()
        {
            PermissionPolicyRole GRUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "GRUserRole"));
            if (GRUserRole == null)
            {
                GRUserRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                GRUserRole.Name = "GRUserRole";

                GRUserRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                GRUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                GRUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                GRUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                GRUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Goods Return", SecurityPermissionState.Allow);
                GRUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Goods Return/Items/GoodsReturn_ListView", SecurityPermissionState.Allow);

                //GR
                GRUserRole.AddTypePermissionsRecursively<GoodsReturn>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturn>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturnAttachements>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturnAttachements>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturnDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturnDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturnDocStatus>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodsReturnDocStatus>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //GRN
                GRUserRole.AddTypePermissionsRecursively<GoodReceipt>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceipt>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceiptDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceiptDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceiptAttachment>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceiptAttachment>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceiptDocStatus>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<GoodReceiptDocStatus>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //PO
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //SAP
                GRUserRole.AddTypePermissionsRecursively<vw_costcenter>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_GRN>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_items>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_ItemSeries>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_PO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_RecvPO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_PR>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_taxes>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_vendors>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_Currency>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_GRNInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_InvoiceBudget>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<vw_POInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //SystemUsers
                GRUserRole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //Doctype
                GRUserRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //Department
                GRUserRole.AddTypePermissionsRecursively<Departments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<DepartmentDocs>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //File data
                GRUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Create, SecurityPermissionState.Allow);
                GRUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Delete, SecurityPermissionState.Allow);
            }
            return GRUserRole;
        }

        private PermissionPolicyRole CreateINVUserRole()
        {
            PermissionPolicyRole InvoiceUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "InvoiceUserRole"));
            if (InvoiceUserRole == null)
            {
                InvoiceUserRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                InvoiceUserRole.Name = "InvoiceUserRole";

                InvoiceUserRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                InvoiceUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                InvoiceUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                InvoiceUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                InvoiceUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/A/P Invoice", SecurityPermissionState.Allow);
                InvoiceUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/A/P Invoice/Items/APInvoice_ListView", SecurityPermissionState.Allow);

                //AP Invoice
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoice>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoice>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoiceDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoiceDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoiceAttachment>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoiceAttachment>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoiceDocStatues>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<APInvoiceDocStatues>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //GRN
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceipt>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceipt>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceiptDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceiptDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceiptAttachment>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceiptAttachment>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceiptDocStatus>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<GoodReceiptDocStatus>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //PO
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //SAP
                InvoiceUserRole.AddTypePermissionsRecursively<vw_costcenter>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_GRN>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_items>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_ItemSeries>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_PO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_RecvPO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_PR>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_taxes>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_vendors>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_Currency>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_GRNInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_InvoiceBudget>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<vw_POInv>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //SystemUsers
                InvoiceUserRole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //Doctype
                InvoiceUserRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //Department
                InvoiceUserRole.AddTypePermissionsRecursively<Departments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<DepartmentDocs>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //File data
                InvoiceUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Create, SecurityPermissionState.Allow);
                InvoiceUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Delete, SecurityPermissionState.Allow);
            }
            return InvoiceUserRole;
        }

        private PermissionPolicyRole CreateAppUserRole()
        {
            PermissionPolicyRole ApprovalUserRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "ApprovalUserRole"));
            if (ApprovalUserRole == null)
            {
                ApprovalUserRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                ApprovalUserRole.Name = "ApprovalUserRole";

                ApprovalUserRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
                ApprovalUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                ApprovalUserRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Request", SecurityPermissionState.Allow);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Request/Items/PurchaseRequests_ListView", SecurityPermissionState.Deny);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Request/Items/Approved", SecurityPermissionState.Allow);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Request/Items/Correction", SecurityPermissionState.Allow);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order", SecurityPermissionState.Allow);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order/Items/PurchaseOrder_ListView", SecurityPermissionState.Deny);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order/Items/Purchase Order Approved", SecurityPermissionState.Allow);
                ApprovalUserRole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchase Order/Items/Purchase Order Pending Approval", SecurityPermissionState.Allow);

                //PO
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequests>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequests>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestAppStages>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestAppStages>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestAppStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestAppStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestAttachments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestAttachments>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestDocStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseRequestDocStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //PO
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderDetails>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStages>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderAppStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderAttachments>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<PurchaseOrderDocStatuses>(SecurityOperations.Create, SecurityPermissionState.Allow);

                //SAP
                ApprovalUserRole.AddTypePermissionsRecursively<vw_costcenter>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_GRN>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_items>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_ItemSeries>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_PO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_RecvPO>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_PR>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_taxes>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<vw_vendors>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //SystemUsers
                ApprovalUserRole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.ReadOnlyAccess, SecurityPermissionState.Allow);

                //Doctype
                ApprovalUserRole.AddTypePermissionsRecursively<DocTypes>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //Department
                ApprovalUserRole.AddTypePermissionsRecursively<Departments>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<DepartmentDocs>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);

                //File data
                ApprovalUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Create, SecurityPermissionState.Allow);
                ApprovalUserRole.AddTypePermissionsRecursively<FileData>(SecurityOperations.Delete, SecurityPermissionState.Allow);
            }
            return ApprovalUserRole;
        }
    }
}
