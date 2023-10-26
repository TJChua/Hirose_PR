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
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [XafDisplayName("PR Item")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class vw_PR : XPLiteObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public vw_PR(Session session)
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

        [Key]
        [Browsable(true)]
        //private string _DocNo;
        [XafDisplayName("Doc No")]
        [Appearance("DocNo", Enabled = false)]
        [Index(0), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string DocNo
        {
            get; set;
        }

        [XafDisplayName("PR Doc Num")]
        [Appearance("SAPDocNum", Enabled = false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string SAPDocNum
        {
            get; set;
        }

        [XafDisplayName("PRLine")]
        [Appearance("PRLine", Enabled = false)]
        [Index(3), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int PRLine
        {
            get; set;
        }

        [XafDisplayName("Item Code")]
        [NoForeignKey]
        [Appearance("ItemCode", Enabled = false)]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemCode
        {
            get; set;
        }

        [XafDisplayName("Item Description")]
        [Appearance("ItemDescrip", Enabled = false)]
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemDescrip
        {
            get; set;
        }

        [XafDisplayName("UOM")]
        [Appearance("UOM", Enabled = false)]
        [Index(9), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string UOM
        {
            get; set;
        }

        [XafDisplayName("Open Qty")]
        [Appearance("OpenQty", Enabled = false)]
        [Index(10), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int OpenQty
        {
            get; set;
        }

        [XafDisplayName("Quantity")]
        [Appearance("Quantity", Enabled = false)]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public int Quantity
        {
            get; set;
        }

        [XafDisplayName("Tax")]
        [Appearance("Tax", Enabled = false)]
        [Index(15), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string Tax
        {
            get; set;
        }

        [XafDisplayName("Unit Price")]
        [Appearance("UnitPrice", Enabled = false)]
        [Index(18), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public decimal UnitPrice
        {
            get; set;
        }

        [XafDisplayName("Total (RM)")]
        [Appearance("Total", Enabled = false)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Total
        {
            get; set;
        }

        [XafDisplayName("BoCode")]
        [Appearance("BoCode", Enabled = false)]
        [Index(23), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string BoCode
        {
            get; set;
        }
    }
}