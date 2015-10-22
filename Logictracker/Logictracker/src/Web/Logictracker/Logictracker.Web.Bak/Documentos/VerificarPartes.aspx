<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="VerificarPartes.aspx.cs" Inherits="Logictracker.Documentos.Documentos_VerificarPartes" Title="Untitled Page" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
.grid_container
{
     margin: auto; 
     width: 820px; 
     border: solid 1px #E1A500;
     margin-bottom: 10px;
     background-color: #FCE091;
}
</style>
<asp:Panel ID="panel" runat="server" GroupingText="Buscar Partes" Width="700px" style="margin: auto;">
<table style="width: 100%; position: relative; z-index: 100;">
<tr>
    <td>
        <asp:Label runat="server" ID="lblEmpresa"  Font-Bold="true" Text="Empresa"></asp:Label>
        <br />
        <cwc:TransportistaDropDownList ID="cbAseguradora" runat="server" AutoPostBack="false"></cwc:TransportistaDropDownList>
    </td>
    <td>
        <asp:Label runat="server" ID="lblInicio"  Font-Bold="true" Text="Periodo"></asp:Label>
        <br />
        <asp:UpdatePanel ID="updPeriodo" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:DropDownList ID="cbPeriodo" runat="server" DataTextField="Descripcion" DataValueField="Id"></asp:DropDownList>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btGuardar" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </td>
    <td>
        <asp:Button ID="btBuscar" runat="server" onclick="btnSearch_Click" Text="Buscar" />
    </td>
</tr>
</table>
</asp:Panel>
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server"></cwc:ProgressLabel>
    
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server"></cwc:InfoLabel>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <ContentTemplate>
<asp:Panel ID="panelResultado" runat="server" Visible="false">
<div class="grid_container">
                <C1:C1GridView ID="gridResumen" runat="server" 
                                DataKeyNames="Id"
                                GridLines="None" 
                                AutoGenerateColumns="False" 
                                CssClass="Grid" 
                                BorderStyle="None"  
                                Width="820px" 
                                Height="300px" 
                                ScrollSettings-ScrollMode="ScrollBar"
                                ScrollSettings-ScrollOrientation="Both"
                                OnRowDataBound="gridResumen_ItemDataBound"
                                AllowColMoving="false"
                                AllowGrouping ="false"
                                AllowSorting = "false" 
                                 >
                                
                    <HeaderStyle CssClass="Grid_Header" Height="20px" Font-Bold="true" />
                    <RowStyle CssClass="Grid_Item Grid_Item_Parte"  />
                    <AlternatingRowStyle CssClass="Grid_Alternating_Item" />
                    <FooterStyle CssClass="Grid_Header" />
                <Columns>
                    <c1h:C1ResourceGroupColumn DataField="Equipo" SortDirection="Ascending" Aggregate="Custom" HeaderText="{1}: {0}"/>
                    <C1:C1BoundField DataField="Interno" HeaderText="Vehiculo" >
                        <ItemStyle Width="80px" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="Codigo" HeaderText="Parte" >
                        <ItemStyle Width="80px" />
                    </C1:C1BoundField>                    
                    <C1:C1BoundField DataField="Horas" HeaderText="Horas" >
                        <ItemStyle Width="200px" Font-Size="XX-Small" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="KmTotal" HeaderText="Km" >
                        <ItemStyle Width="140px" Font-Size="XX-Small" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="Importe" HeaderText="Importe" DataFormatString="${0:0.00}" >
                        <ItemStyle Width="200px" HorizontalAlign="Right" Font-Size="XX-Small" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1TemplateField HeaderText="">
                        <ItemTemplate> 
                            <asp:CheckBox ID="chkVerificar" runat="server" />
                        </ItemTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                    </C1:C1TemplateField>
                </Columns>
                           
                </C1:C1GridView>  
                <div style="text-align: right;">
                    <asp:Button ID="btGuardar" runat="server" Text="Guardar" OnClick="btGuardar_Click" Visible="false" />
                </div>
                </div>
</asp:Panel>
</ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

