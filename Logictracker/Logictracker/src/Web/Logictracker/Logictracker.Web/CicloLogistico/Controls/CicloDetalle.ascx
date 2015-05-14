<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CicloDetalle.ascx.cs" Inherits="Logictracker.CicloLogistico.Controls.ControlDetalleCiclo" ClassName="ControlDetalleCiclo" %>

<table style="width: 100%;" cellpadding="0"><tr>
    <td style="background-color: #CCCCCC; padding: 3px; text-align:center; border: solid 1px #999999; width: 20px;"><asp:Label ID="lblOrden" runat="server" Text="1" /></td>
    <td style="border: solid 1px #999999">
        <table style="width: 100%;" cellspacing="0" >
        <tr><td style="background-color: #DDDDDD; vertical-align:top; width: 33%;">
        
            <div style="font-size: x-small; font-weight: bold; padding: 3px; background-color: #CCCCCC;">Datos Generales</div>
        
            <table style="width: 100%;padding: 5px;"><tr><td style="width: 60px;">
            
            <asp:Textbox ID="txtCodigo" runat="server" MaxLength="32" Width="100%" style="font-weight: bold;" />
            
            <AjaxToolkit:TextBoxWatermarkExtender ID="tweCodigo" runat="server" TargetControlID="txtCodigo" WatermarkText="Codigo" WatermarkCssClass="LogicWatermark" />
            
            </td><td style="text-align: right;">
            
            Duracion <asp:Textbox ID="txtDuracion" runat="server" MaxLength="3" Width="25px" /> min            
            
            </td></tr><tr><td colspan="2">
            
            <asp:Textbox ID="txtDescripcion" runat="server" MaxLength="128" Width="98%" style="font-weight: bold;" />
            
            <AjaxToolkit:TextBoxWatermarkExtender ID="tweDescripcion" runat="server" TargetControlID="txtDescripcion" WatermarkText="Descripcion" WatermarkCssClass="LogicWatermark" />            
            
            </td></tr><tr><td colspan="2">
            
            <asp:CheckBox ID="chkRepite" runat="server" Text="Repite" />
            |
            <asp:CheckBox ID="chkObligatorio" runat="server" Text="Obligatorio" />
            
            </td></tr></table>
            
        </td>
        
        <td style=" vertical-align:top; width: 33%;">
            <div style="font-size: x-small; font-weight: bold; padding: 3px; background-color: #CCCCCC;">Evento</div>
        
            <table style="width: 100%;padding: 5px;"><tr>
                <td>
                    Tipo
                </td><td style="text-align: right;">
                    <asp:DropDownList ID="cbTipo" runat="server" OnSelectedIndexChanged="cbTipo_SelectedIndexChanged" AutoPostBack="true" Width="200px" />
                </td></tr><tr><td>
    
                    <asp:UpdatePanel ID="updEvento" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>                        
                            <asp:MultiView ID="multiEvento" runat="server" ActiveViewIndex="0">
                                <asp:View ID="viewTiempo" runat="server">
                                    </td><td><asp:Textbox ID="txtMinutos" runat="server" MaxLength="3" Width="25px" /> minutos
                                </asp:View>
                                <asp:View ID="viewMensaje" runat="server">
                                    Evento
                                    </td><td style="text-align: right;">
                                    <asp:DropDownList ID="cbTipoEvento" runat="server" Width="200px" AutoPostBack="true" 
                                            onselectedindexchanged="cbTipoEvento_SelectedIndexChanged" />
                                    <br />
                                    <asp:DropDownList ID="cbEventos" runat="server" Width="200px" />
                                </asp:View>
                                <asp:View ID="viewGeoRef" runat="server">
                                    Geo-Referencia
                                    </td><td style="text-align: right;">
                                    <asp:DropDownList ID="cbTipoReferenciaGeografica" runat="server" Width="200px" 
                                            AutoBind="false"  AutoPostBack="true" 
                                            onselectedindexchanged="cbTipoReferenciaGeografica_SelectedIndexChanged" />
                                    <br />
                                    <asp:DropDownList ID="cbReferenciaGeografica" runat="server" Width="200px" />
                                </asp:View>
                                <asp:View ID="viewCiclo" runat="server">
                                    Ciclo
                                    </td><td style="text-align: right;">
                                    <asp:DropDownList ID="cbCicloLogistico" runat="server" Width="200px" />
                                </asp:View>
                            </asp:MultiView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr></table>
        </td>
            
        <td style="background-color: #DDDDDD; vertical-align:top; width: 33%;">
            <div style="font-size: x-small; font-weight: bold; padding: 3px; background-color: #CCCCCC;">Control Manual <span style="position: absolute;"><asp:Checkbox ID="chkControlManual" runat="server" AutoPostBack="true" OnCheckedChanged="chkControlManual_CheckedChanged" style="position: relative; bottom: 3px;" /></span></div>
            <table style="width: 100%;padding: 5px;"><tr>
                <td>
                    <asp:UpdatePanel ID="updMensajes" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:DropDownList ID="cbTipoMensaje" runat="server" Width="200px" 
                                Enabled="false" AutoPostBack="True" 
                                onselectedindexchanged="cbTipoMensaje_SelectedIndexChanged" /><br />
                            <asp:DropDownList ID="cbMensajes" runat="server" Width="200px" Enabled="false" /><br />
                            <asp:CheckBox ID="chkControlManualObligatorio" runat="server" Text="Obligatorio" Enabled="false" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr></table>
            
        </td>
        
        </tr>
        </table>
    </td>
        <td style="background-color: #CCCCCC; padding: 3px; text-align:center; border: solid 1px #999999; width: 20px;"><asp:LinkButton ID="btEliminar" runat="server" Text="X" OnClick="btEliminar_Click" OnClientClick="return confirm('¿Desea eliminar este elemento?');"></asp:LinkButton></td>
</tr></table>