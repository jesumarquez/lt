<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Intercalar.aspx.cs" Inherits="Logictracker.CicloLogistico.Distribucion.Intercalar" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css" />

    <script type="text/javascript">
        function getDimensions() {
            var winWidth, winHeight;
            var d = document;
            if (typeof window.innerWidth != 'undefined') {
                winWidth = window.innerWidth;
                winHeight = window.innerHeight;
            } else {
                if (d.documentElement &&
                    typeof d.documentElement.clientWidth != 'undefined' &&
                        d.documentElement.clientWidth != 0) {
                    winWidth = d.documentElement.clientWidth;
                    winHeight = d.documentElement.clientHeight;
                } else {
                    if (d.body &&
                        typeof d.body.clientWidth != 'undefined') {
                        winWidth = d.body.clientWidth;
                        winHeight = d.body.clientHeight;
                    }
                }
            }
            return { width: winWidth, height: winHeight };
        }
        function Resize() {
            var size = getDimensions();
            var div = $get('progress');
            div.style.width = size.width + "px";
            div.style.height = size.height + "px";
            ResizeDetalles();
        }
        function ResizeDetalles() {
            var divResultados = $get('divResultados');
            var divDetalles = $get('divDetalles');

            var size = getDimensions();
            var height = divResultados.offsetHeight;
            var s = parseInt(size.height - height, 10) - 80;
            divDetalles.style.height = s + "px";
        }
    </script>

