<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.CicloLogistico_RecorridoAlta" Codebehind="RecorridoAlta.aspx.cs" %>
<%@ Register Src="~/App_Controls/EditLine.ascx" TagName="EditLine" TagPrefix="uc1" %>

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
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" OnSelectedIndexChanged="cbLinea_SelectedIndexChanged" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        
        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
        <asp:TextBox id="txtCodigo" runat="server" Width="100%" MaxLength="64" />
        
        <cwc:ResourceLabel ID="lblNombre" runat="server" ResourceName="Labels" VariableName="NAME" />
        <asp:TextBox id="txtNombre" runat="server" Width="100%" MaxLength="128" />
        
        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESVIO" />
        <div runat="server">
            <asp:TextBox id="txtDesvio" runat="server" Width="60px" MaxLength="6" Text="100" />
            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="METROS" />
        </div>
        
  </cwc:AbmTitledPanel>
</td>
   <td style="vertical-align: top; width:600px;">
   <div style="position: relative;">
            <div style="position: absolute; top: -1px; right: -3px; z-index: 999999;" class="LogicButton_Background">
                <asp:UpdatePanel runat="server" ID="updBtInvertir" ChildrenAsTriggers="true" UpdateMode="Conditional"><ContentTemplate>
                <asp:Button runat="server" ID="btLimpiar" Text="Limpiar" OnClick="btLimpiar_Click" CssClass="LogicButton LogicButton_RecoClear"/>

                <asp:Button runat="server" ID="btInvertir" Text="Invertir Sentido" OnClick="btInvertir_Click" CssClass="LogicButton LogicButton_RecoInvert"/>
                </ContentTemplate></asp:UpdatePanel>
           </div>
        <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="RECORRIDO" TitleResourceName="Labels" Height="320px">
            
           <uc1:EditLine ID="EditLine1" runat="server" Width="600px" Height="500px" />
            <asp:UpdatePanel runat="server" ID="updMap" UpdateMode="Conditional"><ContentTemplate>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="EditLine1" />
            </Triggers>
            </asp:UpdatePanel>
        </cwc:AbmTitledPanel>
            
</div>        
    </td>
</tr>
</table>
</cwc:AbmTabPanel>
</asp:Content>

