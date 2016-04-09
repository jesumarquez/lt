 <%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Documentos.ReporteVencimiento" Title="Untitled Page" Codebehind="ReporteVencimiento.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <table style="width: 100%; padding: 3px;"> 
        <tr>
            <td valign="top">
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="FECHA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFecha" runat="server" Mode="Date" TimeMode="Start" />        
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbLocacion" runat="server"></cwc:LocacionDropDownList>
            </td>
            <td valign="top">
                <cwc:ResourceLabel id="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities"></cwc:ResourceLabel> 
                <br />
                <cwc:PlantaDropDownList ID="cbPlanta" runat="server" AddAllItem="true" ParentControls="cbLocacion"></cwc:PlantaDropDownList>
            </td>
            <td valign="top">
                <cwc:ResourceLabel id="lblTransportista" runat="server" Font-Bold="true" VariableName="PARENTI07" ResourceName="Entities"></cwc:ResourceLabel> 
                <br />
                <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" AddAllItem="true" ParentControls="cbLocacion,cbPlanta" ></cwc:TransportistaDropDownList>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblParenti25" runat="server" ListControlTargetID="cbTipoDocumento" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI25" ForeColor="Black" />
                <br />
                <cwc:TipoDocumentoListBox ID="cbTipoDocumento" runat="server" SelectionMode="Multiple" AutoPostBack="false" ParentControls="cbPlanta" />                
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DIAS_AVISO"></cwc:ResourceLabel>
                <asp:UpdatePanel ID="updDias" runat="server" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDiasAviso" runat="server" Width="50px" Text="30"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br/>
                <asp:CheckBox ID="chkConAviso" runat="server" />
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="SOLO_CON_AVISO"></cwc:ResourceLabel>
            </td>
        </tr>
    </table>
</asp:Content>

