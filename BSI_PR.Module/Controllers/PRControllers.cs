using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSI_PR.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using CrystalDecisions.CrystalReports.Engine;
using System.Web;
using DevExpress.XtraReports.Configuration;
using System.Configuration;
using CrystalDecisions.Shared;
using System.IO;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Web.SystemModule;

namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PRControllers : ViewController
    {
        GenControllers genCon;
        public PRControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(PurchaseRequests);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            //this.Pass.Active.SetItemValue("Active", View.GetType() == typeof(DetailView));
            if (View.GetType() == typeof(DetailView))
            {
                //this.Pass.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
                ((DetailView)View).ViewEditModeChanged += PRControllers_ViewEditModeChanged;
            }

            this.Cancelled.Active.SetItemValue("Enabled", false);

            if (typeof(PurchaseRequests).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
                {
                    if (View.Id == "PurchaseRequests_DetailView")
                    {
                        this.Cancelled.Active.SetItemValue("Enabled", true);
                    }
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenControllers>();
        }

        private void PRControllers_ViewEditModeChanged(object sender, EventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                //this.Pass.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
           
            }
        }

        protected override void OnDeactivated()
        {
            if (View.GetType() == typeof(DetailView))
            {
                ((DetailView)View).ViewEditModeChanged -= PRControllers_ViewEditModeChanged;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void Accept_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsAccepted = true;
                selectedObject.IsRejected = false;
                PurchaseRequestDocStatuses ds = ObjectSpace.CreateObject<PurchaseRequestDocStatuses>();
                ds.DocStatus = DocumentStatus.Accepted;
                ds.DocRemarks = p.ParamString;
                selectedObject.PurchaseRequestDocStatus.Add(ds);

                PurchaseRequestAppStatuses dsApp = ObjectSpace.CreateObject<PurchaseRequestAppStatuses>();
                dsApp.AppStatus = ApprovalStatuses.Not_Applicable;
                dsApp.AppRemarks = "";
                selectedObject.PurchaseRequestAppStatus.Add(dsApp);

                //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

                //ModificationsController controller = Frame.GetController<ModificationsController>();
                //if (controller != null)
                //{
                //    controller.SaveAction.DoExecute();
                //}
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                #region approval
                XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                List<string> ToEmails = new List<string>();
                string emailbody = "";
                string emailsubject = "";
                string emailaddress = "";
                Guid emailuser;
                DateTime emailtime = DateTime.Now;
                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetApproval", new OperandValue(user.UserName), new OperandValue(selectedObject.Oid), new OperandValue("PurchaseRequests"));

                IObjectSpace emailos = Application.CreateObjectSpace();
                EmailSents emailobj = null;

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        string receiver = null;
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            if (receiver == null)
                            {
                                receiver = row.Values[4].ToString();
                            }
                            else
                            {
                                receiver = receiver + "/" + row.Values[4];
                            }
                        }

                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            emailbody = "Dear " + receiver + "," + System.Environment.NewLine + System.Environment.NewLine +
                                row.Values[3] + System.Environment.NewLine + GeneralSettings.appurl + row.Values[2].ToString() +
                                System.Environment.NewLine + System.Environment.NewLine + "Regards," + System.Environment.NewLine +
                                //selectedObject.CreateUser.FName.ToString();
                                "E-PO System";

                            emailsubject = "Purchase Request Approval";
                            emailaddress = row.Values[1].ToString();
                            emailuser = (Guid)row.Values[0];

                            ToEmails.Add(emailaddress);

                            emailobj = emailos.CreateObject<EmailSents>();
                            emailobj.CreateDate = (DateTime?)emailtime;
                            emailobj.EmailUser = emailobj.Session.GetObjectByKey<SystemUsers>(emailuser);
                            emailobj.EmailAddress = emailaddress;
                            //assign body will get error???
                            //emailobj.EmailBody = emailbody;
                            emailobj.Remarks = emailsubject;
                            emailobj.PurchaseRequest = emailobj.Session.GetObjectByKey<PurchaseRequests>(selectedObject.Oid);

                            emailobj.Save();
                        }
                    }
                }

                if (ToEmails.Count > 0)
                {
                    if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                    {
                        if (emailos.IsModified)
                            emailos.CommitChanges();
                    }
                }

                #endregion

                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
                genCon.openNewView(os, prtrx, ViewEditMode.View);
                genCon.showMsg("Successful", "Passed to approver.", InformationType.Success);
            }
            else
            {
                genCon.showMsg("Fail", "No PR Selected/More Than One PR Selected.", InformationType.Error);
            }
        }

        //private void switchToViewModeAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    ((DetailView)View).ViewEditMode = ViewEditMode.View;
        //    View.BreakLinksToControls();
        //    View.CreateControls();
        //}

        private void Accept_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Pass_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
                StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                if (p.IsErr) return;

                selectedObject.IsPassed = true;
                selectedObject.IsRejected = false;
                selectedObject.ApprovalStatus = ApprovalStatuses.Not_Applicable;
                selectedObject.ApprovalStatus = ApprovalStatuses.Not_Applicable;
                PurchaseRequestDocStatuses ds = ObjectSpace.CreateObject<PurchaseRequestDocStatuses>();
                ds.DocStatus = DocumentStatus.DocPassed;
                ds.DocRemarks = p.ParamString;
                selectedObject.PurchaseRequestDocStatus.Add(ds);
                //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

                if (!selectedObject.DocType.IsPassAccept)
                {
                    //ModificationsController controller = Frame.GetController<ModificationsController>();
                    //if (controller != null)
                    //{
                    //    controller.SaveAction.DoExecute();
                    //}
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
                if (selectedObject.DocType.IsPassAccept)
                {
                    Accept_Execute(sender, e);
                }
                if (!selectedObject.DocType.IsPassAccept)
                {
                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
                    genCon.openNewView(os, prtrx, ViewEditMode.View);
                    genCon.showMsg("Successful", "Passing Done.", InformationType.Success);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PR Selected/More Than One PR Selected.", InformationType.Error);
            }
        }

        private void Pass_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Closed_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.IsClosed = true;
            selectedObject.IsRejected = false;
            PurchaseRequestDocStatuses ds = ObjectSpace.CreateObject<PurchaseRequestDocStatuses>();
            ds.DocStatus = DocumentStatus.Closed;
            ds.DocRemarks = p.ParamString;
            selectedObject.PurchaseRequestDocStatus.Add(ds);
            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                controller.SaveAction.DoExecute();
            }

            IObjectSpace os = Application.CreateObjectSpace();
            PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
            genCon.openNewView(os, prtrx, ViewEditMode.View);
            genCon.showMsg("Successful", "Closed Done.", InformationType.Success);
        }

        private void Closed_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Approval_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count >= 1)
            {
                int totaldoc = 0;

                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                foreach (PurchaseRequests dtl in e.SelectedObjects)
                {
                    IObjectSpace pos = Application.CreateObjectSpace();
                    PurchaseRequests pr = pos.FindObject<PurchaseRequests>(new BinaryOperator("Oid", dtl.Oid));

                    pr.Escalate = null;
                    pr.WhoApprove = pr.WhoApprove + user.UserName + ", ";
                    ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;

                    //if (selectedObject.PurchaseRequestAppStatus.Count() > 0)
                    //    appstatus = selectedObject.PurchaseRequestAppStatus.OrderBy(c => c.Oid).Last().AppStatus;

                    //if (appstatus != ApprovalStatuses.Not_Applicable)
                    //    if (pr.PurchaseRequestAppStatus.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                    //        appstatus = pr.PurchaseRequestAppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

                    if (appstatus == ApprovalStatuses.Not_Applicable)
                        appstatus = ApprovalStatuses.Required_Approval;


                    if (p.IsErr) return;
                    if (appstatus == ApprovalStatuses.Required_Approval && p.AppStatus == ApprovalActions.NA)
                    {
                        genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                        return;
                    }
                    else if (appstatus == ApprovalStatuses.Approved && p.AppStatus == ApprovalActions.Yes)
                    {
                        genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                        return;
                    }
                    else if (appstatus == ApprovalStatuses.Rejected && p.AppStatus == ApprovalActions.No)
                    {
                        genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                        return;
                    }
                    if (p.AppStatus == ApprovalActions.NA)
                    {
                        appstatus = ApprovalStatuses.Required_Approval;
                    }
                    if (p.AppStatus == ApprovalActions.Yes)
                    {
                        appstatus = ApprovalStatuses.Approved;
                    }
                    if (p.AppStatus == ApprovalActions.No)
                    {
                        appstatus = ApprovalStatuses.Rejected;
                    }

                    PurchaseRequestAppStatuses ds = pos.CreateObject<PurchaseRequestAppStatuses>();
                    ds.PurchaseRequest = pos.GetObjectByKey<PurchaseRequests>(pr.Oid); ;
                    ds.AppStatus = appstatus;
                    ds.AppRemarks = p.ParamString;
                    if (appstatus == ApprovalStatuses.Rejected)
                    {
                        ds.AppRemarks = p.ParamString +
                            System.Environment.NewLine + "(Reject User: " + user.FName + ")" +
                            System.Environment.NewLine + "(Reason: Approval Rejected)";
                        ds.CreateUser = pos.GetObjectByKey<SystemUsers>(Guid.Parse("02868725-036A-483B-A123-2932180D4A03")); ;
                    }
                    else
                    {
                        ds.AppRemarks = p.ParamString +
                            System.Environment.NewLine + "(Approved User: " + user.FName + ")";
                    }
                    pr.PurchaseRequestAppStatus.Add(ds);

                    pos.CommitChanges();
                    pos.Refresh();

                    totaldoc++;

                    #region approval
                    XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

                    List<string> ToEmails = new List<string>();
                    string emailbody = "";
                    string emailsubject = "";
                    string emailaddress = "";
                    Guid emailuser;
                    DateTime emailtime = DateTime.Now;
                    SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_Approval", new OperandValue(user.UserName), new OperandValue(pr.Oid), new OperandValue("PurchaseRequests"), new OperandValue((int)appstatus));

                    IObjectSpace emailos = Application.CreateObjectSpace();
                    EmailSents emailobj = null;

                    if (sprocData.ResultSet.Count() > 0)
                    {
                        if (sprocData.ResultSet[0].Rows.Count() > 0)
                        {
                            string receiver = null;
                            foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                            {
                                if (receiver == null)
                                {
                                    receiver = row.Values[4].ToString();
                                }
                                else
                                {
                                    receiver = receiver + "/" + row.Values[4];
                                }
                            }

                            foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                            {
                                emailbody = "Dear " + receiver + "," + System.Environment.NewLine + System.Environment.NewLine +
                                    row.Values[3] + System.Environment.NewLine + GeneralSettings.appurl + row.Values[2].ToString() +
                                    System.Environment.NewLine + System.Environment.NewLine + "Regards," + System.Environment.NewLine +
                                    //pr.CreateUser.FName.ToString();
                                    "E-PO System";

                                if (appstatus == ApprovalStatuses.Approved)
                                    emailsubject = "Purchase Request Approval";
                                else if (appstatus == ApprovalStatuses.Rejected)
                                    emailsubject = "Purchase Request Approval Rejected";

                                emailaddress = row.Values[1].ToString();
                                emailuser = (Guid)row.Values[0];

                                ToEmails.Add(emailaddress);

                                emailobj = emailos.CreateObject<EmailSents>();
                                emailobj.CreateDate = (DateTime?)emailtime;
                                emailobj.EmailUser = emailobj.Session.GetObjectByKey<SystemUsers>(emailuser);
                                emailobj.EmailAddress = emailaddress;
                                //assign body will get error???
                                emailobj.EmailBody = emailbody;
                                emailobj.Remarks = emailsubject;
                                emailobj.PurchaseRequest = emailobj.Session.GetObjectByKey<PurchaseRequests>(pr.Oid);

                                emailobj.Save();
                            }
                        }
                    }

                    if (ToEmails.Count > 0)
                    {
                        if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                        {
                            if (emailos.IsModified)
                                emailos.CommitChanges();
                        }
                    }
                    #endregion

                }

                ObjectSpace.CommitChanges(); //This line persists created object(s).
                ObjectSpace.Refresh();

                genCon.showMsg("Info", "Total Document : " + totaldoc + " Approval Done.", InformationType.Info);
            }
            else
            {
                genCon.showMsg("Fail", "No PR selected.", InformationType.Error);
            }
            //IObjectSpace os = Application.CreateObjectSpace();
            //PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
            //genCon.openNewView(os, prtrx, ViewEditMode.View);

            //IObjectSpace os = Application.CreateObjectSpace();
            //PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
            //genCon.openNewView(os, prtrx, ViewEditMode.View);
            //genCon.showMsg("Successful", "Approval Done.", InformationType.Success);

        }

        private void Approval_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            PurchaseRequests selectedObject = (PurchaseRequests)View.CurrentObject;

            //if (((DetailView)View).ViewEditMode == ViewEditMode.View)
            //{
            //    genCon.showMsg("Failed", "Viewing Document cannot proceed.", InformationType.Error);
            //    err = true;
            //}
            //else
            //{
                //if (!selectedObject.IsAccepted)
                //{
                //    genCon.showMsg("Failed", "Document is not Accepted.", InformationType.Error);
                //    err = true;
                //}
                //if (!selectedObject.IsApprovalUserCheck)
                //{
                //    genCon.showMsg("Failed", "This is not a Approval User process.", InformationType.Error);
                //    err = true;
                //}
                //if (selectedObject.IsClosed)
                //{
                //    genCon.showMsg("Failed", "Document is Closed.", InformationType.Error);
                //    err = true;
                //}
                //if (selectedObject.ApprovalStatus == ApprovalStatuses.Not_Applicable)
                //{
                //    genCon.showMsg("Failed", "Document is not Approval Required.", InformationType.Error);
                //    err = true;
                //}
            //}
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            ApprovalStatuses appstatus = ApprovalStatuses.Required_Approval;

            //if (selectedObject.PurchaseRequestAppStatus.Where(p => p.CreateUser.Oid == user.Oid).Count() > 0)
            //     appstatus = selectedObject.PurchaseRequestAppStatus.Where(p => p.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStatus;

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            switch (appstatus)
            {
                case ApprovalStatuses.Required_Approval:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                    break;
                case ApprovalStatuses.Approved:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    break;
                case ApprovalStatuses.Rejected:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                    break;
            }
            ((ApprovalParameters)dv.CurrentObject).IsErr = err;
            ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void Duplicate_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                PurchaseRequests PurchaseRequest = (PurchaseRequests)View.CurrentObject;
                IObjectSpace os = Application.CreateObjectSpace();
                PurchaseRequests newPR = os.CreateObject<PurchaseRequests>();
                
                newPR.DocDate = PurchaseRequest.DocDate;
                newPR.Vendor = newPR.Session.GetObjectByKey<vw_vendors>(PurchaseRequest.Vendor.BoCode);
                newPR.Department = newPR.Session.GetObjectByKey<Departments>(PurchaseRequest.Department.Oid);
                newPR.RefNo = PurchaseRequest.RefNo;
                // newPR.ApprovalStatus = PurchaseRequest.ApprovalStatus;
                newPR.DocType = newPR.Session.GetObjectByKey<DocTypes>(PurchaseRequest.DocType.Oid);
                newPR.IsBudget = PurchaseRequest.IsBudget;
                newPR.Remarks = PurchaseRequest.Remarks;
                newPR.DocDisc= PurchaseRequest.DocDisc;

                foreach (PurchaseRequestDetails dtl in PurchaseRequest.PurchaseRequestDetail)
                {
                    PurchaseRequestDetails newPRdetails = os.CreateObject<PurchaseRequestDetails>();

                    newPRdetails.Item = newPRdetails.Session.GetObjectByKey<vw_items>(dtl.Item.BoCode);
                    newPRdetails.ItemDesc = dtl.ItemDesc;
                    newPRdetails.Series = newPRdetails.Session.GetObjectByKey<vw_ItemSeries>(dtl.Series.BoCode);
                    newPRdetails.DocDate = dtl.DocDate;
                    newPRdetails.DelDate = dtl.DelDate;
                    newPRdetails.Quantity = dtl.Quantity;
                    newPRdetails.UOM = dtl.UOM;
                    newPRdetails.DiscP = dtl.DiscP;
                    newPRdetails.LineTotal = dtl.LineTotal;
                    newPRdetails.RefNo = dtl.RefNo;
                    newPRdetails.Remarks = dtl.Remarks;
                    newPRdetails.Amount = dtl.Amount;
                    newPRdetails.TaxAmount = dtl.TaxAmount;
                    if (dtl.Tax != null)
                    {
                        newPRdetails.Tax = newPRdetails.Session.GetObjectByKey<vw_taxes>(dtl.Tax.BoCode);
                    }
                    newPR.PurchaseRequestDetail.Add(newPRdetails);

                }

                ShowViewParameters svp = new ShowViewParameters();
                DetailView dv = Application.CreateDetailView(os, newPR);
                dv.ViewEditMode = ViewEditMode.Edit;
                dv.IsRoot = true;
                svp.CreatedView = dv;

                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                //genCon.openNewView(os, newPR, ViewEditMode.Edit);
                genCon.showMsg("Success", "Duplicate Success.", InformationType.Success);
            }
            catch (Exception)
            {
                genCon.showMsg("Fail", "Duplicate Fail.", InformationType.Error);
            }
        }

        protected string GetScript(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);

            //To Download PDF
            //return @"var mainDocument = window.parent.document;
            //var iframe = mainDocument.getElementById('reportout');
            //if (iframe != null) {
            //  mainDocument.body.removeChild(iframe);
            //}
            //iframe = mainDocument.createElement('iframe');
            //iframe.setAttribute('id', 'reportout');
            //iframe.style.width = 0 + 'px';
            //iframe.style.height = 0 + 'px';
            //iframe.style.display = 'none';
            //mainDocument.body.appendChild(iframe);
            //mainDocument.getElementById('reportout').contentWindow.location = 'DownloadFile.aspx?filename=" + fileInfo.Name + "';";

            //To View PDF
            //return @"var newWindow = window.open();
            //newWindow.document.write('<iframe src=""Download.aspx?filename=" + fileInfo.Name + @""" frameborder =""0"" allowfullscreen style=""width: 100%;height: 100%"" type=""application / pdf"" position:absolute ></iframe>');
            //";
            return @"var newWindow = window.open('about:blank', '_blank');
            newWindow.document.write('<iframe src=""Download.aspx?filename=" + fileInfo.Name + "&embedded=true"+ @""" frameborder =""0"" allowfullscreen style=""width: 100%;height: 100%"" type=""application/pdf""></iframe>');
            ";


            

        }
       
        public void MsgBox(string PopUpMsg)
        {
            try
            {
                MessageOptions options = new MessageOptions();
                options.Duration = 7000;
                options.Message = string.Format(PopUpMsg);
                options.Type = InformationType.Error;
                options.Web.Position = InformationPosition.Right;
                options.Win.Caption = "Error";
                options.Win.Type = WinMessageType.Flyout;
                Application.ShowViewStrategy.ShowMessage(options);
            }
            catch
            {
                throw new Exception(PopUpMsg);
            }
        }

        private void PrintPR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                PurchaseRequests PR = (PurchaseRequests)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\PRreport.rpt"));
                    strServer = GeneralSettings.B1Server;
                    strDatabase = GeneralSettings.ReportDB;
                    strUserID = GeneralSettings.B1DbUserName;
                    strPwd = GeneralSettings.B1DbPassword;
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.SetParameterValue("PRNumber", PR.DocNum);


                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_PR_" + PR.DocNum + "_" + user.UserName + "_"
                       + DateTime.Parse(PR.DocDate.ToString()).ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                 

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", GetScript(filename));
                }
                catch (Exception ex)
                {
                    MsgBox(ex.Message);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PR Selected.", InformationType.Error);
            }

        }

        private void PrintPO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            if (e.SelectedObjects.Count > 0)
            {
                PurchaseRequests PR = (PurchaseRequests)View.CurrentObject;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\PO.rpt"));
                    strServer = GeneralSettings.B1Server;
                    strDatabase = GeneralSettings.B1CompanyDB;
                    strUserID = GeneralSettings.B1DbUserName;
                    strPwd = GeneralSettings.B1DbPassword;
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.SetParameterValue("DocNum", PR.DocNum);


                    filename = GeneralSettings.ReportPath.ToString() + GeneralSettings.B1CompanyDB.ToString()
                       + "_PO_" + PR.DocNum + "_" + user.UserName + "_"
                       + DateTime.Parse(PR.DocDate.ToString()).ToString("yyyyMMdd") + DateTime.Now.ToString("hhmmss") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();



                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", GetScript(filename));
                }
                catch (Exception ex)
                {
                    MsgBox(ex.Message);
                }
            }
            else
            {
                genCon.showMsg("Fail", "No PO Selected.", InformationType.Error);
            }
        }

        private void PostSAP_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
            //StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            //if (p.IsErr) return;


            IObjectSpace ios = Application.CreateObjectSpace();
            PurchaseRequests iobj = ios.GetObjectByKey<PurchaseRequests>(selectedObject.Oid);

            if (iobj.PurchaseRequestStatus != "Posted")
            {
                #region post PO

                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                if (string.IsNullOrEmpty(user.CurrDept))
                    user.CurrDept = user.DefaultDept == null ? "" : user.DefaultDept.BoCode;



                if (genCon.ConnectSAP(user.CurrDept))
                {
                    PurchaseRequests selectedSI = (PurchaseRequests)View.CurrentObject;
                    GeneralSettings.oCompany.StartTransaction();

                    int temp = 0;

                    ShowViewParameters svp = new ShowViewParameters();
                    DetailView dv = Application.CreateDetailView(ios, iobj);
                    dv.ViewEditMode = ViewEditMode.View;
                    svp.CreatedView = dv;

                    if (iobj.PurchaseRequestStatus != "Posted")
                    {
                        this.genCon.Active.SetItemValue("Enabled", false);
                        temp = genCon.PostAPIVtoSAP(iobj);
                        if (temp == 1)
                        {
                            if (GeneralSettings.oCompany.InTransaction)
                                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                            ios.CommitChanges();

                            selectedObject.IsPosted = true;
                            selectedObject.IsRejected = false;

                            IObjectSpace os = Application.CreateObjectSpace();
                            vw_PO PRNum = os.FindObject<vw_PO>(new BinaryOperator("PRNO", selectedObject.DocNum));

                            selectedObject.PO = selectedObject.Session.GetObjectByKey<vw_PO>(PRNum.PRNO);

                            //PurchaseRequestDocStatuses ds = ObjectSpace.CreateObject<PurchaseRequestDocStatuses>();
                            //ds.DocStatus = DocumentStatus.DocPassed;
                            //ds.DocRemarks = p.ParamString;
                            //selectedObject.PurchaseRequestDocStatus.Add(ds);
                            //selectedObject.OnPropertyChanged("ClaimTrxDocStatus");



                            //IObjectSpace os = Application.CreateObjectSpace();
                            //PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
                            //genCon.openNewView(os, prtrx, ViewEditMode.View);
                            genCon.showMsg("Successful", "Posting Done.", InformationType.Success);

                            //ios.CommitChanges();
                        }
                        else if (temp <= 0)
                        {
                            if (GeneralSettings.oCompany.InTransaction)
                                GeneralSettings.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                            IObjectSpace os = Application.CreateObjectSpace();
                            PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
                            genCon.openNewView(os, prtrx, ViewEditMode.View);
                            genCon.showMsg("Failed", "Posting Failed.", InformationType.Error);
                        }
                        GeneralSettings.oCompany.Disconnect();
                    }

                }
                #endregion

                ObjectSpace.CommitChanges(); //This line persists created object(s).
                ObjectSpace.Refresh();
            }
        }

        private void Cancelled_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

            try { 
                    PurchaseRequests selectedObject = (PurchaseRequests)e.CurrentObject;
                    StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
                    if (p.IsErr) return;

                    //selectedObject.IsClosed = true;
                    selectedObject.IsCancelled = true;
                    selectedObject.IsRejected = false;
                    PurchaseRequestDocStatuses ds = ObjectSpace.CreateObject<PurchaseRequestDocStatuses>();
                    ds.DocStatus = DocumentStatus.Cancelled;
                    ds.DocRemarks = p.ParamString;
                    selectedObject.PurchaseRequestDocStatus.Add(ds);
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();

                    IObjectSpace os = Application.CreateObjectSpace();
                    PurchaseRequests prtrx = os.FindObject<PurchaseRequests>(new BinaryOperator("Oid", selectedObject.Oid));
                    genCon.openNewView(os, prtrx, ViewEditMode.View);
                    genCon.showMsg("Successful", "Document Cancelled.", InformationType.Success);

            }
            catch (Exception)
            {
                genCon.showMsg("Fail", "Cancellation Fail.", InformationType.Error);
            }
        }

        private void Cancelled_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }
    }
}
