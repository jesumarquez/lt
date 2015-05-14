<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="Comparativo.aspx.cs" Inherits="Logictracker.Documentos.Comparativo" Title="Untitled Page" %>

<%@ Register src="../App_Controls/Pickers/DatePicker.ascx" tagname="DatePicker" tagprefix="uc1" %>

<%@ Register src="Controls/DatosParte.ascx" tagname="DatosParte" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
.tab
{
    margin: 0px;
    padding: 0px;
    list-style: none;
    position: relative;
    top: 1px;
    right: 1px;
}
.tab li
{
    text-align: center;
    padding-top: 10px;
    background-image: url(tab.gif);
    width: 117px;
    height: 21px;
    float:left;
    margin-right: 3px;
}
.tab li a
{
    color: #000000;
    font-weight: bold;
}
.tab li.tab2
{
    background-image: url(tab2.gif);
}
.tabhead
{
    border: none;
    width: 100%;
}
.panelContent
{
    padding: 10px;
    width: 845px;
    margin: auto;
}
.tabcontent
{
    background-color: #3A81B1;
    height: 350px;
    vertical-align: top;
    text-align: center;
}
.grid_container
{
     margin: auto; 
     width: 820px; 
     border: solid 1px #E1A500;
     margin-bottom: 10px;
     background-color: #FCE091;
}
.parte_pager
{
    margin-left: 17px;
    padding: 3px 10px 3px 10px;
    background-color: #FCE091;
    border: solid 1px #E1A500;
    text-align: center;
    width: 60px;
    font-weight: bold;
    font-size: 9px;
}
.Grid_Item_Parte
{
    padding-top: 5px;
    padding-bottom: 3px;
    height: auto;
}
</style>

<div style="margin: 20px;">

