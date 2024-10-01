using BSI_PR.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.BaseImpl;
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
        GenControllers genCon;

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
            genCon = Frame.GetController<GenControllers>();

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
                if (uploadedFile.FileName == "" || uploadedFile.FileName.Contains('!') || uploadedFile.FileName.Contains('*') ||
                    uploadedFile.FileName.Contains('(') || uploadedFile.FileName.Contains(')') || uploadedFile.FileName.Contains(';') ||
                    uploadedFile.FileName.Contains(':') || uploadedFile.FileName.Contains('@') || uploadedFile.FileName.Contains('&') ||
                    uploadedFile.FileName.Contains('=') || uploadedFile.FileName.Contains('+') || uploadedFile.FileName.Contains('$') ||
                    uploadedFile.FileName.Contains(',') || uploadedFile.FileName.Contains('/') || uploadedFile.FileName.Contains('?') ||
                    uploadedFile.FileName.Contains('#') || uploadedFile.FileName.Contains('[') || uploadedFile.FileName.Contains(']'))
                {
                    genCon.showMsg("Fail", "Invalid file name. File Name should not include symbol as !*'();:@&=+$,/?#[], please reupload again.", InformationType.Error);
                    return;
                }
            }

            try
            {
                IObjectSpace os = Application.CreateObjectSpace();

                foreach (UploadedFile uploadedFile in uploadControl.UploadedFiles)
                {
                    AttachmentStaging fileData = os.CreateObject<AttachmentStaging>();
                    fileData.Remarks = ViewCurrentObject.Remarks;
                    fileData.File = os.CreateObject<FileData>();
                    fileData.File.LoadFromStream(uploadedFile.FileName, uploadedFile.FileContent);
                    fileData.LinkOid = ViewCurrentObject.LinkOid;
                    fileData.FileOid = fileData.File.Oid.ToString();
                }
                os.CommitChanges();

                genCon.showMsg("Success", "Upload Success.", InformationType.Success);
            }
            catch(Exception ex)
            {
                genCon.showMsg("Fail", ex.Message, InformationType.Error);
            }
        }
    }
}
