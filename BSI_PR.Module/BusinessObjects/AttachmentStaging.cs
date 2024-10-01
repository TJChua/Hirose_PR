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
    public class AttachmentStaging : FileAttachmentBase
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public AttachmentStaging(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string _FileOid;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("FileOid")]
        [Appearance("FileOid", Enabled = false)]
        public string FileOid
        {
            get { return _FileOid; }
            set
            {
                SetPropertyValue("FileOid", ref _FileOid, value);
            }
        }

        private string _LinkOid;
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("LinkOid")]
        [Appearance("LinkOid", Enabled = false)]
        public string LinkOid
        {
            get { return _LinkOid; }
            set
            {
                SetPropertyValue("LinkOid", ref _LinkOid, value);
            }
        }

        private string _Remarks;
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Remarks")]
        [Appearance("Remarks", Enabled = false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }
    }
}