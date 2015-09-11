<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Soporte.SoporteTicketSoporteAlta" ValidateRequest="false" Codebehind="TicketSoporteAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    <%--TOOLBAR--%>
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_TICKET" >    
        <table id="tbTipoPunto" style="width: 90%; margin: auto; margin-top: 30px;" cellpadding="5">
            <tr>
                <td colspan="2">
                    <asp:Panel ID="panelResolucion" runat="server" Visible="false" CssClass="abmpanel">
                        <table>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblResolve" runat="server" ResourceName="Labels" VariableName="SUPPORT_RESOLUTION" />
                                </td>
                                <td>
                                    <asp:DropDownList ID="cbResolucion" runat="server" Width="200px" />
                                </td>
                            </tr>
                        </table>  
                    </asp:Panel> 
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 50%;">
                    <%--COLUMNA IZQ--%>
                    <cwc:TitledPanel ID="titPanelIncidencia" runat="server" Title="Incidencia" Height="330px">
                        <asp:UpdatePanel ID="updProblema" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                        </td>
                                        <td>
                                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="180px" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="NIVEL" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="upNivel" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:NivelDropDownList ID="cbNivel" runat="server" Width="180px" ParentControls="cbEmpresa" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />                            
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="TYPE" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="cbTipoProblema" runat="server" Width="180px" AutoPostBack="true" OnSelectedIndexChanged="CbTipoProblemaSelectedIndexChanged" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="updVehiculo" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="180px" ParentControls="cbEmpresa" AddNoneItem="true" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />                            
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="AUDSUP03" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="upCategoria" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:CategoriaDropDownList ID="cbCategoria" runat="server" Width="180px" ParentControls="cbEmpresa" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>                                            
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel17" runat="server" ResourceName="Entities" VariableName="PARENTI08" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:DispositivoDropDownList ID="cbDispositivo" runat="server" Width="180px" ParentControls="cbEmpresa" AddNoneItem="true" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Entities" VariableName="AUDSUP04" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="upSubcategoria" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:SubcategoriaDropDownList ID="cbSubcategoria" runat="server" Width="180px" ParentControls="cbEmpresa,cbCategoria" AddNoneItem="true" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="cbCategoria" EventName="SelectedIndexChanged" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="SUPPORT_NRO_PARTE" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNroParte" runat="server" Width="180px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                        </td>
                                        <td colspan="3">               
                                            <asp:TextBox ID="txtDescripcion" runat="server" Rows="6" TextMode="MultiLine" Width="98%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="FECHA" />
                                        </td>
                                        <td colspan="3">
                                            <asp:UpdatePanel ID="updInicio" runat="server" >
                                                <ContentTemplate>
                                                    <cwc:DateTimePicker ID="dtInicio" runat="server" IsValidEmpty="false" Mode="DateTime" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="UPLOAD_FILE" />
                                        </td>
                                        <td>
                                            <asp:FileUpload ID="filUpload" runat="server" />
                                        </td>
                                        <td colspan="2">     
                                            <asp:Panel ID="panelCambioEstado" runat="server" Visible="false">
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="ESTADO" />
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="cbEstado" runat="server" Width="200px" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbTipoProblema" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </cwc:TitledPanel>
                </td>
                <td style="vertical-align: top; width: 50%;">
                    <%--COLUMNA DER--%>
                    <asp:UpdatePanel ID="upGridEstados" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TitledPanel ID="TitledPanel1" runat="server"  Title="Historia" Height="300px" ScrollBars="Auto">
                                <C1:C1GridView ID="gridEstados" DataKeyField="Id" runat="server" Width="100%" CellPadding="10" 
                                GridLines="Horizontal" OnRowDataBound="GridEstadosItemDataBound" AutoGenerateColumns="False" SkinID="SmallGrid">
                                    <Columns>
                                        <c1h:C1ResourceBoundColumn DataField="Fecha" ResourceName="Labels" VariableName="FECHA" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                        <c1h:C1ResourceBoundColumn DataField="Estado" ResourceName="Labels" VariableName="STATE" />
                                        <c1h:C1ResourceBoundColumn DataField="Usuario" ResourceName="Entities" VariableName="USUARIO"  />
                                        <c1h:C1ResourceTemplateColumn HeaderText="Duracion" >
                                            <ItemStyle HorizontalAlign="Right" />
                                            <HeaderStyle HorizontalAlign="Right" />
                                        </c1h:C1ResourceTemplateColumn>
                                    </Columns>
                                </C1:C1GridView>
                            </cwc:TitledPanel>
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <cwc:TitledPanel ID="panGeneral" runat="server"  Title="Contacto" Height="220px">
                        <table style="width: 100%">
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="NOMBRE" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNombre" runat="server" Width="98%" MaxLength="255" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="TELEFONO" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtTelefono" runat="server" Width="98" MaxLength="50" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="MAIL" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMail" runat="server" Width="98%" MaxLength="128" />
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="SUPPORT_INFORM" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:TextBox ID="txtMailList" runat="server" Width="98%" MaxLength="128" TextMode="MultiLine" Rows="2" />
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
                <td>
                    <cwc:TitledPanel ID="panelTiempos" runat="server" Title="Tiempos" Height="188px" ScrollBars="Auto">
                        <asp:Literal ID="litTiempos" runat="server" />
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table style="width:100%; padding: 0px; border-spacing: 0px;">
                        <tr>
                            <td style="width: 75%; vertical-align: top;">
                                <asp:UpdatePanel ID="upComentarios" runat="server" UpdateMode="Conditional" >
                                    <ContentTemplate>
                                        <cwc:TitledPanel ID="TitledPanel4" runat="server" Title="Comentarios" Height="220px" >
                                            <table style="width: 100%;">
                                                <asp:Repeater ID="repChat" runat="server" OnItemDataBound="RepChatItemDataBound">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style="vertical-align: top; width: 160px; padding: 3px; background-color: #f7f7ff;">
                                                                <asp:Label ID="lblFecha" runat="server" />
                                                                <br />
                                                                <b><asp:Label ID="lblUsuario" runat="server" /></b>
                                                            </td>
                                                            <td style="padding: 10px;background-color: #f7f7ff;">
                                                                <asp:Label ID="lblDescripcion" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                            <hr />
                                            <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="SUPPORT_ADD_COMMENT" />
                                            <br />
                                            <asp:TextBox ID="txtComentario" runat="server" Rows="6" TextMode="MultiLine" Width="98%" />                        
                                        </cwc:TitledPanel> 
                                    </ContentTemplate>
                                </asp:UpdatePanel> 
                            </td>
                            <td style="width: 10px;">
                            </td>
                            <td style=" vertical-align: top;">
                                <cwc:TitledPanel ID="TitledPanel5" runat="server" Title="Archivos" Height="188px" ScrollBars="Auto">
                                    <asp:Literal ID="litFiles" runat="server" />
                                </cwc:TitledPanel>
                            </td>
                        </tr>
                    </table>      
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>

