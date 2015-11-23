<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionTipoCocheAlta" Codebehind="TipoCocheAlta.aspx.cs" %>  

<%@ Register Src="~/App_Controls/IconPicker.ascx" TagName="IconPicker" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">

    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_TIPO_VEHICULO" >
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_TIPO_VEHICULO" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" AddAllItem="false" Width="100%" />
                        
                        <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <asp:UpdatePanel ID="upPlanta" runat="server">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" ParentControls="ddlLocacion" AddAllItem="true" Width="100%" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="6" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" /> 
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:TitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="ICONOGRAFIA" TitleResourceName="Labels" Height="150px">
                        <table style="width: 100%; text-align:left" cellpadding="5">
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="ICONO_DEFAULT" />
                                </td>
                                <td>
                                    <uc:IconPicker ID="iconDeafult" runat="server"  ParentControls="ddlLocacion,ddlPlanta" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="ICONO_ENHORA" />
                                </td>
                                <td>
                                    <uc:IconPicker ID="iconNormal" runat="server"  ParentControls="ddlLocacion,ddlPlanta" />
                                </td>
                            </tr>
                            <tr style="height: 50px" valign="middle">
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="ICONO_ATRASO" />
                                </td>
                                <td>
                                    <uc:IconPicker ID="iconAtraso" runat="server"  ParentControls="ddlLocacion,ddlPlanta" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="ICONO_ADELANTO" />
                                </td>
                                <td>
                                    <uc:IconPicker ID="iconAdelanto" runat="server"  ParentControls="ddlLocacion,ddlPlanta" />
                                </td>
                            </tr>
                        </table>   
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleVariableName="VALORES_REFERENCIA" TitleResourceName="Labels" Height="120px">
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_ALCANZABLE" />
                        <c1:C1NumericInput runat="server" ID="npMaxSpeed" DecimalPlaces="0" MaxValue="999" MinValue="0" Width="100px" Height="15px" Value="200" SmartInputMode="true" />
                        
                        <cwc:ResourceLabel ID="lblKilometros" runat="server" ResourceName="Labels" VariableName="KILOMETROS_REFERENCIA" />
                        <c1:C1NumericInput runat="server" ID="npKilometros" DecimalPlaces="0" MaxValue="999999" MinValue="0" Width="100px" Height="15px" Value="0" SmartInputMode="true" />
                        
                        <cwc:ResourceLabel ID="lblVelocidadPromedio" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO" />
                        <c1:C1NumericInput runat="server" ID="npVelocidadPromedio" DecimalPlaces="0" MaxValue="999" MinValue="0" Width="100px" Height="15px" Value="20" SmartInputMode="true" />
                        
                        <cwc:ResourceLabel ID="lblCapacidadCarga" runat="server" ResourceName="Labels" VariableName="CAPACIDAD_CARGA" />
                        <c1:C1NumericInput runat="server" ID="npCapacidadCarga" DecimalPlaces="0" MaxValue="999999" MinValue="0" Width="100px" Height="15px" Value="0" SmartInputMode="true" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="CAPACIDAD_COMBUSTIBLE" />
                        <c1:C1NumericInput runat="server" ID="txtCapacidad" DecimalPlaces="0" MaxValue="999999" MinValue="0" Width="100px" Height="15px" Value="0" SmartInputMode="true" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="DESVIO_MINIMO" />
                        <c1:C1NumericInput runat="server" ID="npDesvioMinimo" DecimalPlaces="0" MaxValue="999" MinValue="0" Width="100px" Height="15px" Value="0" SmartInputMode="true" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="DESVIO_MAXIMO" />
                        <c1:C1NumericInput runat="server" ID="npDesvioMaximo" DecimalPlaces="0" MaxValue="999" MinValue="0" Width="100px" Height="15px" Value="0" SmartInputMode="true" />
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;"> 
                    <cwc:AbmTitledPanel ID="AbmTitledPanel4" runat="server" Height="120px" TitleVariableName="COMPORTAMIENTO" TitleResourceName="Labels">
                        
                        <br /><cwc:ResourceCheckBox ID="chkControlaKilometraje" runat="server" ResourceName="Labels" VariableName="CONTROLA_KILOMETRAJE" />
                        <br /><cwc:ResourceCheckBox runat="server" ID="chkControlaTurno" ResourceName="Labels" VariableName="CONTROLA_TURNO" />
                        <br /><cwc:ResourceCheckBox runat="server" ID="chkControlaConsumo" ResourceName="Labels" VariableName="CONTROLA_CONSUMOS" />
                        <br /><cwc:ResourceCheckBox runat="server" ID="chkSeguimientoPersona" ResourceName="Labels" VariableName="SEGUIMIENTO_PERSONA" />
                        <br /><cwc:ResourceCheckBox runat="server" ID="chkNoEsVehiculo" ResourceName="Labels" VariableName="NO_ES_VEHICULO" />

                        <br />
                        <asp:UpdatePanel ID="updChkEsControlAcceso" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkEsControlAcceso" runat="server" ResourceName="Labels" VariableName="ES_PUERTA" Width="100%" OnCheckedChanged="ChkEsControlAccesoOnCheckedChanged" AutoPostBack="true" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br />
                        <asp:UpdatePanel runat="server" ID="pnlControlAcceso">
                            <ContentTemplate>                                
                                <cwc:ResourceButton ID="btnGenerar" runat="server" ResourceName="Labels" VariableName="GENERAR_ACCESOS" OnClick="BtnGenerarOnClick" Enabled="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>
