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
    [Appearance("ClaimTrxEmailSents1", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxEmailSents2", AppearanceItemType = "Action", TargetItems = "Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxEmailSents3", AppearanceItemType = "Action", TargetItems = "Link", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClaimTrxEmailSents4", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [DefaultProperty("StatusInfo")]
    [XafDisplayName("Email Sent")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class EmailSents : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public EmailSents(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;

        }

        [NonPersistent]
        public string StatusInfo
        {
            get
            {
                string temp = "";
                if (CreateUser != null)
                {
                    temp = string.Format("{0:yyyy/MM/dd HH:mm:ss}", CreateDate);
                    temp += " [" + CreateUser.UserName + "]";
                }
                return temp;
            }
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

        private SystemUsers _EmailUser;
        [XafDisplayName("Email User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("EmailUser", Enabled = false)]
        public SystemUsers EmailUser
        {
            get { return _EmailUser; }
            set
            {
                SetPropertyValue("EmailUser", ref _EmailUser, value);
            }
        }
        private string _EmailAddress;
        [XafDisplayName("Remarks")]
        [Appearance("EmailAddress", Enabled = false)]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string EmailAddress
        {
            get { return _EmailAddress; }
            set
            {
                SetPropertyValue("EmailAddress", ref _EmailAddress, value);
            }
        }
        private string _EmailBody;
        [XafDisplayName("Email Body")]
        [Appearance("EmailBody", Enabled = false)]
        [Index(12), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string EmailBody
        {
            get { return _EmailBody; }
            set
            {
                SetPropertyValue("EmailBody", ref _EmailBody, value);
            }
        }
        private string _Remarks;
        [XafDisplayName("Remarks")]
        [Appearance("Remarks", Enabled = false)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private PurchaseRequests _PurchaseRequest;
        [Index(99), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("PurchaseRequest", Enabled = false)]
        public PurchaseRequests PurchaseRequest
        {
            get { return _PurchaseRequest; }
            set { SetPropertyValue("PurchaseRequest", ref _PurchaseRequest, value); }
        }

        private PurchaseOrder _PurchaseOrder;
        [Index(99), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("PurchaseOrder", Enabled = false)]
        public PurchaseOrder PurchaseOrder
        {
            get { return _PurchaseOrder; }
            set { SetPropertyValue("PurchaseOrder", ref _PurchaseOrder, value); }
        }

    }
}