<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionTanqueAlta" Codebehind="TanqueAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
 
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="100px"> 
        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
        <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="50" />
        <cwc:ResourceLabel ID="lblCapacidad" runat="server" ResourceName="Labels" VariableName="CAPACIDAD" />
        <c1:C1NumericInput ID="npCapacidad" runat="server" MinValue="0" MaxValue="999999999" Width="120px" Height="15px" Value="0" DecimalPlaces="2" OnValueChanged="refresh_Difference"/>                
    </cwc:AbmTitledPanel>
</td>
</tr>
<tr>
<td style="width: 100%; vertical-align: top;"colspan="2">
    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CONFIGURACION_ALARMAS" TitleResourceName="Labels" > 
        <cwc:ResourceLabel ID="lblTiempoSinReportar" runat="server" ResourceName="Labels" VariableName="TIEMPO_SIN_REPORTAR" />
        <c1:C1NumericInput runat="server" ID="npSinReportar" Width="120px" MaxValue="99999999" MinValue="0" Value="0" Height="15px" DecimalPlaces="0"/>
        <cwc:ResourceLabel ID="lblStockRepo" runat="server" ResourceName="Labels" VariableName="STOCK_REPOSICION"  />
        <c1:C1NumericInput ID="npStockRepo" runat="server" MinValue="0" MaxValue="999999999" Width="120px" Value="0" DecimalPlaces="2" Height="15px"/>
        <cwc:ResourceLabel ID="lblStockCritico" runat="server" ResourceName="Labels" VariableName="STOCK_CRITICO" />
        <c1:C1NumericInput ID="npStockCritico" runat="server" MinValue="0" MaxValue="999999999" Width="120px" Value="0" DecimalPlaces="2" Height="15px"/>
        <cwc:ResourceLabel ID="lblAguaMin" runat="server" ResourceName="Labels" VariableName="AGUA_MIN" />
        <c1:C1NumericInput ID="npAguaMin" runat="server" MinValue="0" MaxValue="999999999" Width="120px" Value="0" DecimalPlaces="2" Height="15px"/>
        <cwc:ResourceLabel ID="lblAguaMax" runat="server" ResourceName="Labels" VariableName="AGUA_MAX" />
        <c1:C1NumericInput ID="npAguaMax" runat="server" MinValue="0" MaxValue="999999999" Width="120px" Value="0" DecimalPlaces="2" Height="15px"/>        
        <cwc:ResourceLabel ID="lblPorcentaje" runat="server" ResourceName="Labels" VariableName="PORCENTAJE_CAPACIDAD_TANQUE" />
        <c1:C1PercentInput ID="npPorcent" runat="server" MinValue="0" MaxValue="100" Width="120px" Value="0" DecimalPlaces="0" Height="15px" OnValueChanged="refresh_Difference"/>
        <cwc:ResourceLabel ID="lblDif" runat="server" ResourceName="Labels" VariableName="MAX_DIF_TEORICO_Y_REAL" />
         <asp:UpdatePanel ID="upDiferencia" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <asp:TextBox runat="server" ID="txtMaxVolRealvsTeorico" Width="200" Enabled="false"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="npPorcent" EventName="ValueChanged" />
                <asp:AsyncPostBackTrigger ControlID="npCapacidad" EventName="ValueChanged" />
            </Triggers>
         </asp:UpdatePanel>
</cwc:AbmTitledPanel>
</td>
</tr>
</table>
</asp:Content>

