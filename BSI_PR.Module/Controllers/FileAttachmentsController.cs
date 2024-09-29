using BSI_PR.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSI_PR.Module.Controllers
{
    public class FileAttachmentsController : ObjectViewController <DetailView, MultiUpload>
    {
        public FileAttachmentsController()
        {
            TargetViewId = "MultiUpload_DetailView";
        }
        protected XafCallbackManager CallbackManager
        {
            get { return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager; }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            ControlViewItem fileAttachmentsControl = (ControlViewItem)View.FindItem("FileAttachmentsControl");
            if (fileAttachmentsControl.Control != null)
            {
                ASPxUploadControl uploadControl = (ASPxUploadControl)fileAttachmentsControl.Control;
                uploadControl.UploadMode = UploadControlUploadMode.Advanced;
                uploadControl.ShowUploadButton = true;
                uploadControl.AdvancedModeSettings.EnableDragAndDrop = true;
                uploadControl.AdvancedModeSettings.EnableFileList = true;
                uploadControl.AdvancedModeSettings.EnableMultiSelect = true;
                uploadControl.FilesUploadComplete += UploadControl_FilesUploadComplete;
                uploadControl.ClientSideEvents.FilesUploadStart = "function (s, e) { e.cancel = !confirm('All changes will be saved to the database. Continue?'); }";
                string callbackScript = CallbackManager.GetScript();
                uploadControl.ClientSideEvents.FilesUploadComplete = $"function(s, e) {{ {callbackScript} }}";
            }
        }

        private void UploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {
            ASPxUploadControl uploadControl = (ASPxUploadControl)sender;
            foreach (UploadedFile uploadedFile in uploadControl.UploadedFiles)
            {
                //PortfolioFileData fileData = ObjectSpace.CreateObject<PortfolioFileData>();
                //fileData.Resume = ViewCurrentObject;
                //fileData.DocumentType = DocumentType.Unknown;
                //fileData.File = ObjectSpace.CreateObject<FileData>();
                //fileData.File.LoadFromStream(uploadedFile.FileName, uploadedFile.FileContent);
            }
            ObjectSpace.CommitChanges();
        }
    }
}
