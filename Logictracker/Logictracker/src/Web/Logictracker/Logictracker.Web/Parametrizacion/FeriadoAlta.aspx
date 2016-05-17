<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Parametrizacion.Parametrizacion_FeriadoAlta" Title="Untitled Page" Codebehind="FeriadoAlta.aspx.cs" %>

<%@ Register Src="~/App_Controls/Pickers/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 50%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_FERIADO"
                    TitleResourceName="Labels" Height="115px">
                    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="200px" OnInitialBinding="ddlEmpresa_PreBind"
                        AddAllItem="true" />
                    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" OnInitialBinding="cbLinea_PreBind"
                                ParentControls="ddlEmpresa" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="FECHA" />
                    <uc3:DatePicker ID="dpFecha" runat="server" IsValidEmpty="false" Width="170" />
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="200px" MaxLength="32" />
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>
