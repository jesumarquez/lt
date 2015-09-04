<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"  Inherits="Logictracker.CicloLogistico.BocaDeCargaAlta" Codebehind="BocaDeCargaAlta.aspx.cs" %>
      
<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >  
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="320px">
    
        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" />
        
        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        
        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
        <asp:TextBox id="txtCodigo" runat="server" Width="100%" MaxLength="64" />
        
        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
        <asp:TextBox id="txtDescripcion" runat="server" Width="100%" MaxLength="128" />
        
        <cwc:ResourceLabel ID="lblRendimiento" runat="server" ResourceName="Labels" VariableName="RENDIMIENTO" />
        <asp:TextBox id="txtRendimiento" runat="server" Width="100%" MaxLength="16" />
        
        <cwc:ResourceLabel ID="lblHorasLaborales" runat="server" ResourceName="Labels" VariableName="HORAS_LABORALES" />
        <asp:TextBox id="txtHorasLaborales" runat="server" Width="100%" MaxLength="16" Text="8" />
        
        <cwc:ResourceLabel ID="lblInicioActividad" runat="server" ResourceName="Labels" VariableName="INICIO_ACTIVIDAD" />
        <cwc:DateTimePicker ID="dtInicioActividad" runat="server" Mode="Time" IsValidEmpty="true" />
        
  </cwc:AbmTitledPanel>
</td>
   <td style="vertical-align: top; width: 50%;">
    
</td>
</tr>
</table>
</cwc:AbmTabPanel>

</asp:Content>
