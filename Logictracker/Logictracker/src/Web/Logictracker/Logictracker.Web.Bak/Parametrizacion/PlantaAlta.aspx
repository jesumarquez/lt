<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="PlantaAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.PlantaAlta" %>

<%@ Register Src="~/App_Controls/SelectGeoRefference.ascx" TagName="SelectGeoRefference" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/EditEntityGeoRef.ascx" TagName="EditEntityGeoRef" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_GENERALES" Height="238px">

                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <asp:UpdatePanel ID="upLocacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:LocacionDropDownList ID="cbLocacion" runat="server" Width="98%" OnSelectedIndexChanged="cbLocacion_SelectedIndexChanged" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
              
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="10" />
              
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="NAME" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
              
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="TELEFONO" />
                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="32" Width="98%" />
               
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="MAIL" />
                    <asp:TextBox ID="txtMail" runat="server" MaxLength="64" Width="98%" />
              
                    <cwc:ResourceLabel ID="lblUsoHorario" runat="server" ResourceName="Labels" VariableName="USO_HORARIO" />
                    <cwc:TimeZoneDropDownList ID="ddlTimeZone" runat="server" Width="98%" />
               
               </cwc:AbmTitledPanel>
            </td>
            <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA DER--%>
                <cwc:TitledPanel ID="tpComportamientos" runat="server" TitleResourceName="Labels" TitleVariableName="COMPORTAMIENTOS"  Height="238px">
                    <br />
                    <cwc:ResourceCheckBox ID="chkInterface" runat="server" ResourceName="Labels" VariableName="HABILITAR_INTERFACE_WS"  />
                    <br />
                    <br />
                    <asp:UpdatePanel ID="updIdentificaChoferes" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:ResourceCheckBox ID="chkIdentificaChoferes" runat="server" ResourceName="Labels" VariableName="IDENTIFICA_CHOFERES" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                </cwc:TitledPanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleResourceName="Labels" TitleVariableName="REFERENCIA_GEOGRAFICA">
                    <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                        <asp:CheckBox ID="chkExistente" runat="server" /> Seleccionar una Referencia Geografica existente.
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="panSelectGeoRef" runat="server" style="display: none;">
                        <asp:UpdatePanel ID="updSel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc1:SelectGeoRefference ID="SelectGeoRef1" runat="server" ParentControls="cbLocacion" Height="200px" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br />
                        <br />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="panNewGeoRef" runat="server">
                        <uc1:EditEntityGeoRef ID="EditEntityGeoRef1" runat="server" ParentControls="cbLocacion" />
                    </asp:Panel>
                </cwc:TitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>
