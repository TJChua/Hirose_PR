using BSI_PR.Module;
using BSI_PR.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#region update log
// TJC - 20220111 - Auto email ver 0.1


#endregion

namespace Integration
{
    class Code
    {
        private SortedDictionary<string, List<string>> logs = new SortedDictionary<string, List<string>>();
        private DateTime nulldate;
        // Start ver 0.1
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DataSourceConnectionString"].ToString());
        // End ver 0.1

        public Code(SecurityStrategyComplex security, IObjectSpaceProvider ObjectSpaceProvider)
        {
            logs.Clear();

            // Start ver 0.1
            string autoemail = "";
            autoemail = ConfigurationManager.AppSettings["AutoEmail"].ToString().ToUpper();
            if (autoemail == "Y" || autoemail == "YES" || autoemail == "TRUE" || autoemail == "1")
            {
                string updateflag = null;
                string command = "SELECT CONVERT(VARCHAR(5),AutoEmailTime,108), Flag FROM AutoEmailConfig";
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd4 = new SqlCommand(command, conn);
                SqlDataReader reader4 = cmd4.ExecuteReader();
                while (reader4.Read())
                {
                    if (updateflag == null)
                    {
                        updateflag = reader4.GetString(1).ToString();
                    }
                }
                conn.Close();


                if (updateflag == "N")
                {
                    WriteLog("[Log]", "--------------------------------------------------------------------------------");
                    WriteLog("[Log]", "Auto Email Begin:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                    IObjectSpace ListObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                    IObjectSpace securedObjectSpace = ObjectSpaceProvider.CreateObjectSpace();

                    IList<PurchaseOrder> POlist = ListObjectSpace.GetObjects<PurchaseOrder>
                        (CriteriaOperator.Parse("NextApprover is not null AND ApprovalStatus = ?", 1));


                    string userpos = null;
                    string users = null;

                    foreach (PurchaseOrder podtl in POlist)
                    {
                        string[] approvals = podtl.NextApprover.Split(',');

                        foreach (string dtl2 in approvals)
                        {
                            bool addusers = true;
                            //string approval = Regex.Replace(dtl2, " ", "");

                            if (dtl2 != " ")
                            {
                                if (users != null)
                                {
                                    string[] user = users.Split('/');

                                    foreach (string dtluser in user)
                                    {
                                        if (dtluser == dtl2)
                                        {
                                            addusers = false;
                                        }
                                    }

                                    if (addusers == true)
                                    {
                                        users = users + dtl2 + "/";
                                    }
                                }
                                else
                                {
                                    users = users + dtl2 + "/";
                                }

                                userpos = userpos + dtl2 + "@" + podtl.DocNum + "/";
                            }
                        }
                    }

                    if (userpos != null)
                    {
                        string[] userpo = userpos.Split('/');
                        string[] finaluser = users.Split('/');

                        foreach (string dtlfinal in finaluser)
                        {
                            if (dtlfinal != "")
                            {
                                string body = null;
                                int totalcount = 0;
                                int rowcount = 1;

                                foreach (string dtlfinalpo in userpo)
                                {
                                    string[] finalpo = dtlfinalpo.Split('@');

                                    if (finalpo[0].ToString() != "")
                                    {
                                        if (finalpo[0].ToString() == dtlfinal)
                                        {
                                            body = body + System.Environment.NewLine +
                                                rowcount + ") " + finalpo[1].ToString();
                                            totalcount += 1;
                                            rowcount += 1;
                                        }
                                    }
                                }

                                List<string> ToEmails = new List<string>();
                                string emailbody = "";
                                string emailsubject = "";
                                string emailaddress = "";
                                Guid emailuser;
                                DateTime emailtime = DateTime.Now;

                                IObjectSpace os = ObjectSpaceProvider.CreateObjectSpace();
                                SystemUsers sentuser = os.FindObject<SystemUsers>(CriteriaOperator.Parse("FName = ?", dtlfinal));

                                if (sentuser != null)
                                {
                                    emailbody = "Dear " + sentuser.FName + "," + System.Environment.NewLine + System.Environment.NewLine +
                                        "Purchase Order List" + System.Environment.NewLine + "__________________________________" + System.Environment.NewLine +
                                        body + System.Environment.NewLine + "__________________________________" + System.Environment.NewLine +
                                        "Total :" + totalcount + " Documents" + System.Environment.NewLine +
                                        System.Environment.NewLine + System.Environment.NewLine +
                                        "Thank you.";
                                    emailsubject = "[Reminder] PO Pending Approval (" + totalcount + ")";
                                    emailaddress = sentuser.UserEmail;
                                    emailuser = sentuser.Oid;

                                    ToEmails.Add(emailaddress);

                                    if (ToEmails.Count > 0)
                                    {
                                        if (SendEmail(emailsubject, emailbody, ToEmails) == 1)
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    WriteLog("[Error]", dtlfinal + " ID's email not configured.");
                                }
                            }
                        }
                    }

                    string commandend = "UPDATE AutoEmailConfig SET Flag = 'Y'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdend = new SqlCommand(commandend, conn);
                    SqlDataReader readerend = cmdend.ExecuteReader();
                    conn.Close();

                    WriteLog("[Success]", "Message: Auto Email End.");
                }
            }
            // End ver 0.1

            WriteLog("[Log]", "--------------------------------------------------------------------------------");
            WriteLog("[Log]", "Positing Begin:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

            #region Connect to SAP  
            SAPCompany sap = new SAPCompany();
            if (sap.connectSAP())
            {
                WriteLog("[Log]", "Connected to SAP:[" + sap.oCom.CompanyName + "] Time:[" + DateTime.Now.ToString("hh:mm:ss tt") + "]");
            }
            else
            {
                WriteLog("[Error]", "SAP Connection:[" + sap.oCom.CompanyDB + "] Message:[" + sap.errMsg + "] Time:[" + DateTime.Now.ToString("hh: mm:ss tt") + "]");
                sap.oCom = null;
                goto EndApplication;
            }
            #endregion

            try
            {
                string temp = "";
                IObjectSpace ListObjectSpace = ObjectSpaceProvider.CreateObjectSpace();
                IObjectSpace securedObjectSpace = ObjectSpaceProvider.CreateObjectSpace();

                temp = ConfigurationManager.AppSettings["APPost"].ToString().ToUpper();
                if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                {
                    #region AP Invoice 
                    IList<APInvoice> aplist = ListObjectSpace.GetObjects<APInvoice>
                    (CriteriaOperator.Parse("IsSubmit = ? AND IsPosted = ?", 1, 0));

                    foreach (APInvoice dtl in aplist)
                    {
                        WriteLog("[Sucess]", "Message:" + dtl.Oid);

                        try
                        {
                            IObjectSpace os = ObjectSpaceProvider.CreateObjectSpace();
                            APInvoice apobj = os.GetObjectByKey<APInvoice>(dtl.Oid);

                            #region Posting
                            if (!sap.oCom.InTransaction) sap.oCom.StartTransaction();

                            int temp1 = 0;

                            temp1 = PostAPIVtoSAP(apobj, ObjectSpaceProvider, sap);
                            if (temp1 == 1)
                            {
                                if (sap.oCom.InTransaction)
                                    sap.oCom.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                                string DocEntry = "";
                                DocEntry = GeneralSettings.oCompany.GetNewObjectKey();
                                apobj.IsPosted = true;
                                apobj.SAPINVNum = DocEntry;
                                DocEntry = null;

                                APInvoiceDocStatues ds = os.CreateObject<APInvoiceDocStatues>();
                                ds.DocStatus = "Posted";
                                ds.DocRemarks = "";
                                apobj.APInvoiceDocStatues.Add(ds);

                                os.CommitChanges();

                                GC.Collect();
                            }
                            else if (temp1 <= 0)
                            {
                                if (sap.oCom.InTransaction)
                                    sap.oCom.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                                GC.Collect();
                            }
                            #endregion

                            os.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            WriteLog("[Error]", "Message: Post AP Invoice Failed - OID : " + dtl.Oid + " (" + ex.Message + ")");
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WriteLog("[Error]", "Message:" + ex.Message);
            }

        // End Update ======================================================================================
        EndApplication:
            WriteLog("[Success]", "Message: End.");
            return;
        }

        private void WriteLog(string lvl, string str)
        {
            FileStream fileStream = null;

            string filePath = "C:\\Portal_Posting_Log\\";
            filePath = filePath + "[" + "Posting" + "] Log_" + System.DateTime.Today.ToString("yyyyMMdd") + "." + "txt";

            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
            if (!dirInfo.Exists) dirInfo.Create();

            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }

            StreamWriter log = new StreamWriter(fileStream);
            string status = lvl.ToString().Replace("[Log]", "");

            //For Portal_UpdateStatus_Log
            log.WriteLine("{0}{1}", status, str.ToString());

            log.Close();
        }

        public int PostAPIVtoSAP(APInvoice oTargetDoc, IObjectSpaceProvider ObjectSpaceProvider, SAPCompany sap)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return 1 = posting successful
            try
            {
                if (!oTargetDoc.IsPosted)
                {
                    bool found = false;
                    foreach (APInvoiceDetails dtl in oTargetDoc.APInvoiceDetails)
                    {
                        if (dtl.LineAmount > 0)
                        {
                            found = true;
                        }
                    }
                    if (!found) return 0;

                    Guid g;
                    // Create and display the value of two GUIDs.
                    g = Guid.NewGuid();

                    if (oTargetDoc.APInvoiceAttachment != null && oTargetDoc.APInvoiceAttachment.Count > 0)
                    {
                        foreach (APInvoiceAttachment obj in oTargetDoc.APInvoiceAttachment)
                        {
                            string fullpath = GeneralSettings.B1AttachmentPath + "[" + g.ToString() + "]" + obj.File.FileName;
                            //string fullpath = GeneralSettings.B1AttachmentPath + obj.File.FileName;
                            using (System.IO.FileStream fs = System.IO.File.OpenWrite(fullpath))
                            {
                                obj.File.SaveToStream(fs);
                            }
                        }
                    }

                    int sapempid = 0;
                    SAPbobsCOM.Documents oDoc = null;

                    oDoc = (SAPbobsCOM.Documents)sap.oCom.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                    oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseInvoices;
                    oDoc.DocType = BoDocumentTypes.dDocument_Items;
                    oDoc.CardCode = oTargetDoc.Vendor.BoCode;
                    oDoc.Address = oTargetDoc.Adress;
                    //oDoc.DiscountPercent = double.Parse(oTargetDoc.DocDisc.ToString());
                    oDoc.NumAtCard = oTargetDoc.DOInvoice;
                    oDoc.Comments = oTargetDoc.Remarks;
                    oDoc.DocDate = oTargetDoc.DocDate;
                    oDoc.UserFields.Fields.Item("U_Portal_No").Value = oTargetDoc.DocNum;
                    oDoc.UserFields.Fields.Item("U_IsPortal").Value = "Y";
                    oDoc.UserFields.Fields.Item("U_InvNo").Value = oTargetDoc.Department.SAPCostCenter.BoCode.ToString() + "-" + DateTime.Now.ToString("yyyyMMdd");



                    if (oTargetDoc.DeliveryNo != null)
                    {
                        oDoc.UserFields.Fields.Item("U_SupplierDO").Value = (oTargetDoc.DeliveryNo).Substring(0, oTargetDoc.DeliveryNo.Length).ToString();
                    }







                    // oDoc.UserFields.Fields.Item("U_PRNo").Value = oTargetDoc.DocNum;

                    //SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //string qry = "FT_CheckBudget '" + oTargetDoc.DocDate + "', '" + oTargetDoc.Department.SAPCostCenter.BoCode + "', '" + oDoc.Lines.AccountCode.ToString() + "', '', '', ''";
                    //rs.DoQuery(qry);
                    //oDoc.UserFields.Fields.Item("U_Budget").Value = "";
                    //oDoc.UserFields.Fields.Item("U_Remain").Value = "";


                    //SqlConnection conx = new SqlConnection(GeneralSettings.connectionString);                  
                    //SqlCommand cmd =new SqlCommand();
                    //cmd.CommandText = " SELECT FORMAT(T4.[CreateDate], 'yyyyMMdd') as [APPDate], FORMAT(T4.[CreateDate], 'hh:mmtt') as [APPTime],  T5.Fname AS [Approver],T7.FNAME AS [Creator] FROM PurchaseRequests T0 LEFT JOIN PurchaseRequestDetails T1 ON T0.Oid = T1.PurchaseRequest  INNER JOIN vw_vendors T2 ON T0.Vendor = T2.BoCode COLLATE DATABASE_DEFAULT  INNER JOIN Departments T3 ON T0.Department = T3.OID  LEFT JOIN [HRS_PR].[dbo].[PurchaseRequestAppStatuses] T4  ON T4.PurchaseRequest = T0.OID AND T4.APPSTATUS = 1  LEFT JOIN [HRS_PR].[dbo].[SystemUsers] T5 ON T5.OID = T4.CreateUser LEFT JOIN [HRS_PR].[dbo].[PermissionPolicyUser] T6 ON T6.OID = T5.OID  LEFT JOIN [HRS_PR].[dbo].[SystemUsers] T7 ON T7.OID = T0.CreateUser LEFT JOIN [HRS_PR].[dbo].[PermissionPolicyUser] T8 ON T8.OID = T7.OID  WHERE t0.DOCNUM = '" + oTargetDoc.DocNum + "'";
                    //cmd.CommandType = System.Data.CommandType.Text;
                    //cmd.Connection = conx;


                    //conx.Open();
                    //try
                    //{

                    //    cmd.ExecuteNonQuery();
                    //    SqlDataReader sqlReader = cmd.ExecuteReader();
                    //    while (sqlReader.Read())
                    //    {

                    //        DateTime APP_DATE = DateTime.ParseExact( sqlReader.GetString(0).ToString(), "yyyyMMdd", null);
                    //        oDoc.UserFields.Fields.Item("U_APPDate").Value = APP_DATE;
                    //        oDoc.UserFields.Fields.Item("U_APPTime").Value = sqlReader.GetString(1).ToString();
                    //        oDoc.UserFields.Fields.Item("U_PRAPP").Value = sqlReader.GetString(2).ToString();
                    //        oDoc.UserFields.Fields.Item("U_Creator").Value = sqlReader.GetString(3).ToString();
                    //    }
                    //    sqlReader.Close();

                    //}
                    //catch(Exception ex)
                    //{

                    //}
                    //conx.Close();


                    //Budget Part

                    if (GeneralSettings.B1APPRseries > 0)
                        oDoc.Series = GeneralSettings.B1APPRseries;

                    int cnt = 0;
                    //string acctcode = "";
                    //string formatcode = "";
                    foreach (APInvoiceDetails dtl in oTargetDoc.APInvoiceDetails)
                    {
                        if (dtl.LineAmount > 0)
                        {
                            cnt++;
                            if (cnt == 1)
                            {
                            }
                            else
                            {
                                oDoc.Lines.Add();
                                oDoc.Lines.SetCurrentLine(oDoc.Lines.Count - 1);
                            }
                            oDoc.Lines.ItemCode = dtl.ItemCode.BoCode;
                            oDoc.Lines.ItemDetails = dtl.ItemDescrip;
                            oDoc.Lines.Quantity = (double)dtl.Quantity;
                            if (dtl.UOM != "") oDoc.Lines.MeasureUnit = dtl.UOM;
                            oDoc.Lines.VatGroup = dtl.Tax.BoCode;

                            // oDoc.Lines.DiscountPercent = 0;

                            if (oTargetDoc.Vendor.BoCurrency == "MYR")
                            {
                                oDoc.Lines.LineTotal = (double)dtl.LineAmount;
                            }
                            else
                            {
                                oDoc.Lines.RowTotalFC = (double)dtl.LineAmount;
                            }

                            oDoc.Lines.UnitPrice = (double)dtl.UnitPriceAfterDisc;



                            if (oTargetDoc.Department != null)
                            {
                                if (oTargetDoc.Department.SAPCostCenter.CostLevel == "1")
                                    oDoc.Lines.CostingCode = oTargetDoc.Department.SAPCostCenter.BoCode;
                                else if (oTargetDoc.Department.SAPCostCenter.CostLevel == "2")
                                    oDoc.Lines.CostingCode2 = oTargetDoc.Department.SAPCostCenter.BoCode;
                                else if (oTargetDoc.Department.SAPCostCenter.CostLevel == "3")
                                    oDoc.Lines.CostingCode3 = oTargetDoc.Department.SAPCostCenter.BoCode;
                                else if (oTargetDoc.Department.SAPCostCenter.CostLevel == "4")
                                    oDoc.Lines.CostingCode4 = oTargetDoc.Department.SAPCostCenter.BoCode;
                                else if (oTargetDoc.Department.SAPCostCenter.CostLevel == "5")
                                    oDoc.Lines.CostingCode5 = oTargetDoc.Department.SAPCostCenter.BoCode;
                            }


                            if (dtl.Series != null)
                            {
                                if (dtl.Series.CostLevel == "2")
                                    oDoc.Lines.CostingCode2 = dtl.Series.BoCode;
                            }

                        }
                    }

                    if (oTargetDoc.Currency == "MYR")
                    {
                        oDoc.DocTotal = double.Parse(oTargetDoc.FinalAmount.ToString());

                    }
                    else
                    {
                        oDoc.DocTotalFc = double.Parse(oTargetDoc.FinalAmount.ToString());

                    }

                    if (oTargetDoc.APInvoiceAttachment != null && oTargetDoc.APInvoiceAttachment.Count > 0)
                    {
                        cnt = 0;
                        SAPbobsCOM.Attachments2 oAtt = (SAPbobsCOM.Attachments2)GeneralSettings.oCompany.GetBusinessObject(BoObjectTypes.oAttachments2);
                        foreach (APInvoiceAttachment dtl in oTargetDoc.APInvoiceAttachment)
                        {

                            cnt++;
                            if (cnt == 1)
                            {
                                if (oAtt.Lines.Count == 0)
                                    oAtt.Lines.Add();
                            }
                            else
                                oAtt.Lines.Add();

                            string attfile = "";
                            string[] fexe = dtl.File.FileName.Split('.');
                            if (fexe.Length <= 2)
                                attfile = fexe[0];
                            else
                            {
                                for (int x = 0; x < fexe.Length - 1; x++)
                                {
                                    if (attfile == "")
                                        attfile = fexe[x];
                                    else
                                        attfile += "." + fexe[x];
                                }
                            }
                            oAtt.Lines.FileName = "[" + g.ToString() + "]" + attfile;
                            //oAtt.Lines.FileName = attfile;
                            if (fexe.Length > 1)
                                oAtt.Lines.FileExtension = fexe[fexe.Length - 1];
                            string path = GeneralSettings.B1AttachmentPath;

                            path = path.Replace("\\\\", "\\");
                            path = path.Substring(0, path.Length - 1);
                            oAtt.Lines.SourcePath = path;
                            oAtt.Lines.Override = SAPbobsCOM.BoYesNoEnum.tNO;
                        }
                        int iAttEntry = -1;
                        if (oAtt.Add() == 0)
                        {
                            iAttEntry = int.Parse(sap.oCom.GetNewObjectKey());
                        }
                        else
                        {
                            string temp = sap.oCom.GetLastErrorDescription();
                            if (sap.oCom.InTransaction)
                            {
                                sap.oCom.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }

                            WriteLog("[Error]", "Message: AP Invoice Posting - " + oTargetDoc.Oid + "-" + temp);
                            return -1;
                        }
                        oDoc.AttachmentEntry = iAttEntry;
                    }


                    oDoc.HandleApprovalRequest();
                    int rc = oDoc.Add();
                    if (rc != 0)
                    {
                        string temp = sap.oCom.GetLastErrorDescription();
                        if (sap.oCom.InTransaction)
                        {
                            sap.oCom.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        WriteLog("[Error]", "Message: AP Invoice Posting - " + oTargetDoc.Oid + "-" + temp);
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                WriteLog("[Error]", "Message: AP Invoice Posting - " + oTargetDoc.Oid + "-" + ex.Message);
                return -1;
            }
        }

        // Start ver 0.1
        public int SendEmail(string MailSubject, string MailBody, List<string> ToEmails)
        {
            try
            {
                // return 0 = sent nothing
                // return -1 = sent error
                // return 1 = sent successful
                if (ConfigurationManager.AppSettings["EmailSend"].ToString() != "Y") return 0;
                if (ToEmails.Count <= 0) return 0;

                MailMessage mailMsg = new MailMessage();

                mailMsg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["EmailName"].ToString());

                foreach (string ToEmail in ToEmails)
                {
                    mailMsg.To.Add(ToEmail);
                }

                mailMsg.Subject = MailSubject;
                //mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMsg.Body = MailBody;

                SmtpClient smtpClient = new SmtpClient
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Host = ConfigurationManager.AppSettings["EmailHost"].ToString(),
                    Port = int.Parse(ConfigurationManager.AppSettings["EmailPort"].ToString()),
                };

                if (Enum.IsDefined(typeof(SmtpDeliveryMethod), ConfigurationManager.AppSettings["DeliveryMethod"].ToString()))
                    smtpClient.DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), ConfigurationManager.AppSettings["DeliveryMethod"].ToString());

                if (!smtpClient.UseDefaultCredentials)
                {
                    if (string.IsNullOrEmpty(GeneralSettings.EmailHostDomain))
                        smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["EmailPassword"].ToString());
                    else
                        smtpClient.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["EmailPassword"].ToString(), ConfigurationManager.AppSettings["EmailHostDomain"].ToString());
                }

                smtpClient.Send(mailMsg);

                mailMsg.Dispose();
                smtpClient.Dispose();

                return 1;
            }
            catch (Exception ex)
            {
                WriteLog("[Error]", "Message: Cannot send email - (" + ex.Message + ")");
                return -1;
            }
        }
        // End ver 0.1
    }
}
