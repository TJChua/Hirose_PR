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
    [XafDisplayName("Goods Return Details")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class GoodsReturnDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public GoodsReturnDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
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
        //[DataSourceCriteria("ReturnToVendor = '1'")]
        //[DataSourceCriteria("Vendor.BoCode = '@this.Vendor.BoCode' and Outlet.BoCode = '@this.Warehouse.BoCode' and ReturnToVendor = '1'")]
        [Appearance("ItemCode", Enabled = false)]
        public vw_items ItemCode
        {
            get { return _ItemCode; }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
            }
        }

        private string _ItemDescrip;
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Item Description")]
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

        private int _Quantity;
        [ImmediatePostData]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Quantity")]
        //[Appearance("Quantity", Enabled = false)]
        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    if (Quantity <= 0)
                    {
                        Quantity = 1;
                    }
                }
            }
        }

        private decimal _UnitPrice;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [ModelDefault("EditMask", "{0:n4}")]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("UnitPrice", Enabled = false)]
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
            }
        }

        private decimal _TaxAmount;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [ModelDefault("EditMask", "{0:n4}")]
        [Index(20), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
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
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Line Amount")]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [ModelDefault("EditMask", "{0:n4}")]
        [Appearance("LineAmount", Enabled = false)]
        public decimal LineAmount
        {
            get
            {
                return ((UnitPrice * Quantity) + TaxAmount);
            }
            set
            {
                SetPropertyValue("LineAmount", ref _LineAmount, value);
            }
        }

        private string _GRND;
        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("GRN Code")]
        [Appearance("GRND", Enabled = false)]
        public string GRND
        {
            get { return _GRND; }
            set { SetPropertyValue("GRND", ref _GRND, value); }
        }

        private int _BaseLine;
        [Index(28), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("BaseLine")]
        [Appearance("BaseLine", Enabled = false)]
        public int BaseLine
        {
            get { return _BaseLine; }
            set { SetPropertyValue("BaseLine", ref _BaseLine, value); }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private string _SAPGRNNum;
        [Index(33), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("SAP GRN Num.")]
        [Appearance("SAPGRNNum", Enabled = false)]
        public string SAPGRNNum
        {
            get { return _SAPGRNNum; }
            set { SetPropertyValue("SAPGRNNum", ref _SAPGRNNum, value); }
        }

        private string _PODocNum;
        [Index(35), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("PODocNum")]
        [Appearance("PODocNum", Enabled = false)]
        public string PODocNum
        {
            get { return _PODocNum; }
            set { SetPropertyValue("PODocNum", ref _PODocNum, value); }
        }

        private int _POOid;
        [Index(38), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("POOid")]
        [Appearance("POOid", Enabled = false)]
        public int POOid
        {
            get { return _POOid; }
            set { SetPropertyValue("POOid", ref _POOid, value); }
        }

        private GoodsReturn _GoodsReturn;
        [Association("GoodsReturn-GoodsReturnDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("GoodReturns", Enabled = false)]
        public GoodsReturn GoodsReturn
        {
            get { return _GoodsReturn; }
            set { SetPropertyValue("GoodsReturn", ref _GoodsReturn, value); }
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
            }
        }

        protected override void OnSaved()
        {
            base.OnSaved();
            this.Reload();
        }
    }
}