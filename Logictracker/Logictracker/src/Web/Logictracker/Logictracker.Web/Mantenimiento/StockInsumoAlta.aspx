<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Mantenimiento.StockInsumoAlta" Codebehind="StockInsumoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="vertical-align: top; width:50%;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblDeposito" runat="server" ResourceName="Entities" VariableName="PARENTI87" Width="100px" />
                        <asp:UpdatePanel ID="upDeposito" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:DepositoDropDownList ID="cbDeposito" runat="server" Enabled="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblInsumo" runat="server" ResourceName="Entities" VariableName="PARENTI58" Width="100px" />
                        <asp:UpdatePanel ID="upInsumo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:InsumoDropDownList ID="cbInsumo" runat="server" Enabled="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="vertical-align: top; width:50%;">
                    <cwc:AbmTitledPanel ID="panelTopRight" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblCapacidadMaxima" runat="server" ResourceName="Labels" VariableName="CAPACIDAD_MAXIMA" />
                        <asp:TextBox ID="txtCapacidadMaxima" runat="server" MaxLength="12" />
                        
                        <cwc:ResourceLabel ID="lblPuntoReposicion" runat="server" ResourceName="Labels" VariableName="PUNTO_REPOSICION" />
                        <asp:TextBox ID="txtPuntoReposicion" runat="server" MaxLength="12" />
                        
                        <cwc:ResourceLabel ID="lblStockCritico" runat="server" ResourceName="Labels" VariableName="NIVEL_CRITICO" />
                        <asp:TextBox ID="txtStockCritico" runat="server" MaxLength="12" />
                        
                        <cwc:ResourceLabel ID="lblStockActual" runat="server" ResourceName="Labels" VariableName="STOCK_ACTUAL" />
                        <asp:TextBox ID="txtStockActual" runat="server" MaxLength="12" Enabled="false" />
                        
                        <cwc:ResourceLabel ID="lblAlarmaActiva" runat="server" ResourceName="Labels" VariableName="ALARMA_ACTIVA" />
                        <asp:CheckBox ID="chkAlarmaActiva" runat="server" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
