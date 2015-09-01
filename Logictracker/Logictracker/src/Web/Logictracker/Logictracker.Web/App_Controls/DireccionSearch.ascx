<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="DireccionSearch.ascx.cs" Inherits="Logictracker.App_Controls.DireccionSearch" %>

<asp:Panel ID="panelControl" runat="server">
    <asp:UpdatePanel ID="updControl" runat="server" RenderMode="Inline" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <AjaxToolkit:TabContainer ID="tab" runat="server">
                <AjaxToolkit:TabPanel ID="tabDireccion" runat="server">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server"  ResourceName="Labels" VariableName="ADDRESS"></cwc:ResourceLabel>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Label ID="lblDireccion" runat="server" />
                        <br />
                        <br />
                        <asp:Label ID="lblLatitud" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="lblLongitud" runat="server" Text=""></asp:Label>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabSmartSearch" runat="server">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server"  ResourceName="Labels" VariableName="ADDRESS_SMART_SEARCH"></cwc:ResourceLabel>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Panel ID="panelSmartSearch" runat="server" DefaultButton="btSmartSearch">
                            <div style="text-align: right; padding-top: 5px;">
                                <asp:TextBox ID="txtSmartSearch" runat="server" Width="90%" CssClass="LogicTextbox" />
                                <cwc:ResourceTextBoxWatermarkExtender ID="txwSmartSearch" TargetControlID="txtSmartSearch"
                                    ResourceName="Labels" VariableName="ADDRESS"
                                     WatermarkCssClass="LogicWatermark" Enabled="True"
                                    runat="server" />
                                <cwc:ResourceButton ID="btSmartSearch" runat="server"  ResourceName="Controls" VariableName="BUTTON_SEARCH"  OnClick="BtSmartSearchClick" OnClientClick="return true;" CssClass="LogicButton_Big" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabNormalSearch" runat="server">
                    <HeaderTemplate>
                         <cwc:ResourceLabel ID="ResourceLabel3" runat="server"  ResourceName="Labels" VariableName="ADDRESS_SEARCH"></cwc:ResourceLabel>
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:Panel ID="panelNormalSearch" runat="server" DefaultButton="btNormalSearch">
                        <div style="text-align: right; padding-top: 5px;">
                            <asp:TextBox ID="txtCalle" runat="server" Width="90%"  CssClass="LogicTextbox"></asp:TextBox>
                            <cwc:ResourceTextBoxWatermarkExtender ID="txwCalle" TargetControlID="txtCalle" 
                                ResourceName="Labels" VariableName="ADDRESS_STREET"
                                WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
                            <asp:TextBox ID="txtAltura" runat="server" Width="90%" CssClass="LogicTextbox"></asp:TextBox>
                            <cwc:ResourceTextBoxWatermarkExtender ID="txwAltura" TargetControlID="txtAltura"
                                ResourceName="Labels" VariableName="ADDRESS_STREET_NUMBER"
                                WatermarkCssClass="LogicWatermark" Enabled="True"
                                runat="server" />
                            <asp:TextBox ID="txtEsquina" runat="server" Width="90%" CssClass="LogicTextbox"></asp:TextBox>
                            <cwc:ResourceTextBoxWatermarkExtender ID="txwEsquina" TargetControlID="txtEsquina"
                                ResourceName="Labels" VariableName="ADDRESS_STREET_INTERSECTION"
                                WatermarkCssClass="LogicWatermark" Enabled="True"
                                runat="server" />
                            <asp:TextBox ID="txtPartido" runat="server" Width="90%" CssClass="LogicTextbox"></asp:TextBox>
                            <cwc:ResourceTextBoxWatermarkExtender ID="txwPartido" TargetControlID="txtPartido"
                                ResourceName="Labels" VariableName="ADDRESS_LOCATION"
                                WatermarkCssClass="LogicWatermark"
                                Enabled="True" runat="server" />
                            <asp:DropDownList ID="cbProvincia" runat="server" Width="90%" DataTextField="Nombre" DataValueField="Id" CssClass="LogicCombo">
                            </asp:DropDownList>
                        
                            <cwc:ResourceButton ID="btNormalSearch" runat="server"  ResourceName="Controls" VariableName="BUTTON_SEARCH"  OnClick="BtNormalSearchClick" CssClass="LogicButton_Big" />
                        </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabLatLon" runat="server">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server"  ResourceName="Labels" VariableName="ADDRESS_LATLON_SEARCH"></cwc:ResourceLabel>
                    </HeaderTemplate>
                    <ContentTemplate>
                    <asp:Panel ID="panelLatLon" runat="server" DefaultButton="btLatLon">
                        <div style="text-align: right; padding-top: 5px;">
                            <asp:TextBox ID="txtLatitud" runat="server" Width="90%" CssClass="LogicTextbox"></asp:TextBox>
                            <cwc:ResourceTextBoxWatermarkExtender ID="txwLatitud" TargetControlID="txtLatitud"
                                ResourceName="Labels" VariableName="ADDRESS_LATITUDE"
                                WatermarkCssClass="LogicWatermark" Enabled="True"
                                runat="server" />
                            <asp:TextBox ID="txtLongitud" runat="server" Width="90%" CssClass="LogicTextbox"></asp:TextBox>
                            <cwc:ResourceTextBoxWatermarkExtender ID="txwLongitud" TargetControlID="txtLongitud"
                                ResourceName="Labels" VariableName="ADDRESS_LONGITUDE"
                                WatermarkCssClass="LogicWatermark" Enabled="True"
                                runat="server" />
                        
                            <cwc:ResourceButton ID="btLatLon" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtLatLonClick" CssClass="LogicButton_Big" />
                        </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabResult" runat="server" Enabled="false" Width="100%">
                    <HeaderTemplate>
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server"  ResourceName="Labels" VariableName="ADDRESS_RESULT"></cwc:ResourceLabel>
                    </HeaderTemplate>
                    <ContentTemplate>
                    <asp:Panel ID="panelResult" runat="server" DefaultButton="btAceptar">
                        <div>
                            <asp:ListBox ID="cbResults" runat="server" Width="100%" Height="200px"></asp:ListBox>
                            <div style="text-align: right; padding-top: 5px;">
                                <cwc:ResourceButton ID="btAceptar" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="BtAceptarClick" CssClass="LogicButton_Big" />
                                <cwc:ResourceButton ID="btCancelar" runat="server"  ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelarClick" CssClass="LogicButton_Big" />
                            </div>
                        </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
            </AjaxToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
