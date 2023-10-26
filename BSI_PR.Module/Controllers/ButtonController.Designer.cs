
namespace BSI_PR.Module.Controllers
{
    partial class ButtonController
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
            this.Accept_PO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Pass_GRN = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Pass_GoodsReturn = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CopyFromPR = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CopyFromPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CopyFromGRN = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Approval_PO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.INV_CopyFromPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.INV_CopyFromGRN = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Pass_Invoice = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.EscalateUser = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.EscalateUser_PO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.Closed_PO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Cancel_GRN = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Cancel_Invoice = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.BudgetInvoice = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Escalate_SystemUser = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.BudgetPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Cancel_PO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Post_Invoice = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.DepartmantFilter = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.PrintPO1 = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.Duplicate_PO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CompareAtt = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CancelLeave = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.PrintDepartmentBudget = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.LevelOfApp = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            this.Pass_JapanPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Duplicate_POJapan = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.Closed_POJapan = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.PrintPOJapan = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.EscalateUserJapan = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.Cancel_POJapan = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.BudgetPOJapan = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.Approval_POJapan = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.CompareAttJapan = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.DuplicateBudget = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.DuplicateBudgetData = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // Pass
            // 
            this.Pass.AcceptButtonCaption = null;
            this.Pass.CancelButtonCaption = null;
            this.Pass.Caption = "Submit";
            this.Pass.ConfirmationMessage = null;
            this.Pass.Id = "Pass_PO";
            this.Pass.ToolTip = null;
            this.Pass.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Pass_CustomizePopupWindowParams);
            this.Pass.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Pass_Execute);
            // 
            // Accept_PO
            // 
            this.Accept_PO.AcceptButtonCaption = null;
            this.Accept_PO.CancelButtonCaption = null;
            this.Accept_PO.Caption = "Accept";
            this.Accept_PO.ConfirmationMessage = null;
            this.Accept_PO.Id = "Accept_PO";
            this.Accept_PO.ToolTip = null;
            this.Accept_PO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Accept_PO_CustomizePopupWindowParams);
            this.Accept_PO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Accept_PO_Execute);
            // 
            // Pass_GRN
            // 
            this.Pass_GRN.AcceptButtonCaption = null;
            this.Pass_GRN.CancelButtonCaption = null;
            this.Pass_GRN.Caption = "Submit";
            this.Pass_GRN.ConfirmationMessage = null;
            this.Pass_GRN.Id = "Pass_GRN";
            this.Pass_GRN.ToolTip = null;
            this.Pass_GRN.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Pass_GRN_CustomizePopupWindowParams);
            this.Pass_GRN.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Pass_GRN_Execute);
            // 
            // Pass_GoodsReturn
            // 
            this.Pass_GoodsReturn.AcceptButtonCaption = null;
            this.Pass_GoodsReturn.CancelButtonCaption = null;
            this.Pass_GoodsReturn.Caption = "Submit";
            this.Pass_GoodsReturn.ConfirmationMessage = null;
            this.Pass_GoodsReturn.Id = "Pass_GoodsReturn";
            this.Pass_GoodsReturn.ToolTip = null;
            this.Pass_GoodsReturn.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Pass_GoodsReturn_CustomizePopupWindowParams);
            this.Pass_GoodsReturn.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Pass_GoodsReturn_Execute);
            // 
            // CopyFromPR
            // 
            this.CopyFromPR.AcceptButtonCaption = null;
            this.CopyFromPR.CancelButtonCaption = null;
            this.CopyFromPR.Caption = "Copy From PR";
            this.CopyFromPR.ConfirmationMessage = null;
            this.CopyFromPR.Id = "CopyFromPR";
            this.CopyFromPR.ToolTip = null;
            this.CopyFromPR.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromPR_CustomizePopupWindowParams);
            this.CopyFromPR.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromPR_Execute);
            // 
            // CopyFromPO
            // 
            this.CopyFromPO.AcceptButtonCaption = null;
            this.CopyFromPO.CancelButtonCaption = null;
            this.CopyFromPO.Caption = "Copy From PO";
            this.CopyFromPO.ConfirmationMessage = null;
            this.CopyFromPO.Id = "CopyFromPO";
            this.CopyFromPO.ToolTip = null;
            this.CopyFromPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromPO_CustomizePopupWindowParams);
            this.CopyFromPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromPO_Execute);
            // 
            // CopyFromGRN
            // 
            this.CopyFromGRN.AcceptButtonCaption = null;
            this.CopyFromGRN.CancelButtonCaption = null;
            this.CopyFromGRN.Caption = "Copy From GRN";
            this.CopyFromGRN.ConfirmationMessage = null;
            this.CopyFromGRN.Id = "CopyFromGRN";
            this.CopyFromGRN.ToolTip = null;
            this.CopyFromGRN.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromGRN_CustomizePopupWindowParams);
            this.CopyFromGRN.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromGRN_Execute);
            // 
            // Approval_PO
            // 
            this.Approval_PO.AcceptButtonCaption = null;
            this.Approval_PO.CancelButtonCaption = null;
            this.Approval_PO.Caption = "Approval";
            this.Approval_PO.ConfirmationMessage = null;
            this.Approval_PO.Id = "Approval_PO";
            this.Approval_PO.ToolTip = null;
            this.Approval_PO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_PO_CustomizePopupWindowParams);
            this.Approval_PO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Approval_PO_Execute);
            // 
            // INV_CopyFromPO
            // 
            this.INV_CopyFromPO.AcceptButtonCaption = null;
            this.INV_CopyFromPO.CancelButtonCaption = null;
            this.INV_CopyFromPO.Caption = "Copy From PO";
            this.INV_CopyFromPO.ConfirmationMessage = null;
            this.INV_CopyFromPO.Id = "INV_CopyFromPO";
            this.INV_CopyFromPO.ToolTip = null;
            this.INV_CopyFromPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.INV_CopyFromPO_CustomizePopupWindowParams);
            this.INV_CopyFromPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.INV_CopyFromPO_Execute);
            // 
            // INV_CopyFromGRN
            // 
            this.INV_CopyFromGRN.AcceptButtonCaption = null;
            this.INV_CopyFromGRN.CancelButtonCaption = null;
            this.INV_CopyFromGRN.Caption = "Copy From GRN";
            this.INV_CopyFromGRN.ConfirmationMessage = null;
            this.INV_CopyFromGRN.Id = "INV_CopyFromGRN";
            this.INV_CopyFromGRN.ToolTip = null;
            this.INV_CopyFromGRN.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.INV_CopyFromGRN_CustomizePopupWindowParams);
            this.INV_CopyFromGRN.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.INV_CopyFromGRN_Execute);
            // 
            // Pass_Invoice
            // 
            this.Pass_Invoice.AcceptButtonCaption = null;
            this.Pass_Invoice.CancelButtonCaption = null;
            this.Pass_Invoice.Caption = "Submit";
            this.Pass_Invoice.ConfirmationMessage = null;
            this.Pass_Invoice.Id = "Pass_Invoice";
            this.Pass_Invoice.ToolTip = null;
            this.Pass_Invoice.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Pass_Invoice_CustomizePopupWindowParams);
            this.Pass_Invoice.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Pass_Invoice_Execute);
            // 
            // EscalateUser
            // 
            this.EscalateUser.Caption = "Escalate";
            this.EscalateUser.ConfirmationMessage = null;
            this.EscalateUser.Id = "EscalateUser";
            this.EscalateUser.ToolTip = null;
            this.EscalateUser.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.EscalateUser_Execute);
            // 
            // EscalateUser_PO
            // 
            this.EscalateUser_PO.Caption = "Escalate";
            this.EscalateUser_PO.ConfirmationMessage = null;
            this.EscalateUser_PO.Id = "EscalateUser_PO";
            this.EscalateUser_PO.ToolTip = null;
            this.EscalateUser_PO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.EscalateUser_PO_Execute);
            // 
            // Closed_PO
            // 
            this.Closed_PO.AcceptButtonCaption = null;
            this.Closed_PO.CancelButtonCaption = null;
            this.Closed_PO.Caption = "Closed";
            this.Closed_PO.ConfirmationMessage = null;
            this.Closed_PO.Id = "Closed_PO";
            this.Closed_PO.ToolTip = null;
            this.Closed_PO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Closed_PO_CustomizePopupWindowParams);
            this.Closed_PO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Closed_PO_Execute);
            // 
            // Cancel_GRN
            // 
            this.Cancel_GRN.AcceptButtonCaption = null;
            this.Cancel_GRN.CancelButtonCaption = null;
            this.Cancel_GRN.Caption = "Cancel";
            this.Cancel_GRN.ConfirmationMessage = null;
            this.Cancel_GRN.Id = "Cancel_GRN";
            this.Cancel_GRN.ToolTip = null;
            this.Cancel_GRN.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Cancel_GRN_CustomizePopupWindowParams);
            this.Cancel_GRN.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Cancel_GRN_Execute);
            // 
            // Cancel_Invoice
            // 
            this.Cancel_Invoice.AcceptButtonCaption = null;
            this.Cancel_Invoice.CancelButtonCaption = null;
            this.Cancel_Invoice.Caption = "Cancel";
            this.Cancel_Invoice.ConfirmationMessage = null;
            this.Cancel_Invoice.Id = "Cancel_Invoice";
            this.Cancel_Invoice.ToolTip = null;
            this.Cancel_Invoice.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Cancel_Invoice_CustomizePopupWindowParams);
            this.Cancel_Invoice.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Cancel_Invoice_Execute);
            // 
            // BudgetInvoice
            // 
            this.BudgetInvoice.AcceptButtonCaption = null;
            this.BudgetInvoice.CancelButtonCaption = null;
            this.BudgetInvoice.Caption = "Budget";
            this.BudgetInvoice.ConfirmationMessage = null;
            this.BudgetInvoice.Id = "BudgetInvoice";
            this.BudgetInvoice.ToolTip = null;
            this.BudgetInvoice.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.BudgetInvoice_CustomizePopupWindowParams);
            this.BudgetInvoice.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.BudgetInvoice_Execute);
            // 
            // Escalate_SystemUser
            // 
            this.Escalate_SystemUser.Caption = "Escalate User";
            this.Escalate_SystemUser.ConfirmationMessage = null;
            this.Escalate_SystemUser.Id = "Escalate_SystemUser";
            this.Escalate_SystemUser.ToolTip = null;
            this.Escalate_SystemUser.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.Escalate_SystemUser_Execute);
            // 
            // BudgetPO
            // 
            this.BudgetPO.AcceptButtonCaption = null;
            this.BudgetPO.CancelButtonCaption = null;
            this.BudgetPO.Caption = "Budget";
            this.BudgetPO.ConfirmationMessage = null;
            this.BudgetPO.Id = "BudgetPO";
            this.BudgetPO.ToolTip = null;
            this.BudgetPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.BudgetPO_CustomizePopupWindowParams);
            this.BudgetPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.BudgetPO_Execute);
            // 
            // Cancel_PO
            // 
            this.Cancel_PO.AcceptButtonCaption = null;
            this.Cancel_PO.CancelButtonCaption = null;
            this.Cancel_PO.Caption = "Cancel";
            this.Cancel_PO.ConfirmationMessage = null;
            this.Cancel_PO.Id = "Cancel_PO";
            this.Cancel_PO.ToolTip = null;
            this.Cancel_PO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Cancel_PO_CustomizePopupWindowParams);
            this.Cancel_PO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Cancel_PO_Execute);
            // 
            // Post_Invoice
            // 
            this.Post_Invoice.AcceptButtonCaption = null;
            this.Post_Invoice.CancelButtonCaption = null;
            this.Post_Invoice.Caption = "Post";
            this.Post_Invoice.ConfirmationMessage = null;
            this.Post_Invoice.Id = "Post_Invoice";
            this.Post_Invoice.ToolTip = null;
            this.Post_Invoice.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Post_Invoice_CustomizePopupWindowParams);
            this.Post_Invoice.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Post_Invoice_Execute);
            // 
            // DepartmantFilter
            // 
            this.DepartmantFilter.Caption = "Departmant Filter";
            this.DepartmantFilter.ConfirmationMessage = null;
            this.DepartmantFilter.Id = "DepartmantFilter";
            this.DepartmantFilter.ToolTip = null;
            this.DepartmantFilter.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.DepartmantFilter_Execute);
            // 
            // PrintPO1
            // 
            this.PrintPO1.Caption = "Print PO";
            this.PrintPO1.ConfirmationMessage = null;
            this.PrintPO1.Id = "PrintPO1";
            this.PrintPO1.ToolTip = null;
            this.PrintPO1.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintPO1_Execute);
            // 
            // Duplicate_PO
            // 
            this.Duplicate_PO.Caption = "Duplicate";
            this.Duplicate_PO.ConfirmationMessage = null;
            this.Duplicate_PO.Id = "Duplicate_PO";
            this.Duplicate_PO.ToolTip = null;
            this.Duplicate_PO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.Duplicate_PO_Execute);
            // 
            // CompareAtt
            // 
            this.CompareAtt.Caption = "Attachment Compare";
            this.CompareAtt.ConfirmationMessage = null;
            this.CompareAtt.Id = "CompareAtt";
            this.CompareAtt.ToolTip = null;
            this.CompareAtt.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CompareAtt_Execute);
            // 
            // CancelLeave
            // 
            this.CancelLeave.AcceptButtonCaption = null;
            this.CancelLeave.CancelButtonCaption = null;
            this.CancelLeave.Caption = "Cancel";
            this.CancelLeave.Category = "ObjectsCreation";
            this.CancelLeave.ConfirmationMessage = null;
            this.CancelLeave.Id = "CancelLeave";
            this.CancelLeave.ToolTip = null;
            this.CancelLeave.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.LACancel_CustomizePopupWindowParams);
            this.CancelLeave.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.LACancel_Execute);
            // 
            // PrintDepartmentBudget
            // 
            this.PrintDepartmentBudget.Caption = "Print";
            this.PrintDepartmentBudget.ConfirmationMessage = null;
            this.PrintDepartmentBudget.Id = "PrintDepartmentBudget";
            this.PrintDepartmentBudget.ToolTip = null;
            this.PrintDepartmentBudget.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintDepartmentBudget_Execute);
            // 
            // LevelOfApp
            // 
            this.LevelOfApp.Caption = "Level Of Approval";
            this.LevelOfApp.ConfirmationMessage = null;
            this.LevelOfApp.Id = "LevelOfApp";
            this.LevelOfApp.ToolTip = null;
            this.LevelOfApp.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.LevelOfApp_Execute);
            // 
            // Pass_JapanPO
            // 
            this.Pass_JapanPO.AcceptButtonCaption = null;
            this.Pass_JapanPO.CancelButtonCaption = null;
            this.Pass_JapanPO.Caption = "Submit";
            this.Pass_JapanPO.ConfirmationMessage = null;
            this.Pass_JapanPO.Id = "Pass_JapanPO";
            this.Pass_JapanPO.ToolTip = null;
            this.Pass_JapanPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Pass_JapanPO_CustomizePopupWindowParams);
            this.Pass_JapanPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Pass_JapanPO_Execute);
            // 
            // Duplicate_POJapan
            // 
            this.Duplicate_POJapan.Caption = "Duplicate";
            this.Duplicate_POJapan.ConfirmationMessage = null;
            this.Duplicate_POJapan.Id = "Duplicate_POJapan";
            this.Duplicate_POJapan.ToolTip = null;
            this.Duplicate_POJapan.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.Duplicate_POJapan_Execute);
            // 
            // Closed_POJapan
            // 
            this.Closed_POJapan.AcceptButtonCaption = null;
            this.Closed_POJapan.CancelButtonCaption = null;
            this.Closed_POJapan.Caption = "Closed";
            this.Closed_POJapan.ConfirmationMessage = null;
            this.Closed_POJapan.Id = "Closed_POJapan";
            this.Closed_POJapan.ToolTip = null;
            this.Closed_POJapan.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Closed_POJapan_CustomizePopupWindowParams);
            this.Closed_POJapan.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Closed_POJapan_Execute);
            // 
            // PrintPOJapan
            // 
            this.PrintPOJapan.Caption = "Print";
            this.PrintPOJapan.ConfirmationMessage = null;
            this.PrintPOJapan.Id = "PrintPOJapan";
            this.PrintPOJapan.ToolTip = null;
            this.PrintPOJapan.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PrintPOJapan_Execute);
            // 
            // EscalateUserJapan
            // 
            this.EscalateUserJapan.Caption = "Escalate";
            this.EscalateUserJapan.ConfirmationMessage = null;
            this.EscalateUserJapan.Id = "EscalateUserJapan";
            this.EscalateUserJapan.ToolTip = null;
            this.EscalateUserJapan.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.EscalateUserJapan_Execute);
            // 
            // Cancel_POJapan
            // 
            this.Cancel_POJapan.AcceptButtonCaption = null;
            this.Cancel_POJapan.CancelButtonCaption = null;
            this.Cancel_POJapan.Caption = "Cancel";
            this.Cancel_POJapan.ConfirmationMessage = null;
            this.Cancel_POJapan.Id = "Cancel_POJapan";
            this.Cancel_POJapan.ToolTip = null;
            this.Cancel_POJapan.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Cancel_POJapan_CustomizePopupWindowParams);
            this.Cancel_POJapan.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Cancel_POJapan_Execute);
            // 
            // BudgetPOJapan
            // 
            this.BudgetPOJapan.AcceptButtonCaption = null;
            this.BudgetPOJapan.CancelButtonCaption = null;
            this.BudgetPOJapan.Caption = "Budget";
            this.BudgetPOJapan.ConfirmationMessage = null;
            this.BudgetPOJapan.Id = "BudgetPOJapan";
            this.BudgetPOJapan.ToolTip = null;
            this.BudgetPOJapan.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.BudgetPOJapan_CustomizePopupWindowParams);
            this.BudgetPOJapan.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.BudgetPOJapan_Execute);
            // 
            // Approval_POJapan
            // 
            this.Approval_POJapan.AcceptButtonCaption = null;
            this.Approval_POJapan.CancelButtonCaption = null;
            this.Approval_POJapan.Caption = "Approval";
            this.Approval_POJapan.ConfirmationMessage = null;
            this.Approval_POJapan.Id = "Approval_POJapan";
            this.Approval_POJapan.ToolTip = null;
            this.Approval_POJapan.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.Approval_POJapan_CustomizePopupWindowParams);
            this.Approval_POJapan.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.Approval_POJapan_Execute);
            // 
            // CompareAttJapan
            // 
            this.CompareAttJapan.Caption = "Attachment Compare";
            this.CompareAttJapan.ConfirmationMessage = null;
            this.CompareAttJapan.Id = "CompareAttJapan";
            this.CompareAttJapan.ToolTip = null;
            this.CompareAttJapan.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CompareAttJapan_Execute);
            // 
            // DuplicateBudget
            // 
            this.DuplicateBudget.Caption = "Duplicate";
            this.DuplicateBudget.Category = "ObjectsCreation";
            this.DuplicateBudget.ConfirmationMessage = null;
            this.DuplicateBudget.Id = "DuplicateBudget";
            this.DuplicateBudget.ToolTip = null;
            this.DuplicateBudget.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DuplicateBudget_Execute);
            // 
            // DuplicateBudgetData
            // 
            this.DuplicateBudgetData.Caption = "Duplicate";
            this.DuplicateBudgetData.Category = "ObjectsCreation";
            this.DuplicateBudgetData.ConfirmationMessage = null;
            this.DuplicateBudgetData.Id = "DuplicateBudgetData";
            this.DuplicateBudgetData.ToolTip = null;
            this.DuplicateBudgetData.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DuplicateBudgetData_Execute);
            // 
            // ButtonController
            // 
            this.Actions.Add(this.Pass);
            this.Actions.Add(this.Accept_PO);
            this.Actions.Add(this.Pass_GRN);
            this.Actions.Add(this.Pass_GoodsReturn);
            this.Actions.Add(this.CopyFromPR);
            this.Actions.Add(this.CopyFromPO);
            this.Actions.Add(this.CopyFromGRN);
            this.Actions.Add(this.Approval_PO);
            this.Actions.Add(this.INV_CopyFromPO);
            this.Actions.Add(this.INV_CopyFromGRN);
            this.Actions.Add(this.Pass_Invoice);
            this.Actions.Add(this.EscalateUser);
            this.Actions.Add(this.EscalateUser_PO);
            this.Actions.Add(this.Closed_PO);
            this.Actions.Add(this.Cancel_GRN);
            this.Actions.Add(this.Cancel_Invoice);
            this.Actions.Add(this.BudgetInvoice);
            this.Actions.Add(this.Escalate_SystemUser);
            this.Actions.Add(this.BudgetPO);
            this.Actions.Add(this.Cancel_PO);
            this.Actions.Add(this.Post_Invoice);
            this.Actions.Add(this.DepartmantFilter);
            this.Actions.Add(this.PrintPO1);
            this.Actions.Add(this.Duplicate_PO);
            this.Actions.Add(this.CompareAtt);
            this.Actions.Add(this.CancelLeave);
            this.Actions.Add(this.PrintDepartmentBudget);
            this.Actions.Add(this.LevelOfApp);
            this.Actions.Add(this.Pass_JapanPO);
            this.Actions.Add(this.Duplicate_POJapan);
            this.Actions.Add(this.Closed_POJapan);
            this.Actions.Add(this.PrintPOJapan);
            this.Actions.Add(this.EscalateUserJapan);
            this.Actions.Add(this.Cancel_POJapan);
            this.Actions.Add(this.BudgetPOJapan);
            this.Actions.Add(this.Approval_POJapan);
            this.Actions.Add(this.CompareAttJapan);
            this.Actions.Add(this.DuplicateBudget);
            this.Actions.Add(this.DuplicateBudgetData);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Pass;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Accept_PO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Pass_GRN;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Pass_GoodsReturn;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromPR;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromPO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromGRN;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Approval_PO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction INV_CopyFromPO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction INV_CopyFromGRN;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Pass_Invoice;
        private DevExpress.ExpressApp.Actions.SimpleAction EscalateUser;
        private DevExpress.ExpressApp.Actions.SimpleAction EscalateUser_PO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Closed_PO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Cancel_GRN;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Cancel_Invoice;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction BudgetInvoice;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction Escalate_SystemUser;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction BudgetPO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Cancel_PO;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Post_Invoice;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction DepartmantFilter;
        private DevExpress.ExpressApp.Actions.SimpleAction PrintPO1;
        private DevExpress.ExpressApp.Actions.SimpleAction Duplicate_PO;
        private DevExpress.ExpressApp.Actions.SimpleAction CompareAtt;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelLeave;
        private DevExpress.ExpressApp.Actions.SimpleAction PrintDepartmentBudget;
        private DevExpress.ExpressApp.Actions.SingleChoiceAction LevelOfApp;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Pass_JapanPO;
        private DevExpress.ExpressApp.Actions.SimpleAction Duplicate_POJapan;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Closed_POJapan;
        private DevExpress.ExpressApp.Actions.SimpleAction PrintPOJapan;
        private DevExpress.ExpressApp.Actions.SimpleAction EscalateUserJapan;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Cancel_POJapan;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction BudgetPOJapan;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction Approval_POJapan;
        private DevExpress.ExpressApp.Actions.SimpleAction CompareAttJapan;
        private DevExpress.ExpressApp.Actions.SimpleAction DuplicateBudget;
        private DevExpress.ExpressApp.Actions.SimpleAction DuplicateBudgetData;
    }
}
