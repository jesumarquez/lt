<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="LineaTelefonicaAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.LineaTelefonicaAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="80px">
                        
                        <cwc:ResourceLabel ID="lblNumeroLinea" runat="server" ResourceName="Labels" VariableName="NUMERO_LINEA" />
                        <asp:TextBox ID="txtNumeroLinea" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblImei" runat="server" ResourceName="Labels" VariableName="SIM" />
                        <asp:TextBox ID="txtImei" runat="server" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="AbmTabPlanes" runat="server" ResourceName="Menu" VariableName="PAR_PLANES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="PAR_PLANES" TitleResourceName="Menu" Height="130px">
                        
                        <table style="width: 100%; border-spacing: 10px;">
                            <tr>
                                <td valign="top">                                                
                                    <C1:C1GridView ID="gridPlanes" runat="server" SkinID="SmallGrid" Visible="false" OnRowDataBound="GridPlanes_RowDataBound">
                                        <Columns>          
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DESDE" >
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDesde" runat="server" />
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="HASTA" >
                                                <ItemTemplate>
                                                    <asp:Label ID="lblHasta" runat="server" />
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI73" >
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPlan" runat="server" />
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                            <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="EMPRESA" >
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEmpresa" runat="server" />
                                                </ItemTemplate>
                                            </c1h:C1ResourceTemplateColumn>
                                         </Columns>
                                    </C1:C1GridView>
                                </td>
                            </tr>
                        </table>                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="AbmTabAlta" runat="server" ResourceName="Labels" VariableName="AGREGAR_PLAN">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="80px">
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESDE" />
                        <cwc:DateTimePicker ID="dtDesde" runat="server" Mode="Date" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="HASTA" />
                        <cwc:DateTimePicker ID="dtHasta" runat="server" Mode="Date" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="EMPRESA" />
                        <cwc:EmpresaTelefonicaDropDownList ID="cbEmpresa" runat="server" Width="40%" AddAllItem="true" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI73" />
                        <asp:UpdatePanel ID="upPlan" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                <cwc:PlanDropDownList ID="cbPlan" runat="server" Width="40%" ParentControls="cbEmpresa" AddAllItem="false" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:Label ID="lbl" runat="server" />
                        <cwc:ResourceButton ID="btnGuardar" runat="server" ResourceName="Controls" VariableName="BUTTON_SAVE" OnClick="BtnGuardar_OnClick" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
        
</asp:Content>
