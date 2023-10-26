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
    [XafDisplayName("Goods Receipt PO")]
    [NavigationItem("Goods Receipt PO")]
    [DefaultProperty("DocNum")]
    [Appearance("HideCopyFrom", AppearanceItemType.Action, "True", TargetItems = "CopyFromPO", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "ListView")]
    [Appearance("HideCopyFrom1", AppearanceItemType.Action, "True", TargetItems = "CopyFromPO", Criteria = "not IsNew or Vendor == null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HidePass", AppearanceItemType.Action, "True", TargetItems = "Pass_GRN", Criteria = "GoodReceiptStatus != 'New' or IsNew", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEditButton", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "(GoodReceiptStatus != 'New')", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideCancel", AppearanceItemType = "Action", TargetItems = "Cancel_GRN", Criteria = "(GoodReceiptStatus = 'Cancel') or IsValid = 1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class GoodReceipt : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public GoodReceipt(Session session)
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
            DocNumSeq = Session.FindObject<DepartmentDocs>(CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 2));
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
        [Index(20), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Document Date"), ToolTip("Enter Text")]
        [Appearance("DocDate", Enabled = false)]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);
            }
        }

        private vw_vendors _Vendor;
        [ImmediatePostData]
        [NoForeignKey]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Vendor")]
        [Appearance("Vendor", Enabled = false, Criteria = "not IsNew")]
        public vw_vendors Vendor
        {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
            }
        }

        private Departments _Department;
        [XafDisplayName("Department")]
        [DataSourceProperty("CreateUser.Department")]
        [Index(13), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("Department", Enabled = false)]
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
                    if (GoodReceiptDetails != null)
                        rtn += GoodReceiptDetails.Sum(p => p.LineAmount);

                    return rtn;
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

        private string _GRNRef;
        [XafDisplayName("SAP GRN Num")]
        [Index(10), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save)]
        //[Appearance("Remarks", Enabled = false, Criteria = "IsPassed")]
        public string GRNRef
        {
            get { return _GRNRef; }
            set
            {
                SetPropertyValue("GRNRef", ref _GRNRef, value);
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

        private string _POScanner;
        [ImmediatePostData]
        [XafDisplayName("PO Ref No")]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string POScanner
        {
            get { return _POScanner; }
            set
            {
                SetPropertyValue("POScanner", ref _POScanner, value);
            }
        }

        private string _DOInvoice;
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("DO No")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DOInvoice", Enabled = false, Criteria = "IsSubmit")]
        public string DOInvoice
        {
            get { return _DOInvoice; }
            set { SetPropertyValue("DOInvoice", ref _DOInvoice, value); }
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

        private bool _IsClosed;
        [ImmediatePostData]
        [XafDisplayName("IsClosed")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(181), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
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
        [XafDisplayName("Goods Receipt Status")]
        public string GoodReceiptStatus
        {
            get
            {
                if (IsClosed)
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
                GoodsReturnDetails GRItem;
                GRItem = Session.FindObject<GoodsReturnDetails>(CriteriaOperator.Parse("GRND = ?", this.DocNum));

                if (GRItem != null)
                {
                    foreach (GoodReceiptDetails GRNItem in this.GoodReceiptDetails)
                    {
                        if (GRNItem.GoodReceipts != null)
                        {
                            if (GRNItem.Oid == GRItem.BaseLine)
                            {
                                return true;
                            }
                        }
                    }
                }

                APInvoiceDetails INVItem;
                INVItem = Session.FindObject<APInvoiceDetails>(CriteriaOperator.Parse("DocNum = ?", this.DocNum));

                if (INVItem != null)
                {
                    foreach (GoodReceiptDetails GRNItem in this.GoodReceiptDetails)
                    {
                        if (GRNItem.GoodReceipts != null)
                        {
                            if (GRNItem.Oid == INVItem.LinkOid)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        [Association("GoodReceipt-GoodReceiptDetails")]
        [XafDisplayName("Goods Receipt Details")]
        public XPCollection<GoodReceiptDetails> GoodReceiptDetails
        {
            get { return GetCollection<GoodReceiptDetails>("GoodReceiptDetails"); }
        }

        [DevExpress.Xpo.Aggregated, Association("GoodReceipt-GoodReceiptAttachment")]
        [XafDisplayName("Attachment")]
        public XPCollection<GoodReceiptAttachment> GoodReceiptAttachment
        {
            get { return GetCollection<GoodReceiptAttachment>("GoodReceiptAttachment"); }
        }

        [Association("GoodReceipt-GoodReceiptDocStatus")]
        [XafDisplayName("Audit Trail")]
        public XPCollection<GoodReceiptDocStatus> GoodReceiptDocStatus
        {
            get { return GetCollection<GoodReceiptDocStatus>("GoodReceiptDocStatus"); }
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
                    GoodReceiptDocStatus ds = new GoodReceiptDocStatus(Session);
                    ds.DocStatus = "New";
                    ds.DocRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.UpdateDate = DateTime.Now;
                    this.GoodReceiptDocStatus.Add(ds);
                }
            }
        }
    }
}