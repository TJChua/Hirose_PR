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
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.ConditionalAppearance;

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty("FName")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class SystemUsers : PermissionPolicyUser
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public SystemUsers(Session session)
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
        //    set { SetPropertyValue("PersistentProperty", ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}
        private string _UserEmail;
        [Size(150)]
        [RuleRequiredField(DefaultContexts.Save)]
        public string UserEmail
        {
            get { return _UserEmail; }
            set { SetPropertyValue("UserEmail", ref _UserEmail, value); }
        }

        private string _FName;
        [RuleRequiredField(DefaultContexts.Save)]
        public string FName
        {
            get { return _FName; }
            set { SetPropertyValue("FName", ref _FName, value); }
        }

        private string _NickName;
        public string NickName
        {
            get { return _NickName; }
            set { SetPropertyValue("NickName", ref _NickName, value); }
        }

        private Departments _DefaultDept;
        [NoForeignKey]
        [RuleRequiredField(DefaultContexts.Save)]
        public Departments DefaultDept
        {
            get { return _DefaultDept; }
            set { SetPropertyValue("DefaultDept", ref _DefaultDept, value); }
        }

        private vw_vendors _Vendor;
        [NoForeignKey]
        public vw_vendors Vendor
        {
            get { return _Vendor; }
            set { SetPropertyValue("Vendor", ref _Vendor, value); }
        }

        [Browsable(false)]
        public string CurrDept { get; set; }

        [Browsable(false)]
        public string CurrDocType { get; set; }

        [Association("UserDepartment")]
        [XafDisplayName("Department")]
        public XPCollection<Departments> Department
        {
            get { return GetCollection<Departments>("Department"); }
        }

        [Browsable(false)]
        [Association("ApprovalTriggers")]
        [XafDisplayName("Trigger User")]
        public XPCollection<Approvals> TriggerApproval
        {
            get { return GetCollection<Approvals>("TriggerApproval"); }
        }

        [Browsable(false)]
        [Association("ApprovalUsers")]
        [XafDisplayName("Approve User")]
        public XPCollection<Approvals> UserApproval
        {
            get { return GetCollection<Approvals>("UserApproval"); }
        }
    }
}