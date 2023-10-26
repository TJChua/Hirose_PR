using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using SAPbobsCOM;
using System.Net.Mail;
using System.Net;
using BSI_PR.Module.BusinessObjects;
using DevExpress.Xpo.DB;
using System.Data;
using System.Data.SqlClient;


namespace BSI_PR.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GenControllers : ViewController
    {
        public GenControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        public void openNewView(IObjectSpace os, object target, ViewEditMode viewmode)
        {
            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(os, target);
            dv.ViewEditMode = viewmode;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

        }
        public void showMsg(string caption, string msg, InformationType msgtype)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            //options.Message = string.Format("{0} task(s) have been successfully updated!", e.SelectedObjects.Count);
            options.Message = string.Format("{0}", msg);
            options.Type = msgtype;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            Application.ShowViewStrategy.ShowMessage(options);
        }

        public bool ConnectSAP(string Department)
        {
            if (GeneralSettings.B1Post)
            {
                if (GeneralSettings.oCompany == null)
                {
                    GeneralSettings.oCompany = new SAPbobsCOM.Company();
                }

                if (GeneralSettings.oCompany != null && !GeneralSettings.oCompany.Connected)
                {
                    bool exist = Enum.IsDefined(typeof(SAPbobsCOM.BoDataServerTypes), GeneralSettings.B1DbServerType);
                    if (exist)
                        GeneralSettings.oCompany.DbServerType = (SAPbobsCOM.BoDataServerTypes)Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), GeneralSettings.B1DbServerType);

                    exist = Enum.IsDefined(typeof(SAPbobsCOM.BoSuppLangs), GeneralSettings.B1Language);
                    if (exist)
                        GeneralSettings.oCompany.language = (SAPbobsCOM.BoSuppLangs)Enum.Parse(typeof(SAPbobsCOM.BoSuppLangs), GeneralSettings.B1Language);

                    GeneralSettings.oCompany.Server = GeneralSettings.B1Server;
                    GeneralSettings.oCompany.CompanyDB = GeneralSettings.B1CompanyDB;
                    GeneralSettings.oCompany.LicenseServer = GeneralSettings.B1License;
                    GeneralSettings.oCompany.DbUserName = GeneralSettings.B1DbUserName;
                    GeneralSettings.oCompany.DbPassword = GeneralSettings.B1DbPassword;
                    //GeneralSettings.oCompany.UserName = GeneralSettings.B1UserName;
                    //GeneralSettings.oCompany.Password = GeneralSettings.B1Password;

                    SqlConnection connection = new SqlConnection(GeneralSettings.connectionString);
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT U_User,U_Password FROM [HRS_LIVE].[dbo].[@Posting] where code='"+Department+"'";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = connection;


                    connection.Open();
                    try
                    {

                        cmd.ExecuteNonQuery();
                        SqlDataReader sqlReader = cmd.ExecuteReader();
                        while (sqlReader.Read())
                        {
                            GeneralSettings.oCompany.UserName = sqlReader.GetString(0).ToString();
                            GeneralSettings.oCompany.Password = sqlReader.GetString(1).ToString();
                        }
                        sqlReader.Close();

                    }
                    catch (Exception ex)
                    {
                       if(ex.HResult.ToString()== "-2146232015")
                        {
                            showMsg("Failed","Missing Indirect User In SAP for Department - "+Department, InformationType.Error);
                        }
                        else
                        {
                            showMsg("Failed", ex.Message, InformationType.Error);
                        }

                        
                    }
                    connection.Close();


                    if (GeneralSettings.oCompany.Connect() != 0)
                    {
                        showMsg("Failed", GeneralSettings.oCompany.GetLastErrorDescription(), InformationType.Error);
                    }

                }

                return GeneralSettings.oCompany.Connected;
            
            }
            else
            {
                return false;
            }
        }
      
    
        public int PostAPIVtoSAP(PurchaseRequests oTargetDoc)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return 1 = posting successful
            try
            {
                if (!GeneralSettings.B1Post)
                    return 0;

                //if (oTargetDoc.IsClosed && !oTargetDoc.IsPosted)
                if (!oTargetDoc.IsPosted)
                //  if (!oTargetDoc.IsClosed)
                {
                    bool found = false;
                    foreach (PurchaseRequestDetails dtl in oTargetDoc.PurchaseRequestDetail)
                    {
                        if (dtl.Amount > 0)
                        {
                            found = true;
                        }
                    }
                    if (!found) return 0;

                    Guid g;
                    // Create and display the value of two GUIDs.
                    g = Guid.NewGuid();

                    if (oTargetDoc.PurchaseRequestAttachment != null && oTargetDoc.PurchaseRequestAttachment.Count > 0)
                    {
                        foreach (PurchaseRequestAttachments obj in oTargetDoc.PurchaseRequestAttachment)
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
                    //DateTime reqdate = DateTime.Today;
                    //foreach (PMSClaimEF.Module.BusinessObjects.ClaimTrxDocStatuses dtl in oTargetDoc.ClaimTrxDocStatus)
                    //{
                    //    if (dtl.DocStatus == BusinessObjects.DocumentStatus.DocPassed)
                    //    {
                    //        reqdate = (DateTime)dtl.CreateDate;
                    //    }
                    //}


                    SAPbobsCOM.Documents oDoc = null;
                    if (oTargetDoc.Company.ESAPDoc == ESAPDocs.DraftVendorPO)
                    {
                        //oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                        //oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseOrders;

                        oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
                       // oDoc.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseOrders;
                    }
                    else if (oTargetDoc.Company.ESAPDoc == ESAPDocs.VendorPO)
                    {
                        oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
                    }
                    oDoc.DocType = BoDocumentTypes.dDocument_Items;
                    oDoc.CardCode = oTargetDoc.Vendor.BoCode;
                    oDoc.Address = oTargetDoc.Adress;
                    oDoc.DiscountPercent = double.Parse(oTargetDoc.DocDisc.ToString());
                    oDoc.NumAtCard = oTargetDoc.RefNo;
                    oDoc.Comments = oTargetDoc.Remarks;
                    if (sapempid > 0)
                        oDoc.DocumentsOwner = sapempid;
                    oDoc.DocDate = oTargetDoc.DocDate;
                    oDoc.UserFields.Fields.Item("U_PRNo").Value = oTargetDoc.DocNum;

                    //SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //string qry = "FT_CheckBudget '" + oTargetDoc.DocDate + "', '" + oTargetDoc.Department.SAPCostCenter.BoCode + "', '" + oDoc.Lines.AccountCode.ToString() + "', '', '', ''";
                    //rs.DoQuery(qry);
                    //oDoc.UserFields.Fields.Item("U_Budget").Value = "";
                    //oDoc.UserFields.Fields.Item("U_Remain").Value = "";


                    SqlConnection conx = new SqlConnection(GeneralSettings.connectionString);                  
                    SqlCommand cmd =new SqlCommand();
                    cmd.CommandText = " SELECT FORMAT(T4.[CreateDate], 'yyyyMMdd') as [APPDate], FORMAT(T4.[CreateDate], 'hh:mmtt') as [APPTime],  T5.Fname AS [Approver],T7.FNAME AS [Creator] FROM PurchaseRequests T0 LEFT JOIN PurchaseRequestDetails T1 ON T0.Oid = T1.PurchaseRequest  INNER JOIN vw_vendors T2 ON T0.Vendor = T2.BoCode COLLATE DATABASE_DEFAULT  INNER JOIN Departments T3 ON T0.Department = T3.OID  LEFT JOIN [HRS_PR].[dbo].[PurchaseRequestAppStatuses] T4  ON T4.PurchaseRequest = T0.OID AND T4.APPSTATUS = 1  LEFT JOIN [HRS_PR].[dbo].[SystemUsers] T5 ON T5.OID = T4.CreateUser LEFT JOIN [HRS_PR].[dbo].[PermissionPolicyUser] T6 ON T6.OID = T5.OID  LEFT JOIN [HRS_PR].[dbo].[SystemUsers] T7 ON T7.OID = T0.CreateUser LEFT JOIN [HRS_PR].[dbo].[PermissionPolicyUser] T8 ON T8.OID = T7.OID  WHERE t0.DOCNUM = '" + oTargetDoc.DocNum + "'";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = conx;


                    conx.Open();
                    try
                    {
                      
                        cmd.ExecuteNonQuery();
                        SqlDataReader sqlReader = cmd.ExecuteReader();
                        while (sqlReader.Read())
                        {

                            DateTime APP_DATE = DateTime.ParseExact( sqlReader.GetString(0).ToString(), "yyyyMMdd", null);
                            oDoc.UserFields.Fields.Item("U_APPDate").Value = APP_DATE;
                            oDoc.UserFields.Fields.Item("U_APPTime").Value = sqlReader.GetString(1).ToString();
                            oDoc.UserFields.Fields.Item("U_PRAPP").Value = sqlReader.GetString(2).ToString();
                            oDoc.UserFields.Fields.Item("U_Creator").Value = sqlReader.GetString(3).ToString();
                        }
                        sqlReader.Close();
                   
                    }
                    catch(Exception ex)
                    {
    
                    }
                    conx.Close();


                    //Budget Part
                    
                    if (GeneralSettings.B1APPRseries > 0)
                        oDoc.Series = GeneralSettings.B1APPRseries;

                    int cnt = 0;
                    //string acctcode = "";
                    //string formatcode = "";
                    foreach (PurchaseRequestDetails dtl in oTargetDoc.PurchaseRequestDetail)
                    {
                        if (dtl.Amount > 0)
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
                            oDoc.Lines.ItemCode = dtl.Item.BoCode;
                            oDoc.Lines.ItemDetails = dtl.ItemDesc;
                            oDoc.Lines.Quantity = (double)dtl.Quantity;
                            if (dtl.UOM != "")oDoc.Lines.MeasureUnit = dtl.UOM;
                            oDoc.Lines.VatGroup = dtl.Tax.BoCode;
                            oDoc.Lines.UnitPrice = (double)dtl.Amount;
                      
                            
                            
                            if (oTargetDoc.Vendor.BoCurrency == "MYR")
                            {
                                oDoc.Lines.LineTotal = (double)dtl.LineTotal;
                            }
                            else
                            {
                                oDoc.Lines.RowTotalFC = (double)dtl.LineTotal;
                            }

                          
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
                               if( dtl.Series.CostLevel=="2")
                                    oDoc.Lines.CostingCode2 = dtl.Series.BoCode;
                            }

                        }
                    }
                  

                    if (oTargetDoc.PurchaseRequestAttachment != null && oTargetDoc.PurchaseRequestAttachment.Count > 0)
                    {
                        cnt = 0;
                        SAPbobsCOM.Attachments2 oAtt = (SAPbobsCOM.Attachments2)GeneralSettings.oCompany.GetBusinessObject(BoObjectTypes.oAttachments2);
                        foreach (PurchaseRequestAttachments dtl in oTargetDoc.PurchaseRequestAttachment)
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
                            oAtt.Lines.FileName = "["+g.ToString()+"]" + attfile;
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
                            iAttEntry = int.Parse(GeneralSettings.oCompany.GetNewObjectKey());
                        }
                        else
                        {
                            string temp = GeneralSettings.oCompany.GetLastErrorDescription();
                            if (GeneralSettings.oCompany.InTransaction)
                            {
                                GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            showMsg("Failed", temp, InformationType.Error);
                            return -1;
                        }
                        oDoc.AttachmentEntry = iAttEntry;
                    }


                    oDoc.HandleApprovalRequest();
                    int rc = oDoc.Add();
                    if (rc != 0)
                    {
                        string temp = GeneralSettings.oCompany.GetLastErrorDescription();
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", temp, InformationType.Error);
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
                showMsg("Error", ex.Message, InformationType.Error);
                return -1;
            }
        }



        public int PostAPIVtoSAP2(APInvoice oTargetDoc)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return 1 = posting successful
            try
            {
                if (!GeneralSettings.B1Post)
                    return 0;

                //if (oTargetDoc.IsClosed && !oTargetDoc.IsPosted)
                if (!oTargetDoc.IsPosted)
                //  if (!oTargetDoc.IsClosed)
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
                            string fullpath = GeneralSettings.B1AttachmentPath + "[" + g.ToString() + "]" + obj.File.FileName ;
                            //string fullpath = GeneralSettings.B1AttachmentPath + obj.File.FileName;
                            using (System.IO.FileStream fs = System.IO.File.OpenWrite(fullpath))
                            {
                                obj.File.SaveToStream(fs);
                            }
                        }
                    }

                    int sapempid = 0;
                    SAPbobsCOM.Documents oDoc = null;

                    oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
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
                    oDoc.UserFields.Fields.Item("U_InvNo").Value = oTargetDoc.Department.SAPCostCenter.BoCode.ToString()+"-"+DateTime.Now.ToString("yyyyMMdd");



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
                            iAttEntry = int.Parse(GeneralSettings.oCompany.GetNewObjectKey());
                        }
                        else
                        {
                            string temp = GeneralSettings.oCompany.GetLastErrorDescription();
                            if (GeneralSettings.oCompany.InTransaction)
                            {
                                GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            showMsg("Failed", temp, InformationType.Error);
                            return -1;
                        }
                        oDoc.AttachmentEntry = iAttEntry;
                    }


                    oDoc.HandleApprovalRequest();
                    int rc = oDoc.Add();
                    if (rc != 0)
                    {
                        string temp = GeneralSettings.oCompany.GetLastErrorDescription();
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", temp, InformationType.Error);
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
                showMsg("Error", ex.Message, InformationType.Error);
                return -1;
            }
        }




        public int SendEmail(string MailSubject, string MailBody, List<string> ToEmails)
        {
            try
            {
                // return 0 = sent nothing
                // return -1 = sent error
                // return 1 = sent successful
                if (!GeneralSettings.EmailSend) return 0;
                if (ToEmails.Count <= 0) return 0;

                MailMessage mailMsg = new MailMessage();

                mailMsg.From = new MailAddress(GeneralSettings.Email, GeneralSettings.EmailName);

                foreach (string ToEmail in ToEmails)
                {
                    mailMsg.To.Add(ToEmail); 
                }

                mailMsg.Subject = MailSubject;
                //mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMsg.Body = MailBody;

                SmtpClient smtpClient = new SmtpClient
                {
                    EnableSsl = GeneralSettings.EmailSSL,
                    UseDefaultCredentials = GeneralSettings.EmailUseDefaultCredential,
                    Host = GeneralSettings.EmailHost,
                    Port = int.Parse(GeneralSettings.EmailPort),
                };

                if (Enum.IsDefined(typeof(SmtpDeliveryMethod), GeneralSettings.DeliveryMethod))
                    smtpClient.DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), GeneralSettings.DeliveryMethod);

                if (!smtpClient.UseDefaultCredentials)
                {
                    if (string.IsNullOrEmpty(GeneralSettings.EmailHostDomain))
                        smtpClient.Credentials = new NetworkCredential(GeneralSettings.Email, GeneralSettings.EmailPassword);
                    else
                        smtpClient.Credentials = new NetworkCredential(GeneralSettings.Email, GeneralSettings.EmailPassword, GeneralSettings.EmailHostDomain);
                }

                smtpClient.Send(mailMsg);

                mailMsg.Dispose();
                smtpClient.Dispose();

                return 1;
            }
            catch (Exception ex)
            {
                showMsg("Cannot send email", ex.Message, InformationType.Error);
                return -1;
            }
        }

        public int PostInvoicetoSAP(APInvoice oTargetDoc)
        {
            // return 0 = post nothing
            // return -1 = posting error
            // return 1 = posting successful
            try
            {
                if (!GeneralSettings.B1Post)
                    return 0;

                if (!oTargetDoc.IsSubmit)
                {
                    bool found = false;
                    DateTime postdate = DateTime.Now;

                    //foreach (PurchaseOrderDetails dtl in oTargetDoc.PurchaseOrderDetails)
                    //{
                    //    if (dtl.LineAmount > 0)
                    //    {
                    //        found = true;
                    //    }
                    //}
                    //if (!found) return 0;

                    Guid g;
                    // Create and display the value of two GUIDs.
                    g = Guid.NewGuid();

                    if (oTargetDoc.APInvoiceAttachment != null && oTargetDoc.APInvoiceAttachment.Count > 0)
                    {
                        foreach (APInvoiceAttachment obj in oTargetDoc.APInvoiceAttachment)
                        {
                            string fullpath = GeneralSettings.B1AttachmentPath + g.ToString() + obj.File.FileName;
                            using (System.IO.FileStream fs = System.IO.File.OpenWrite(fullpath))
                            {
                                obj.File.SaveToStream(fs);
                            }
                        }
                    }

                    int sapempid = 0;
                    SAPbobsCOM.Documents oDoc = null;

                    oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                    oDoc.DocObjectCode = BoObjectTypes.oPurchaseInvoices;
                    // oDoc = (SAPbobsCOM.Documents)GeneralSettings.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);

                    oDoc.DocType = BoDocumentTypes.dDocument_Items;
                    oDoc.CardCode = oTargetDoc.Vendor.BoCode;
                    oDoc.CardName = oTargetDoc.Vendor.BoName;
                    oDoc.Address = oTargetDoc.Adress;
                    oDoc.DiscountPercent = (double)oTargetDoc.DocDisc;
                    oDoc.DocDate = oTargetDoc.DocDate;
                    oDoc.TaxDate = oTargetDoc.DocDate;
                    oDoc.NumAtCard = oTargetDoc.DOInvoice;
                    oDoc.Comments = oTargetDoc.Remarks;
                    oDoc.UserFields.Fields.Item("U_Portal_No").Value = oTargetDoc.DocNum;
                    oDoc.UserFields.Fields.Item("U_IsPortal").Value = "Y";

                    if (sapempid > 0)
                        oDoc.DocumentsOwner = sapempid;
                    oDoc.DocDate = postdate;

                    if (GeneralSettings.B1APPRseries > 0)
                        oDoc.Series = GeneralSettings.B1APPRseries;

                    int cnt = 0;
                    foreach (APInvoiceDetails dtl in oTargetDoc.APInvoiceDetails)
                    {
                        //if (dtl.LineAmount > 0)
                        //{
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
                        oDoc.Lines.ItemDetails = dtl.ItemCode.BoName;
                        oDoc.Lines.Quantity = (double)dtl.Quantity;
                        oDoc.Lines.UnitPrice = (double)dtl.UnitPrice;
                        if (dtl.Tax != null)
                        {
                            oDoc.Lines.VatGroup = dtl.Tax.BoCode;
                        }
                        oDoc.Lines.TaxTotal = (double)dtl.TaxAmount;
                        if (dtl.UOM != "")
                        {
                            oDoc.Lines.MeasureUnit = dtl.UOM;
                        }

                        if (oTargetDoc.Vendor.BoCurrency == "MYR")
                        {
                            oDoc.Lines.LineTotal = (double)dtl.LineAmount;
                        }
                        else
                        {
                            oDoc.Lines.RowTotalFC = (double)dtl.LineAmount;
                        }

                        if (oTargetDoc.Department != null)
                        {
                            oDoc.Lines.CostingCode = oTargetDoc.Department.SAPCostCenter.BoCode;
                        }

                        if (dtl.Series != null)
                        {
                            oDoc.Lines.CostingCode2 = dtl.Series.BoCode;
                        }

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
                            oAtt.Lines.FileName = g.ToString() + attfile;
                            if (fexe.Length > 1)
                                oAtt.Lines.FileExtension = fexe[fexe.Length - 1];
                            string path = GeneralSettings.B1AttachmentPath;
                            path = path.Replace("\\\\", "\\");
                            path = path.Substring(0, path.Length - 1);
                            oAtt.Lines.SourcePath = path;
                            oAtt.Lines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                        }
                        int iAttEntry = -1;
                        if (oAtt.Add() == 0)
                        {
                            iAttEntry = int.Parse(GeneralSettings.oCompany.GetNewObjectKey());
                        }
                        else
                        {
                            string temp = GeneralSettings.oCompany.GetLastErrorDescription();
                            if (GeneralSettings.oCompany.InTransaction)
                            {
                                GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            showMsg("Failed", temp, InformationType.Error);
                            return -1;
                        }
                        oDoc.AttachmentEntry = iAttEntry;
                    }

                    int rc = oDoc.Add();
                    if (rc != 0)
                    {
                        string temp = GeneralSettings.oCompany.GetLastErrorDescription();
                        if (GeneralSettings.oCompany.InTransaction)
                        {
                            GeneralSettings.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                        }
                        showMsg("Failed", temp, InformationType.Error);
                        return -1;
                    }
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                showMsg("Error", ex.Message, InformationType.Error);
                return -1;
            }
        }

        public int UpdateBudget(int department, string budgetcategoryname, int month, string year, decimal total, IObjectSpace os, string status)
        {
            try
            {
                BudgetCategory budget = os.FindObject<BudgetCategory>(CriteriaOperator.Parse("Department.Oid = ? and IsActive = ?",
                  department, "True"));

                if (budget != null)
                {
                    foreach (BudgetCategoryDetails dtl in budget.BudgetCategoryDetails)
                    {
                        if (dtl.BudgetCategory.BudgetCategoryName == budgetcategoryname)
                        {
                            foreach (BudgetCategoryAmount dtl2 in dtl.BudgetCategoryAmount)
                            {
                                if (((int)dtl2.Month) + 1 == month && dtl2.Year == year)
                                {
                                    if (status == "Cancel")
                                    {
                                        dtl2.MonthlyBudgetBalance = dtl2.MonthlyBudgetBalance + total;
                                        break;
                                    }

                                    if (status == "Add")
                                    {
                                        dtl2.MonthlyBudgetBalance = dtl2.MonthlyBudgetBalance - total;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                os.CommitChanges();
            }
            catch (Exception)
            {
                return 0;
            }

            return 1;
        }
    }
}
