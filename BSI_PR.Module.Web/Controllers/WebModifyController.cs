﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using BSI_PR.Module.BusinessObjects;
using BSI_PR.Module.Controllers;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.XtraReports.UI;

#region update log
// TJC - 20210503 - Fixed double DocNum and no DocNum ver 0.1
// TJC - 20210520 - Fixed double DocNum and no DocNum again ver 0.2
// TJC - 20211012 - Fixed no save if not new document ver 0.3
// TJC - 20211209 - Write file in folder ver 0.4
// TJC - 20211209 - Limit PO attachment no over 5MB ver 0.5
// TJC - 20230607 - add PO Japan ver 0.6

#endregion

namespace BSI_PR.Module.Web.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class WebModifyController : WebModificationsController
    {
        // Start ver 0.5
        GenControllers genCon;
        // End ver 0.5
        public WebModifyController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ModificationsController>().SaveAndNewAction.Active.SetItemValue("Enabled", false);
            Frame.GetController<ModificationsController>().SaveAndCloseAction.Active.SetItemValue("Enabled", false);
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            // Start ver 0.5
            genCon = Frame.GetController<GenControllers>();
            // End ver 0.5
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        protected override void Save(SimpleActionExecuteEventArgs args)
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());

            if (View.ObjectTypeInfo.Type == typeof(PurchaseRequests))
            {
                PurchaseRequests newPR = (PurchaseRequests)args.CurrentObject;
                if (newPR.IsNew == true)
                {
                    base.Save(args);
                    newPR.DocNum = newPR.Department.DocPrefix + "-PR-" + newPR.DocNumSeq.NextDocNo;
                    base.Save(args);
                    newPR.DocNumSeq.NextDocNo++;
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
                else
                {

                    base.Save(args);
                }


                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder newPO = (PurchaseOrder)args.CurrentObject;
                if (newPO.IsNew == true)
                {

                    try
                    {
                        base.Save(args);
                        // start ver 0.2
                        //newPO.DocNum = newPO.Department.DocPrefix + "-PO-" + newPO.DocNumSeq.NextDocNo;
                        // end ver 0.2
                        // start ver 0.1
                        IObjectSpace os = Application.CreateObjectSpace();
                        DepartmentDocs tempdoc = os.FindObject<DepartmentDocs>
                            (CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 1));
                        // start ver 0.2
                        newPO.DocNum = newPO.Department.DocPrefix + "-PO-" + tempdoc.NextDocNo;
                        // end ver 0.2
                        tempdoc.NextDocNo++;
                        os.CommitChanges();
                        // end ver 0.1

                        // Start ver 0.4
                        FileInfo fileInfo = new FileInfo(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                        DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                        if (dirInfo.Exists)
                        {
                            string[] Files = Directory.GetFiles(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                            foreach (string file in Files)
                            {
                                File.Delete(file);
                            }
                        }
                        else
                        {
                            dirInfo.Create();
                        }
                        foreach (PurchaseOrderAttachments dtl2 in newPO.PurchaseOrderAttachments)
                        {
                            string tempFileLocation = string.Concat(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\", "\\", dtl2.File.FileName);
                            try
                            {
                                FileData fd = ObjectSpace.FindObject<FileData>(CriteriaOperator.Parse("Oid = ?", dtl2.File.Oid)); // Use any other IObjectSpace APIs to query required data.  

                                if (fd != null)
                                {
                                    Stream sourceStream = new MemoryStream();
                                    ((IFileData)fd).SaveToStream(sourceStream);
                                    sourceStream.Position = 0;

                                    using (var fileStream = new FileStream(tempFileLocation, FileMode.Create, FileAccess.Write))
                                    {
                                        sourceStream.CopyTo(fileStream);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        // End ver 0.4
                    }
                    catch (Exception ex)
                    {
                            //error message
                    }
                    //New added
                    //-------------------------------------------------------------------------------
                    decimal DocDisc = 0;
                    decimal Accumulated = 0;
                    decimal totalRow = 0;
                    int i = 0;
                    DocDisc = newPO.DocDiscAmount;

                    foreach (PurchaseOrderDetails dtl in newPO.PurchaseOrderDetails)
                    {
                        totalRow += dtl.LineTotal;
                    }
                    //New added
                    //-------------------------------------------------------------------------------

                    foreach (PurchaseOrderDetails dtl in newPO.PurchaseOrderDetails)
                    {
                        dtl.PODocNum = newPO.DocNum;

                        //New added
                        //-------------------------------------------------------------------------------
                        if (newPO.PurchaseOrderDetails.Count != (i + 1))
                        {
                            dtl.DocDiscAmount = Math.Round((dtl.LineTotal/ totalRow)* DocDisc,2);
                            Accumulated += Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                        }
                        else
                        {
                            dtl.DocDiscAmount = DocDisc - Accumulated;
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                        }
                        i++;
                        //New added
                        //-------------------------------------------------------------------------------

                    }
                    base.Save(args);
                    // start ver 0.1
                    //newPO.DocNumSeq.NextDocNo++;
                    //ObjectSpace.CommitChanges();
                    //ObjectSpace.Refresh();
                    // end ver 0.1
                }
                else
                {
                    //New added
                    //-------------------------------------------------------------------------------
                    decimal DocDisc = 0;
                    decimal Accumulated = 0;
                    decimal totalRow = 0;
                    int i = 0;
                    DocDisc = newPO.DocDiscAmount;

                    foreach (PurchaseOrderDetails dtl in newPO.PurchaseOrderDetails)
                    {
                        totalRow += dtl.LineTotal;
                    }
                    //New added
                    //-------------------------------------------------------------------------------
                    foreach (PurchaseOrderDetails dtl in newPO.PurchaseOrderDetails)
                    {
                        dtl.PODocNum = newPO.DocNum;

                        //New added
                        //-------------------------------------------------------------------------------
                        if (newPO.PurchaseOrderDetails.Count != (i + 1))
                        {
                            dtl.DocDiscAmount = Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                            Accumulated += Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                        }
                        else
                        {
                            dtl.DocDiscAmount = DocDisc - Accumulated;
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                        }
                        i++;
                        //New added
                        //-------------------------------------------------------------------------------

                    }

                    // Start ver 0.4
                    FileInfo fileInfo = new FileInfo(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                    DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                    if (dirInfo.Exists)
                    {
                        string[] Files = Directory.GetFiles(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                        foreach (string file in Files)
                        {
                            File.Delete(file);
                        }
                    }
                    else
                    {
                        dirInfo.Create();
                    }
                    foreach (PurchaseOrderAttachments dtl2 in newPO.PurchaseOrderAttachments)
                    {
                        string tempFileLocation = string.Concat(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\", "\\", dtl2.File.FileName);
                        try
                        {
                            FileData fd = ObjectSpace.FindObject<FileData>(CriteriaOperator.Parse("Oid = ?", dtl2.File.Oid)); // Use any other IObjectSpace APIs to query required data.  

                            if (fd != null)
                            {
                                Stream sourceStream = new MemoryStream();
                                ((IFileData)fd).SaveToStream(sourceStream);
                                sourceStream.Position = 0;

                                using (var fileStream = new FileStream(tempFileLocation, FileMode.Create, FileAccess.Write))
                                {
                                    sourceStream.CopyTo(fileStream);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    // End ver 0.4

                    base.Save(args);
                }

                if (newPO.DocDiscAmount > 0)
                {
                    string Updtaxamount = "UPDATE T0 SET T0.TotalTaxAmount = T2.TaxAmount " +
                        "FROM PurchaseOrder T0 " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(D0.LineTotal, 0) - (ISNULL(D1.DocDiscAmount, 0) / " +
                        "D2.LineTotal * ISNULL(D0.LineTotal, 0))) * (ISNULL(D0.TaxRate, 0) / 100) as TaxAmount, " +
                        "D0.PurchaseOrder FRom PurchaseOrderDetails D0 " +
                        "INNER JOIN PurchaseOrder D1 on D0.PurchaseOrder = D1.OID " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(LineTotal, 0)) as LineTotal, PurchaseOrder FRom PurchaseOrderDetails D0 " +
                        "WHERE D0.GCRecord is null and PurchaseOrder is not null and D0.PurchaseOrder = '" + newPO.Oid + "' " +
                        "GROUP BY PurchaseOrder " +
                        ") D2 on D1.OID = D2.PurchaseOrder " +
                        "WHERE D0.GCRecord is null and D0.PurchaseOrder is not null and D0.PurchaseOrder = '" + newPO.Oid + "' " +
                        "GROUP BY D0.PurchaseOrder, D1.DocDiscAmount, D2.LineTotal, ISNULL(D0.TaxRate, 0) " +
                        ") T2 on T0.OID = T2.PurchaseOrder " +
                        "WHERE T0.DocNum = '" + newPO.DocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdtax = new SqlCommand(Updtaxamount, conn);
                    SqlDataReader readertax = cmdtax.ExecuteReader();
                    conn.Close();
                }
                else
                {
                    string Updtaxamount = "UPDATE T0 SET T0.TotalTaxAmount = T1.TaxAmount " +
                        "FROM PurchaseOrder T0 " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(TaxAmount, 0)) as TaxAmount, PurchaseOrder FRom PurchaseOrderDetails D0 " +
                        "WHERE D0.GCRecord is null and PurchaseOrder is not null and D0.PurchaseOrder = '" + newPO.Oid + "' " +
                        "GROUP BY PurchaseOrder " +
                        ") T1 on T0.OID = PurchaseOrder " +
                        "WHERE T0.DocNum = '" + newPO.DocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdtax = new SqlCommand(Updtaxamount, conn);
                    SqlDataReader readertax = cmdtax.ExecuteReader();
                    conn.Close();
                }

                string UpdAmount = "UPDATE T0 SET T0.Amount = T1.LineTotal, T0.FinalAmount = T1.LineTotal - ISNULL(T0.DocDiscAmount, 0) + ISNULL(TotalTaxAmount, 0) " +
                    "FROM PurchaseOrder T0 " +
                    "INNER JOIN " +
                    "( " +
                    "SELECT SUM(ISNULL(LineTotal, 0)) as LineTotal, PurchaseOrder FRom PurchaseOrderDetails D0 " +
                    "WHERE D0.GCRecord is null and PurchaseOrder is not null and D0.PurchaseOrder = '" + newPO.Oid + "' " +
                    "GROUP BY PurchaseOrder " +
                    ") T1 on T0.OID = PurchaseOrder " +
                    "WHERE T0.DocNum = '" + newPO.DocNum + "'";
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmdupd = new SqlCommand(UpdAmount, conn);
                SqlDataReader readerupd = cmdupd.ExecuteReader();
                conn.Close();

                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else if (View.ObjectTypeInfo.Type == typeof(GoodReceipt))
            {
                GoodReceipt newGRN = (GoodReceipt)args.CurrentObject;
                if (newGRN.IsNew == true)
                {
                    base.Save(args);
                    newGRN.DocNum = newGRN.Department.DocPrefix + "-GRN-" + newGRN.DocNumSeq.NextDocNo;

                    foreach (GoodReceiptDetails dtl in newGRN.GoodReceiptDetails)
                    {
                        dtl.GRNDocNum = newGRN.DocNum;
                    }
                    base.Save(args);
                    newGRN.DocNumSeq.NextDocNo++;
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
                else
                {

                    base.Save(args);
                }


               ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else if (View.ObjectTypeInfo.Type == typeof(GoodsReturn))
            {
                GoodsReturn newReturn = (GoodsReturn)args.CurrentObject;
                if (newReturn.IsNew == true)
                {
                    base.Save(args);
                    newReturn.DocNum = newReturn.Department.DocPrefix + "-GR-" + newReturn.DocNumSeq.NextDocNo;
                    base.Save(args);
                    newReturn.DocNumSeq.NextDocNo++;
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
                else
                {

                    base.Save(args);
                }


              ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else if (View.ObjectTypeInfo.Type == typeof(APInvoice))
            {
                APInvoice newinv = (APInvoice)args.CurrentObject;
                if (newinv.IsNew == true)
                {
                    base.Save(args);
                    // start ver 0.2
                    //newinv.DocNum = newinv.Department.DocPrefix + "-INV-" + newinv.DocNumSeq.NextDocNo;
                    // end ver 0.2
                    // start ver 0.1
                    IObjectSpace os = Application.CreateObjectSpace();
                    DepartmentDocs tempdoc = os.FindObject<DepartmentDocs>
                        (CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 4));
                    // start ver 0.2
                    newinv.DocNum = newinv.Department.DocPrefix + "-INV-" + tempdoc.NextDocNo;
                    // end ver 0.2
                    tempdoc.NextDocNo++;
                    os.CommitChanges();
                    // end ver 0.1

                    //New added
                    //-------------------------------------------------------------------------------
                    //decimal Discounts = 0;
                    //decimal TaxAmount = 0;

                    //foreach (APInvoiceDetails dtl in newinv.APInvoiceDetails)
                    //{
                    //    dtl.DocDiscAmountAfter = Math.Round((dtl.DocDiscUnitAmount * dtl.Quantity), 6);
                    //    Discounts += Decimal.Parse(dtl.DocDiscAmountAfter.ToString());
                    //    dtl.TaxAmount = Math.Round((dtl.TaxUnitAmount * dtl.Quantity), 6);

                    //}
                    //newinv.DocDiscAmount = Discounts;

                    //New added
                    //-------------------------------------------------------------------------------



                    base.Save(args);
                    // start ver 0.1
                    //newinv.DocNumSeq.NextDocNo++;
                    //ObjectSpace.CommitChanges();
                    //ObjectSpace.Refresh();
                    // end ver 0.1
                }
                // start ver 0.3
                else
                {
                    base.Save(args);
                }
                // start ver 0.3
                //else
                //{
                //    //New added
                //    //-------------------------------------------------------------------------------
                //    decimal Discounts = 0;

                //    foreach (APInvoiceDetails dtl in newinv.APInvoiceDetails)
                //    {
                //        dtl.DocDiscAmountAfter = Math.Round((dtl.DocDiscUnitAmount * dtl.Quantity), 6);
                //        Discounts += Decimal.Parse(dtl.DocDiscAmountAfter.ToString());
                //        dtl.TaxAmount = Math.Round((dtl.TaxUnitAmount * dtl.Quantity), 6);
                //    }
                //    newinv.DocDiscAmount = Discounts;
                //    //New added
                //    //-------------------------------------------------------------------------------

                //    base.Save(args);
                //}

                IObjectSpace trxos = Application.CreateObjectSpace();
                APInvoice trx = trxos.FindObject<APInvoice>(new BinaryOperator("Oid", newinv.Oid));

                if (trx != null)
                {
                    trx.PONum = null;
                    string getporef = "SELECT DocNum FROM APInvoiceDetails WHERE APInvoice = '" + trx.Oid + "' GROUP BY DocNum";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(getporef, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (trx.PONum != null)
                        {
                            trx.PONum = trx.PONum + ", " + reader.GetString(0);
                        }
                        else
                        {
                            trx.PONum = reader.GetString(0);
                        }
                    }
                    cmd.Dispose();
                    conn.Close();
                }

                trxos.CommitChanges();

                if (trx.DocDiscAmount > 0)
                {
                    string Updtaxamount = "UPDATE T0 SET T0.TotalTaxAmount = T2.TaxAmount " +
                        "FROM APInvoice T0 " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(D0.LineAmount, 0) - (ISNULL(D1.DocDiscAmount, 0) / D2.LineTotal * ISNULL(D0.LineAmount, 0))) " +
                        "* (ISNULL(D0.TaxRate, 0) / 100) as TaxAmount, " +
                        "D0.APInvoice FRom APInvoiceDetails D0 " +
                        "INNER JOIN APInvoice D1 on D0.APInvoice = D1.OID " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(LineAmount, 0)) as LineTotal, APInvoice FRom APInvoiceDetails D0 " +
                        "WHERE D0.GCRecord is null and APInvoice is not null and D0.APInvoice = '" + trx.Oid + "' " +
                        "GROUP BY APInvoice " +
                        ") D2 on D1.OID = D2.APInvoice " +
                        "WHERE D0.GCRecord is null and D0.APInvoice is not null and D0.APInvoice = '" + trx.Oid + "' " +
                        "GROUP BY D0.APInvoice, D1.DocDiscAmount, D2.LineTotal, ISNULL(D0.TaxRate, 0) " +
                        ") T2 on T0.OID = T2.APInvoice " +
                        "WHERE T0.DocNum = '" + trx.DocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdtax = new SqlCommand(Updtaxamount, conn);
                    SqlDataReader readertax = cmdtax.ExecuteReader();
                    conn.Close();
                }
                else
                {
                    string Updtaxamount = "UPDATE T0 SET T0.TotalTaxAmount = T1.TaxAmount " +
                        "FROM APInvoice T0 " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(TaxAmount, 0)) as TaxAmount, APInvoice FRom APInvoiceDetails D0 " +
                        "WHERE D0.GCRecord is null and APInvoice is not null and D0.APInvoice = '" + trx.Oid + "' " +
                        "GROUP BY APInvoice " +
                        ") T1 on T0.OID = APInvoice " +
                        "WHERE T0.DocNum = '" + trx.DocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdtax = new SqlCommand(Updtaxamount, conn);
                    SqlDataReader readertax = cmdtax.ExecuteReader();
                    conn.Close();
                }

                string UpdAmount = "UPDATE T0 SET T0.Amount = T1.LineTotal, T0.FinalAmount = T1.LineTotal - ISNULL(T0.DocDiscAmount, 0) + ISNULL(TotalTaxAmount, 0) " +
                    "FROM APInvoice T0 " +
                    "INNER JOIN " +
                    "( " +
                    "SELECT SUM(ISNULL(LineAmount, 0)) as LineTotal, APInvoice FRom APInvoiceDetails D0 " +
                    "WHERE D0.GCRecord is null and APInvoice is not null and D0.APInvoice = '" + trx.Oid + "' " +
                    "GROUP BY APInvoice " +
                    ") T1 on T0.OID = APInvoice " +
                    "WHERE T0.DocNum = '" + trx.DocNum + "'";
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmdupd = new SqlCommand(UpdAmount, conn);
                SqlDataReader readerupd = cmdupd.ExecuteReader();
                conn.Close();

                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            // Start ver 0.5
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderAttachments))
            {
                PurchaseOrderAttachments newatt = (PurchaseOrderAttachments)args.CurrentObject;

                if (newatt.File.Size > 5242880)
                {
                    genCon.showMsg("Error", "This file is too large to upload. The maximum supported file size: 5.00 MB (5,242,880 bytes).", InformationType.Error);
                }
                else
                {
                    base.Save(args);
                }
            }
            // End ver 0.5
            // Start ver 0.6
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderJapan))
            {
                PurchaseOrderJapan newPO = (PurchaseOrderJapan)args.CurrentObject;
                if (newPO.IsNew == true)
                {

                    try
                    {
                        base.Save(args);
                        // start ver 0.2
                        //newPO.DocNum = newPO.Department.DocPrefix + "-PO-" + newPO.DocNumSeq.NextDocNo;
                        // end ver 0.2
                        // start ver 0.1
                        IObjectSpace os = Application.CreateObjectSpace();
                        DepartmentDocs tempdoc = os.FindObject<DepartmentDocs>
                            (CriteriaOperator.Parse("Department = ? and DocType.PRType = ?", user.DefaultDept.Oid, 5));
                        // start ver 0.2
                        newPO.DocNum = newPO.Department.DocPrefix + "-JPO-" + tempdoc.NextDocNo;
                        // end ver 0.2
                        tempdoc.NextDocNo++;
                        os.CommitChanges();
                        // end ver 0.1

                        // Start ver 0.4
                        FileInfo fileInfo = new FileInfo(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                        DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                        if (dirInfo.Exists)
                        {
                            string[] Files = Directory.GetFiles(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                            foreach (string file in Files)
                            {
                                File.Delete(file);
                            }
                        }
                        else
                        {
                            dirInfo.Create();
                        }
                        foreach (PurchaseOrderJapanAttachments dtl2 in newPO.PurchaseOrderJapanAttachments)
                        {
                            string tempFileLocation = string.Concat(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\", "\\", dtl2.File.FileName);
                            try
                            {
                                FileData fd = ObjectSpace.FindObject<FileData>(CriteriaOperator.Parse("Oid = ?", dtl2.File.Oid)); // Use any other IObjectSpace APIs to query required data.  

                                if (fd != null)
                                {
                                    Stream sourceStream = new MemoryStream();
                                    ((IFileData)fd).SaveToStream(sourceStream);
                                    sourceStream.Position = 0;

                                    using (var fileStream = new FileStream(tempFileLocation, FileMode.Create, FileAccess.Write))
                                    {
                                        sourceStream.CopyTo(fileStream);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        // End ver 0.4
                    }
                    catch (Exception ex)
                    {
                        //error message
                    }
                    //New added
                    //-------------------------------------------------------------------------------
                    decimal DocDisc = 0;
                    decimal Accumulated = 0;
                    decimal totalRow = 0;
                    int i = 0;
                    DocDisc = newPO.DocDiscAmount;

                    foreach (PurchaseOrderJapanDetails dtl in newPO.PurchaseOrderJapanDetails)
                    {
                        totalRow += dtl.LineTotal;
                    }
                    //New added
                    //-------------------------------------------------------------------------------

                    foreach (PurchaseOrderJapanDetails dtl in newPO.PurchaseOrderJapanDetails)
                    {
                        dtl.PODocNum = newPO.DocNum;

                        //New added
                        //-------------------------------------------------------------------------------
                        if (newPO.PurchaseOrderJapanDetails.Count != (i + 1))
                        {
                            dtl.DocDiscAmount = Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                            Accumulated += Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                        }
                        else
                        {
                            dtl.DocDiscAmount = DocDisc - Accumulated;
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                        }
                        i++;
                        //New added
                        //-------------------------------------------------------------------------------

                    }
                    base.Save(args);
                    // start ver 0.1
                    //newPO.DocNumSeq.NextDocNo++;
                    //ObjectSpace.CommitChanges();
                    //ObjectSpace.Refresh();
                    // end ver 0.1
                }
                else
                {
                    //New added
                    //-------------------------------------------------------------------------------
                    decimal DocDisc = 0;
                    decimal Accumulated = 0;
                    decimal totalRow = 0;
                    int i = 0;
                    DocDisc = newPO.DocDiscAmount;

                    foreach (PurchaseOrderJapanDetails dtl in newPO.PurchaseOrderJapanDetails)
                    {
                        totalRow += dtl.LineTotal;
                    }
                    //New added
                    //-------------------------------------------------------------------------------

                    foreach (PurchaseOrderJapanDetails dtl in newPO.PurchaseOrderJapanDetails)
                    {
                        dtl.PODocNum = newPO.DocNum;

                        //New added
                        //-------------------------------------------------------------------------------
                        if (newPO.PurchaseOrderJapanDetails.Count != (i + 1))
                        {
                            dtl.DocDiscAmount = Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                            Accumulated += Math.Round((dtl.LineTotal / totalRow) * DocDisc, 2);
                        }
                        else
                        {
                            dtl.DocDiscAmount = DocDisc - Accumulated;
                            dtl.DocDiscUnitAmount = dtl.DocDiscAmount / dtl.Quantity;
                        }
                        i++;
                        //New added
                        //-------------------------------------------------------------------------------

                    }

                    // Start ver 0.4
                    FileInfo fileInfo = new FileInfo(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                    DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                    if (dirInfo.Exists)
                    {
                        string[] Files = Directory.GetFiles(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\");
                        foreach (string file in Files)
                        {
                            File.Delete(file);
                        }
                    }
                    else
                    {
                        dirInfo.Create();
                    }
                    foreach (PurchaseOrderJapanAttachments dtl2 in newPO.PurchaseOrderJapanAttachments)
                    {
                        string tempFileLocation = string.Concat(ConfigurationManager.AppSettings["AttachmentFile"].ToString() + newPO.DocNum + "\\", "\\", dtl2.File.FileName);
                        try
                        {
                            FileData fd = ObjectSpace.FindObject<FileData>(CriteriaOperator.Parse("Oid = ?", dtl2.File.Oid)); // Use any other IObjectSpace APIs to query required data.  

                            if (fd != null)
                            {
                                Stream sourceStream = new MemoryStream();
                                ((IFileData)fd).SaveToStream(sourceStream);
                                sourceStream.Position = 0;

                                using (var fileStream = new FileStream(tempFileLocation, FileMode.Create, FileAccess.Write))
                                {
                                    sourceStream.CopyTo(fileStream);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    // End ver 0.4

                    base.Save(args);
                }

                if (newPO.DocDiscAmount > 0)
                {
                    string Updtaxamount = "UPDATE T0 SET T0.TotalTaxAmount = T2.TaxAmount " +
                        "FROM PurchaseOrderJapan T0 " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(D0.LineTotal, 0) - (ISNULL(D1.DocDiscAmount, 0) / " +
                        "D2.LineTotal * ISNULL(D0.LineTotal, 0))) * (ISNULL(D0.TaxRate, 0) / 100) as TaxAmount, " +
                        "D0.PurchaseOrderJapan FRom PurchaseOrderJapanDetails D0 " +
                        "INNER JOIN PurchaseOrderJapan D1 on D0.PurchaseOrderJapan = D1.OID " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(LineTotal, 0)) as LineTotal, PurchaseOrderJapan FRom PurchaseOrderJapanDetails D0 " +
                        "WHERE D0.GCRecord is null and PurchaseOrderJapan is not null and D0.PurchaseOrderJapan = '" + newPO.Oid + "' " +
                        "GROUP BY PurchaseOrderJapan " +
                        ") D2 on D1.OID = D2.PurchaseOrderJapan " +
                        "WHERE D0.GCRecord is null and D0.PurchaseOrderJapan is not null and D0.PurchaseOrderJapan = '" + newPO.Oid + "' " +
                        "GROUP BY D0.PurchaseOrderJapan, D1.DocDiscAmount, D2.LineTotal, ISNULL(D0.TaxRate, 0) " +
                        ") T2 on T0.OID = T2.PurchaseOrderJapan " +
                        "WHERE T0.DocNum = '" + newPO.DocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdtax = new SqlCommand(Updtaxamount, conn);
                    SqlDataReader readertax = cmdtax.ExecuteReader();
                    conn.Close();
                }
                else
                {
                    string Updtaxamount = "UPDATE T0 SET T0.TotalTaxAmount = T1.TaxAmount " +
                        "FROM PurchaseOrderJapan T0 " +
                        "INNER JOIN " +
                        "( " +
                        "SELECT SUM(ISNULL(TaxAmount, 0)) as TaxAmount, PurchaseOrderJapan FRom PurchaseOrderJapanDetails D0 " +
                        "WHERE D0.GCRecord is null and PurchaseOrderJapan is not null and D0.PurchaseOrderJapan = '" + newPO.Oid + "' " +
                        "GROUP BY PurchaseOrderJapan " +
                        ") T1 on T0.OID = PurchaseOrderJapan " +
                        "WHERE T0.DocNum = '" + newPO.DocNum + "'";
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    SqlCommand cmdtax = new SqlCommand(Updtaxamount, conn);
                    SqlDataReader readertax = cmdtax.ExecuteReader();
                    conn.Close();
                }

                string UpdAmount = "UPDATE T0 SET T0.Amount = T1.LineTotal, T0.FinalAmount = T1.LineTotal - ISNULL(T0.DocDiscAmount, 0) + ISNULL(TotalTaxAmount, 0) " +
                    "FROM PurchaseOrderJapan T0 " +
                    "INNER JOIN " +
                    "( " +
                    "SELECT SUM(ISNULL(LineTotal, 0)) as LineTotal, PurchaseOrderJapan FRom PurchaseOrderJapanDetails D0 " +
                    "WHERE D0.GCRecord is null and PurchaseOrderJapan is not null and D0.PurchaseOrderJapan = '" + newPO.Oid + "' " +
                    "GROUP BY PurchaseOrderJapan " +
                    ") T1 on T0.OID = PurchaseOrderJapan " +
                    "WHERE T0.DocNum = '" + newPO.DocNum + "'";
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmdupd = new SqlCommand(UpdAmount, conn);
                SqlDataReader readerupd = cmdupd.ExecuteReader();
                conn.Close();

                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            // End ver 0.6
            else
            {
                base.Save(args);
            }
        }
    }
}
