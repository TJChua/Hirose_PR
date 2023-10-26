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
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    [XafDisplayName("PRDetails")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class PurchaseRequestDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;

            Tax = Session.FindObject<vw_taxes>(new BinaryOperator("BoCode", "X1"));
            Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "999"));
            DelDate = DateTime.Today;
            DocDate = DateTime.Today;
            //DocDate = DateTime.Today;
        }

        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue("PersistentProperty", ref _PersistentProperty, value); }
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

        private DocTypes _DocType;
        [Browsable(false)]
        public virtual DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private DateTime? _DocDate;
        [Index(9), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Date"), ToolTip("Enter Date")]
        [Appearance("DocDate", Enabled = false, Criteria = "IsValid")]
        public DateTime? DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);

            }
        }

        private DateTime? _DelDate;
        [Index(10), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Delivery Date"), ToolTip("Enter Date")]
        [Appearance("DelDate", Enabled = false, Criteria = "IsValid")]
        public DateTime? DelDate
        {
            get { return _DelDate; }
            set
            {
                SetPropertyValue("DelDate", ref _DelDate, value);

            }
        }

        private vw_items _Item;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Item")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Item", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        public vw_items Item

        {
            get { return _Item; }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }

        private string _ItemDesc;
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Item Description")]
        [Appearance("ItemDesc", Enabled = false, Criteria = "IsValid")]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public string ItemDesc
        {
            get { return _ItemDesc; }
            set
            {
                SetPropertyValue("ItemDesc", ref _ItemDesc, value);
            }
        }

        private string _RefNo;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        public string RefNo
        {
            get { return _RefNo; }
            set
            {
                SetPropertyValue("RefNo", ref _RefNo, value);
            }
        }

        private string _Remarks;
        [Index(11), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
       // [RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0")]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }


        private decimal _Quantity=1;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "{0:n}")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan, "0")]
        [Appearance("Quantity", Enabled = false, Criteria = "IsValid")]
        [Index(12), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("Quantity")]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }

        private string _UOM;
        [Appearance("UOM", Enabled = false, Criteria = "IsValid")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(19), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("Unit of Measure")]
        public string UOM
        {
            get { return _UOM; }
            set
            {
                SetPropertyValue("UOM", ref _UOM, value);
            }
        }

        private decimal _Amount;
        [ImmediatePostData]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "0.0001")]
        [Appearance("Amount", Enabled = false, Criteria = "IsValid")]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("Unit Price Before Tax")]
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }

        private decimal _DiscP;
        [ImmediatePostData]
        [Appearance("DiscP", Enabled = false, Criteria = "IsValid")]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[RuleValueComparison("DiscPCompare", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "Amount", SkipNullOrEmptyValues = false)]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "0")]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("Discount Amount")]
        public decimal DiscP
        {
            get { return _DiscP; }
            set
            {
                SetPropertyValue("DiscP", ref _DiscP, value);
            }
        }

        private vw_taxes _Tax;
        [NoForeignKey]
        [ImmediatePostData]
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Tax", Enabled = false, Criteria = "IsValid")]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0")]
        [XafDisplayName("Tax Code")]
        public vw_taxes Tax
        {
            get { return _Tax; }
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);
            }
        }

        private vw_ItemSeries _Series;
        [NoForeignKey]
        [Index(25), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Series", Enabled = false, Criteria = "IsValid")]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0")]
        [XafDisplayName("Item Series")]
        public vw_ItemSeries Series
        {
            get { return _Series; }
            set
            {
                SetPropertyValue("Series", ref _Series, value);
            }
        }

        private decimal _TaxAmount;
        [ImmediatePostData]
        [Index(22), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("TaxAmount", Enabled = false, Criteria = "IsValid")]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("Tax Amount Per Unit")]

        public decimal TaxAmount
        {
            get { return _TaxAmount; }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
            }
        }

    
       
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("LineAmount", Enabled = false, Criteria = "IsValid")]
        [XafDisplayName("Unit Price After Discount")]
        public decimal LineAmount
        {
            get { return (Amount -_DiscP + TaxAmount); }

        }

        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("LineAmountWithoutTax", Enabled = false, Criteria = "IsValid")]
        [XafDisplayName("Unit Price Without Tax")]
        public decimal LineAmountWithoutTax
        {
            get { return Math.Round((Amount - _DiscP)*Quantity,2); }

        }

        private decimal _LineTotal;
        [ImmediatePostData]
        [Appearance("LineTotal", Enabled = false, Criteria = "IsValid")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan, "0")]
        [Index(24), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Line Total")]
  
        public decimal LineTotal
        {
            get { return Math.Round((LineAmount * Quantity) ,2); }
            set
            {
                SetPropertyValue("LineTotal", ref _LineTotal, value);
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (this.PurchaseRequest != null)
                {
                    PurchaseRequests PR;
                    PR = Session.FindObject<PurchaseRequests>(CriteriaOperator.Parse("Oid = ?", this.PurchaseRequest.Oid));

                    if (PR != null)
                    {
                        if (PR.ApprovalStatus == ApprovalStatuses.Approved)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private PurchaseRequests _PurchaseRequest;
        [Association("PurchaseRequests-PurchaseRequestDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("PurchaseRequest", Enabled = false)]
        public PurchaseRequests PurchaseRequest
        {
            get { return _PurchaseRequest; }
            set { SetPropertyValue("PurchaseRequest", ref _PurchaseRequest, value); }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
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

                }
            }
        }


    }
}