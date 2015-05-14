<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MensajesVehiculoGrafico.aspx.cs" Inherits="Logictracker.Reportes.Accidentologia.AccidentologiaMensajesVehiculoGrafico" MasterPageFile="~/MasterPages/ReportGraphPage.master" %>

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
                            <cwc:PlantaDropDownList ID="ddlBase" AddAllItem="true" runat="server" OnInitialBinding="DdlBasePreBind" Width="125px"
                                ParentControls="ddlDistrito" />
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
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server" OnInitialBinding="DdlTipoVehiculoPreBind"
                                                         Width="125px" ParentControls="ddlBase"  />
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
                                ParentControls="ddlTipoVehiculo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
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
                    <c1:C1NumericInput ID="npMinutes" runat="server" MaxValue="10" MinValue="0" Value="10" Width="50px" DecimalPlaces="0" Height="15px" />
                </td>
            </tr>
        </table>
</asp:Content>
