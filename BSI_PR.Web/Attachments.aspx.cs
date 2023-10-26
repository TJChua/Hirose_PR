using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BSI_PR.Web
{
    public partial class Attachments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportFolderPath = Server.MapPath("~/Attachments/" + Request.QueryString["DocNum"]); //change the "~/Reports" to your report folder name

                FileInfo fileInfo = new FileInfo(reportFolderPath + "\\");
                DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                if (!dirInfo.Exists) return;

                IEnumerable<string> pdfFiles = Directory.GetFiles(reportFolderPath, "*.pdf");
                int count = ConfigurationManager.AppSettings.Get("AttachmentFile").ToString().Count() + 2;

                //As a common practice server file path should not be shown to the client, use file name instead
                pdfFiles = pdfFiles.Select(o => Path.GetFileNameWithoutExtension(o));

                DropDownList1.DataSource = pdfFiles;
                DropDownList1.DataBind();

                DropDownList2.DataSource = pdfFiles;
                DropDownList2.DataBind();

                string dropdown1 = System.Web.HttpUtility.UrlEncode(DropDownList1.SelectedValue);
                string dropdown2 = System.Web.HttpUtility.UrlEncode(DropDownList2.SelectedValue);

                pdf1.Attributes.Add("Src", ("~/Attachments/" + Request.QueryString["DocNum"] + "/" + dropdown1 + ".pdf").Replace("+", "%20"));
                pdf2.Attributes.Add("Src", ("~/Attachments/" + Request.QueryString["DocNum"] + "/" + dropdown2 + ".pdf").Replace("+", "%20"));
            }
        }

        protected void DropDownList1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            string compare1 = System.Web.HttpUtility.UrlEncode(DropDownList1.SelectedItem.Text.ToString());
            pdf1.Attributes.Add("Src", ("~/Attachments/" + Request.QueryString["DocNum"] + "/" + compare1 + ".pdf").Replace("+", "%20"));
        }

        protected void DropDownList2_SelectedIndexChanged1(object sender, EventArgs e)
        {
            string compare2 = System.Web.HttpUtility.UrlEncode(DropDownList2.SelectedItem.Text.ToString());
            pdf2.Attributes.Add("Src", ("~/Attachments/" + Request.QueryString["DocNum"] + "/" + compare2 + ".pdf").Replace("+", "%20"));
        }
    }
}