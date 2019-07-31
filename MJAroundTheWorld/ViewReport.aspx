<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewReport.aspx.cs" Inherits="MJAroundTheWorld.ViewReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_blank" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin-top: -53px; margin-left: -42px;">
            <asp:ScriptManager ID="sptMan" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer Width="100%" Height="1000px" ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" ProcessingMode="Remote" ShowToolBar="False" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
                <ServerReport ReportServerUrl="http://gr-sqlexp-01/reportserver" ReportPath="/MJ Around the World/MJAroundTheWorld" />
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>