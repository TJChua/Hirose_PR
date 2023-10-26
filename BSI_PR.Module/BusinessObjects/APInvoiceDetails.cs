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
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("DisableNew", AppearanceItemType = "Action", TargetItems = "New", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    [XafDisplayName("AP Invoice Details")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class APInvoiceDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public APInvoiceDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
            CreateDate = DateTime.Now;

            Quantity = 1;
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

        private string _DocNo;
        [Index(3), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("Doc No")]
        [Appearance("DocNo", Enabled = false)]
        public string DocNo
        {
            get { return _DocNo; }
            set
            {
                SetPropertyValue("DocNo", ref _DocNo, value);
            }
        }

        private vw_items _ItemCode;
        [NoForeignKey]
        [ImmediatePostData]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Item Code")]
        [Appearance("ItemCode", Enabled = false)]
        public vw_items ItemCode
        {
            get { return _ItemCode; }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
                if (!IsLoading && value != null)
                {
                    GLAcc = ItemCode.ECExepnses + "-" + ItemCode.ACCTNAME;
                }
                else if (!IsLoading && value == null)
                {
                    GLAcc = null;
                }
            }
        }

        private string _ItemDescrip;
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Item Description")]
        [Size(254)]
        [Appearance("ItemDescrip", Enabled = false)]
        public string ItemDescrip
        {
            get { return _ItemDescrip; }
            set
            {
                SetPropertyValue("ItemDescrip", ref _ItemDescrip, value);
            }
        }

        private string _UOM;
        [Index(9), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("UOM")]
        [Appearance("UOM", Enabled = false)]
        public string UOM
        {
            get { return _UOM; }
            set
            {
                SetPropertyValue("UOM", ref _UOM, value);
            }
        }

        private decimal _Quantity;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "{0:n}")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan, "0")]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Quantity")]
        //[Appearance("Quantity", Enabled = false)]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    if (POOpenQty < Quantity)
                    {
                        Quantity = POOpenQty;
                    }
                }
            }
        }

        private decimal _UnitPrice;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Unit Price")]
        public decimal UnitPrice
        {
            get { return _UnitPrice; }
            set
            {
                SetPropertyValue("UnitPrice", ref _UnitPrice, value);
            }
        }

        private vw_taxes _Tax;
        [ImmediatePostData]
        [NoForeignKey]
        [Index(18), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public vw_taxes Tax
        {
            get { return _Tax; }
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);

                if (!IsLoading)
                {

                    vw_taxes Rate;


                    //Rate = Session.FindObject<vw_taxes>
                    //    (CriteriaOperator.Parse("BoCode = ?", Tax));
                    Rate = Session.FindObject<vw_taxes>(new BinaryOperator("BoCode", Tax));



                    if (Rate != null)
                    {
                        TaxAmount = Math.Round((decimal.Parse(Rate.Rate.ToString()) * LineAmount / 100), 2);
                        TaxRate = decimal.Parse(Rate.Rate.ToString());
                    }
                    else
                    {
                        TaxAmount = 0;
                        TaxRate = 0;
                    }


                }
            }
        }

        private decimal _TaxRate;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "{0:n}")]
        [Appearance("TaxRate", Enabled = false)]
        [Index(12), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("TaxRate")]
        public decimal TaxRate
        {
            get { return _TaxRate; }
            set
            {
                SetPropertyValue("_TaxRate", ref _TaxRate, value);
            }
        }

        private decimal _TaxAmount;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [Index(20), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("TaxAmount", Enabled = false)]
        [XafDisplayName("Tax Amount")]
        public decimal TaxAmount
        {
            get { return _TaxAmount; }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
            }
        }

        private decimal _LineAmount;
        [ImmediatePostData]
        [Index(23), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Line Amount")]
        [DbType("numeric(19,2)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [ModelDefault("EditMask", "{0:n2}")]
        [Appearance("LineAmount", Enabled = false)]
        public decimal LineAmount
        {
            get
            {
                return Math.Round((UnitPriceAfterDisc * Quantity), 2);
            }
            set
            {
                SetPropertyValue("LineAmount", ref _LineAmount, value);
            }
        }

        private string _DocNum;
        [Index(33), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("DocNum")]
        [Appearance("DocNum", Enabled = false)]
        public string DocNum
        {
            get { return _DocNum; }
            set { SetPropertyValue("DocNum", ref _DocNum, value); }
        }

        private int _LinkOid;
        [Index(35), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("LinkOid")]
        [Appearance("LinkOid", Enabled = false)]
        public int LinkOid
        {
            get { return _LinkOid; }
            set { SetPropertyValue("LinkOid", ref _LinkOid, value); }
        }

        private string _GLAcc;
        [NoForeignKey]
        [Index(10), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("GL Account")]
        [Appearance("GLAcc", Enabled = false)]
        public string GLAcc
        {
            get { return _GLAcc; }
            set
            {
                SetPropertyValue("GLAcc", ref _GLAcc, value);
            }
        }

        private decimal _DiscP;
        [ImmediatePostData]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "0")]
        [XafDisplayName("Discount Amount")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [DbType("decimal(19,6)")]
        [Appearance("DiscP", Enabled = false)]
        public decimal DiscP
        {
            get { return _DiscP; }
            set
            {
                SetPropertyValue("DiscP", ref _DiscP, value);
            }
        }

        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Unit Price After Discount")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [DbType("decimal(19,6)")]
        public decimal UnitPriceAfterDisc
        {
            get { return (UnitPrice - _DiscP); }
        }

        private vw_ItemSeries _Series;
        [NoForeignKey]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Series", Enabled = false)]
        [XafDisplayName("Item Series")]
        public vw_ItemSeries Series
        {
            get { return _Series; }
            set
            {
                SetPropertyValue("Series", ref _Series, value);
            }
        }

        [NonPersistent]
        [ImmediatePostData]
        [Index(23), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public decimal POOpenQty
        {
            get
            {
                decimal rtn = 0;
                if (this.DocNum != null && this.LinkOid != 0 && this.ItemCode != null)
                {
                    PurchaseOrderDetails PO;
                    PO = Session.FindObject<PurchaseOrderDetails>(CriteriaOperator.Parse("PODocNum = ? and Oid = ? and Item.BoCode = ?",
                        this.DocNum, this.LinkOid, this.ItemCode.BoCode));

                    if (PO != null)
                    {
                        rtn = PO.OpenQty;
                    }
                }
                return rtn;
            }
        }

        private decimal _DocDiscAmount;
        [Index(24), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("DocDiscAmount")]

        public decimal DocDiscAmount
        {
            get { return _DocDiscAmount; }
            set
            {
                SetPropertyValue("DocDiscAmount", ref _DocDiscAmount, value);
            }
        }

        private decimal _DocDiscUnitAmount;
        [Index(24), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("DocDiscUnitAmount")]

        public decimal DocDiscUnitAmount
        {
            get { return _DocDiscUnitAmount; }
            set
            {
                SetPropertyValue("DocDiscUnitAmount", ref _DocDiscUnitAmount, value);
            }
        }

        private decimal _DocDiscAmountAfter;
        [Index(24), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("DocDiscAmountAfter")]

        public decimal DocDiscAmountAfter
        {
            get {

                decimal rtn = 0;

                rtn = Quantity * DocDiscUnitAmount;

                return rtn;
            }
            set
            {
                SetPropertyValue("DocDiscAmountAfter", ref _DocDiscAmountAfter, value);
            }
        }

        private decimal _TaxUnitAmount;
        [ImmediatePostData]
        [Index(22), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [XafDisplayName("Tax Amount Per Unit")]

        public decimal TaxUnitAmount
        {
            get
            {

                return _TaxUnitAmount;

            }
            set
            {
                SetPropertyValue("TaxUnitAmount", ref _TaxUnitAmount, value);
            }
        }




        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private APInvoice _APInvoice;
        [Association("APInvoice-APInvoiceDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("APInvoice", Enabled = false)]
        public APInvoice APInvoice
        {
            get { return _APInvoice; }
            set { SetPropertyValue("APInvoice", ref _APInvoice, value); }
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
            }
        }

        protected override void OnSaved()
        {
            base.OnSaved();
            this.Reload();
        }
    }
}