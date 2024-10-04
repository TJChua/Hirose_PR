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
    [DefaultClassOptions]
    public class AttachmentHistory : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public AttachmentHistory(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private DocTypes _DocType;
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("DocType")]
        [Appearance("DocType", Enabled = false)]
        public DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private string _DocNum;
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("DocNum")]
        [Appearance("DocNum", Enabled = false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private string _ApprovalUser;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("ApprovalUser")]
        [Appearance("ApprovalUser", Enabled = false)]
        public string ApprovalUser
        {
            get { return _ApprovalUser; }
            set
            {
                SetPropertyValue("ApprovalUser", ref _ApprovalUser, value);
            }
        }
    }
}