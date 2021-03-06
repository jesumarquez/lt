﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="PtoEntregaAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.PtoEntregaAlta" Title="Untitled Page" %>

<%@ Register Src="~/App_Controls/SelectGeoRefference.ascx" TagName="SelectGeoRefference" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/EditEntityGeoRef.ascx" TagName="EditEntityGeoRef" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">   
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels">
    
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" Width="100%" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="100%"  />
        
                    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100%" />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="100%" ParentControls="cbEmpresa" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
        
                    <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="PARENTI18" Width="100%" />
                    <asp:UpdatePanel ID="upCliente" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:ClienteDropDownList ID="ddlCliente" runat="server" Width="100%" ParentControls="cbLinea" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <cwc:ResourceLabel ID="lblResponsable" runat="server" ResourceName="Labels" VariableName="RESPONSABLE_CUENTA" Width="100%" />
                    <asp:UpdatePanel ID="upResponsable" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:EmpleadoDropDownList ID="cbResponsable" runat="server" Width="100%" ParentControls="cbLinea" AddNoneItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
        
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="100%" Font-Size="Small" MaxLength="32" />
        
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" Width="100%" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" Font-Size="Small" MaxLength="64" />
        
                    <cwc:ResourceLabel ID="lblTelefono" runat="server" ResourceName="Labels" VariableName="TELEFONO" Width="100%"/>
                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="32" Width="100%" />
                        
                    <div></div>
                    <asp:Label ID="lblDireccion" runat="server"></asp:Label>
        
                </cwc:AbmTitledPanel>
            </td>
            <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA DER--%>  
                <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_ADICIONALES">
                
                    <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="COMMENT_1" />
                    <asp:TextBox ID="txtComentario1" runat="server" Width="100%" TabIndex="70" MaxLength="50"/>
                
                    <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="COMMENT_2" />
                    <asp:TextBox ID="txtComentario2" runat="server" Width="100%" TabIndex="75" MaxLength="50"/>
                
                    <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="COMMENT_3" />
                    <asp:TextBox ID="txtComentario3" runat="server" Width="100%" TabIndex="80" MaxLength="50"/>
                
                </cwc:AbmTitledPanel>     
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleResourceName="Labels" TitleVariableName="REFERENCIA_GEOGRAFICA">
                    <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                    <asp:CheckBox ID="chkExistente" runat="server" /><cwc:ResourceLabel ID="lblSelecc" runat="server" ResourceName="Labels" VariableName="SELECT_EXISTENT_GEOREF" />
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="panSelectGeoRef" runat="server" style="display: none;">
                    <uc1:SelectGeoRefference ID="SelectGeoRef1" runat="server" ParentControls="cbEmpresa,cbLinea" Height="200px" />
                    <br /><br /><br />
                    </asp:Panel>
                    <asp:Panel ID="panNewGeoRef" runat="server">
                    <uc1:EditEntityGeoRef ID="EditEntityGeoRef1" runat="server" ParentControls="cbEmpresa,cbLinea" />
                    </asp:Panel>
                </cwc:TitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>

