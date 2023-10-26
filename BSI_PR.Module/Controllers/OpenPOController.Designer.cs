
namespace BSI_PR.Module.Controllers
{
    partial class OpenPOController
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
            this.OpenPO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.ExportExcelOpenPO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // OpenPO
            // 
            this.OpenPO.Caption = "Open PO";
            this.OpenPO.ConfirmationMessage = null;
            this.OpenPO.Id = "OpenPO";
            this.OpenPO.ToolTip = "OpenPO";
            this.OpenPO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.OpenPO_Execute);
            // 
            // ExportExcelOpenPO
            // 
            this.ExportExcelOpenPO.Caption = "Export Excel";
            this.ExportExcelOpenPO.ConfirmationMessage = null;
            this.ExportExcelOpenPO.Id = "ExportExcelOpenPO";
            this.ExportExcelOpenPO.ToolTip = "Export Excel";
            this.ExportExcelOpenPO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExportExcelOpenPO_Execute);
            // 
            // OpenPOController
            // 
            this.Actions.Add(this.OpenPO);
            this.Actions.Add(this.ExportExcelOpenPO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction OpenPO;
        private DevExpress.ExpressApp.Actions.SimpleAction ExportExcelOpenPO;
    }
}
