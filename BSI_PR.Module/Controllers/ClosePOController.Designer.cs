
namespace BSI_PR.Module.Controllers
{
    partial class ClosePOController
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
            this.ClosePO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.ExportExcelClosePO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ClosePO
            // 
            this.ClosePO.Caption = "Close PO";
            this.ClosePO.ConfirmationMessage = null;
            this.ClosePO.Id = "ClosePO";
            this.ClosePO.ToolTip = "ClosePO";
            this.ClosePO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ClosePO_Execute);
            // 
            // ExportExcelClosePO
            // 
            this.ExportExcelClosePO.Caption = "Export Excel";
            this.ExportExcelClosePO.ConfirmationMessage = null;
            this.ExportExcelClosePO.Id = "ExportExcelClosePO";
            this.ExportExcelClosePO.ToolTip = "Export Excel";
            this.ExportExcelClosePO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExportExcelClosePO_Execute);
            // 
            // ClosePOController
            // 
            this.Actions.Add(this.ClosePO);
            this.Actions.Add(this.ExportExcelClosePO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ClosePO;
        private DevExpress.ExpressApp.Actions.SimpleAction ExportExcelClosePO;
    }
}
