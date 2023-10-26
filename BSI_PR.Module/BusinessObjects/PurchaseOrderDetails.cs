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


    [XafDisplayName("PODetails")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class PurchaseOrderDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrderDetails(Session session)
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

          //Add by YJ
            //MOULDING
            if(CreateUser.DefaultDept.BoCode.ToString() =="MO")
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "008"));
            //PLATING
            else if (CreateUser.DefaultDept.BoCode.ToString() == "PL")
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "007"));
            //STAMPING
            else if (CreateUser.DefaultDept.BoCode.ToString() == "ST")
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "009"));
            //IC 
            else if (CreateUser.DefaultDept.BoCode.ToString() == "ICAMC")
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "002"));
            //SUBCON
            else if (CreateUser.DefaultDept.BoCode.ToString() == "SUBCON")
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "002"));
            //HSI
            else if (CreateUser.DefaultDept.BoCode.ToString() == "HSI")
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "012"));
            else
                Series = Session.FindObject<vw_ItemSeries>(new BinaryOperator("BoCode", "999"));


            DelDate = DateTime.Today;
            DocDate = DateTime.Today;
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
        [Size(254)]
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

        private decimal _OpenQty = 1;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "{0:n}")]
        [Appearance("OpenQty", Enabled = false)]
        [Index(12), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("OpenQty")]
        public decimal OpenQty
        {
            get { return _OpenQty; }
            set
            {
                SetPropertyValue("OpenQty", ref _OpenQty, value);
            }
        }

        private decimal _Quantity = 1;
        [ImmediatePostData]
        [DbType("numeric(19,6)")]
        [ModelDefault("DisplayFormat", "{0:n}")]
        [ModelDefault("EditMask", "{0:n}")]
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan, "0")]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Quantity")]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading && value != 0)
                {
                    OpenQty = Quantity;
                }

                if (!IsLoading)
                {

                    vw_taxes Rate;


                    //Rate = Session.FindObject<vw_taxes>
                    //    (CriteriaOperator.Parse("BoCode = ?", Tax));
                    Rate = Session.FindObject<vw_taxes>(new BinaryOperator("BoCode", Tax));

                    if (Rate != null)
                    {
                        TaxAmount = Math.Round((decimal.Parse(Rate.Rate.ToString()) * LineAmount * Quantity / 100), 2);
                        TaxRate = decimal.Parse(Rate.Rate.ToString());
                    }
                    else
                    {
                        TaxAmount = 0;
                        TaxRate = 0;
                    }


                    if (decimal.Parse(Rate.Rate.ToString()) > 0)
                    {
                        TaxAbleAmount = LineTotal;
                      
                    }
                    else
                    {
                        TaxAbleAmount = 0;

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

        private string _UOM;
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
        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "0.000001")]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[ModelDefault("DisplayFormat", "{0:n}")]
        //[ModelDefault("EditMask", "{0:n}")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [DbType("decimal(19,6)")]
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
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[ModelDefault("DisplayFormat", "{0:n}")]
        //[ModelDefault("EditMask", "{0:n}")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        [DbType("decimal(19,6)")]
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

                if (!IsLoading)
                {

                    vw_taxes Rate;


                    //Rate = Session.FindObject<vw_taxes>
                    //    (CriteriaOperator.Parse("BoCode = ?", Tax));
                    Rate = Session.FindObject<vw_taxes>(new BinaryOperator("BoCode", Tax));

                    if (Rate != null)
                    {
                        TaxAmount = Math.Round((decimal.Parse(Rate.Rate.ToString()) * LineAmount * Quantity / 100), 2);
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

        private vw_taxes _Tax;
        [NoForeignKey]
        [ImmediatePostData]
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        //[RuleRequiredField(DefaultContexts.Save, TargetCriteria = "Amount > 0")]
        [XafDisplayName("Tax Code")]
        public vw_taxes Tax
        {
            get {

                return _Tax; 
           
            }
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
                        TaxAmount = Math.Round((decimal.Parse(Rate.Rate.ToString()) * LineAmount *Quantity / 100), 2);
                        TaxRate = decimal.Parse(Rate.Rate.ToString());
                    }
                    else
                    {
                       TaxAmount = 0;
                        TaxRate = 0;
                    }

                    if (decimal.Parse(Rate.Rate.ToString())>0)
                    {
                        TaxAbleAmount = LineTotal;
                    }
                    else
                    {
                        TaxAbleAmount = 0;

                    }


                }

            }
        }

        private vw_ItemSeries _Series;
        [NoForeignKey]
        [Index(25), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("Tax Amount Per Unit")]

        public decimal TaxAmount
        {
            get 
            {
             
                return _TaxAmount;
                
            }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
            }
        }



        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Unit Price After Discount")]
        //[ModelDefault("DisplayFormat", "{0:n}")]
        //[ModelDefault("EditMask", "{0:n}")]

        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]
        public decimal LineAmount
        {
            get { return (Amount - _DiscP); }

        }

        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("Unit Price Without Tax")]
        [ModelDefault("DisplayFormat", "{0:n6}")]
        [ModelDefault("EditMask", "{0:n6}")]

        //[ModelDefault("DisplayFormat", "{0:n2}")]
        //[ModelDefault("EditMask", "{0:n2}")]
        public decimal LineAmountWithoutTax
        {
            get { return Math.Round((Amount - _DiscP) * Quantity, 2); }

        }

        private decimal _LineTotal;
        [ImmediatePostData]
        //[ModelDefault("DisplayFormat", "{0:n}")]
        //[ModelDefault("EditMask", "{0:n}")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [ModelDefault("EditMask", "{0:n2}")]

        [RuleValueComparison("", DefaultContexts.Save, ValueComparisonType.GreaterThan, "0")]
        [Index(24), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [XafDisplayName("Line Total")]

        public decimal LineTotal
        {
            //get { return Math.Round((LineAmount * Quantity) + TaxAmount, 2); }
            get { return Math.Round((LineAmount * Quantity), 2, MidpointRounding.AwayFromZero); }
            set
            {
                SetPropertyValue("LineTotal", ref _LineTotal, value);
            }
        }

        //New added
        //-------------------------------------------------------------------------------
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


        private decimal _TaxAbleAmount;
        [ImmediatePostData]
        [Index(24), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("TaxAbleAmount")]

        public decimal TaxAbleAmount
        {
            get { return _TaxAbleAmount; 
            }
            set
            {
                SetPropertyValue("TaxAbleAmount", ref _TaxAbleAmount, value);
            }
        }




        //New added
        //-------------------------------------------------------------------------------


        private string _DocNum;
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(26), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("DocNum")]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private int _PROid;
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(28), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        //[Appearance("Amount2", FontColor = "White", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='001'")]
        //[Appearance("Amount3", FontColor = "Yellow", Criteria = "(ClaimType.IsDetail or ClaimType.IsNote) and Region.BoCode='002'")]
        [XafDisplayName("PROid")]
        public int PROid
        {
            get { return _PROid; }
            set
            {
                SetPropertyValue("PROid", ref _PROid, value);
            }
        }

        private string _PODocNum;
        [Index(30), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [XafDisplayName("PODocNum")]
        [Appearance("PODocNum", Enabled = false)]
        public string PODocNum
        {
            get { return _PODocNum; }
            set
            {
                SetPropertyValue("PODocNum", ref _PODocNum, value);
            }
        }

        private PurchaseOrder _PurchaseOrder;
        [Association("PurchaseOrder-PurchaseOrderDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("PurchaseOrder", Enabled = false)]
        public PurchaseOrder PurchaseOrder
        {
            get { return _PurchaseOrder; }
            set { SetPropertyValue("PurchaseOrder", ref _PurchaseOrder, value); }
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