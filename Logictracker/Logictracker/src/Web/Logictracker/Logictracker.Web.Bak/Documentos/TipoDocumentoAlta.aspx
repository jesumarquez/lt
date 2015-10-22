<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TipoDocumentoAlta.aspx.cs" Inherits="Logictracker.Documentos.Documentos_TipoDocumentoAlta" Title="Untitled Page" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Import Namespace="Logictracker.Culture" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">  

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >  
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 100%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels">

    <cwc:ResourceLabel ID="lblDistritto" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="false" />

    <cwc:ResourceLabel ID="lblBase" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AddAllItem="true" ParentControls="cbEmpresa" />
    
    <cwc:ResourceLabel ID="lblNombre" runat="server" ResourceName="Labels" VariableName="NOMBRE" Width="100%" />
    <asp:TextBox ID="txtNombre" runat="server" Width="100%"></asp:TextBox>
   
    <cwc:ResourceLabel ID="lblDescri" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" Width="100%" />
    <asp:TextBox ID="txtDescripcion" runat="server" Width="100%"></asp:TextBox>

    <cwc:ResourceLabel ID="lblParenti03" runat="server" ResourceName="Labels" VariableName="APLICAR_A_PARENTI03"></cwc:ResourceLabel>
    <asp:CheckBox id="chkVehiculo" runat="server" />

    <cwc:ResourceLabel ID="lblParenti09" runat="server" ResourceName="Labels" VariableName="APLICAR_A_PARENTI09"></cwc:ResourceLabel>
    <asp:CheckBox id="chkEmpleado" runat="server" />

    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="APLICAR_A_PARENTI17"></cwc:ResourceLabel>
    <asp:CheckBox id="chkTransportista" runat="server" />

    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="APLICAR_A_PARENTI19"></cwc:ResourceLabel>
    <asp:CheckBox id="chkEquipo" runat="server" />

    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="FECHA_VENCIMIENTO"></cwc:ResourceLabel>
    <asp:CheckBox id="chkVencimiento" runat="server" />
    
    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="FECHA_PRESENTACION_OBLIGATORIA"></cwc:ResourceLabel>
    <asp:CheckBox id="chkPresentacion" runat="server" />
    
    <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="CONTROLA_CONSUMOS"></cwc:ResourceLabel>
    <asp:CheckBox id="chkControlaConsumo" runat="server" />

    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="PRIMER_AVISO"></cwc:ResourceLabel>       
    <div id="Div1" runat="server">
        <asp:TextBox id="txtPrimerAviso" runat="server" Width="40px" MaxLength="3" Text="60" />
        <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="DIAS_ANTES_DEL_VENCIMIENTO"></cwc:ResourceLabel>
    </div>
    
    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="SEGUNDO_AVISO"></cwc:ResourceLabel>
    <div runat="server">
        <asp:TextBox id="txtSegundoAviso" runat="server" Width="40px" MaxLength="3" Text="30" />
        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="DIAS_ANTES_DEL_VENCIMIENTO"></cwc:ResourceLabel>
    </div>
    
    </cwc:AbmTitledPanel></td></tr></table>
</cwc:AbmTabPanel>   
    
    
<cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="PARAMETROS" >  
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 100%; vertical-align: top;">

    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="PARAMETROS" TitleResourceName="Labels">
    <asp:UpdatePanel ID="updParams" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <C1:C1GridView ID="gridParametros" runat="server" UseEmbeddedVisualStyles="false" SkinId="SmallGrid"
                DataKeyNames="Id" AllowColMoving="false" AllowGrouping="false" AllowSorting="false"
                OnRowDataBound="gridParametros_ItemDataBound"
                OnRowCommand="gridParametros_ItemCommand">
                <Columns>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_PARAMETRO">
                        <ItemTemplate>
                            <asp:TextBox ID="txtNombreParam" runat="server" Width="150px"  Font-Bold="true"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle Width="150px" />
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_TIPODATO">
                        <ItemTemplate>
                            <asp:DropDownList ID="cbTipoParam" runat="server" Width="150px" AutoPostBack="true" OnSelectedIndexChanged="cbTipoParam_SelectedIndexChanged">
                            </asp:DropDownList>
                        </ItemTemplate>
                        <ItemStyle Width="150px" />
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_TAMANO">
                        <ItemTemplate>
                            <asp:TextBox ID="txtLargo" runat="server" Width="40px"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle Width="40px" />
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_PRECISION">
                        <ItemTemplate>
                            <asp:TextBox ID="txtPrecision" runat="server" Width="40px"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle Width="40px" />
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_OBLIGATORIO">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkObligatorio" runat="server" />
                        </ItemTemplate>
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_VALOR_DEFAULT">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefault" runat="server" Width="150px"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle Width="150px" />
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_ORDEN">
                        <ItemTemplate>
                            <asp:TextBox ID="txtOrden" runat="server" Width="50px"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle Width="50px" />
                    </c1h:C1ResourceTemplateColumn>
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPODOC_REPETIR">
                        <ItemTemplate>
                            <asp:TextBox ID="txtRepeticion" runat="server" Width="50px" Text="1"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle Width="50px" />
                    </c1h:C1ResourceTemplateColumn>
                    <C1:C1TemplateField>
                        <ItemTemplate>
                            <cwc:ResourceLinkButton id="btEliminarParam" runat="server" ResourceName="Controls" VariableName="BUTTON_DELETE" CommandName="Eliminar" OnClientClick="return ConfirmDeleteParameterTipoDoc();" />
                        </ItemTemplate>
                        <ItemStyle Width="150px" />
                    </C1:C1TemplateField>
                </Columns>    
            </C1:C1GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btNewParam" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    
    <div class="Grid_Header">
        <cwc:ResourceLinkButton ID="btNewParam" runat="server" OnClick="btNewParam_Click" ResourceName="Controls" VariableName="BUTTON_ADD_PARAMETER" />
    </div>
</cwc:AbmTitledPanel>
</td>
</tr>
</table>

</cwc:AbmTabPanel>

<script type="text/javascript">
function ConfirmDeleteParameterTipoDoc()
{
    return confirm('<%=CultureManager.GetSystemMessage("CONFIRM_DELETE_PARAM_TIPODOC") %>');
}
</script>
</asp:Content>

