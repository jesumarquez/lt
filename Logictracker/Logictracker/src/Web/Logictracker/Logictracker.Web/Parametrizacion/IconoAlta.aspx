<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Parametrizacion.Parametrizacion_Icono_Alta" Title="Untitled Page" Codebehind="IconoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" AsyncPostBacks="false" ResourceName="Menu" VariableName="PAR_ICONOS" />
    
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />
    
    
    <table id="tbTipoPunto"  style="width: 90%; margin: auto; margin-top: 30px;" cellpadding="5">
    <tr>
        <td>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <cwc:TitledPanel ID="panelUpload" runat="server" TitleResourceName="Labels" TitleVariableName="NUEVO_ICONO">
                        <table style="width: 100%"><tr><td>
                            <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        </td><td>
                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="true" Width="100%" />
                        </td><td>
                        </td></tr><tr><td>
                            <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        </td><td>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="cbLinea_SelectedIndexChanged" />
                        </td><td>
                        </td></tr><tr><td>
                            <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        </td><td>
                            <asp:TextBox ID="txtDescripcion" runat="server" MaxLength="64" Width="100%" />
                        </td><td style="width: 50%">
                            <div>
                                <asp:ImageButton ID="imgAnchor" runat="server" Visible="false" style="border: solid 1px #000000; cursor: crosshair;" OnClick="imgAnchor_Click" />
                                <asp:Image ID="imgAnchorPointer" runat="server" ImageUrl="~/images/anchor.gif" Visible="false" Width="10px" Height="10px" />                            
                            </div>
                        </td></tr><tr><td>
                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="IMAGEN" />
                        </td><td>
                        <asp:FileUpload ID="filIcono" runat="server" />
                        </td><td>
                        </td></tr></table>
                    </cwc:TitledPanel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gridIconos" EventName="ItemCommand" />
                    <asp:AsyncPostBackTrigger ControlID="imgAnchor" EventName="Click" />
                </Triggers>
                </asp:UpdatePanel>
        </td>
    </tr>
    <tr>
        <td>
            <cwc:TitledPanel ID="panGeneral" runat="server" TitleResourceName="Menu" TitleVariableName="PAR_ICONOS">
                <asp:Panel ID="panelFilter" runat="server" DefaultButton="btFilter">
                    <table style="width: 100%"><tr><td>
                        <asp:TextBox ID="txtFilter" runat="server" MaxLength="64" Width="100%" />
                    </td><td style="width: 60px">
                        <asp:Button ID="btFilter" runat="server" Text="Buscar" OnClick="btFilter_Click" Width="60px" />
                    </td></tr></table>
                </asp:Panel>
                <div style="height: 300px;overflow-y: scroll;">
                    <asp:UpdatePanel ID="updIconos" runat="server" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <asp:Repeater ID="gridIconos" runat="server" OnItemDataBound="gridIconos_ItemDataBound" OnItemCommand="gridIconos_ItemCommand">
                            <ItemTemplate>
                                <div style="float: left; margin-top: 5px; margin-left: 3px; width: 64px; height: 64px;">
                                    <asp:Literal ID="litAconym" runat="server"></asp:Literal>
                                        <asp:Button ID="Button1" runat="server" Text="" Width="64px" Height="64px" Style="background-color: White; border: solid 1px #c0c0c0; " CommandName="Select" CommandArgument="0" />
                                        <br />
                                        <div  style="background-color: #f0f0f0; font-size: 9px; position: relative; left: 2px; bottom: 12px; width: 60px; overflow: hidden; height: 10px;">
                                            <asp:Label ID="lblDescri" runat="server" Text=""></asp:Label>
                                        </div>
                                    </acronym>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btFilter" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                    </asp:UpdatePanel>
                </div>
            </cwc:TitledPanel>
        </td>
    </tr>    
    </table>
    
</asp:Content>
