using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSI_PR.Module.BusinessObjects
{
    //public enum DocumentStatus
    //{
    //    DRAFT = 0,
    //    OPEN = 1,
    //    CLOSE = 3
    //}

    //public enum MaintenanceStatus
    //{
    //    DRAFT = 0,
    //    OPEN = 1,
    //    WIP = 2,
    //    CLOSE = 3
    //}

    //public enum Region
    //{
    //    Local = 0,
    //    Oversea = 1
    //}

    public enum DocumentStatus
    {
        Create = 0,
        Submit = 1,
        DocPassed = 2,
        Accepted = 3,
        Cancelled = 4,
        Rejected = 5,
        Closed = 6,
        Reopen = 7,
        Posted = 8

    }

    public enum ESAPDocs
    {
        DraftVendorPO = 1,
        VendorPO = 0
    }

    public enum ApprovalStatuses
    {
        Not_Applicable = 0,
        Approved = 1,
        Required_Approval = 2,
        Rejected = 3
    }

    public enum ApprovalActions
    {
        NA = 0,
        Yes = 1,
        No = 2
    }
    public enum ApprovalTypes
    {
        Budget = 0,
        Document = 1,
        SQL = 2
    }
    public enum PRTypes
    {
        PR = 0,
        PO = 1,
        GRN = 2,
        GR = 3,
        Invoice = 4,
        Japan = 5
    }

    public enum YesNo
    {
       Y=1,
       N=0
    }

    public enum POStatus
    {
        All = 'A', //No Filter
        Full_Open = 'F', // Filter where sum(quantity) = SUM(OpenQty)
        Partial_Open = 'P' // Filter where  SUM(OpenQty) < sum(quantity)
    }

    public enum LeaveStatus
    {
        Open = 0,
        Closed = 1,
        Cancel = 2
    }

    public enum CategoryMonth
    {
        January = 0,
        February = 1,
        March = 2,
        April = 3,
        May = 4,
        June = 5,
        July = 6,
        August = 7,
        September = 8,
        October = 9,
        November = 10,
        December = 11
    }
}