</head>
<body id="monitor">
    <form id="intercalador" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200"
            minSize="200" maxSize="500" title="<a href='../'><div class='logo_online'> </div></a>"
            tabPosition="top"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="260"
            split="false" minSize="260" maxSize="260" collapsible="true" title="Filtros">
        </cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="400"
            split="True" minSize="200" collapsible="true" title="Viajes"></cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter"
            West="rgWest" East="rgEast" runat="server">
        </cc1:LayoutManager>
        <asp:Panel ID="pnlManager" runat="server">
        </asp:Panel>
        <!--West: Filtros-->
        <asp:Panel ID="WestPanel" runat="server">
            <asp:Panel runat="server" ID="panelFiltros" style="position: relative;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    
                    <%--Empresa, Linea, Tipo Vehiculo--%>
                    <table id="tbFilters">
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                </div>
                            </th>
                            <td>
                                <cwc:LocacionDropDownList runat="server" ID="cbEmpresa" AddAllItem="False" CssClass="LogicCombo"
                                    Width="100%" AutoPostBack="True" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                </div>
                            </th>
                            <td>
                                <cwc:PlantaDropDownList runat="server" ID="cbLinea" AddAllItem="True" ParentControls="cbEmpresa"
                                    CssClass="LogicCombo" Width="100%" AutoPostBack="True" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel runat="server" ID="lblFecha" ResourceName="Labels" VariableName="FECHA"></cwc:ResourceLabel>
                                </div>
                            </th>
                            <td>
                                <cwc:DateTimePicker runat="server" ID="dtFecha" Mode="Date" TimeMode="None" />
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <div class="header" style="width: 100%; font-weight: bold;">
                                    Entrega a intercalar</div>
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel runat="server" ID="lblCliente" ResourceName="Entities" VariableName="PARENTI18" />
                                </div>
                            </th>
                            <td>
                                <cwc:ClienteDropDownList runat="server" ID="cbCliente" AddAllItem="False" ParentControls="cbLinea"
                                    CssClass="LogicCombo" Width="100%" AutoPostBack="True" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel runat="server" ID="lblPuntoEntrega" ResourceName="Entities" VariableName="PARENTI44" />
                                </div>
                            </th>
                            <td>
                                <cwc:PuntoDeEntregaDropDownList runat="server" ID="cbPuntoEntrega" AddAllItem="False" ParentControls="cbEmpresa,cbLinea,cbCliente" CssClass="LogicCombo" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblDemora" runat="server" ResourceName="Labels" VariableName="Servicio" />
                                </div>
                            </th>
                            <td>
                                <cwc:TipoServicioCicloDropDownList runat="server" ID="cbTipoServicio" ParentControls="cbLinea"
                                    CssClass="LogicCombo" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    <cwc:ResourceLabel runat="server" ID="ResourceLabel2" ResourceName="Labels" VariableName="RADIO" />
                                </div>
                            </th>
                            <td>
                                <asp:TextBox runat="server" ID="txtRadio" Width="80px" Text="10000" CssClass="LogicTextbox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th>
                                <div class="header">
                                    Restricci&oacute;n
                                </div>
                            </th>
                            <td>
                                <table><tr><td>
                                <asp:CheckBox ID="chkPeso" runat="server" Text="Peso" Checked="True"/>
                                </td><td>
                                <asp:CheckBox ID="chkVolumen" runat="server" Text="Volumen"  Checked="True"/>
                                </td></tr></table>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <div class="header" style="cursor:pointer; width: 100%;" onclick="hour_invert();">
                                    Horario</div>
                            </th>
                        </tr>
                    </table>
                    <asp:Panel runat="server" id="panelDisableFilters" class="progress" Visible="False">
                    </asp:Panel>
                    
                     </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="btSearch" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btGuardar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
                        <div>
                                <table id="hoursel"><tr>
                                    <td>00</td><td>01</td><td>02</td><td>03</td><td>04</td><td>05</td><td>06</td><td>07</td><td>08</td><td>09</td><td>10</td><td>11</td>
                                </tr><tr>
                                    <td>12</td><td>13</td><td>14</td><td>15</td><td>16</td><td>17</td><td>18</td><td>19</td><td>20</td><td>21</td><td>22</td><td>23</td>
                                </tr></table>
                 </div>
                 </asp:Panel>
                 <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hidHourSelected" Value="000000000000000000000000"/>
                    <div style="text-align: right;">
                        <cwc:ResourceButton runat="server" ID="btSearch" ResourceName="Controls" VariableName="BUTTON_SEARCH"
                                    CssClass="LogicButton_Big" OnClick="BtnSearchClick" />
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="btSearch" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btGuardar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <script type="text/javascript">
                var hidden = '<%=hidHourSelected.ClientID %>';
                var mouseDown = false;
                function ctd(td) {
                    hour_click({ target: td });
                    $addHandler(td, 'mousedown', hour_down);
                    $addHandler(td, 'mouseup', hour_up);
                    $addHandler(td, 'mouseenter', hour_enter);
                }
                function hour_click(evt) {
                    var td = evt.target;
                    var hid = $get(hidden);
                    var idx = parseInt(td.innerHTML, 10);
                    if (isNaN(idx)) return;
                    var state = hid.value.charAt(idx);
                    if (state == '1') {
                        td.className = '';
                        hid.value = hid.value.replaceAt(idx, "0");
                    } else {
                        td.className = 'selected';
                        hid.value = hid.value.replaceAt(idx, "1");
                    }
                    window.getSelection().removeAllRanges();
                }
                function hour_down(evt) {
                    mouseDown = true;
                    hour_click(evt);
                }
                function hour_up(evt) {
                    mouseDown = false;
                }
                function hour_enter(evt) {
                    if (!mouseDown) return;
                    hour_click(evt);
                }

                String.prototype.replaceAt = function(index, c) {
                    return this.substr(0, index) + c + this.substr(index + 1);
                };

                var tbl = $get('hoursel');
                var tds = tbl.getElementsByTagName('td');
                for (t in tds) {
                    ctd(tds[t]);
                }
                function hour_invert() {
                    var tbl = $get('hoursel');
                    var tds = tbl.getElementsByTagName('td');
                    var hid = $get(hidden);
                    var flag = hid.value;
                    var newstate = flag.indexOf('1') < 0 ? '1' : '0';
                    for (var i = 0; i < flag.length; i++) {
                        var td = tds[i];
                        var state = flag.charAt(i);
                        if (state != newstate) {
                            hour_click({ target: td });
                        }
                    }
                }
                function ConfirmSave() {
                    var ok = confirm('Se guardará la entrega intercalada en el viaje seleccionado \n ¿Está seguro?');
                    if (!ok) return false;
                    var informar = confirm('¿Desea informar al chofer la nueva ruta?');
                    $get('<%=hidMail.ClientID %>').value = informar ? '1' : '0';
                    return true;
                }
            </script>
        </asp:Panel>
        <asp:Panel ID="EastPanel" runat="server">
            <div id="intercalador_result">
                <cwc:InfoLabel ID="infoLabel1" runat="server" />
                <table style="height: 100%;">
                    <tr style="height: 50%;">
                        <td>
                            <div id="divResultados" style="height: 160px; overflow-y: auto; overflow-x: hidden;">
                            <asp:UpdatePanel runat="server" ID="updResult" ChildrenAsTriggers="True" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:HiddenField runat="server" ID="hidMail" Value="0"/>
                                    <c1:C1GridView runat="server" ID="grid" ShowHeader="False" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id"
                                        OnRowDataBound="grid_RowDataBound" OnSelectedIndexChanging="grid_SelectedIndexChanging">
                                        <Columns>
                                            <c1:C1TemplateField>
                                                <ItemTemplate>
                                                    <table class="resultado">
                                                        <tr>
                                                            <td class="color" style='<asp:Literal ID="litColor" runat="server" />'>
                                                                <b><asp:Label runat="server" ID="lblOrden" Text="0"></asp:Label></b>
                                                            </td><td style="width: 36px;">
                                                                <asp:Image runat="server" ID="imgVehiculo"/>
                                                            </td><td>
                                                                <b><asp:Label runat="server" ID="lblCodigo"></asp:Label></b>
                                                                <span class="change">(<asp:Label runat="server" ID="lblCantidad"></asp:Label> entregas)</span>
                                                                <br />
                                                                <asp:Label runat="server" ID="lblVehiculo"></asp:Label>
                                                            </td>
                                                            <td style="text-align: right;">
                                                                <b>
                                                                    <asp:Label runat="server" ID="lblHora"></asp:Label></b><br />
                                                                <asp:Label runat="server" ID="lblMinutos" CssClass="change"></asp:Label><br />
                                                                <asp:Label runat="server" ID="lblDistancia" CssClass="change"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                            </c1:C1TemplateField>
                                        </Columns>
                                        <SelectedRowStyle CssClass="Grid_MouseOver_Item"></SelectedRowStyle>
                                    </c1:C1GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                                <asp:UpdatePanel runat="server" ID="UpdatePanel3" ChildrenAsTriggers="True" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="panelSave" CssClass="header" Visible="False">
                                            <table><tr><td>
                                            <cwc:ResourceButton runat="server" ID="btInstructions" ResourceName="Controls" VariableName="Ver Hoja de Ruta" CssClass="LogicButton_Big" OnClick="btInstructions_Click" />
                                            </td><td style="text-align: right;">
                                            <cwc:ResourceButton runat="server" ID="btGuardar" ResourceName="Controls" VariableName="BUTTON_SAVE" CssClass="LogicButton_Big" OnClick="btGuardar_Click" OnClientClick="return ConfirmSave();"/>
                                            </td></tr></table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>    
                                
                                <div id="divDetalles" style="overflow-y: auto; overflow-x: hidden;">
                                <asp:UpdatePanel runat="server" ID="updDetalles" ChildrenAsTriggers="True" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:MultiView runat="server" ID="multiDetalles" ActiveViewIndex="0">
                                        <asp:View runat="server" ID="viewDetalles">
                                    <AjaxToolkit:ReorderList ID="reorderDetalles" runat="server" AllowReorder="true" DragHandleAlignment="Top" SortOrderField="Orden" PostBackOnReorder="True"
                                        OnItemDataBound="reorderDetalles_ItemDataBound" OnItemReorder="reorderDetalles_ItemReorder">
                                        <DragHandleTemplate>
                                            <asp:Panel runat="server" ID="panelHandle">
                                                <table class="resultado Grid_Selected_Item">
                                                    <tr><td class="color" style='cursor:pointer; height: 42px; <asp:Literal ID="litColor" runat="server" />'>
                                                        <b><asp:Label runat="server" ID="lblOrden"></asp:Label></b>
                                                    </td><td>
                                                             <asp:Label runat="server" ID="lblTipoServicio" CssClass="change"></asp:Label><br/>
                                                        <b><asp:Label runat="server" ID="lblDescripcion"></asp:Label></b>
                                                        
                                                        <br/>
                                                        <asp:Label runat="server" ID="lblCliente"></asp:Label>                                                                
                                                    </td>
                                                    <td style="text-align: right;">
                                                        <asp:Label runat="server" ID="lblHora"></asp:Label><br/>
                                                        <asp:Label runat="server" ID="lblDemora" CssClass="change"></asp:Label>
                                                    </td></tr>
                                                </table>
                                            </asp:Panel>
                                        </DragHandleTemplate>
                                        <ItemTemplate>
                                            <asp:Panel runat="server" ID="panelItem">
                                                <table class="resultado <%#Container.DataItemIndex % 2 == 0 ? "Grid_Item" : "Grid_Alternating_Item" %>">
                                                    <tr><td class="color" style='height: 42px; <asp:Literal ID="litColor" runat="server" />'>
                                                        <b><asp:Label runat="server" ID="lblOrden"></asp:Label></b>
                                                    </td><td>
                                                             <asp:Label runat="server" ID="lblTipoServicio" CssClass="change"></asp:Label><br/>
                                                        <b><asp:Label runat="server" ID="lblDescripcion"></asp:Label></b>
                                                        <br/>
                                                        <asp:Label runat="server" ID="lblCliente"></asp:Label>                                                                
                                                    </td>
                                                    <td style="text-align: right;">
                                                        <asp:Label runat="server" ID="lblHora"></asp:Label><br/>
                                                        <asp:Label runat="server" ID="lblDemora" CssClass="change"></asp:Label>
                                                    </td></tr>
                                                </table>
                                            </asp:Panel>
                                        </ItemTemplate>
                                    </AjaxToolkit:ReorderList> 
                                    </asp:View>
                                    <asp:View runat="server" ID="viewInstructions">
                                        <asp:Label runat="server" ID="lblInstructions"></asp:Label>
                                    </asp:View>
                                    </asp:MultiView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="CenterPanel" runat="server">
            <mon:Monitor ID="Monitor1" runat="server" Width="800px" Height="500px" />
        </asp:Panel>
        <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div id="progress" class="progress">
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>

        <script type="text/javascript">
            $addHandler(window, 'resize', Resize);
            Resize();
        </script>
    </form>
</body>
</html>
