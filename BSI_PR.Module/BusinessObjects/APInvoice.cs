using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

// 20240926 - new enhancement - ver 0.1

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("A/P Invoice")]
    [NavigationItem("A/P Invoice")]
    [DefaultProperty("DocNum")]
    [Appearance("HideCopyFromPO", AppearanceItemType.Action, "True", TargetItems = "INV_CopyFromPO", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "ListView")]
    [Appearance("HideCopyFromPO1", AppearanceItemType.Action, "True", TargetItems = "INV_CopyFromPO", Criteria = "not IsNew or Vendor == null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCopyFromGRN", AppearanceItemType.Action, "True", TargetItems = "INV_CopyFromGRN", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "ListView")]
    [Appearance("HideCopyFromGRN1", AppearanceItemType.Action, "True", TargetItems = "INV_CopyFromGRN", Criteria = "not IsNew or  Vendor == null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HidePass", AppearanceItemType.Action, "True", TargetItems = "Pass_Invoice", Criteria = "APInvoiceStatus != 'New' or IsNew", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEditButton", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "(APInvoiceStatus != 'New' and APInvoiceStatus != 'Submitted')", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideCancel", AppearanceItemType = "Action", TargetItems = "Cancel_Invoice", Criteria = "(APInvoiceStatus = 'Cancel') or (IsSubmit=1) or IsNew = 1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideDelete", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HidePost2", AppearanceItemType = "Action", TargetItems = "Post_Invoice", Criteria = "1=1",  Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("HidePost2", AppearanceItemType = "Action", TargetItems = "Post_Invoice", Criteria = "APInvoiceStatus = 'Posted' or APInvoiceStatus = 'New'", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEditButton5", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "(APInvoiceStatus = 'Cancel') ", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("InvCurrencyRateRule", DefaultContexts.Save, "IsValid2 = 0", "Incorrect currency rate.")]
    [RuleCriteria("InvOutsideRange", DefaultContexts.Save, "IsValid3 = 1", "Posting Period Locked")]
    // Start ver 0.1
    [RuleCriteria("InvDupInvNo", DefaultContexts.Save, "IsValid4 = 0", "Same vendor Inv. No. found.")]
    // End ver 0.1




    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class APInvoice : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public APInvoice(Session session)
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

            CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);

            CreateDate = DateTime.Now;
            DocDate = DateTime.Today;
            Department = Session.FindObject<Departments>(new BinaryOperator("BoCode", user.CurrDept, BinaryOperatorType.Equal));
            DocNumSeq = Session.FindObject<DepartmentDocs>(CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 4));
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
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
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

        private DepartmentDocs _DocNumSeq;
        [Index(12), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Browsable(false)]
        public DepartmentDocs DocNumSeq
        {
            get { return _DocNumSeq; }
            set
            {
                SetPropertyValue("DocNumSeq", ref _DocNumSeq, value);
            }
        }

        private DateTime _DocDate;
        [ImmediatePostData]
        [Index(20), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Document Date"), ToolTip("Enter Text")]
       //[Appearance("DocDate", Enabled = false)]
        [Appearance("DocDate", Enabled = true, Criteria = "IsNew")]
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
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [DataSourceCriteria("ValidFor = 'Y' and '@this.DocDate' >= validFrom and '@this.DocDate' <= validTo")]
        [XafDisplayName("Vendor")]
        [Appearance("Vendor", Enabled = false, Criteria = "not IsNew")]
        public vw_vendors Vendor
        {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
                {
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
        }

        private string _Address;
        [Size(200)]
        [Index(5), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("Adress", Enabled = false, Criteria = "not IsNew")]
        public string Adress
        {
            get { return _Address; }
            set
            {
                SetPropertyValue("Adress", ref _Address, value);
            }
        }

        private string _Currency;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Document Currency"), ToolTip("")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [XafDisplayName("Department")]
        [DataSourceProperty("CreateUser.Department")]
        [Index(13), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Department", Enabled = false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private decimal _Amount;
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [ModelDefault("EditMask", "{0:n4}")]
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Amount", Enabled = false)]
        public decimal Amount
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (APInvoiceDetails != null)
                        rtn += APInvoiceDetails.Sum(p => p.LineAmount);

                    return Math.Round((((100 - DocDisc) / 100) * rtn), 2);
                }
                else
                {
                    return _Amount;
                }
            }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }


        [NonPersistent]
        [ImmediatePostData]
        [Index(40), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Subtotal
        {
            get
            {
                decimal rtn = 0;

                if (Session.IsObjectsSaving != true)
                {
                    if (APInvoiceDetails != null)
                        rtn += APInvoiceDetails.Sum(p => p.LineAmount);

                    //if (PurchaseRequestDetail != null)
                    //    rtn += PurchaseRequestDetail.Sum(p => p.LineTotal);

                }
                return rtn;
            }
        }

        private string _PRGRNDoc;
        [XafDisplayName("PRGRNDoc")]
        [Index(10), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save)]
        //[Appearance("Remarks", Enabled = false, Criteria = "IsPassed")]
        public string PRGRNDoc
        {
            get { return _PRGRNDoc; }
            set
            {
                SetPropertyValue("PRGRNDoc", ref _PRGRNDoc, value);
            }
        }

        private string _Remarks;
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save)]
        //[Appearance("Remarks", Enabled = false, Criteria = "IsPassed")]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private string _SAPINVNum;
        [ImmediatePostData]
        [XafDisplayName("SAP Invoice Number")]
        [Appearance("SAPINVNum", Enabled = false)]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string SAPINVNum
        {
            get { return _SAPINVNum; }
            set
            {
                SetPropertyValue("SAPINVNum", ref _SAPINVNum, value);
            }
        }

        // Start ver 0.1
        private string _PONum;
        [XafDisplayName("PO No.")]
        [Size(9999)]
        [Appearance("PONum", Enabled = false)]
        [Index(16), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string PONum
        {
            get { return _PONum; }
            set
            {
                SetPropertyValue("PONum", ref _PONum, value);
            }
        }
        // Start ver 0.1

        private string _DOInvoice;
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Invoice No")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DOInvoice", Enabled = false, Criteria = "IsPosted")]
        public string DOInvoice
        {
            get { return _DOInvoice; }
            set { SetPropertyValue("DOInvoice", ref _DOInvoice, value); }
        }


        private string _DeliveryNo;
        [Index(19), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("DO No")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DeliveryNo", Enabled = false, Criteria = "IsSubmit")]
        public string DeliveryNo
        {
            get { return _DeliveryNo; }
            set { SetPropertyValue("DeliveryNo", ref _DeliveryNo, value); }
        }


        private double _CurrRate;
        [Index(20), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
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

        private decimal _DocDisc;
        [ImmediatePostData]
        [Index(23), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Discount %")]
        [Appearance("DocDisc", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
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
        [Appearance("DocDisc Amount")]

        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        public decimal DocDiscAmount
        {
            //get { return _DocDiscAmount; }
            //set
            //{
            //    SetPropertyValue("DocDiscAmount", ref _DocDiscAmount, value);
            //}
            get
            {
                ////if (Session.IsObjectsSaving != true )
                
                //if (Session.IsObjectsLoading!=true && Session.IsObjectsSaving==true)
                //{
                //    //return _DocDiscAmount; 
                //    decimal rtn = 0;
                //    if (APInvoiceDetails != null)
                //        rtn += APInvoiceDetails.Sum(p => p.DocDiscAmountAfter);

                //    _DocDiscAmount = rtn;
                //    return rtn;
                //}
                //else
                //{
                    return _DocDiscAmount;
                //}
            }
            set
            {

                SetPropertyValue("DocDiscAmount", ref _DocDiscAmount, value);
            }
        }

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
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (APInvoiceDetails != null)
                        rtn += APInvoiceDetails.Sum(p => p.TaxAmount);

                    decimal subtotal = 0;
                    if (APInvoiceDetails != null)
                        subtotal += APInvoiceDetails.Sum(p => p.LineAmount);


                    decimal TaxSum = 0;

                    if (DocDiscAmount > 0)
                    {
                        foreach (APInvoiceDetails dtl in APInvoiceDetails)
                        {
                            TaxSum += Math.Round(((dtl.LineAmount - (DocDiscAmount / subtotal * dtl.LineAmount)) * (dtl.TaxRate / 100)), 2);
                        }

                    }
                    else
                    {
                        TaxSum = rtn;
                    }

                    return TaxSum;
                }
                else
                {
                    return _TotalTaxAmount;
                }
            }
            set
            {
                SetPropertyValue("TotalTaxAmount", ref _TotalTaxAmount, value);
            }
        }

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
                final = Math.Round((Subtotal - DocDiscAmount + TotalTaxAmount), 2);
                _FinalAmount = final;
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




        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }


        private bool _IsSubmit;
        [ImmediatePostData]
        [XafDisplayName("Submit")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(180), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsSubmit", Enabled = false)]
        public bool IsSubmit
        {
            get { return _IsSubmit; }
            set
            {
                SetPropertyValue("IsSubmit", ref _IsSubmit, value);
            }
        }

        private bool _IsCancel;
        [ImmediatePostData]
        [XafDisplayName("Cancel")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(181), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsCancel", Enabled = false)]
        public bool IsCancel
        {
            get { return _IsCancel; }
            set
            {
                SetPropertyValue("IsCancel", ref _IsCancel, value);
            }
        }

        private bool _IsClosed;
        [ImmediatePostData]
        [XafDisplayName("Closed")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(182), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("IsClosed", Enabled = false)]
        public bool IsClosed
        {
            get { return _IsClosed; }
            set
            {
                SetPropertyValue("IsClosed", ref _IsClosed, value);
            }
        }

        [Index(200), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("AP Invoice Status")]
        public string APInvoiceStatus
        {
            get
            {
                if (IsPosted)
                    return "Posted";
                if (IsClosed)
                    return "Closed";
                if (IsCancel)
                    return "Cancel";
                if (IsSubmit)
                    return "Submitted";
                return "New";

            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                PermissionPolicyRole AdminRole = Session.FindObject<PermissionPolicyRole>(CriteriaOperator.Parse("IsCurrentUserInRole('Administrators')"));
              
                if (this.IsSubmit == true && AdminRole != null)
                {
                    return true;
                }

                if (this.IsSubmit == false)
                {
                    return true;
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
        //public bool IsValid3
        //{
        //    get
        //    {
        //        PostingPeriod postingperiod;
        //        postingperiod = Session.FindObject<PostingPeriod>(CriteriaOperator.Parse("Oid = ?", 1));
        //        if (postingperiod != null)
        //        {

        //            if (postingperiod.FromDate <= DocDate && postingperiod.ToDate >= DocDate)
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}
  
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
        public bool IsValid3
        {
            get
            {
                //if (IsNew == true)
                //{
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
                //}
                //else
                //{
                //    return true;
                //}
            }
        }

        // Start ver 0.1
        [Browsable(false)]
        public bool IsValid4
        {
            get
            {
                if (!string.IsNullOrEmpty(this.DOInvoice))
                {
                     APInvoice invoice = Session.FindObject<APInvoice>(CriteriaOperator.Parse("DOInvoice = ?", this.DOInvoice));

                    if (invoice != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        // End ver 0.1

        [Association("APInvoice-APInvoiceDetails")]
        [XafDisplayName("AP Invoice Details")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("APInvoiceDetails", Enabled = false, Criteria = "IsSubmit")]
        public XPCollection<APInvoiceDetails> APInvoiceDetails
        {
            get { return GetCollection<APInvoiceDetails>("APInvoiceDetails"); }
        }

        [DevExpress.Xpo.Aggregated, Association("APInvoice-APInvoiceAttachment")]
        [XafDisplayName("Attachment")]
        public XPCollection<APInvoiceAttachment> APInvoiceAttachment
        {
            get { return GetCollection<APInvoiceAttachment>("APInvoiceAttachment"); }
        }

        [Association("APInvoice-APInvoiceDocStatues")]
        [XafDisplayName("Audit Trail")]
        public XPCollection<APInvoiceDocStatues> APInvoiceDocStatues
        {
            get { return GetCollection<APInvoiceDocStatues>("APInvoiceDocStatues"); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                if (user != null)
                {
                    UpdateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                }
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {
                    APInvoiceDocStatues ds = new APInvoiceDocStatues(Session);
                    ds.DocStatus = "New";
                    ds.DocRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.UpdateDate = DateTime.Now;
                    this.APInvoiceDocStatues.Add(ds);
                }
            }
        }
    }
}