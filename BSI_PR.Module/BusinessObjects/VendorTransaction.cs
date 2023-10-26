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
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Editors;
using System.Globalization;
using System.Web.UI.WebControls;

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Vendor Transaction")]
    [NavigationItem("Vendor Transaction")]
    [Appearance("HideSaveButton", AppearanceItemType = "Action", TargetItems = "Save", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideDeleteButton", AppearanceItemType = "Action", TargetItems = "Delete", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideSaveAndCloseButton", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideSaveAndNewButton", AppearanceItemType = "Action", TargetItems = "SaveAndNew", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideEdit", AppearanceItemType = "Action", TargetItems = "Edit", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideNew", AppearanceItemType = "Action", TargetItems = "New", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("HideValidate", AppearanceItemType = "Action", TargetItems = "Validation", Criteria = "1=1", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]


    public class VendorTransaction : XPObject//, IStateMachineProvider
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public VendorTransaction(Session session)
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
            YesNo = YesNo.Y;
            DocDate2 = DateTime.Today;
            
        }
     


        private DateTime _DocDate2;
        [NonPersistent, RuleRequiredField()]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Date"), ToolTip("Enter Text")]
        //[Appearance("DocDate", Enabled = true)]
        public DateTime DocDate2
        {
            get { return _DocDate2; }
            set
            {
                SetPropertyValue("DocDate2", ref _DocDate2, value);
            }
        }

        private Departments _Department;
        [NonPersistent]
        [XafDisplayName("Department"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[DataSourceProperty("user.Department")]
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Department", Enabled = false)]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private YesNo _YesNo;
        [NonPersistent]
        [XafDisplayName("Show Details"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[DataSourceProperty("user.Department")]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Show Details", Enabled = true)]
        public YesNo YesNo
        {
            get { return _YesNo; }
            set
            {
                SetPropertyValue("YesNo", ref _YesNo, value);
            }
        }
    }
}