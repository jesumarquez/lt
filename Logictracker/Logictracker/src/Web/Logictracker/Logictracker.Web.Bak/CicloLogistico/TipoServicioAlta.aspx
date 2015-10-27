<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TipoServicioAlta.aspx.cs" Inherits="Logictracker.CicloLogistico.TipoServicioAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >  
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style=" vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="140px">
    
        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" />
        
        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="True" ParentControls="cbEmpresa" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        
        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
        <asp:TextBox id="txtCodigo" runat="server" Width="100%" MaxLength="64" />
        
        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
        <asp:TextBox id="txtDescripcion" runat="server" Width="100%" MaxLength="128" />
        
        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DEMORA" />
        <div id="Div1" runat="server">
            <asp:TextBox id="txtDemora" runat="server" Width="60px" MaxLength="6" Text="0" />
            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="MINUTOS" />
        </div>
        
        <cwc:ResourceLabel ID="lblDefault" runat="server" ResourceName="Labels" VariableName="DEFAULT" />
        <asp:CheckBox runat="server" ID="chkDefault" />
        
  </cwc:AbmTitledPanel>
</td>
    <td style="vertical-align: top; width:600px;">
    </td>
</tr>
</table>
</cwc:AbmTabPanel>
</asp:Content>

