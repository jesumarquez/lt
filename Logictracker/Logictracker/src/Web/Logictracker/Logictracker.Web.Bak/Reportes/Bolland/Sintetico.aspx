<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" CodeFile="Sintetico.aspx.cs" Inherits="Logictracker.Reportes.Bolland.Sintetico" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <table width="100%" style="font-size: x-small">
<tr>
    <td>
        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
        <br />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" />        
        <br />        
        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
        <br />
        <asp:UpdatePanel ID="updLinea" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>        
    </td>
    <td>
        <cwc:ResourceLabel ID="lblTipoVehiculoFiltro" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
        <br />
        <asp:UpdatePanel ID="updTipoVehiculo" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" AddAllItem="true" runat="server" ParentControls="cbLinea" Width="200px" AutoPostBack="True"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>         
        <br/>        
        <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
        <br />
        <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="200px" ParentControls="cbTipoVehiculo"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="cbTipovehiculo" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </td>
    <td>
        <table><tr><td colspan="2">
            <cwc:PeriodoDropDownList ID="cbPeriodo" runat="server" Width="260px" ParentControls="cbEmpresa" OnSelectedIndexChanged="CbPeriodoSelectedIndexChanged" AutoPostBack="True" />
        </td></tr>
        <tr><td>
        <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline">
            <ContentTemplate>
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" Mode="Date" />
            </ContentTemplate>
            <Triggers><asp:AsyncPostBackTrigger ControlID="cbPeriodo"/></Triggers>
        </asp:UpdatePanel>
        
        </td><td>
        
        <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
        <br />
        <div style="text-align: right;">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
            <ContentTemplate>
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" Mode="Date" />
            </ContentTemplate>
            <Triggers><asp:AsyncPostBackTrigger ControlID="cbPeriodo"/></Triggers>
        </asp:UpdatePanel>
        </div>
        
        </td></tr></table>
    </td>      
</tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" Runat="Server">
    <style type="text/css">
        #DetalleSuperior table {
            width: 100%;
            font-size: 12px;
        }
        #DetalleSuperior .section td{
            border: solid 1px #d0d0d0;
            padding: 5px;            
        }
        #DetalleSuperior .title {
            font-weight: bold;
            font-size: 14px;
            width: 200px;
            text-align: center;
            color: white;
            background-color: #b0b0b0;
        }
        #DetalleSuperior .label {
            font-weight: bold;
            float: left;
            width: auto;
            margin-right: 5px;
            height: 20px;
        }
        #DetalleSuperior .value {
            width: auto;
            height: 20px;
        }
    </style>
    <asp:Panel id="panelDetalleSuperior" runat="server" Visible="False" >
    <div id="DetalleSuperior">
    <table class="section">
        <tr>
            <td class="title">Recorrido</td>
            <td>
                <div class="label">Total</div>
                <div class="value"><asp:Label runat="server" ID="lblRecorridoTotal" /> km</div>
            </td>
            <td>
                <div class="label">Promedio Diario</div>
                <div class="value"><asp:Label runat="server" ID="lblRecorridoPromedio" /> km</div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Vehiculo</td>
            <td>
                <div class="label">Interno</div>
                <div class="value"><asp:Label runat="server" ID="lblInterno" /></div>
                <br/>
                <div class="label">Patente</div>
                <div class="value"><asp:Label runat="server" ID="lblPatente" /></div>
                <br/>
                <div class="label">Referencia</div>
                <div class="value"><asp:Label runat="server" ID="lblReferencia" /></div>
            </td>
            <td>
                <div class="label">Tipo de Vehiculo</div>
                <div class="value"><asp:Label runat="server" ID="lblTipoVehiculo" /></div>
                <br/>
                <div class="label">Marca</div>
                <div class="value"><asp:Label runat="server" ID="lblMarca" /></div>
                <br/>
                <div class="label">Modelo</div>
                <div class="value"><asp:Label runat="server" ID="lblModelo" />
                (<asp:Label runat="server" ID="lblAnio" />)
                </div>
            </td>
            <td>
                <div class="label">Total de días</div>
                <div class="value"><asp:Label runat="server" ID="lblTotalDias" /></div>
                <br/>
                <div class="label">Dias con actividad</div>
                <div class="value"><asp:Label runat="server" ID="lblDiasConActividad" /></div>
                <br/>
                <div class="label">Dias sin Actividad</div>
                <div class="value"><asp:Label runat="server" ID="lblDiasSinActividad" /></div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Velocidad</td>
            <td>
                <div class="label">Maxima Alcanzada</div>
                <div class="value"><asp:Label runat="server" ID="lblVelocidadMaxima" /> km/h</div>
            </td>
            <td>
                <div class="label">Promedio</div>
                <div class="value"><asp:Label runat="server" ID="lblVelocidadPromedio" /> km/h</div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">RPM</td>
            <td>
                <div class="label">Estipulada</div>
                <div class="value"><asp:Label runat="server" ID="lblRpmEstipulada" /> rpm</div>
                <br/>
                <div class="label">Estáticas</div>
                <div class="value"><asp:Label runat="server" ID="lblRpmEstatica" /> rpm</div>
                <br/>
                <div class="label">Dinámicas</div>
                <div class="value"><asp:Label runat="server" ID="lblRpmDinamica" /> rpm</div>
            </td>
            <td class="title">Motor</td>
            <td>
                <div class="label">Tiempo en Movimiento</div>
                <div class="value"><asp:Label runat="server" ID="lblTiempoMovimiento" /></div>
                <br/>
                <div class="label">Tiempo Detenido</div>
                <div class="value"><asp:Label runat="server" ID="lblTiempoDetenido" /></div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Desconexiones</td>
            <td>
                <div class="value"><asp:Label runat="server" ID="lblDesconexiones" /></div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Variaciones de Velocidad</td>
            <td>
                <div class="label">Aceleraciones</div>
                <div class="value"><asp:Label runat="server" ID="lblAceleraciones" /></div>
                <br/>
                <div class="label">Desaceleraciones</div>
                <div class="value"><asp:Label runat="server" ID="lblDesaceleraciones" /></div>
            </td>
            <td class="title">Infracciones de Velocidad</td>
            <td>
                <div class="label">Cantidad de Infracciones</div>
                <div class="value"><asp:Label runat="server" ID="lblInfracciones" /></div>
            </td>
        </tr>
    </table>
    </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" Runat="Server">
    <style type="text/css">
        #DetalleInferior .title {
            font-weight: bold;
            font-size: 14px;
            text-align: center;
            color: white;
            background-color: #b0b0b0;
            padding: 5px;
        }

    </style>
    <asp:Panel id="panelDetalleInferior" runat="server" Visible="False" >
    <div id="DetalleInferior">
    <div class="title">Listado de Conductores</div>
    <C1:C1GridView ID="grid" runat="server" UseEmbeddedVisualStyles="false" SkinId="SmallGrid" AllowColMoving="false" AllowGrouping="false" AllowSorting="false" OnRowDataBound="GridRowDataBound">
        <Columns>
            <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI09" />
            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="FECHA" />
            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIEMPO" />
            <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="TARJETA" />
        </Columns>    
    </C1:C1GridView>
    </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" Runat="Server">
    <style type="text/css">
        #DetalleSuperiorPrint table {
            width: 100%;
            font-size: 10px;
        }
        #DetalleSuperiorPrint .section td{
            border: solid 1px #d0d0d0;         
        }
        #DetalleSuperiorPrint .title {
            font-weight: bold;
            font-size: 12px;
            width: 160px;
            text-align: center;
            color: white;
            background-color: #b0b0b0;
        }
        #DetalleSuperiorPrint .label {
            font-weight: bold;
            float: left;
            width: auto;
            margin-right: 5px;
        }
        #DetalleSuperiorPrint .value {
            width: auto;
        }
    </style>
    <asp:Panel id="panelDetalleSuperiorPrint" runat="server" >
    <div id="DetalleSuperiorPrint">
    <table class="section">
        <tr>
            <td class="title">Recorrido</td>
            <td>
                <div class="label">Total</div>
                <div class="value"><asp:Label runat="server" ID="lblRecorridoTotalPrint" /> km</div>
            </td>
            <td>
                <div class="label">Promedio Diario</div>
                <div class="value"><asp:Label runat="server" ID="lblRecorridoPromedioPrint" /> km</div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Vehiculo</td>
            <td>
                <div class="label">Interno</div>
                <div class="value"><asp:Label runat="server" ID="lblInternoPrint" /></div>
                <br/>
                <div class="label">Patente</div>
                <div class="value"><asp:Label runat="server" ID="lblPatentePrint" /></div>
                <br/>
                <div class="label">Referencia</div>
                <div class="value"><asp:Label runat="server" ID="lblReferenciaPrint" /></div>
            </td>
            <td>
                <div class="label">Tipo de Vehiculo</div>
                <div class="value"><asp:Label runat="server" ID="lblTipoVehiculoPrint" /></div>
                <br/>
                <div class="label">Marca</div>
                <div class="value"><asp:Label runat="server" ID="lblMarcaPrint" /></div>
                <br/>
                <div class="label">Modelo</div>
                <div class="value"><asp:Label runat="server" ID="lblModeloPrint" />
                (<asp:Label runat="server" ID="lblAnioPrint" />)
                </div>
            </td>
            <td>
                <div class="label">Total de días</div>
                <div class="value"><asp:Label runat="server" ID="lblTotalDiasPrint" /></div>
                <br/>
                <div class="label">Dias con actividad</div>
                <div class="value"><asp:Label runat="server" ID="lblDiasConActividadPrint" /></div>
                <br/>
                <div class="label">Dias sin Actividad</div>
                <div class="value"><asp:Label runat="server" ID="lblDiasSinActividadPrint" /></div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Velocidad</td>
            <td>
                <div class="label">Maxima Alcanzada</div>
                <div class="value"><asp:Label runat="server" ID="lblVelocidadMaximaPrint" /> km/h</div>
            </td>
            <td>
                <div class="label">Promedio</div>
                <div class="value"><asp:Label runat="server" ID="lblVelocidadPromedioPrint" /> km/h</div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">RPM</td>
            <td>
                <div class="label">Estipulada</div>
                <div class="value"><asp:Label runat="server" ID="lblRpmEstipuladaPrint" /> rpm</div>
                <br/>
                <div class="label">Estáticas</div>
                <div class="value"><asp:Label runat="server" ID="lblRpmEstaticaPrint" /> rpm</div>
                <br/>
                <div class="label">Dinámicas</div>
                <div class="value"><asp:Label runat="server" ID="lblRpmDinamicaPrint" /> rpm</div>
            </td>
            <td class="title">Motor</td>
            <td>
                <div class="label">Tiempo en Movimiento</div>
                <div class="value"><asp:Label runat="server" ID="lblTiempoMovimientoPrint" /></div>
                <br/>
                <div class="label">Tiempo Detenido</div>
                <div class="value"><asp:Label runat="server" ID="lblTiempoDetenidoPrint" /></div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Desconexiones</td>
            <td>
                <div class="value"><asp:Label runat="server" ID="lblDesconexionesPrint" /></div>
            </td>
        </tr>
    </table>
    <table class="section">
        <tr>
            <td class="title">Variaciones de Velocidad</td>
            <td>
                <div class="label">Aceleraciones</div>
                <div class="value"><asp:Label runat="server" ID="lblAceleracionesPrint" /></div>
                <br/>
                <div class="label">Desaceleraciones</div>
                <div class="value"><asp:Label runat="server" ID="lblDesaceleracionesPrint" /></div>
            </td>
            <td class="title">Infracciones de Velocidad</td>
            <td>
                <div class="label">Cantidad de Infracciones</div>
                <div class="value"><asp:Label runat="server" ID="lblInfraccionesPrint" /></div>
            </td>
        </tr>
    </table>
    </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" Runat="Server">
    <style type="text/css">
        #DetalleInferiorPrint .title {
            font-weight: bold;
            font-size: 12px;
            text-align: center;
            color: white;
            background-color: #b0b0b0;
            padding: 5px;
        }

    </style>
    <asp:Panel id="panelDetalleInferiorPrint" runat="server" >
    <div id="DetalleInferiorPrint">
    <div class="title">Listado de Conductores</div>
    <C1:C1GridView ID="gridPrint" runat="server" UseEmbeddedVisualStyles="false" SkinId="PrintGrid" AllowColMoving="false" AllowGrouping="false" AllowSorting="false" OnRowDataBound="GridRowDataBound">
        <Columns>
            <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI09" />
            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="FECHA" />
            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIEMPO" />
            <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="TARJETA" />
        </Columns>    
    </C1:C1GridView>
    </div>
    </asp:Panel>
</asp:Content>

