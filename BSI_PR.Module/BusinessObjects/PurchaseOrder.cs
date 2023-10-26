using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Purchase Order")]
    [NavigationItem("Purchase Order")]
    [DefaultProperty("DocNum")]
    [RuleCriteria("PurchaseOrderDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [Appearance("HideNew", AppearanceItemType = "Action", TargetItems = "New", Criteria = "not IsPRUserCheck", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideCopyFrom", AppearanceItemType.Action, "True", TargetItems = "CopyFromPR", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "ListView")]
    [Appearance("HideCopyFrom1", AppearanceItemType.Action, "True", TargetItems = "CopyFromPR", Criteria ="not IsNew or Vendor == null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HidePass", AppearanceItemType.Action, "True", TargetItems = "Pass_PO", Criteria = "(not (PurchaseRequestStatus = 'New' or PurchaseRequestStatus = 'Rejected')) or DocNum=null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideApproval", AppearanceItemType = "Action", TargetItems = "Approval_PO", Criteria = "((PurchaseRequestStatus != 'Accepted') or (ApprovalStatus != 'Required_Approval' ))", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideApproval1", AppearanceItemType = "Action", TargetItems = "Approval_PO", Context = "PurchaseOrder_ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideApproval2", AppearanceItemType = "Action", TargetItems = "Approval_PO", Context = "PurchaseOrder_ListView_Approved", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideApproval3", AppearanceItemType = "Action", TargetItems = "Approval_PO", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEditButton", AppearanceItemType = "Action", TargetItems = "Edit", Criteria = "1=1", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEditButton2", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "(PurchaseRequestStatus = 'Cancelled') or (PurchaseRequestStatus = 'Posted') or (PurchaseRequestStatus = 'Closed') or ((PurchaseRequestStatus == 'Accepted') and (ApprovalStatus != 'Required_Approval'))", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEditButton3", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "not IsPRUserCheck", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEditButton4", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "(PurchaseRequestStatus = 'Cancel') ", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEscalate", AppearanceItemType = "Action", TargetItems = "EscalateUser_PO", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("HideEscalate1", AppearanceItemType = "Action", TargetItems = "EscalateUser_PO", Criteria = "(PurchaseRequestStatus = 'New') or (PurchaseRequestStatus = 'Cancelled') or (PurchaseRequestStatus = 'Posted') or (PurchaseRequestStatus = 'Closed') or ((PurchaseRequestStatus == 'Accepted') and (ApprovalStatus != 'Required_Approval'))", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideClosed", AppearanceItemType = "Action", TargetItems = "Closed_PO", Criteria = "(ApprovalStatus != 'Approved') OR (PurchaseRequestStatus = 'Closed') OR (PurchaseRequestStatus = 'Cancel')", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideDelete", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideCancel", AppearanceItemType = "Action", TargetItems = "Cancel_PO", Criteria = "(PurchaseRequestStatus = 'Cancel') or (ApprovalStatus = 'Required_Approval') or IsValid = 1 or IsNew = 1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("POCurrencyRateRule", DefaultContexts.Save, "IsValid2 = 0", "Incorrect currency rate.")]
   // [Appearance("HidePrintPO", AppearanceItemType = "Action", TargetItems = "PrintPO1", Criteria = "(PurchaseRequestStatus = 'Cancel') or  (ApprovalStatus = 'Required_Approval') or (ApprovalStatus = 'Rejected') or (ApprovalStatus <> 'Approved')", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("POOutsideRange", DefaultContexts.Save, "IsValid3 = 1", "Posting Period Locked")]
   
    [Appearance("HideDuplicatePO", AppearanceItemType = "Action", TargetItems = "Duplicate_PO", Criteria = "IsValid4 = 1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    //[Appearance("HideCompareAtt", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;CompareAtt", Criteria = "1=1", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("HidePrintPO", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;PrintPO1", Criteria = "1=1", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    


    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class PurchaseOrder : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrder(Session session)
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

            Department = Session.FindObject<Departments>(new BinaryOperator("BoCode", user.CurrDept, BinaryOperatorType.Equal));

            CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
            CreateDate = DateTime.Now;
            DocDate = DateTime.Today;
            PaymentTerms = Session.FindObject<vw_PaymentTerms>(new BinaryOperator("BoCode", 3, BinaryOperatorType.Equal));
            //ApprovalStatus = ApprovalStatuses.Not_Applicable;

            Company = Session.FindObject<Companies>(new BinaryOperator("BoCode", GeneralSettings.hq, BinaryOperatorType.Equal));
            if (user.Vendor != null)
            {
                string temp = user.Vendor.BoCode;
                Vendor = Session.FindObject<vw_vendors>(new BinaryOperator("BoCode", temp));
            }

            DocNumSeq = Session.FindObject<DepartmentDocs>(CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 1));

            IsBudget = true;
            IsPRSuperCheck = false;
            IsPRUserCheck = true;
            IsAcceptUserCheck = false;
            IsVerifyUserCheck = false;

           



            if (CreateUser != null)
            {

                //

                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.ApprovalRole).Count() > 0)
                {
                    IsApprovalUserCheck = true;
                }
                //

                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.POsuperuserrole).Count() > 0)
                {
                    this.IsPRSuperCheck = true;
                    this.IsPRUserCheck = true;
                }
                //else if (this.CreateUser != null)
                //    if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.PRuserrole).Count() > 0 && CreateUser.Oid == this.CreateUser.Oid)
                //        this.IsPRUserCheck = true;

                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.Acceptancerole).Count() > 0)
                    this.IsAcceptUserCheck = true;

                if (CreateUser.Roles.Where(p => p.Name == GeneralSettings.verifyrole).Count() > 0)
                    this.IsVerifyUserCheck = true;

            }

            DevExpress.Xpo.Metadata.XPClassInfo myClass = Session.GetClassInfo(typeof(DocTypes));
            CriteriaOperator myCriteria = new BinaryOperator("IsActive", true);
            SortingCollection sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("BoCode", DevExpress.Xpo.DB.SortingDirection.Ascending));

            var LDocType = Session.GetObjects(myClass, myCriteria, sortProps, 0, false, true);

            foreach (var dtl in LDocType)
            {
                if (((DocTypes)dtl).BoCode == "PO")
                {
                    this.DocType = Session.FindObject<DocTypes>(new BinaryOperator("BoCode", ((DocTypes)dtl).BoCode));
                }
            }
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue(nameof(PersistentProperty), ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("CreateUser", Enabled = false)]
        public SystemUsers CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("CreateDate", Enabled = false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private SystemUsers _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateUser", Enabled = false)]
        public SystemUsers UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateDate", Enabled = false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private DocTypes _DocType;
        [XafDisplayName("Doc Type"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(304), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public virtual DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private Companies _Company;
        [XafDisplayName("Company"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(305), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public virtual Companies Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }

        private DepartmentDocs _DocNumSeq;
        [Index(305), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Browsable(false)]
        public DepartmentDocs DocNumSeq
        {
            get { return _DocNumSeq; }
            set
            {
                SetPropertyValue("DocNumSeq", ref _DocNumSeq, value);
            }
        }

        private bool _IsBudget;
        [ImmediatePostData]
        [XafDisplayName("Budget?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(1), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsBudget", Enabled = true)]
        public bool IsBudget
        {
            get { return _IsBudget; }
            set
            {
                SetPropertyValue("IsBudget", ref _IsBudget, value);
            }
        }

        private string _DocNum;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Document No")]
        [Appearance("DocNum", Enabled = false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }


        private DateTime _DocDate;
        [ImmediatePostData]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        //[ModelDefault("DisplayFormat", "{0:D}")] //LongDate
        //[ModelDefault("EditMask", "D")]
        //[ModelDefault("DisplayFormat", "{0:d}")] //ShortDate
        //[ModelDefault("EditMask", "d")]
        //[ModelDefault("DisplayFormat", "{0:dd/MM/yyyy}")]
        //[ModelDefault("EditMask", "dd/MM/yyyy")]
        //[ModelDefault("DisplayFormat", "{0:f}")] //LongDate+ShortTime
        //[ModelDefault("EditMask", "f")]
        //[ModelDefault("DisplayFormat", "{0: ddd, dd MMMM yyyy hh:mm:ss tt}")]
        //[ModelDefault("EditMask", "ddd, dd MMMM yyyy hh:mm:ss tt")]
        [XafDisplayName("Document Date"), ToolTip("Enter Text")]
        [Appearance("DocDate", Enabled = false, Criteria = "IsPassed")]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);
                if (!IsLoading)
                { 
                    if (Currency != null)
                    {
                        Currency tmp;
                        tmp = Session.FindObject<Currency>
                            (CriteriaOperator.Parse("CurrencyCode.BoCode = ? and FromDate <= ? and ToDate >= ?", Currency, DocDate, DocDate));

                        if (tmp != null)
                        {
                            CurrRate = tmp.Rate;
                        }
                        else
                        {
                            CurrRate = 0;
                        }
                    }
                }

                
            }
        }

        private vw_vendors _Vendor;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Vendor"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [DataSourceCriteria("ValidFor = 'Y' and '@this.DocDate' >= validFrom and '@this.DocDate' <= validTo")]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Vendor", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        public vw_vendors Vendor
        {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);

                if (!IsLoading)
                {
                    if (value == null)
                    {
                        Adress = null;
                        Currency = null;

                    }
                    else
                    {
                        Adress = Vendor.BoAddress;
                        Currency = Vendor.BoCurrency;

                        if (Currency != null)
                        {
                            Currency tmp;
                            tmp = Session.FindObject<Currency>
                                (CriteriaOperator.Parse("CurrencyCode.BoCode = ? and FromDate <= ? and ToDate >= ?", Currency, DocDate, DocDate));

                            if (tmp != null)
                            {
                                CurrRate = tmp.Rate;
                            }
                            else
                            {
                                CurrRate = 0;
                            }
                        }

                    }
                }


            }
        }

        private string _Currency;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Document Currency"), ToolTip("")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(33), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Currency", Enabled = false)]
        public string Currency
        {
            get { return _Currency; }
            set
            {
                SetPropertyValue("Currency", ref _Currency, value);
            }
        }

        private Departments _Department;
        [XafDisplayName("Department"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [DataSourceProperty("CreateUser.Department")]
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [RuleRequiredField(DefaultContexts.Save)]

        [Appearance("Department", Enabled = false)]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private string _RefNo;
        [Index(30), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("RefNo", Enabled = false, Criteria = "IsPassed")]
        public string RefNo
        {
            get { return _RefNo; }
            set
            {
                SetPropertyValue("RefNo", ref _RefNo, value);
            }
        }


        private string _QuoNo;
        [Index(30), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("Quotation No")]
        public string QuoNo
        {
            get { return _QuoNo; }
            set
            {
                SetPropertyValue("QuoNo", ref _QuoNo, value);
            }
        }




        private string _Remarks;
        [Size(200)]
        [Index(44), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Remarks", Enabled = false, Criteria = "IsPassed")]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        [NonPersistent]
        [ImmediatePostData]
        [Index(40), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal Amount
        {
            get
            {
                decimal rtn = 0;
                if (PurchaseOrderDetails != null)
                    rtn += PurchaseOrderDetails.Sum(p => p.LineTotal);

                //if (PurchaseRequestDetail != null)
                //    rtn += PurchaseRequestDetail.Sum(p => p.LineTotal);

                return rtn;
            }
        }



        private decimal _DocDisc;
        [ImmediatePostData]
        [Index(41), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Discount %"), ToolTip("")]
        [Appearance("DocDisc", Enabled = false, Criteria = "IsPassed")]

        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        // [EditorAlias("VPDec")]

        public decimal DocDisc
        {
            get { return _DocDisc; }
            set
            {
                SetPropertyValue("DocDisc", ref _DocDisc, value);
            }
        }




        private decimal _DocDiscAmount;
        [ImmediatePostData]
        [Index(41), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Discount Amount"), ToolTip("")]
        [Appearance("DocDisc Amount", Enabled = false, Criteria = "IsPassed")]

        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        // [EditorAlias("VPDec")]

        public decimal DocDiscAmount
        {
            get { return _DocDiscAmount; }
            set
            {
                SetPropertyValue("DocDiscAmount", ref _DocDiscAmount, value);
            }
        }




        //[NonPersistent]
        private decimal _TotalTaxAmount;
        [ImmediatePostData]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(18,6)")]
        [Index(42), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("TotalTaxAmount", Enabled = false)]


        public decimal TotalTaxAmount
        {
            get
            {
                decimal rtn = 0;
                decimal TaxSum = 0;

                if (DocDiscAmount > 0)
                {
                    decimal subtotal = 0;
                    if (PurchaseOrderDetails != null)
                        subtotal += PurchaseOrderDetails.Sum(p => p.LineTotal);

                    foreach (PurchaseOrderDetails dtl in PurchaseOrderDetails)
                    {
                        TaxSum += Math.Round(((dtl.LineTotal - (DocDiscAmount / subtotal * dtl.LineTotal)) * (dtl.TaxRate / 100)),2);
                    }

                }
                else
                {
                    if (PurchaseOrderDetails != null)
                        rtn += PurchaseOrderDetails.Sum(p => p.TaxAmount);

                    TaxSum = rtn;
                }

                return TaxSum;
            }
            set
            {
                SetPropertyValue("TotalTaxAmount", ref _TotalTaxAmount, value);
            }
        }

        //[NonPersistent]
        private decimal _FinalAmount;
        [ImmediatePostData]
        [Index(43), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("FinalAmount", Enabled = false)]
        public decimal FinalAmount
        {
            get
            {

                decimal final = 0;
                //if (_DocDisc > 0)
                //{
                    //final = Math.Round(TotalTaxAmount + (((100 - _DocDisc) / 100) * Amount), 2);
                    final = Math.Round((Amount -DocDiscAmount + TotalTaxAmount),2);
                //}
                //else
                //{
                //    final = Math.Round((TotalTaxAmount + Amount), 2);

                //}
                return final;

            }
            set
            {
                SetPropertyValue("FinalAmount", ref _FinalAmount, value);
            }
        }

        private string _Address;
        [Size(200)]
        [Index(32), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("Adress", Enabled = false, Criteria = "IsPassed")]
        public string Adress
        {
            get { return _Address; }
            set
            {
                SetPropertyValue("Adress", ref _Address, value);
            }
        }



        private vw_PaymentTerms _PaymentTerms;
        [NoForeignKey]
        [XafDisplayName("Payment Terms"), ToolTip("Payment Term")]
        [Index(45), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Payment Term", Enabled = true)]
        public vw_PaymentTerms PaymentTerms
        {
            get { return _PaymentTerms; }
            set
            {
                SetPropertyValue("PaymentTerms", ref _PaymentTerms, value);
            }
        }


        [NonPersistent]
        // [ImmediatePostData]
        [Index(89), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int length
        {
            get
            {

                return DocNum.ToString().Length;
            }
        }


        private vw_PO _PO;
        //[NonPersistent]
        [NoForeignKey]
        [XafDisplayName("SAP Status"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(45), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(true)]
        [Appearance("PO", Enabled = false, Criteria = "IsPassed")]
        public vw_PO PO
        {
            get { return _PO; }
            set
            {
                SetPropertyValue("PO", ref _PO, value);
            }
        }

        public string PO_TYPE
        {
            get { return _PO == null ? "" : _PO.DocType; }
        }

        private ApprovalStatuses _ApprovalStatus;
        [Index(50), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("ApprovalStatus", Enabled = false)]
        public ApprovalStatuses ApprovalStatus
        {
            get { return _ApprovalStatus; }
            set
            {
                SetPropertyValue("ApprovalStatus", ref _ApprovalStatus, value);
            }
        }






        private SystemUsers _Escalate;
        [Index(53), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Escalate", Enabled = false)]
        [XafDisplayName("Escalate")]
        public SystemUsers Escalate
        {
            get { return _Escalate; }
            set { SetPropertyValue("Escalate", ref _Escalate, value); }
        }

        private double _CurrRate;
        [Index(55), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Currency Rate")]
        [Appearance("CurrRate", Enabled = false)]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        public double CurrRate
        {
            get { return _CurrRate; }
            set
            {
                SetPropertyValue("CurrRate", ref _CurrRate, value);
            }
        }

        private vw_BudgetData _BudgetCategoryData;
        [ImmediatePostData]
        [NoForeignKey]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(58), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("BudgetCategoryData", Enabled = false, Criteria = "IsPassed")]
        [DataSourceCriteria("Department = '@this.Department.BoName' and IsActive = '1'")]
        [XafDisplayName("Expenses Budget Category")]
        public vw_BudgetData BudgetCategoryData
        {
            get { return _BudgetCategoryData; }
            set
            {
                SetPropertyValue("BudgetCategoryData", ref _BudgetCategoryData, value);
                if (!IsLoading && value != null)
                {
                    BudgetBalance = 0;
                    MonthlyBudgetBalance = 0;
                    if (Department != null)
                    {
                        BudgetCategory budget;
                        budget = Session.FindObject<BudgetCategory>
                            (CriteriaOperator.Parse("Department.Oid = ? and IsActive = ?", this.Department.Oid, "True"));

                        if (budget != null)
                        {
                            foreach (BudgetCategoryDetails dtl in budget.BudgetCategoryDetails)
                            {
                                if (dtl.BudgetCategory.BudgetCategoryName == this.BudgetCategoryData.BudgetCategoryName)
                                {
                                    decimal remainbudget = 0;
                                    decimal currmonth = 0;
                                    decimal yearbalance = 0;
                                    foreach (BudgetCategoryAmount dtl2 in dtl.BudgetCategoryAmount)
                                    {
                                        //Start From April
                                        if ((((int)dtl2.Month) + 1 < this.DocDate.Month && dtl2.Year == this.DocDate.Year.ToString() &&
                                            dtl2.Month != CategoryMonth.January && dtl2.Month != CategoryMonth.February && dtl2.Month != CategoryMonth.March))
                                        {
                                            remainbudget += dtl2.MonthlyBudgetBalance;
                                        }

                                        if ((dtl2.Year == this.DocDate.Year.ToString() &&
                                            (dtl2.Month != CategoryMonth.January && dtl2.Month != CategoryMonth.February && dtl2.Month != CategoryMonth.March)) ||
                                            (dtl2.Year == this.DocDate.AddYears(1).Year.ToString() &&
                                            (dtl2.Month == CategoryMonth.January || dtl2.Month == CategoryMonth.February || dtl2.Month == CategoryMonth.March)))
                                        {
                                            yearbalance = yearbalance + dtl2.MonthlyBudgetBalance;
                                        }

                                        // Before April
                                        if (this.DocDate.Month < 4)
                                        {
                                            if (dtl2.Year == this.DocDate.AddYears(-1).Year.ToString() &&
                                            (dtl2.Month != CategoryMonth.January && dtl2.Month != CategoryMonth.February && dtl2.Month != CategoryMonth.March))
                                            {
                                                remainbudget += dtl2.MonthlyBudgetBalance;
                                            }

                                            if (dtl2.Year == this.DocDate.Year.ToString() && ((int)dtl2.Month) + 1 < this.DocDate.Month)
                                            {
                                                remainbudget += dtl2.MonthlyBudgetBalance;
                                            }

                                            if ((dtl2.Year == this.DocDate.Year.ToString() &&
                                                (dtl2.Month == CategoryMonth.January || dtl2.Month == CategoryMonth.February || dtl2.Month == CategoryMonth.March)) ||
                                                (dtl2.Year == this.DocDate.AddYears(-1).Year.ToString() &&
                                                (dtl2.Month != CategoryMonth.January && dtl2.Month != CategoryMonth.February && dtl2.Month != CategoryMonth.March)))
                                            {
                                                yearbalance = yearbalance + dtl2.MonthlyBudgetBalance;
                                            }
                                        }

                                        // Current month
                                        if (((int)dtl2.Month) == this.DocDate.Month - 1 && dtl2.Year == this.DocDate.Year.ToString())
                                        {
                                            currmonth = dtl2.MonthlyBudgetBalance;
                                        }
                                    }

                                    BudgetBalance = (double)yearbalance;
                                    MonthlyBudgetBalance = (double)currmonth + (double)remainbudget;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (!IsLoading && value != null)
                {
                    BudgetBalance = 0;
                    MonthlyBudgetBalance = 0;
                }
            }
        }

        private double _BudgetBalance;
        [ImmediatePostData]
        [Index(60), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Budget Balance")]
        [Appearance("BudgetBalance", Enabled = false)]
        //[Appearance("BudgetBalance1", BackColor = "#FF4433", FontColor = "Black", Criteria = "BudgetBalance < Amount")]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        public double BudgetBalance
        {
            get { return _BudgetBalance; }
            set
            {
                SetPropertyValue("BudgetBalance", ref _BudgetBalance, value);
            }
        }

        private double _MonthlyBudgetBalance;
        [ImmediatePostData]
        [Index(63), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Monthly Budget Balance")]
        [Appearance("MonthlyBudgetBalance", Enabled = false)]
        [Appearance("MonthlyBudgetBalance1", BackColor = "#FF4433", FontColor = "Black", Criteria = "MonthlyBudgetBalance < Amount")]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        public double MonthlyBudgetBalance
        {
            get { return _MonthlyBudgetBalance; }
            set
            {
                SetPropertyValue("MonthlyBudgetBalance", ref _MonthlyBudgetBalance, value);
            }
        }

        private string _Requestor;
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(65), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Requestor")]
        public string Requestor
        {
            get { return _Requestor; }
            set
            {
                SetPropertyValue("Requestor", ref _Requestor, value);
            }
        }

        private DateTime? _ApproveDate;
        [Index(305), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("ApproveDate", Enabled = false)]
        public DateTime? ApproveDate
        {
            get { return _ApproveDate; }
            set
            {
                SetPropertyValue("ApproveDate", ref _ApproveDate, value);
            }
        }


        private string _NextApprover;
        [ImmediatePostData]
        [XafDisplayName("Next Approver")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(306), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Next Approver", Enabled = false)]
        public string NextApprover
        {
            get { return _NextApprover; }
            set
            {
                SetPropertyValue("NextApprover", ref _NextApprover, value);
            }
        }


        private bool _IsCancelled;
        [ImmediatePostData]
        [XafDisplayName("Cancelled")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(100), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsCancelled", Enabled = false)]
        public bool IsCancelled
        {
            get { return _IsCancelled; }
            set
            {
                SetPropertyValue("IsCancelled", ref _IsCancelled, value);
            }
        }
        private bool _IsPassed;
        [ImmediatePostData]
        [XafDisplayName("Passed")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(101), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsPassed", Enabled = false)]
        public bool IsPassed
        {
            get { return _IsPassed; }
            set
            {
                SetPropertyValue("IsPassed", ref _IsPassed, value);
            }
        }
        private bool _IsAccepted;
        [ImmediatePostData]
        [XafDisplayName("Accepted")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(102), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsAccepted", Enabled = false)]
        public bool IsAccepted
        {
            get { return _IsAccepted; }
            set
            {
                SetPropertyValue("IsAccepted", ref _IsAccepted, value);
            }
        }
        private bool _IsRejected;
        [ImmediatePostData]
        [XafDisplayName("Rejected")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(103), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsRejected", Enabled = false)]
        public bool IsRejected
        {
            get { return _IsRejected; }
            set
            {
                SetPropertyValue("IsRejected", ref _IsRejected, value);
            }
        }
        private bool _IsClosed;
        [ImmediatePostData]
        [XafDisplayName("Closed")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(104), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsClosed", Enabled = false)]
        public bool IsClosed
        {
            get { return _IsClosed; }
            set
            {
                SetPropertyValue("IsClosed", ref _IsClosed, value);
            }
        }
        private bool _IsPosted;
        [ImmediatePostData]
        [XafDisplayName("Posted")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(105), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsPosted", Enabled = false)]
        public bool IsPosted
        {
            get { return _IsPosted; }
            set
            {
                SetPropertyValue("IsPosted", ref _IsPosted, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Index(200), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Purchase Order Status")]
        public string PurchaseRequestStatus
        {
            get
            {
                if (IsCancelled)
                    return "Cancel";
                else if (IsPosted)
                    return "Posted";
                else if (IsClosed)
                    return "Closed";
                else if (IsRejected)
                    return "Rejected";
                else if (IsAccepted)
                    return "Accepted";
                else if (IsPassed)
                    return "Passed";
                return "New";
            }
        }

        [Browsable(false), NonPersistent]
        public bool IsPRUserCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsPRSuperCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsAcceptUserCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsVerifyUserCheck { get; set; }
        [Browsable(false), NonPersistent]
        public bool IsApprovalUserCheck { get; set; }

        //[NonPersistent]
        //[ImmediatePostData]
        //[XafDisplayName("ApprovalUser")]
        //[Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("ApprovalUser", Enabled = false)]
        //public Guid ApprovalUser
        //{
        //    get
        //    {
        //        int count = 0;
        //        Guid rtn = Guid.Parse("02868725-036A-483B-A123-2932180D4A03");

        //        foreach (PurchaseOrderAppStatuses dtl in this.PurchaseOrderAppStatuses)
        //        {
        //            if (dtl.AppStatus == ApprovalStatuses.Approved && dtl.CreateUser.Oid != Guid.Parse("02868725-036A-483B-A123-2932180D4A03"))
        //            {
        //                count++;
        //            }
        //        }

        //        foreach (PurchaseOrderAppStages dtl2 in this.PurchaseOrderAppStages)
        //        {
        //            if (count == 0)
        //            {
        //                Approvals appuser;
        //                appuser = Session.FindObject<Approvals>(CriteriaOperator.Parse("Oid = ?", dtl2.Approval.Oid));

        //                rtn = dtl2.Approval.ApprovalUser.Where(p => p.UserApproval != null).First().Oid;
        //            }

        //            count--;
        //        }

        //        return rtn;
        //    }
        //}

        private string _AppUser;
        [ImmediatePostData]
        [XafDisplayName("AppUser")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("AppUser", Enabled = false)]
        public string AppUser
        {
            get { return _AppUser; }
            set
            {
                SetPropertyValue("AppUser", ref _AppUser, value);
            }
        }

        private string _WhoApprove;
        [ImmediatePostData]
        [XafDisplayName("WhoApprove")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("WhoApprove", Enabled = false)]
        public string WhoApprove
        {
            get { return _WhoApprove; }
            set
            {
                SetPropertyValue("WhoApprove", ref _WhoApprove, value);
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                APInvoiceDetails InvItem;
                InvItem = Session.FindObject<APInvoiceDetails>(CriteriaOperator.Parse("DocNum = ?", this.DocNum));

                if (InvItem != null)
                {
                    foreach (PurchaseOrderDetails PRitem in this.PurchaseOrderDetails)
                    {
                        if (InvItem.APInvoice != null)
                        {
                            if (PRitem.Oid == InvItem.LinkOid)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid2
        {
            get
            {
                if (this.Currency != "MYR" && this.CurrRate == 0)
                    return true;

                return false;
            }
        }

        [Browsable(false)]
        [ImmediatePostData]
        public bool IsValid3
        {
            get
            {
                if (IsNew == true)
                {


                    vw_SAPPostingPeriod postingperiod;
                    postingperiod = Session.FindObject<vw_SAPPostingPeriod>(CriteriaOperator.Parse("FromDate <= ? and ToDate >= ?", DocDate, DocDate));
                    if (postingperiod != null)
                    {

                        if (postingperiod.FromDate <= DocDate && postingperiod.ToDate >= DocDate && postingperiod.Status.ToString() == "Unlocked")
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [Browsable(false)]
        public bool IsValid4
        {
            get
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                if (user.Roles.Where(p => p.Name == GeneralSettings.ApprovalRole).Count() > 0)
                {
                    return true;
                }

                return false;
            }
        }

        [Association("PurchaseOrder-PurchaseOrderDetails")]
        //[DevExpress.Xpo.Aggregated]
        [Appearance("PurchaseOrderDetails", Enabled = false, Criteria = "IsPassed and not IsRejected")]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Items")]
        public XPCollection<PurchaseOrderDetails> PurchaseOrderDetails
        {
            get { return GetCollection<PurchaseOrderDetails>("PurchaseOrderDetails"); }
        }
        [Association("PurchaseOrder-PurchaseOrderAttachments")]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Attachment")]
        public XPCollection<PurchaseOrderAttachments> PurchaseOrderAttachments
        {
            get { return GetCollection<PurchaseOrderAttachments>("PurchaseOrderAttachments"); }
        }
        [Association("PurchaseOrder-PurchaseOrderDocStatuses")]
        [XafDisplayName("Status")]
        public XPCollection<PurchaseOrderDocStatuses> PurchaseOrderDocStatuses
        {
            get { return GetCollection<PurchaseOrderDocStatuses>("PurchaseOrderDocStatuses"); }
        }
        [Association("PurchaseOrder-PurchaseOrderAppStatuses")]
        [XafDisplayName("App Status")]
        public XPCollection<PurchaseOrderAppStatuses> PurchaseOrderAppStatuses
        {
            get { return GetCollection<PurchaseOrderAppStatuses>("PurchaseOrderAppStatuses"); }
        }
        [Association("PurchaseOrder-PurchaseOrderAppStages")]
        [XafDisplayName("App Stage")]
        public XPCollection<PurchaseOrderAppStages> PurchaseOrderAppStages
        {
            get { return GetCollection<PurchaseOrderAppStages>("PurchaseOrderAppStages"); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {
                    //DepartmentDocs doc = Session.FindObject<DepartmentDocs>(CriteriaOperator.Parse("Department.Oid=? and DocType.Oid=?", Department.Oid, DocType.Oid));
                    //DocNumSeq = doc.NextDocNo;
                    //DocNum = DocNumSeq.ToString();
                    //DocNumSeq.NextDocNo++;
                    //DocNumSeq.NextDocNo++;
                    //DocNum = Department.DocPrefix + DocNumSeq.NextDocNo;

                    //DocNumSeq = com.n
                    PurchaseOrderDocStatuses ds = new PurchaseOrderDocStatuses(Session);
                    ds.DocStatus = DocumentStatus.Create;
                    ds.DocRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.UpdateDate = DateTime.Now;
                    this.PurchaseOrderDocStatuses.Add(ds);

                }
            }
        }
        protected override void OnSaved()
        {
            base.OnSaved();
            this.Reload();
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();

            //PO = Session.FindObject<vw_PO>(CriteriaOperator.Parse("PRNO=?", DocNum));

            IsPRSuperCheck = false;
            IsPRUserCheck = false;
            IsAcceptUserCheck = false;
            IsVerifyUserCheck = false;
            IsApprovalUserCheck = false;

            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            if (user != null)
            {
                if (user.Roles.Where(p => p.Name == GeneralSettings.POsuperuserrole).Count() > 0)
                {
                    this.IsPRSuperCheck = true;
                    this.IsPRUserCheck = true;
                }
                else if (this.CreateUser != null)
                    //if (user.Roles.Where(p => p.Name == GeneralSettings.POUserRole).Count() > 0 && user.Oid == this.CreateUser.Oid)
                    if (user.Roles.Where(p => p.Name == GeneralSettings.POUserRole).Count() > 0)
                        this.IsPRUserCheck = true;

                if (user.Roles.Where(p => p.Name == GeneralSettings.Acceptancerole).Count() > 0)
                    this.IsAcceptUserCheck = true;

                if (user.Roles.Where(p => p.Name == GeneralSettings.verifyrole).Count() > 0)
                    this.IsVerifyUserCheck = true;

                if (user.Roles.Where(p => p.Name == GeneralSettings.ApprovalRole).Count() > 0)
                {
                    bool found = false;

                    if (Escalate == null)
                    {
                        foreach (PurchaseOrderAppStages dtl in this.PurchaseOrderAppStages)
                        {
                            if (dtl.Approval.ApprovalUser.Where(p => p.Oid == user.Oid).Count() > 0)
                                found = true;
                        }
                    }
                    else
                    {
                        found = true;
                    }

                    this.IsApprovalUserCheck = found;
                }

            }

        }
    }
}