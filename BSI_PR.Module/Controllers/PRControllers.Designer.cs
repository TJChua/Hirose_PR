namespace BSI_PR.Module.Controllers
{
    partial class PRControllers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Pass = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Accept = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Closed = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Approval = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Duplicate = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.PrintPR = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.PrintPO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.Post = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.Cancelled = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // Pass
            // 
            this.Pass.AcceptButtonCaption = null;
            this.Pass.CancelButtonCaption = null;
            this.Pass.Caption = "Submit";
            this.Pass.ConfirmationMessage = null;
            this.Pass.Id = "Pass";
            this.Pass.ToolTip = "Pass";
            this.Pass.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Pass_CustomizePopupWindowParams);
            this.Pass.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Pass_Execute);
            // 
            // Accept
            // 
            this.Accept.AcceptButtonCaption = null;
            this.Accept.CancelButtonCaption = null;
            this.Accept.Caption = "Accept";
            this.Accept.ConfirmationMessage = null;
            this.Accept.Id = "Accept";
            this.Accept.ToolTip = "Accept";
            this.Accept.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Accept_CustomizePopupWindowParams);
            this.Accept.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Accept_Execute);
            // 
            // Closed
            // 
            this.Closed.AcceptButtonCaption = null;
            this.Closed.CancelButtonCaption = null;
            this.Closed.Caption = "Closed";
            this.Closed.ConfirmationMessage = null;
            this.Closed.Id = "Closed";
            this.Closed.ToolTip = "Closed";
            this.Closed.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Closed_CustomizePopupWindowParams);
            this.Closed.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Closed_Execute);
            // 
            // Approval
            // 
            this.Approval.AcceptButtonCaption = null;
            this.Approval.CancelButtonCaption = null;
            this.Approval.Caption = "Approval";
            this.Approval.ConfirmationMessage = null;
            this.Approval.Id = "Approval";
            this.Approval.ToolTip = null;
            this.Approval.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_CustomizePopupWindowParams);
            this.Approval.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Approval_Execute);
            // 
            // Duplicate
            // 
            this.Duplicate.Caption = "Duplicate";
            this.Duplicate.ConfirmationMessage = null;
            this.Duplicate.Id = "Duplicate";
            this.Duplicate.ToolTip = null;
            this.Duplicate.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.Duplicate_Execute);
            // 
            // PrintPR
            // 
            this.PrintPR.Caption = "Print PR";
            this.PrintPR.ConfirmationMessage = null;
            this.PrintPR.Id = "PrintPR";
            this.PrintPR.ToolTip = null;
            this.PrintPR.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintPR_Execute);
            // 
            // PrintPO
            // 
            this.PrintPO.Caption = "Print PO";
            this.PrintPO.ConfirmationMessage = null;
            this.PrintPO.Id = "PrintPO";
            this.PrintPO.ToolTip = null;
            this.PrintPO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintPO_Execute);
            // 
            // Post
            // 
            this.Post.Caption = "Post To SAP";
            this.Post.ConfirmationMessage = null;
            this.Post.Id = "Post";
            this.Post.ToolTip = null;
            this.Post.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PostSAP_Execute);
            // 
            // Cancelled
            // 
            this.Cancelled.AcceptButtonCaption = null;
            this.Cancelled.CancelButtonCaption = null;
            this.Cancelled.Caption = "Cancel";
            this.Cancelled.ConfirmationMessage = null;
            this.Cancelled.Id = "Cancelled";
            this.Cancelled.ToolTip = "Cancelled";
            this.Cancelled.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Cancelled_CustomizePopupWindowParams);
            this.Cancelled.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Cancelled_Execute);
            // 
            // PRControllers
            // 
            this.Actions.Add(this.Pass);
            this.Actions.Add(this.Accept);
            this.Actions.Add(this.Closed);
            this.Actions.Add(this.Approval);
            this.Actions.Add(this.Duplicate);
            this.Actions.Add(this.PrintPR);
            this.Actions.Add(this.PrintPO);
            this.Actions.Add(this.Post);
            this.Actions.Add(this.Cancelled);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Pass;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Accept;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Closed;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Approval;
        private DevExpress.ExpressApp.Actions.SimpleAction Duplicate;
        private DevExpress.ExpressApp.Actions.SimpleAction PrintPR;
        private DevExpress.ExpressApp.Actions.SimpleAction PrintPO;
        private DevExpress.ExpressApp.Actions.SimpleAction Post;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Cancelled;
    }
}
