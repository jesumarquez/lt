<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ParteReport.aspx.cs" Inherits="Logictracker.Documentos.Documentos_ParteReport" Title="Untitled Page" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">  
    <table style="width: 100%; padding: 3px;">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblEmpresa"  Font-Bold="true" Text="Empresa"></asp:Label>
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="cbAseguradora" runat="server" ParentControls="cbPlanta" AutoPostBack="true"></cwc:TransportistaDropDownList>
                    </ContentTemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged"></asp:AsyncPostBackTrigger>
                    </triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbLocacion" runat="server"></cwc:LocacionDropDownList>
            </td>
            <td>
                <cwc:ResourceLabel id="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" ></cwc:ResourceLabel> 
                <br />
                <cwc:PlantaDropDownList ID="cbPlanta" runat="server" AddAllItem="true" ParentControls="cbLocacion"></cwc:PlantaDropDownList>
            </td>
            <td>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <cwc:ResourceLabel id="lblMobil" runat="server" VariableName="PARENTI03" ResourceName="Entities" Font-Bold="true"></cwc:ResourceLabel> 
                        <br />
                        <cwc:MovilDropDownList id="ddlMovil" runat="server" Width="120px" AddAllItem="true" ParentControls="cbAseguradora"></cwc:MovilDropDownList> 
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbAseguradora" EventName="SelectedIndexChanged"></asp:AsyncPostBackTrigger>
                    </triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:Label runat="server" ID="Label3"  Font-Bold="true" Text="Equipo"></asp:Label>
                <br />
                <cwc:EquipoDropDownList ID="ddlEquipo" Width="120px" runat="server" AddAllItem="true" ParentControls="cbPlanta" />
            </td>
            <td>
                <asp:Label runat="server" ID="Label2"  Font-Bold="true" Text="Estado"></asp:Label>
                <br />
                <asp:DropDownList ID="cbEstado" runat="server">
                    <asp:ListItem Selected="True" Text="Todos" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="Sin Controlar" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Controlado" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Verificado" Value="2"></asp:ListItem>
                </asp:DropDownList>
                
            </td>
        </tr>
    </table>  

    <table style="width: 100%;">
        <tr>
            <td>
                <table>
                <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Label runat="server" ID="Label1"  Font-Bold="true" Text="Periodo"></asp:Label>
                            &nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="btAnioDown" runat="server" CommandName="Down" OnCommand="ChangeAnio">&lt;&lt;</asp:LinkButton>                
                            <asp:Label runat="server" ID="lblAnioPeriodo"  Text=""></asp:Label>                   
                            <asp:LinkButton ID="btAnioUp" runat="server" CommandName="Up" OnCommand="ChangeAnio">&gt;&gt;</asp:LinkButton>                
                            <br />
                            <asp:DropDownList ID="cbPeriodo" runat="server" Width="200px" DataTextField="Descripcion" DataValueField="Id" AutoPostBack="true" onselectedindexchanged="cbPeriodo_SelectedIndexChanged"></asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    -&gt;
                </td>
                <td>
                    <asp:Label runat="server" ID="lblInicio"  Font-Bold="true" Text="Desde"></asp:Label>
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:DateTimePicker ID="dtInicio" runat="server" Mode="Date" TimeMode="Start" IsValidEmpty="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbPeriodo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblFin"  Font-Bold="true" Text="Hasta"></asp:Label>
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:DateTimePicker ID="dtFin" runat="server" Mode="Date" TimeMode="End" IsValidEmpty="false" />                            
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbPeriodo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

