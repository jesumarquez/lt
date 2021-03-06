﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="CategoriaAccesoAlta.aspx.cs" Inherits="Logictracker.ControlAcceso.CategoriaAccesoAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="60%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="60%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="NAME" />
                        <asp:TextBox ID="txtNombre" runat="server" MaxLength="64"></asp:TextBox>
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" MaxLength="255"></asp:TextBox>
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>

