<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportPage.master" AutoEventWireup="true" CodeFile="ControlarDistribucion.aspx.cs" Inherits="Logictracker.CicloLogistico.Distribucion.ControlarDistribucion" %>
<%@ Import Namespace="Logictracker.Security" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table><tr><td>
        <cwc:ResourceLabel runat="server" ID="lblEmpresa" ResourceName="Entities" VariableName="PARENTI01" OnSelectedIndexChanged="CbEmpresaSelectedIndexChanged" /><br/>
        <cwc:LocacionDropDownList runat="server" ID="cbEmpresa" AddAllItem="False" Width="200px" CssClass="LogicCombo" />
    </td><td>
        <cwc:PeriodoDropDownList runat="server" ID="cbPeriodo" ParentControls="cbEmpresa" Width="200px" CssClass="LogicCombo" />
    </td></tr></table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentReport" Runat="Server">
    <div id="control_distribucion">
    <asp:UpdatePanel ID="updCompleto" runat="server" RenderMode="Inline" ChildrenAsTriggers="false" UpdateMode="Conditional" Visible="False">
        <ContentTemplate>
            <AjaxToolkit:TabContainer ID="tab" runat="server" AutoPostBack="True" OnActiveTabChanged="TabActiveTabChanged">
                <AjaxToolkit:TabPanel ID="tabVehiculos" runat="server">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server"  ResourceName="Entities" VariableName="PARENTI03"/>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <c1:C1GridView runat="server" ID="gridVehiculos" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id">
                            <Columns>
                                <c1:C1TemplateField>
                                    <ItemStyle Width="20px"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" ID="chkSelect"/>
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                        <input type="checkbox" onchange="checkAll(this);"/>
                                    </HeaderTemplate>
                                </c1:C1TemplateField>
                                <c1h:C1ResourceBoundColumn DataField="Interno" ResourceName="Entities" VariableName="PARENTI03" />
                                <c1h:C1ResourceBoundColumn DataField="Viajes" ResourceName="Labels" VariableName="VIAJES" />
                            </Columns>
                        </c1:C1GridView>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabControl" runat="server">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server"  ResourceName="Labels" VariableName="Control"/>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="updControl" ChildrenAsTriggers="True" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table id="navigator">
                                    <tr>
                                        <td class="move" onclick="if(this.getElementsByTagName('a').length > 0) location.href = this.getElementsByTagName('a')[0].href;">
                                            <asp:LinkButton runat="server" ID="btAnterior" Text="<<" CommandName="Bck" OnCommand="ControlCommand" CssClass="button" OnClientClick="return false;" />
                                        </td>
                                        <td>
                                            <table><tr>
                                                <td class="index"><asp:TextBox runat="server" ID="txtIndexControl" /></td>
                                                <td class="text">de</td>
                                                <td class="count"><asp:Label runat="server" ID="lblCountControl" /></td>
                                                <td class="go"><cwc:ResourceLinkButton runat="server" ID="btIr" ResourceName="Controls" VariableName="Ir" CommandName="Go" OnCommand="ControlCommand" /></td>
                                            </tr></table>
                                        </td>
                                        <td class="move" onclick="if(this.getElementsByTagName('a').length > 0) location.href = this.getElementsByTagName('a')[0].href;">
                                            <asp:LinkButton runat="server" ID="btSiguiente" Text=">>" CommandName="Fwd" OnCommand="ControlCommand" CssClass="button" OnClientClick="return false;" />
                                        </td>
                                    </tr>
                                </table>
                                <div id="data">
                                    <div>
                                        <table  class="header">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td class="odd">
                                                                <div class="title"><cwc:ResourceLabel runat="server" ID="ResourceLabel4" ResourceName="Entities" VariableName="PARENTI02" /></div>
                                                                <div class="data"><asp:Label runat="server" ID="lblBase" /></div>
                                                            </td>
                                                            <td class="even">
                                                                <div class="title"><cwc:ResourceLabel runat="server" ID="ResourceLabel6" ResourceName="Labels" VariableName="DATE" /></div>
                                                                <div class="data"><asp:Label runat="server" ID="lblFecha" /></div>
                                                            </td>
                                                            <td class="odd">
                                                                <div class="title"><asp:Label runat="server" ID="lblTipo" /></div>
                                                                <div class="data"><asp:Label runat="server" ID="lblCodigo" /></div>
                                                            </td>
                                                            <td class="even">
                                                                <div class="title"><cwc:ResourceLabel runat="server" ID="lblVehiculoTitle" ResourceName="Entities" VariableName="PARENTI03" /></div>
                                                                <div class="data"><asp:Label runat="server" ID="lblVehiculo" /></div>
                                                            </td>
                                                            <td class="odd">
                                                                <div class="title"><cwc:ResourceLabel runat="server" ID="ResourceLabel5" ResourceName="Entities" VariableName="PARENTI09" /></div>
                                                                <div class="data"><asp:Label runat="server" ID="lblEmpleado" /></div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="text-align: right;">
                                                    <cwc:ResourceButton runat="server" ID="ResourceButton1" ResourceName="Controls" VariableName="BUTTON_MAP" CssClass="LogicButton_Big" CommandName="Map" OnCommand="MapCommand"/>
                                                    <cwc:ResourceButton runat="server" ID="btAplicar" ResourceName="Controls" VariableName="BUTTON_SAVE" CssClass="LogicButton_Big LogicButton_Save" CommandName="Save" OnCommand="SaveCommand"/>
                                                    <cwc:ResourceButton runat="server" ID="btControlar" ResourceName="Labels" VariableName="Controlar" CssClass="LogicButton_Big LogicButton_Check" CommandName="Control" OnCommand="SaveCommand"/>
                                                    <cwc:ResourceButton runat="server" ID="btModificar" ResourceName="Labels" VariableName="Modificar" CssClass="LogicButton_Big LogicButton_Cancel" CommandName="Cancel" OnCommand="SaveCommand"/>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <c1:C1GridView runat="server" ID="gridEntregas" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id" OnRowDataBound="GridEntregasRowDataBound" ShowFooter="True">
                                        <Columns>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ENTREGA" >
                                                <ItemStyle  HorizontalAlign="Left"></ItemStyle>
                                                <ItemTemplate>
                                                    <div class="title"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Tipo%></div>
                                                    <div class="data"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Entrega%></div>
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="PROGRAMADO"  >
                                                <ItemStyle  HorizontalAlign="Center"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <div class="title"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Programado.ToDisplayDateTime().ToString("dd-MM-yyyy")%></div>
                                                    <div class="data"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Programado.ToDisplayDateTime().ToString("HH:mm")%></div>
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ENTRADA"  >
                                                <ItemStyle  HorizontalAlign="Center"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <div class="title"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Entrada.HasValue ? (Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Entrada.Value.ToDisplayDateTime().ToString("dd-MM-yyyy") : string.Empty%></div>
                                                    <div class="data"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Entrada.HasValue ? (Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty%></div>
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="MANUAL" >
                                                <ItemStyle  HorizontalAlign="Center"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <div class="title"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Manual.HasValue ? (Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Manual.Value.ToDisplayDateTime().ToString("dd-MM-yyyy") : string.Empty%></div>
                                                    <div class="data"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Manual.HasValue ? (Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty%></div>
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="SALIDA" >
                                                <ItemStyle  HorizontalAlign="Center"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <div class="title"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Salida.HasValue ? (Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Salida.Value.ToDisplayDateTime().ToString("dd-MM-yyyy") : string.Empty%></div>
                                                    <div class="data"><%#(Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Salida.HasValue ? (Container.DataItem as Logictracker.CicloLogistico.Distribucion.EntregaAControlar).Salida.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty%></div>
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceBoundColumn DataField="KmGps" DataFormatString="{0:0.0}" ResourceName="Labels" VariableName="Km Gps">
                                                <ItemStyle  HorizontalAlign="Right"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                                <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                            </c1h:C1ResourceBoundColumn>
                                            <c1h:C1ResourceBoundColumn DataField="KmCalculado" DataFormatString="{0:0.0}" ResourceName="Labels" VariableName="Km Calculado">
                                                <ItemStyle  HorizontalAlign="Right"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                                <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                            </c1h:C1ResourceBoundColumn>
                                            <c1h:C1ResourceTemplateColumn  ResourceName="Labels" VariableName="Km Controlado" >
                                                <ItemStyle  HorizontalAlign="Right"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
                                                <FooterStyle HorizontalAlign="Right"></FooterStyle>
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="txtKmControlado" CssClass="LogicTextbox km" Width="50px" /> km
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                        </Columns>
                                    </c1:C1GridView>
                                    
                                    
                                    
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabResumen" runat="server">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server"  ResourceName="Labels" VariableName="Resumen"/>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server" ID="updResumen">
                            <ContentTemplate>
                                
                                <c1:C1GridView runat="server" ID="gridResumen" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id" OnRowCommand="GridResumenRowCommand">
                                    <Columns>
                                        <c1h:C1ResourceBoundColumn DataField="Vehiculo" SortExpression="Vehiculo" ResourceName="Entities" VariableName="PARENTI03" />
                                        <c1h:C1ResourceBoundColumn DataField="Tipo" SortExpression="Tipo" ResourceName="Labels" VariableName="TYPE" />
                                        <c1h:C1ResourceBoundColumn DataField="Codigo" SortExpression="Codigo" ResourceName="Labels" VariableName="CODE"  Aggregate="Count"/>
                                        <c1h:C1ResourceBoundColumn DataField="Inicio" SortExpression="Inicio" ResourceName="Labels" VariableName="DATE" DataFormatString = "{0:d}" />
                                        <c1h:C1ResourceBoundColumn DataField="Paradas" SortExpression="Paradas" ResourceName="Labels" VariableName="ENTREGAS" Aggregate="Sum" DataFormatString="{0:0}" />
                                        <c1h:C1ResourceBoundColumn DataField="KmControlado" SortExpression="KmControlado" ResourceName="Labels" VariableName="Km Controlado" Aggregate="Sum" DataFormatString="{0:0.00} km"  />
                                        <c1:C1TemplateField>
                                            <ItemStyle Width="200px"></ItemStyle>
                                            <ItemTemplate>
                                                <cwc:ResourceLinkButton runat="server" CommandName="Modificar" ResourceName="Labels" VariableName="Modificar" />
                                            </ItemTemplate>
                                        </c1:C1TemplateField>
                                    </Columns>
                                </c1:C1GridView>
                        
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
            </AjaxToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function checkAll(el) {
            var parent = el;
            for (var i = 0; i < 8; i++) parent = parent.parentNode;
            var inputs = parent.getElementsByTagName('input');
            for(var i = 0; i < inputs.length; i++) {
                var input = inputs[i];
                if(input.type == 'checkbox') {
                    input.checked = el.checked;
                }
            }
        }

        function calcularTotal(el) {
            var parent = el;
            for (var i = 0; i < 8; i++) parent = parent.parentNode;
            var inputs = parent.getElementsByTagName('input');
            var total = 0.0;
            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];
                if (input.type == 'text' && input.id.indexOf('txtKmControlado') > -1) {
                    total += parseFloat(input.value.replace(",","."));
                }
            }
            $get('kmTotal').innerHTML = total + " km";
        }
    </script>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPrint" Runat="Server">
    <asp:UpdatePanel runat="server" ID="upPrint" UpdateMode="Conditional">
        <ContentTemplate>
            <c1:C1GridView runat="server" ID="gridPrint" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

