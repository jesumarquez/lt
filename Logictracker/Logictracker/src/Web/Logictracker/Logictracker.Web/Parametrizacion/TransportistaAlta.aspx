<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TransportistaAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.TransportistaAlta" Title="Transportista" %>

<%@ Register Src="~/App_Controls/SelectGeoRefference.ascx" TagName="SelectGeoRefference" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/EditEntityGeoRef.ascx" TagName="EditEntityGeoRef" TagPrefix="uc1" %>
<%@ Register src="~/App_Controls/DocumentList.ascx" tagname="DocumentList" tagprefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">
<cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="240px">
    <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="100%" />
    
    <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
    <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="100%" ParentControls="cbEmpresa" OnSelectedIndexChanged="cbLinea_SelectedIndexChanged" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
    
    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DESCRIPCION"></cwc:ResourceLabel>
    <asp:TextBox runat="server" id="txtDescripcion" Width="100%" MaxLength="64" />
    
    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="TELEFONO"></cwc:ResourceLabel>
    <asp:TextBox ID="txtTelefono" runat="server" Width="100%" MaxLength="20" />
    
    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="MAIL"></cwc:ResourceLabel>
    <asp:TextBox ID="txtMail" runat="server"  Width="100%" MaxLength="128" />
    
    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="CONTACTO"></cwc:ResourceLabel>
    <asp:TextBox ID="txtContacto" runat="server" Width="100%" MaxLength="128" />
     
     <div runat="server"></div>
     <asp:UpdatePanel ID="updIdentificaChoferes" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <cwc:ResourceCheckBox ID="chkIdentificaChoferes" runat="server" ResourceName="Labels" VariableName="IDENTIFICA_CHOFERES" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
</cwc:AbmTitledPanel>
</td>
<td style="width: 50%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="TARIFAS" TitleResourceName="Labels" Height="240px">
            <asp:UpdatePanel ID="updTarifas" runat="server" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <table class="Grid">
                    <tr>
                        <th class="Grid_Header" style="width: 250px">
                            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="CLIENT"></cwc:ResourceLabel>
                        </th>
                        <th class="Grid_Header" style="width: 125px"><cwc:ResourceLabel ID="ResourceLabel7" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TRAMO_CORTO"></cwc:ResourceLabel></th>
                        <th class="Grid_Header" style="width: 125px"><cwc:ResourceLabel ID="ResourceLabel8" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TRAMO_LARGO"></cwc:ResourceLabel></th>
                        <th class="Grid_Header" style="width: 75px"></th>
                    </tr>
                    <tr>
                        <td>
                        <cwc:ResourceLabel ID="ResourceLabel9" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TODOS"></cwc:ResourceLabel>
                        </td>
                        <td>
                            <asp:TextBox runat="server" id="txtTarifaCorto" Width="140px" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" id="txtTarifaLargo" Width="140px" />
                        </td>
                    </tr>   
                        <asp:Repeater ID="RepeaterTarifas" runat="server" onitemdatabound="RepeaterTarifas_ItemDataBound" 
                            onitemcommand="RepeaterTarifas_ItemCommand">
                            <ItemTemplate>        
                                <tr>
                                    <td>
                                        <asp:DropDownList ID="cbCliente" runat="server" Width="100%" DataTextField="Descripcion"
                                            DataValueField="Id" />
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" id="txtTarifaCorto" Width="140px" />
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" id="txtTarifaLargo" Width="140px" />
                                    </td>
                                    <td>
                                        <cwc:ResourceLinkButton ID="btEliminarTarifa" runat="server" CommandName="Delete" ResourceName="Controls" VariableName="BUTTON_DELETE" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>                      
                    <tr>
                        <th colspan="4" class="Grid_Header">
                            <cwc:ResourceLinkButton ID="btNuevaTarifa" runat="server" onclick="btNuevaTarifa_Click"
                                ResourceName="Controls" VariableName="BUTTON_TARIFA_DIFERENCIAL" />
                        </th>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTitledPanel>         
        </td>
        <tr>
            <td colspan=2>
            <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleResourceName="Entities" TitleVariableName="PARENTI05">
                <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                <cwc:ResourceCheckBox ID="chkExistente" ResourceName="Labels" VariableName="SELECCIONAR_GEOREF_EXISTENTE" runat="server" /> 
                </asp:Panel>
                <br />
                <asp:Panel ID="panSelectGeoRef" runat="server" style="display: none;">
                <uc1:SelectGeoRefference ID="SelectGeoRef1" runat="server" ParentControls="owner" Height="200px" />
                <br /><br /><br />
                </asp:Panel>
                <asp:Panel ID="panNewGeoRef" runat="server">
                <uc1:EditEntityGeoRef ID="EditEntityGeoRef1" runat="server" ParentControls="owner" />
                </asp:Panel>
            </cwc:TitledPanel>
            </td>
        </tr>
        </table>
</cwc:AbmTabPanel>


 <cwc:AbmTabPanel ID="DocumentsPanel" runat="server" ResourceName="Entities" VariableName="DOCUMENTOS" >

    <uc1:DocumentList ID="DocumentList1" runat="server" OnlyForTransporter="true" />  

</cwc:AbmTabPanel>
        
</asp:Content>


