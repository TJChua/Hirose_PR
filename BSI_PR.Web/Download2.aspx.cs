using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace BSI_PR.Web
{
    public partial class Download2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + Request.QueryString["filename"];
            if (System.IO.File.Exists(filename))
            {
                FileInfo fileInfo = new FileInfo(filename);
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                //To Download PDF
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);

                //To View PDF
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                Response.ContentType = "application/vnd.ms-excel";
                Response.WriteFile(fileInfo.FullName);
                
                Response.Flush();
                Response.Clear();
                Response.End();
              
            }
        }
    }
}