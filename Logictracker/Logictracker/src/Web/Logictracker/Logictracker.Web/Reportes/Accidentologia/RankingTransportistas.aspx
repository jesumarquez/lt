<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Accidentologia.RankingTransportistas" Codebehind="RankingTransportistas.aspx.cs" %>

<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>    
 
<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server"> 
    <table width="100%">
        <tr valign="top">
            <td align="left">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="150px" />
            </td>
            <td align="left">
                <cwc:ResourceLinkButton id="lblBase" runat="server" ListControlTargetID="ddlBase" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:PlantaListBox ID="ddlBase" runat="server" AutoPostBack="false" ParentControls="ddlDistrito" Width="150px" SelectionMode="Multiple" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLinkButton id="lblTransportista" runat="server" ListControlTargetID="lbTransportista" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TransportistasListBox ID="lbTransportista" runat="server" AutoPostBack="false" ParentControls="ddlDistrito,ddlBase" Width="150px" SelectionMode="Multiple" AddNoneItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />  
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblInicio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                <br/>
                <cwc:ResourceLabel ID="lblFin" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" />
                <cwc:DateTimeRangeValidator MinRange="0" MaxRange="93" StartControlID="dpDesde" EndControlID="dpHasta" runat="server" ID="rangeValid" />
                <br/>
                <cwc:ResourceCheckBox runat="server" ID="chkSinActividad" VariableName="SIN_ACTIVIDAD" ResourceName="Labels" Font-Bold="True"/>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="content2" ContentPlaceHolderID="DetalleInferior" runat="server">
    <table id="tbl_totales" runat="server" cellpadding="5" class="abmpanel" style="width: 100%" visible="false">  
        <tr>
            <td class="panelheader" colspan="2">
                <cwc:ResourceLabel ID="lblTotales" runat="server" ResourceName="Labels" VariableName="TOTALES" Font-Bold="true" />
            </td>
        </tr>
        <tr>
            <td style="width: 16px">
                <asp:TextBox ID="imgGraves" runat="server" Width="14px" Height="8px" BackColor="#ff6666"/>
            </td>
            <td align="left">
                <asp:Label ID="lblGraves" runat="server" Font-Bold="true" Font-Size="6" /> 
            </td>
        </tr>
        <tr>
            <td style="width: 16px">
                <asp:TextBox ID="Image1" runat="server" Width="14px" Height="8px" BackColor="LightYellow"/>
            </td>
            <td align="left">
                <asp:Label ID="lblMedias" runat="server" Font-Bold="true" Font-Size="6" />
            </td>
        </tr>
        <tr>
            <td style="width: 16px">
                <asp:TextBox ID="imgLeves" runat="server" Width="14px" Height="8px" BackColor="LightGreen"/>
            </td>
            <td align="left">
                <asp:Label ID="lblLeves" runat="server" Font-Bold="true" Font-Size="6" /> 
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="content3" ContentPlaceHolderID="DetalleInferiorPrint" runat="server">
    <br />
    <br />
    <br />
    <table id="Table1" runat="server" cellpadding="5" class="abmpanel" style="width: 100%" visible="true">  
        <tr>
            <td class="panelheader" colspan="2">
                <cwc:ResourceLabel ID="lblTotalesPrint" runat="server" ResourceName="Labels" VariableName="TOTALES" Font-Bold="true" />
            </td>
        </tr>
        <tr>
            <td style="width: 16px"> 
                <asp:TextBox ID="TextBox1" runat="server" Width="14px" Height="8px" BackColor="#ff6666"/>
                </td>
            <td align="left">
                <asp:Label ID="lblGravesPrint" runat="server" Font-Bold="true" Font-Size="6" /> 
            </td>
        </tr>
        <tr>
            <td style="width: 16px"> 
                <asp:TextBox ID="TextBox2" runat="server" Width="14px" Height="8px" BackColor="LightYellow"/>
                </td>
            <td align="left">
                <asp:Label ID="lblMediasPrint" runat="server" Font-Bold="true" Font-Size="6" />
            </td>
        </tr>
        <tr>
            <td style="width: 16px">
                <asp:TextBox ID="TextBox3" runat="server" Width="14px" Height="8px" BackColor="LightGreen"/>
            </td>
            <td align="left">
                <asp:Label ID="lblLevesPrint" runat="server" Font-Bold="true" Font-Size="6" /> 
            </td>
        </tr>
    </table>
</asp:Content>

