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
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    [NavigationItem("Setup")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [RuleCriteria("DepartmentsDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Departments : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Departments(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsActive = true;

            DevExpress.Xpo.Metadata.XPClassInfo myClass = Session.GetClassInfo(typeof(DocTypes));
            CriteriaOperator myCriteria = new BinaryOperator("IsActive", true);
            SortingCollection sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));

            var LDocType = Session.GetObjects(myClass, myCriteria, sortProps, 0, false, true);

            //XPCollection<DocTypes> LDocType = new XPCollection<DocTypes>();
            //LDocType.Load();

            int cnt = 0;
            foreach (var dtl in LDocType)
            {
                cnt++;
                DepartmentDocs obj = new DepartmentDocs(Session);
                obj.DocType = Session.FindObject<DocTypes>(new BinaryOperator("Oid", ((DocTypes)dtl).Oid, BinaryOperatorType.Equal));
                obj.NextDocNo = (cnt * 1000000) + 1;
                this.DepartmentDoc.Add(obj);
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

        private string _BoCode;
        [XafDisplayName("Code"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleUniqueValue]
        [Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public string BoCode
        {
            get { return _BoCode; }
            set
            {                
                SetPropertyValue("BoCode", ref _BoCode, value);
            }
        }

        private string _BoName;
        [XafDisplayName("Name"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string BoName
        {
            get { return _BoName; }
            set
            {
                SetPropertyValue("BoName", ref _BoName, value);
            }
        }

        [Index(2), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return BoCode + "-" + BoName; }
        }

        private string _DocPrefix;
        [XafDisplayName("Doc Prefix"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(3)]
        public string DocPrefix
        {
            get { return _DocPrefix; }
            set
            {
                SetPropertyValue("DocPrefix", ref _DocPrefix, value);
            }
        }
        private vw_costcenter _SAPCostCenter;
        [NoForeignKey]
        [XafDisplayName("SAP Cost Center")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(5)]
        public vw_costcenter SAPCostCenter
        {
            get { return _SAPCostCenter; }
            set
            {
                SetPropertyValue("SAPCostCenter", ref _SAPCostCenter, value);
            }
        }
        private bool _IsActive;
        [XafDisplayName("Active")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        private int _SignLevel;
        [XafDisplayName("Signature Level")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(100)]
        public int SignLevel
        {
            get { return _SignLevel; }
            set
            {
                SetPropertyValue("SignLevel", ref _SignLevel, value);
            }
        }



        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }
        [Association("UserDepartment")]
        public XPCollection<SystemUsers> SystemUser
        {
            get { return GetCollection<SystemUsers>("SystemUser"); }
        }

        [Association("Department-DepartmentDocs")]
        [Appearance("DepartmentDoc", Enabled = false)]
        public XPCollection<DepartmentDocs> DepartmentDoc
        {
            get { return GetCollection<DepartmentDocs>("DepartmentDoc"); }
        }

    }
}