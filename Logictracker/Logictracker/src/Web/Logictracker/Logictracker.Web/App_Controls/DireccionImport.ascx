<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DireccionImport.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_DireccionImport" %>

<div>
    <asp:Panel ID="panelUpload" runat="server" DefaultButton="btUpload" CssClass="filterpanel">
        <table style="width: 100%">
            <tr>
                <td>Seleccione el archivo .xls (Excel 97-2003)</td>
                <td><asp:FileUpload ID="filExcel" runat="server" /></td>
                <td><asp:Button ID="btUpload" runat="server" Text="Cargar Archivo" OnClick="BtUploadClick" /></td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:UpdatePanel ID="updProcess" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <asp:Panel ID="panelProcess" runat="server" DefaultButton="btProcess" Visible="false" CssClass="filterpanel" style="background-color: #E7E7E7">
                <table style="width: 100%">
                    <tr>
                        <td colspan="2" class="panelheader">
                            <b>Archivo: <asp:Label ID="lblFileName" runat="server"></asp:Label></b>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 50%; vertical-align: top;">
                            <table style="width: 100%">                
                                <tr>
                                    <td colspan="2">
                                        <b>Importar en</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                    </td>
                                    <td>
                                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" Width="200px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                                    </td>
                                    <td>
                                        <asp:UpdatePanel ID="updLinea" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" ParentControls="cbEmpresa" Width="200px" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI10"></cwc:ResourceLabel>
                                    </td>
                                    <td>
                                        <asp:UpdatePanel ID="updTipoGeoRef" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoGeoRef" runat="server" AddAllItem="false" ParentControls="cbLinea" Width="200px" />    
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="RADIO_METROS"></cwc:ResourceLabel>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtRadio" runat="server" Width="100px" Text="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:CheckBox ID="chkSobreescribir" runat="server" Text="Sobreescribir si existe el código" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 50%; vertical-align: top;">
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
                                    <td><cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESCRIPCION"></cwc:ResourceLabel></td>
                                    <td><asp:DropDownList ID="cbDescripcion" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="CODE"></cwc:ResourceLabel></td>
                                    <td><asp:DropDownList ID="cbCodigo" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Calle</td>
                                    <td><asp:DropDownList ID="cbCalle" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Altura</td>
                                    <td><asp:DropDownList ID="cbAltura" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Esquina</td>
                                    <td><asp:DropDownList ID="cbEsquina" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Partido</td>
                                    <td><asp:DropDownList ID="cbPartido" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Provincia</td>
                                    <td><asp:DropDownList ID="cbProvincia" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Latitud</td>
                                    <td><asp:DropDownList ID="cbLatitud" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Longitud</td>
                                    <td><asp:DropDownList ID="cbLongitud" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Vigencia Desde</td>
                                    <td><asp:DropDownList ID="cbDesde" runat="server" Width="200px" /></td>
                                </tr>
                                <tr>
                                    <td>Vigencia Hasta</td>
                                    <td><asp:DropDownList ID="cbHasta" runat="server" Width="200px" /></td>
                                </tr>
                            </table>            
                        </td>
                    </tr>
                </table>

                <br />

                <div style="text-align: right;">
                    <asp:Button ID="btProcess" runat="server" Text="Procesar" OnClick="BtProcessClick" />
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
</div>
