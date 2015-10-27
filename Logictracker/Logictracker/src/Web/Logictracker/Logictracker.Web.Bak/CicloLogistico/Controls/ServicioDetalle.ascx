<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ServicioDetalle.ascx.cs" Inherits="Logictracker.CicloLogistico.Controls.ControlDetalleServicio" ClassName="ControlDetalleServicio" %>

<asp:UpdatePanel ID="updControl" runat="server" ChildrenAsTriggers="true">
<ContentTemplate>
        <table style="width: 100%;" cellpadding="0"><tr>
            <% if (EsEstado)
               {%>
            <td style="background-color: #CCCCCC; padding: 3px; text-align:center; border: solid 1px #999999; width: 20px;">
                <asp:Label ID="lblOrden" runat="server" Text="1" />
            </td>
            <%
               }%>
            <td style="border: solid 1px #999999">
                <asp:MultiView ID="multiTipoEstado" runat="server" ActiveViewIndex="0">
                <asp:View ID="viewTipoEstadoNormal" runat="server">
                <table style="width: 100%;" cellspacing="0" >
                    <tr>
                        <td style="background-color: #DDDDDD; ">
                            <table style="width: 100%;padding: 5px;">
                                <tr><td style="width: 100%;">         
                                        <asp:Label ID="lblCodigo" runat="server" style="font-weight: bold;"></asp:Label>
                                        |
                                        <asp:Label ID="lblDescripcion" runat="server" style="font-weight: bold;"></asp:Label>        
                
                                </td></tr>
                                <tr><td>
                                        <asp:CheckBox ID="chkObligatorio" runat="server" Text="Obligatorio" TabIndex="100" />
                                </td></tr>
                            </table>
                        </td>
                        
                        <td style=" vertical-align:top; width: 400px;">
                            <table style="width: 100%;padding: 5px;"><tr>
                                <td style="font-size: x-small; font-weight: bold; padding: 3px;">
                                    <asp:Label ID="lblTipo" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right;">   
                                    <asp:UpdatePanel ID="updEvento" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>                        
                                            <asp:MultiView ID="multiEvento" runat="server" ActiveViewIndex="0">
                                                <asp:View ID="viewTiempo" runat="server">
                                                    <asp:Textbox ID="txtMinutos" runat="server" MaxLength="3" Width="25px" TabIndex="100" /> minutos
                                                </asp:View>
                                                <asp:View ID="viewMensaje" runat="server">
                                                    <asp:DropDownList ID="cbTipoEvento" runat="server" Width="200px" 
                                                        AutoPostBack="true" TabIndex="100" 
                                                        onselectedindexchanged="cbTipoEvento_SelectedIndexChanged" />
                                                    <br />
                                                    <asp:DropDownList ID="cbEventos" runat="server" Width="200px" TabIndex="100" />
                                                </asp:View>
                                                <asp:View ID="viewGeoRef" runat="server">
                                                    <asp:DropDownList ID="cbTipoReferenciaGeografica" runat="server" Width="200px" 
                                                        AutoPostBack="true" TabIndex="100" 
                                                        onselectedindexchanged="cbTipoReferenciaGeografica_SelectedIndexChanged" />
                                                    <br />
                                                    <asp:DropDownList ID="cbReferenciaGeografica" runat="server" Width="200px" TabIndex="100" />
                                                </asp:View>
                                            </asp:MultiView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr></table>
                        </td>
            
                        <td style="background-color: #DDDDDD; vertical-align:top; width: 200px;">
                            <asp:Panel ID="panelControlManual" runat="server">
                                <div style="font-size: x-small; font-weight: bold; padding: 3px; background-color: #CCCCCC;">
                                    <asp:Label ID="lblControlManual" runat="server" Text="Control Manual"></asp:Label>
                                </div>
                                <table style="width: 100%;padding: 5px;"><tr>
                                    <td>
                                        <asp:Label ID="lblMensaje" runat="server" style="font-weight: bold;"></asp:Label><br />
                                        <asp:Label ID="lblObligatorioControl" runat="server" Text="(Obligatorio)"></asp:Label>
                                    </td>
                                </tr></table>
                            </asp:Panel>
                        </td>
        
                        <td style="width: 100px; text-align: center;">
                                    <cwc:DateTimePicker ID="dtProgramada" runat="server" Mode="Time" IsValidEmpty="false" SelectedTime="00:00" AutoPostBack="true" OnDateChanged="dtProgramada_DateChanged" />
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate></ContentTemplate>
                                <Triggers> <asp:AsyncPostBackTrigger ControlID="dtProgramada" /></Triggers>
                            </asp:UpdatePanel>
                        </td>
        
                    </tr>
                </table>
            </asp:View>
            <asp:View ID="viewTipoEstadoCiclo" runat="server">
                <asp:UpdatePanel ID="updEstados" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="panelEstados" runat="server">
                    </asp:Panel>
                </ContentTemplate>
                </asp:UpdatePanel>
            </asp:View>
            </asp:MultiView>
            
            </td>
        </tr></table>
        <% if (EsEstado)
               {%>
        <asp:Panel ID="panelNuevo" runat="server" style="background-color: #CCCCCC; border: solid 1px #999999; padding: 5px;margin: 2px;">
            <asp:LinkButton ID="btNuevo" runat="server" OnClick="btNuevo_Click"></asp:LinkButton>
        </asp:Panel>
        <%
               }%>
</ContentTemplate>
</asp:UpdatePanel>

