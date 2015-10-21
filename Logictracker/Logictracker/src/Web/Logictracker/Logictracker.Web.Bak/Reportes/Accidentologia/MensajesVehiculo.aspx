<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MensajesVehiculo.aspx.cs" MasterPageFile="~/MasterPages/ReportGridPage.master"
    Inherits="Logictracker.Reportes.Accidentologia.Accidentologia_MensajesVehiculo" Title="Mensajes Vehiculo" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">  
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" OnInitialBinding="DdlDistritoPreBind" Width="125px" />
                </td>
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
                    <cwc:ResourceLabel ID="lblTipo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel id="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipo" runat="server" AddAllItem="true" OnInitialBinding="DdlTipoPreBind"
                                                             Width="125px" ParentControls="ddlBase" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblMovil" runat="server" Font-Bold="true" VariableName="PARENTI03" ResourceName="Entities" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upMovil" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="ddlMovil" UseOptionGroup="true" OptionGroupProperty="Estado" runat="server" OnInitialBinding="DdlMovilPreBind" Width="125px"
                                ParentControls="ddlTipo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblFecha" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="FECHA_HORA" />
                    <br />
                    <cwc:DateTimePicker ID="dtpFecha" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" Width="120px" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblRango" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="MINUTOS" />
                    <br />
                    <c1:C1NumericInput ID="npRango" runat="server" MaxValue="10" MinValue="0" Value="10" Width="50px" DecimalPlaces="0" Height="15px" />
                </td>
                <td>
                    <cwc:ResourceCheckBox ID="chkDirecciones" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_DIRECCIONES" />
                </td>
            </tr>
        </table>
  </asp:Content>
