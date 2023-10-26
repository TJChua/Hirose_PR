<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Attachments.aspx.cs" Inherits="BSI_PR.Web.Attachments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">
<style type="text/css">
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DropDownList ID="DropDownList1" runat="server" Height="40px" style="margin-left: 280px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged1">
            </asp:DropDownList>
            <asp:DropDownList ID="DropDownList2" runat="server" Height="40px" style="margin-left: 450px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged1">
            </asp:DropDownList>
        </div>

        <div class="row">
      <div class="col-sm-6">

           <iframe id="pdf1" runat="server" style="height: 1000px; width:700px;" frameborder ="0"></iframe>
      </div>
      <div class="col-sm-6">
           <iframe id="pdf2" runat="server" style="height: 1000px; width:794px;" frameborder ="0"></iframe>

      </div>
    </div>


       
           
           
      
    </form>
</body>
</html>
