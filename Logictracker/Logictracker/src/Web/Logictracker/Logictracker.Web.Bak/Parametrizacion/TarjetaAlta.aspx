<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TarjetaAlta.aspx.cs" Inherits="Logictracker.Web.Parametrizacion.ParametrizacionTarjetaAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnInitialBinding="cbEmpresa_PreBind" />
        
                    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" OnInitialBinding="cbLinea_PreBind" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="NUMERO" />
                    <asp:TextBox ID="txtNumero" runat="server" MaxLength="4" Width="100%" />
        
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="PIN" />
                    <asp:TextBox ID="txtPIN" runat="server" MaxLength="50" Width="100%" />
                    
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="PIN_HEXA" />
                    <asp:TextBox ID="txtPinHexa" runat="server" MaxLength="50" Width="100%" />
        
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="ACCESS_CODE" />
                    <asp:TextBox ID="txtCodAcceso" runat="server" MaxLength="10" Width="100" />
   
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>
