namespace BSI_PR.Module.Controllers
{
    partial class VendorTransactionController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>


        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ShowTransaction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ShowTransaction
            // 
            this.ShowTransaction.Caption = "Show Transaction";
            this.ShowTransaction.ConfirmationMessage = null;
            this.ShowTransaction.Id = "ShowTransaction";
            this.ShowTransaction.ToolTip = "Show Transaction";
            this.ShowTransaction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ShowTransaction_Execute);
            // 
            // VendorTransactionController
            // 
            this.Actions.Add(this.ShowTransaction);

        }


        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ShowTransaction;
    }
}
