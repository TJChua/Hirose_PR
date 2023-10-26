namespace BSI_PR.Module.Controllers
{
    partial class BudgetController
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
            this.ShowBudget = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.ExportExcel = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ShowBudget
            // 
            this.ShowBudget.Caption = "Show Budget";
            this.ShowBudget.ConfirmationMessage = null;
            this.ShowBudget.Id = "ShowBudget";
            this.ShowBudget.ToolTip = "Show Budget";
            this.ShowBudget.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ShowBudget_Execute);
            // 
            // ExportExcel
            // 
            this.ExportExcel.Caption = "Export Excel";
            this.ExportExcel.ConfirmationMessage = null;
            this.ExportExcel.Id = "ExportExcel";
            this.ExportExcel.ToolTip = "Export Excel";
            this.ExportExcel.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExportExcel_Execute);
            // 
            // BudgetController
            // 
            this.Actions.Add(this.ShowBudget);
            this.Actions.Add(this.ExportExcel);

        }


        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ShowBudget;
        private DevExpress.ExpressApp.Actions.SimpleAction ExportExcel;
    }
}
