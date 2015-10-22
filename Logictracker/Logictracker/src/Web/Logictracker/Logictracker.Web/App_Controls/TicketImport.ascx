<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TicketImport.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_TicketImport" %>

<asp:Panel ID="panelUpload" runat="server" DefaultButton="btUpload" CssClass="filterpanel">
    <table style="width: 100%">
        <tr>
            <td>Seleccione el archivo .xls (Excel 97-2003)</td>
            <td><asp:FileUpload ID="filExcel" runat="server" /></td>
            <td><asp:Button ID="btUpload" runat="server" Text="Cargar Archivo" OnClick="BtUploadClick" /></td>
        </tr>
    </table>
</asp:Panel>

<asp:UpdatePanel ID="updProcess" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <asp:Panel ID="panelProcess" runat="server" DefaultButton="btProcess" Visible="false" CssClass="filterpanel" style="background-color: #E7E7E7">

            <table style="width: 100%"><tr>
            <td colspan="2" class="panelheader"><b>Archivo: <asp:Label ID="lblFileName" runat="server"></asp:Label></b></td>
            </tr><tr><td style="width: 50%; vertical-align: top;">
            
                <table style="width: 100%">                
                    <tr>
                        <td colspan="2"><b>Importar en</b></td>
                    </tr>
                    <tr>
                        <td><cwc:ResourceLabel ID="lblEmpresaTickets" runat="server" ResourceName="Entities" VariableName="PARENTI01" /></td>
                        <td><cwc:LocacionDropDownList ID="ddlEmpresaTickets" runat="server" Width="175px" /></td>
                    </tr>                    
                    <tr>
                        <td><cwc:ResourceLabel ID="lblBaseTickets" runat="server" ResourceName="Entities" VariableName="PARENTI02" /></td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="upBaseTickets" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:PlantaDropDownList ID="ddlBaseTickets" runat="server" Width="175px" ParentControls="ddlEmpresaTickets" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlEmpresaTickets" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>            
                    </tr>
                    <tr>
                        <td><cwc:ResourceLabel ID="lblGeoRefCli" ResourceName="Labels" VariableName="TIPO_GEO_REF_CLI" runat="server" /></td>
                        <td>
                            <asp:UpdatePanel ID="updGeoRefCli" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoGeoRefCli" runat="server" ParentControls="ddlBaseTickets" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlBaseTickets" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td><cwc:ResourceLabel ID="lblGeoRefPoint" ResourceName="Labels" VariableName="TIPO_GEO_REF_POI" runat="server" /></td>
                        <td>
                            <asp:UpdatePanel ID="updGeoRefPoint" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoGeoRefPtoInteres" runat="server" ParentControls="ddlBaseTickets" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlBaseTickets" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                
            </td><td style="width: 50%; vertical-align: top;">   
 
                <table style="width: 100%">
                <tr>
                        <td>Hoja de trabajo</td>
                        <td><asp:DropDownList ID="cbWorksheets" runat="server" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="CbWorksheetsSelectedIndexChanged" /></td>
                    </tr>
                    <tr><td colspan="2">&nbsp;</td></tr>
                    <tr>
                        <td colspan="2"><b>Seleccione las columnas que desea procesar</b></td>
                    </tr>
                    <tr>
                        <td>Codigo Cliente</td>
                        <td><asp:DropDownList ID="cbCodigoCliente" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Descripcion Cliente</td>
                        <td><asp:DropDownList ID="cbDescripcionCliente" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Direccion Cliente</td>
                        <td><asp:DropDownList ID="cbDireccionCliente" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Localidad Cliente</td>
                        <td><asp:DropDownList ID="cbLocalidadCliente" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Provincia Cliente</td>
                        <td><asp:DropDownList ID="cbProvinciaCliente" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Telefono Cliente</td>
                        <td><asp:DropDownList ID="cbTelefonoCliente" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Codigo Pto de Entrega</td>
                        <td><asp:DropDownList ID="cbCodigoPuntoEntrega" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Descripcion Pto de Entrega</td>
                        <td><asp:DropDownList ID="cbDescripcionPuntoEntrega" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Direccion Pto de Entrega</td>
                        <td><asp:DropDownList ID="cbDireccionPuntoEntrega" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Localidad Pto de Entrega</td>
                        <td><asp:DropDownList ID="cbLocalidadPuntoEntrega" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Codigo Ticket</td>
                        <td><asp:DropDownList ID="cbCodigoTicket" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Fecha</td>
                        <td><asp:DropDownList ID="cbFecha" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Cantidad Pedido</td>
                        <td><asp:DropDownList ID="cbCantidadPedido" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Cantidad Acumulada</td>
                        <td><asp:DropDownList ID="cbCantidadAcumulada" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Unidad</td>
                        <td><asp:DropDownList ID="cbUnidad" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Codigo Producto</td>
                        <td><asp:DropDownList ID="cbCodigoProducto" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Comentario 1</td>
                        <td><asp:DropDownList ID="cbComentario1" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Comentario 2</td>
                        <td><asp:DropDownList ID="cbComentario2" runat="server" Width="200px" /></td>
                    </tr>
                    <tr>
                        <td>Comentario 3</td>
                        <td><asp:DropDownList ID="cbComentario3" runat="server" Width="200px" /></td>
                    </tr>
                </table>
            </td></tr></table>

    <br />
    <div style="text-align: right;">
        <cwc:ResourceButton ID="btProcess" runat="server" Width="85px" ResourceName="Controls"
            OnClick="BtProcessClick" VariableName="BUTTON_IMPORT" />
    </div>
    
    </asp:Panel>
    
    </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btUpload" />
            <asp:AsyncPostBackTrigger ControlID="cbWorksheets" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
<br />

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Panel ID="panelProgress" runat="server" Visible="false" CssClass="filterpanel" style="background-color: #E7E7E7">
        <table style="width:100%">
            <tr>
                <td style="width:250px; vertical-align: top;">
                    <asp:Label ID="lblDirs" runat="server"></asp:Label>
                    <br />
                    <asp:Literal ID="litProgress" runat="server"></asp:Literal>
                </td>
                <td>
                    <div style="border: solid 1px black; height: 300px; overflow: auto;">
                        <asp:Label ID="lblResult" runat="server"></asp:Label>
                    </div>
                </td>
        </tr>
        </table>
        </asp:Panel>
        </ContentTemplate>
        <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btProcess" EventName="Click" />
  </Triggers>
</asp:UpdatePanel>
                    