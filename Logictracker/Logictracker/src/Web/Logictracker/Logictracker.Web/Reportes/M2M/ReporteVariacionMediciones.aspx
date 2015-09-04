<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.ReporteVariacionMediciones" MasterPageFile="~/MasterPages/ReportGraphPage.master" Codebehind="ReporteVariacionMediciones.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server"> 
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" OnInitialBinding="DdlDistritoPreBind" Width="125px" />
                </td>                
                <td>
                    <cwc:ResourceLabel ID="lblTipo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI76" />
                    <br />
                    <asp:UpdatePanel id="upTipoEntidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoEntidadDropDownList ID="ddlTipoEntidad" AddAllItem="true" runat="server" OnInitialBinding="DdlTipoEntidadPreBind" Width="125px" ParentControls="ddlBase"  />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />                            
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblSubentidad" runat="server" Font-Bold="true" VariableName="PARENTI81" ResourceName="Entities" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upSubentidad" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:SubEntidadDropDownList ID="ddlSubentidad" runat="server" OnInitialBinding="DdlSubentidadPreBind" Width="125px" ParentControls="ddlEntidad" TipoMedicion="TEMP,CAUDAL" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlEntidad" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblFecha" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="FECHA_HORA" />
                    <br />
                    <cwc:DateTimePicker ID="dtpFecha" IsValidEmpty="false" runat="server" Mode="DateTime" TimeMode="Now" Width="120px" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblRango" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="MINUTOS" />
                    <br />
                    <c1:C1NumericInput ID="npMinutes" runat="server" MaxValue="60" MinValue="0" Value="60" Width="50px" DecimalPlaces="0" Height="15px" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" />
                            <br />
                            <cwc:PlantaDropDownList ID="ddlBase" AddAllItem="true" runat="server" OnInitialBinding="DdlBasePreBind" Width="125px" ParentControls="ddlDistrito" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblEntidad" runat="server" Font-Bold="true" VariableName="PARENTI79" ResourceName="Entities" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upEntidad" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:EntidadDropDownList ID="ddlEntidad" runat="server" OnInitialBinding="DdlEntidadPreBind" Width="125px" ParentControls="ddlTipoEntidad" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top" colspan="3" align="center">
                    
                </td>
            </tr>
        </table>
    </asp:Content>
    
    <asp:Content ID="Content2" ContentPlaceHolderID="DetalleSuperior" runat="Server">
        <asp:UpdatePanel ID="updPanel" runat="server">
            <ContentTemplate>
                <table width="100%" border="0" id="tblLinks" runat="server" >
                    <tr>
                        <td style="width: 33%"> &nbsp; </td>
                        <td style="width: 33%" align="center">
                            <cwc:ResourceLinkButton ID="lnkHistorico" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_HISTORICO" />
                            <asp:Label ID="lbl" runat="server" Text="-" />
                            <cwc:ResourceLinkButton ID="lnkCalidad" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_CALIDAD" />
                        </td>
                        <td style="width: 33%"> &nbsp; </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Content>
