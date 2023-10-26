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
    [XafDisplayName("Goods Return")]
    [NavigationItem("Goods Return")]
    [DefaultProperty("DocNum")]
    [Appearance("HideCopyFrom", AppearanceItemType.Action, "True", TargetItems = "CopyFromGRN", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "ListView")]
    [Appearance("HideCopyFrom1", AppearanceItemType.Action, "True", TargetItems = "CopyFromGRN", Criteria = "not IsNew or Vendor == null", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HidePass", AppearanceItemType.Action, "True", TargetItems = "Pass_GoodsReturn", Criteria = "GoodReturnStatus != 'New' or IsNew", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEditButton", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode; Edit", Criteria = "(GoodReturnStatus != 'New')", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class GoodsReturn : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public GoodsReturn(Session session)
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
            DocNumSeq = Session.FindObject<DepartmentDocs>(CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 3));
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
        //[DataSourceCriteria("GroupCode = '110'")]
        [Index(3), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
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
        [Index(6), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
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
                    if (GoodsReturnDetails != null)
                        rtn += GoodsReturnDetails.Sum(p => p.LineAmount);

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

        private string _GRRef;
        [XafDisplayName("SAP G. Return Num")]
        [Index(10), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save)]
        //[Appearance("Remarks", Enabled = false, Criteria = "IsPassed")]
        public string GRRef
        {
            get { return _GRRef; }
            set
            {
                SetPropertyValue("GRRef", ref _GRRef, value);
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

        private string _InvoiceNo;
        [XafDisplayName("Reference No")]
        [Index(18), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("InvoiceNo", Enabled = false, Criteria = "IsSubmit")]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set
            {
                SetPropertyValue("InvoiceNo", ref _InvoiceNo, value);
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
        [XafDisplayName("IsCancel")]
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

        [Index(200), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Goods Return Status")]
        public string GoodReturnStatus
        {
            get
            {
                if (IsCancel)
                    return "Cancelled";
                if (IsSubmit)
                    return "Submitted";

                return "New";

            }
        }

        [Association("GoodsReturn-GoodsReturnDetails")]
        [XafDisplayName("Goods Return Details")]
        public XPCollection<GoodsReturnDetails> GoodsReturnDetails
        {
            get { return GetCollection<GoodsReturnDetails>("GoodsReturnDetails"); }
        }

        [DevExpress.Xpo.Aggregated, Association("GoodsReturn-GoodsReturnAttachements")]
        [XafDisplayName("Attachment")]
        public XPCollection<GoodsReturnAttachements> GoodsReturnAttachements
        {
            get { return GetCollection<GoodsReturnAttachements>("GoodsReturnAttachements"); }
        }

        [Association("GoodsReturn-GoodsReturnDocStatus")]
        [XafDisplayName("Audit Trail")]
        public XPCollection<GoodsReturnDocStatus> GoodsReturnDocStatus
        {
            get { return GetCollection<GoodsReturnDocStatus>("GoodsReturnDocStatus"); }
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
                    GoodsReturnDocStatus ds = new GoodsReturnDocStatus(Session);
                    ds.DocStatus = "New";
                    ds.DocRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.UpdateDate = DateTime.Now;
                    this.GoodsReturnDocStatus.Add(ds);
                }
            }
        }
    }
}