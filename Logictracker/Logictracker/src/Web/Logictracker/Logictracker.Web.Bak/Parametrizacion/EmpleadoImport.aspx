<%@ Page Language="C#" MasterPageFile="~/MasterPages/ImportPage.master" AutoEventWireup="true" CodeFile="EmpleadoImport.aspx.cs" Inherits="Logictracker.Parametrizacion.EmpleadoImport" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentImport" Runat="Server">


            <table style="width: 100%">                
                <tr>
                    <td><cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" /></td>
                    <td><cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" /></td>
                </tr>                    
                <tr>
                    <td><cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" /></td>
                    <td>
                        <asp:UpdatePanel runat="server" ID="upLinea" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AddAllItem="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>            
                </tr>
            </table>

</asp:Content>

