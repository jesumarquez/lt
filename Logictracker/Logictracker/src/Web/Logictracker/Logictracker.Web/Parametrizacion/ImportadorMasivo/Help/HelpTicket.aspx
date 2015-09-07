<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ImportadorMasivo.Help.Parametrizacion_ImportadorMasivo_Help_HelpTicket" Codebehind="HelpTicket.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <cwc:ResourceLabel ID="lblTitle" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="IMPORT_TICKETS_HELP_TITLE" />
            <br />
            <br />
            <cwc:ResourceLabel ID="lblBody" runat="server" ResourceName="Labels" VariableName="IMPORT_BODY" />
            <br />
            <br />
            <cwc:ResourceLabel ID="lblImp" runat="server" Font-Bold="true" ForeColor="Red" ResourceName="Labels" VariableName="IMPORTANTE" />
            <cwc:ResourceLabel ID="lblImpDescri" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HELP_TICKET_IMPORTANTE_DETAIL" />
            <br />
            <br />
            <cwc:ResourceLabel ID="lblWarning" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="IMPORT_WARNING" />
        </div>
    </form>
</body>
</html>
