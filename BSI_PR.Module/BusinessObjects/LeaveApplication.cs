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

// 2023-09-26 default applicant and date ver 0.1
// 2024-01-18 allow to change fromdate ver 0.2

namespace BSI_PR.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Leave Application")]
    [DefaultProperty("Applicant")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancelleave", AppearanceItemType.Action, "True", TargetItems = "CancelLeave", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "DetailView")]
    [RuleCriteria("EscalateUserOnleaveValidation", DefaultContexts.Save, "IsValid = 0", "User attended escalate task on his/her holiday date range.")]
    [RuleCriteria("UserOnleaveValidation", DefaultContexts.Save, "IsValid1 = 0", "User on leave on that period.")]
    [RuleCriteria("DateOnleaveValidation", DefaultContexts.Save, "IsValid2 = 0", "No backdate allow.")]
    // Start ver 0.2
    [RuleCriteria("FromDateValidation", DefaultContexts.Save, "IsValid3 = 0", "From Date no backdate allow.")]
    // End ver 0.2
    public class LeaveApplication : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public LeaveApplication(Session session)
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

            CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);

            CreateDate = DateTime.Now;

            // Start ver 0.1
            Applicant = Session.GetObjectByKey<SystemUsers>(user.Oid);
            FromDate = DateTime.Today;
            // End ver 0.1
        }

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

        private SystemUsers _Applicant;
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("Applicant", Enabled = false, Criteria = "not IsNew")]
        [XafDisplayName("Applicant")]
        public SystemUsers Applicant
        {
            get { return _Applicant; }
            set { SetPropertyValue("Applicant", ref _Applicant, value); }
        }

        private DateTime _FromDate;
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        // Start ver 0.2
        //[Appearance("FromDate", Enabled = false)]
        // End ver 0.2
        [XafDisplayName("From Date")]
        public DateTime FromDate
        {
            get { return _FromDate; }
            set
            {
                SetPropertyValue("FromDate", ref _FromDate, value);
            }
        }

        private DateTime _ToDate;
        [Index(8), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("To Date")]
        public DateTime ToDate
        {
            get { return _ToDate; }
            set
            {
                SetPropertyValue("ToDate", ref _ToDate, value);
            }
        }

        private LeaveStatus _Status;
        [XafDisplayName("Status")]
        [Appearance("Status", Enabled = false)]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public LeaveStatus Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        private string _Reason;
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Reason")]
        public string Reason
        {
            get { return _Reason; }
            set
            {
                SetPropertyValue("Reason", ref _Reason, value);
            }
        }

        private SystemUsers _Escalate;
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        //[Appearance("Escalate", Enabled = false, Criteria = "not IsNew")]
        [XafDisplayName("Escalate")]
        public SystemUsers Escalate
        {
            get { return _Escalate; }
            set { SetPropertyValue("Escalate", ref _Escalate, value); }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                TimeSpan datecount;
                datecount = ToDate.Date - FromDate.Date;

                for (int i = 0; i <= datecount.Days; i++)
                {
                    LeaveApplication leave;
                    leave = Session.FindObject<LeaveApplication>(CriteriaOperator.Parse(
                        "Escalate.Oid = ? and ? >= FromDate and ? <= ToDate and Status = 0"
                        , this.Applicant.Oid, FromDate.Date.AddDays(i), FromDate.Date.AddDays(i)));

                    if (leave != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid1
        {
            get
            {
                TimeSpan datecount;
                datecount = ToDate.Date - FromDate.Date;

                for (int i = 0; i <= datecount.Days; i++)
                {
                    LeaveApplication leave;
                    leave = Session.FindObject<LeaveApplication>(CriteriaOperator.Parse(
                        "Applicant.Oid = ? and ? >= FromDate and ? <= ToDate and Status = 0"
                        , this.Escalate.Oid, FromDate.Date.AddDays(i), FromDate.Date.AddDays(i)));

                    if (leave != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid2
        {
            get
            {
                if (this.ToDate < this.FromDate)
                {
                    return true;
                }

                return false;
            }
        }

        // Start ver 0.2
        [Browsable(false)]
        public bool IsValid3
        {
            get
            {
                if (this.FromDate < DateTime.Today)
                {
                    return true;
                }

                return false;
            }
        }
        // End ver 0.2

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
    }
}