<asp:Panel ID="panel" runat="server" GroupingText="Buscar Partes" Width="700px" style="margin: auto;">
<table style="width: 100%; position: relative; z-index: 100;">
<tr>
    <td>
        <asp:Label runat="server" ID="lblEmpresa"  Font-Bold="true" Text="Empresa"></asp:Label>
        <br />
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <cwc:TransportistaDropDownList ID="cbAseguradora" runat="server" AutoPostBack="false"></cwc:TransportistaDropDownList>
            </ContentTemplate>
        </asp:UpdatePanel>        
    </td>
    <td>
        <asp:Label runat="server" ID="lblInicio"  Font-Bold="true" Text="Periodo"></asp:Label>
        <br />
        <asp:DropDownList ID="cbPeriodo" runat="server" DataTextField="Descripcion" DataValueField="Id">
        </asp:DropDownList>
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

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:Panel ID="panelContent" runat="server" Visible="false" CssClass="panelContent"> 
            <table class="tabhead" cellspacing="0">
                <tr>
                    <td>
                        <ul class="tab">
                            <li id="liMoviles" runat="server">
                                <asp:LinkButton ID="btViewMoviles" runat="server" 
                                                CommandArgument="0" 
                                                oncommand="btViewTab_Command">
                                    Equipos</asp:LinkButton>
                            </li>
                            <li id="liPartes" runat="server" class="tab2">
                                <asp:LinkButton ID="btViewParte" runat="server" 
                                                CommandArgument="1" 
                                                oncommand="btViewTab_Command">
                                    Partes</asp:LinkButton>
                            </li>
                            <li id="liResumen" runat="server" class="tab2">
                                <asp:LinkButton ID="btViewResumen" runat="server" 
                                                CommandArgument="2" 
                                                oncommand="btViewTab_Command">
                                    Resumen</asp:LinkButton></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td class="tabcontent">  
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <%--MOVILES--%>
            <asp:View ID="View1" runat="server">
            <br />
            <div class="grid_container" style="height: 300px; overflow: auto;">
            
                <C1:C1GridView ID="grid" runat="server"   GridLines="None" 
                                                                UseEmbeddedVisualStyles="false"
                                                                AutoGenerateColumns="False" 
                                                                CssClass="Grid" 
                                                                BorderStyle="None"  
                                                                ShowHeader="true" 
                                                                Width="820px" 
                                                                >
                                                                
                <HeaderStyle CssClass="Grid_Header" Height="20px" Font-Bold="true" />
                <RowStyle CssClass="Grid_Item Grid_Item_Parte"  />
                <AlternatingRowStyle CssClass="Grid_Alternating_Item" />
                <FooterStyle CssClass="Grid_Header" />
    
                <Columns>
                    <C1:C1BoundField DataField="Id" Visible="false" >
                    </C1:C1BoundField>
                    <C1:C1TemplateField HeaderText="">
                        <ItemStyle Width="50px" />
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkSelected"></asp:CheckBox>
                        </ItemTemplate>
                    </C1:C1TemplateField>
                    <C1:C1BoundField DataField="Descripcion" HeaderText="Equipo" >
                        <ItemStyle Width="120px" HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="TiempoTrabajado" HeaderText="Hs Trabajadas" >
                        <ItemStyle Width="150px" HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="Kilometraje" HeaderText="Kilometraje" >
                        <ItemStyle Width="150px" HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="Partes" HeaderText="Partes" >
                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                    </C1:C1BoundField>
                    <C1:C1BoundField DataField="TipoServicioId" Visible="false" >
                    </C1:C1BoundField>
                </Columns>      
                </C1:C1GridView>  
                </div>
                <div style="margin: auto; width: 700px; text-align: right;">
                    <asp:Button ID="btControlar" runat="server" Text="Iniciar Control" onclick="btControlar_Click" />          
                </div>
                
            </asp:View>
            
            <%--PARTES--%>
            <asp:View ID="View2" runat="server">
            
                <uc2:DatosParte ID="DatosParte1" runat="server" />
                <table style="width: 100%">
                    <tr>
                    <td>
                       <div class="parte_pager">
                            <asp:Label ID="lblCurrent" runat="server" Text="1"></asp:Label>
                            de
                            <asp:Label ID="lblCount" runat="server" Text="1"></asp:Label>
                        </div> 
                    </td>
                    <td style="text-align: right;">
                        <asp:Button ID="btCancelarParte" runat="server" Text="Cancelar" 
                            onclick="btCancelarParte_Click" />
                        <asp:Button ID="btSiguienteParte" runat="server" Text="Ignorar" 
                            onclick="btSiguienteParte_Click" />
                        <asp:Button ID="btAceptarParte" runat="server" Text="Aceptar"
                            onclick="btAceptarParte_Click" />
                        <asp:HiddenField ID="hidChanged" runat="server" Value="false" />
                    </td>
                    </tr>
                </table>
            </asp:View>
            
            <%--RESUMEN--%>
            <asp:View ID="View3" runat="server">
                <br />
                <div class="grid_container">
                <C1:C1GridView ID="gridResumen" runat="server" 
                                DataKeyNames="Id"
                                GridLines="None" 
                                AutoGenerateColumns="False" 
                                CssClass="Grid"
                                BorderStyle="None"
                                Width="820px" 
                                Height="320px"
                                UseEmbeddedVisualStyles="false"
                                ScrollSettings-ScrollMode="ScrollBar"
                                ScrollSettings-ScrollOrientation="Both"
                                OnRowCommand="gridResumen_ItemCommand"
                                OnRowDataBound="gridResumen_ItemDataBound"
                                AllowGrouping="true"
                                 >
                                
                    <HeaderStyle    CssClass="Grid_Header" Height="20px" Font-Bold="true" />
                    <RowStyle CssClass="Grid_Item Grid_Item_Parte"  />
                    <AlternatingRowStyle CssClass="Grid_Alternating_Item" />
                    <FooterStyle CssClass="Grid_Header" />
                    
                <Columns>
                <%--    <c1:C1BoundField DataField="Equipo" HeaderText="Custom" />--%>
                    <C1:C1BoundField DataField="Grupo" HeaderText=""/>
                   <%-- <c1h:C1ResourceGroupColumn DataField="Equipo" GroupResourceName="Labels" GroupVariableName="GROUP_EQUIPO" HeaderText="Custom" GroupSingleRow="true" >
                       <GroupInfo HeaderText="Custom" />
                    </c1h:C1ResourceGroupColumn>--%>
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
                            <asp:LinkButton ID="btModificar" runat="server" Text="Modificar" CommandName="Modificar"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                    </C1:C1TemplateField>
                </Columns>
                           
                </C1:C1GridView>  
                </div>
            </asp:View>
        </asp:MultiView>
</td></tr>
</table>   
    </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btBuscar" EventName="Click" />
    </Triggers>
    </asp:UpdatePanel>
</div>
</asp:Content>

