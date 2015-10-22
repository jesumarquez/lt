 <%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="PerfilAlta.aspx.cs"  Inherits="Logictracker.Organizacion.AltaPerfil" Title="Perfiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">

    <style type="text/css">
        li{list-style: none; padding: 0px; margin: 0px;}
        ul{padding: 0px;margin: 0px;}
    </style>
    
    <script type="text/javascript">
        function selCheck(name) { $get(name).checked = !$get(name).checked; }
        function checkAll(parent) { 
            var childs = parent.getElementsByTagName('input');
            for(var i = 0; i < childs.length; i++)
            {
                if(childs[i].type != 'checkbox') continue;
                childs[i].checked = !childs[i].checked; 
            }
        }
    </script>
    
    <cwc:AbmTabPanel ID="tabGeneral" runat="server"  ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 50%">
            <tr>
                <td style="width: 50%; text-align: right;">
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                </td>
                <td style="width: 50%">
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="255" />
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>

    <cwc:AbmTabPanel ID="tabFunciones" runat="server" Title="Asignación de Funciones">

        <asp:UpdatePanel ID="updHack" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="panReorder" runat="server" CssClass="reorder_panel"></asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <AjaxToolkit:ReorderList ID="ReorderList1" runat="server" AllowReorder="true" DragHandleAlignment="Left" SortOrderField="Orden" Visible="false">
            <DragHandleTemplate>
                <div class="reorder_handle"></div>
            </DragHandleTemplate>
            <ItemTemplate>
                <table class="reorder_item">
                    <tr>
                        <td>
                            <asp:HiddenField ID="hidIdFuncion" runat="server" Value="<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Funcion.Id %>" />
                            <span onclick="checkAll(this.parentNode.parentNode);" style="cursor: pointer;">
                                <table>
                                    <tr>
                                        <td style="width: 24px;">
                                            <img src='<%# ResolveUrl("~/" + (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Funcion.Descripcion) + ".image" %>' />
                                        </td>
                                        <td>
                                            <%# Logictracker.Culture.CultureManager.GetMenu((Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Funcion.Descripcion)%>
                                        </td>
                                    </tr>
                                </table>
                            </span>
                        </td>
                        <td style='width: 50px;'><asp:CheckBox ID="chkAlta" runat="server" Checked='<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Alta %>' /></td>
                        <td style='width: 50px;'><asp:CheckBox ID="chkMod" runat="server" Checked='<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Modificacion %>' /></td>
                        <td style='width: 50px;'><asp:CheckBox ID="chkBaja" runat="server" Checked='<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Baja %>' /></td>
                        <td style='width: 50px;'><asp:CheckBox ID="chkConsulta" runat="server" Checked='<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Consulta %>' /></td>
                        <td style='width: 50px;'><asp:CheckBox ID="chkImprimir" runat="server" Checked='<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).Reporte %>' /></td>
                        <td style='width: 50px;'><asp:CheckBox ID="chkMapa" runat="server" Checked='<%# (Container.DataItem as Logictracker.Types.BusinessObjects.MovMenu).VerMapa %>' /></td>
                    </tr>
                </table>
            </ItemTemplate>
        </AjaxToolkit:ReorderList> 
    
    </cwc:AbmTabPanel>

    <cwc:AbmTabPanel ID="tabPermisos" runat="server" Title="Permisos">
     
        <asp:CheckBoxList ID="chkPermisos" runat="server" />
     
    </cwc:AbmTabPanel>
</asp:Content>
