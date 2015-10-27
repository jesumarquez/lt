<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.Dispositivos.ParametroDispositivosAlta" Codebehind="ParametroDispositivos.aspx.cs" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
<table style="width:100%;">
    <tr>
        <td style="width:200px; vertical-align: top;">
            <cwc:TitledPanel ID="panelDispositivo" runat="server" TitleResourceName="Entities" TitleVariableName="PARENTI08">
                <cwc:TipoDispositivoDropDownList ID="cbTipoDispositivo" runat="server" Width="100%" OnSelectedIndexChanged="cbTipoDispositivo_SelectedIndexChanged" />
                <br />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" ParentControls="cbEmpresa" Width="100%" AddAllItem="true" />
                <br />
                <br />
                <cwc:DispositivoListBox ID="cbDispositivo" runat="server" ParentControls="cbLinea,cbTipoDispositivo" SelectionMode="Multiple" Width="100%" Height="300px" AutoPostBack="true" OnSelectedIndexChanged="cbDispositivo_SelectedIndexChanged" />            
            </cwc:TitledPanel>
        </td>
        <td style="vertical-align: top;">
            <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleResourceName="Labels" TitleVariableName="DETALLES">
                <asp:UpdatePanel ID="updDetalles" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <C1:C1GridView ID="grid" runat="server" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id" OnRowDataBound="grid_RowDataBound">
                            <Columns>
                                
                                <c1h:C1ResourceTemplateColumn AllowGroup="false" AllowMove="false" AllowSizing="false">
                                    <HeaderTemplate><input type="checkbox" onchange="checkAll(this);"/></HeaderTemplate>
                                    <ItemTemplate><asp:CheckBox ID="chkEdit" runat="server" Width="100%"></asp:CheckBox></ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                
                                <c1h:C1ResourceBoundColumn DataField="Nombre" ResourceName="Labels" VariableName="PARAMETRO" SortExpression="Parametro"  AllowGroup="false" AllowMove="false" AllowSizing="false" />
                                
                                <c1h:C1ResourceBoundColumn DataField="Descripcion" ResourceName="Labels" VariableName="DESCRIPCION" SortExpression="Descripcion"  AllowGroup="false" AllowMove="false" AllowSizing="false" />
                                
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR" SortExpression="Valor" AllowGroup="false" AllowMove="false" AllowSizing="false">
                                    <ItemTemplate><asp:TextBox ID="txtValor" runat="server" Width="100%"></asp:TextBox></ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                
                                <c1h:C1ResourceBoundColumn DataField="TipoDato" ResourceName="Labels" VariableName="TYPE" SortExpression="Tipo" AllowGroup="false" AllowMove="false" AllowSizing="false" ><ItemStyle Width="50px" /></c1h:C1ResourceBoundColumn>                
                                
                            </Columns>
                        </C1:C1GridView> 
                    </ContentTemplate>
                </asp:UpdatePanel>                
            </cwc:TitledPanel>
        </td>
    </tr>
</table>
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
</script>
</asp:Content>

