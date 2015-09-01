<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="Consultas.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_Consultas" %>
<div id="consultas">
<asp:UpdatePanel ID="updConsultas" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="header">Direccion</div>
        <asp:Panel ID="panelNormalSearch" runat="server" DefaultButton="btNormalSearch">
            <table style="width: 100%">
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtCalle" runat="server" Width="100%" CssClass="LogicTextbox"></asp:TextBox>
                        <AjaxToolkit:TextBoxWatermarkExtender ID="txwCalle" TargetControlID="txtCalle" WatermarkText="Calle" WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 60px;">
                        <asp:TextBox ID="txtAltura" runat="server" Width="100%" CssClass="LogicTextbox"></asp:TextBox>
                        <AjaxToolkit:TextBoxWatermarkExtender ID="txwAltura" TargetControlID="txtAltura" WatermarkText="Altura" WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtEsquina" runat="server" Width="100%" CssClass="LogicTextbox"></asp:TextBox>
                        <AjaxToolkit:TextBoxWatermarkExtender ID="txwEsquina" TargetControlID="txtEsquina" WatermarkText="Esquina" WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtPartido" runat="server" Width="100%" CssClass="LogicTextbox"></asp:TextBox>
                        <AjaxToolkit:TextBoxWatermarkExtender ID="txwPartido" TargetControlID="txtPartido" WatermarkText="Partido/Localidad" WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:DropDownList ID="cbProvincia" runat="server" Width="100%" DataTextField="Nombre" DataValueField="Id"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: left;">
                        <cwc:ResourceCheckBox ID="chkPosicionar" runat="server" ResourceName="Labels" VariableName="POSICIONAR" />
                    </td>
                    <td style="text-align: right;">
                        <asp:Button ID="btNormalSearch" runat="server" Text="Buscar" CssClass="LogicButton_Big" OnClick="btNormalSearch_Click" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="panelResult" runat="server" DefaultButton="btAceptar" Visible="false">
            <div>
                <asp:ListBox ID="cbResults" runat="server" Width="100%"></asp:ListBox>
                <div style="text-align: right; padding-top: 5px;">
                    <asp:Button ID="btAceptar" runat="server" Text="Aceptar" OnClick="btAceptar_Click" CssClass="LogicButton_Big" />
                    <asp:Button ID="btCancelar" runat="server" Text="Cancelar" OnClick="btCancelar_Click" CssClass="LogicButton_Big" />
                </div>
            </div>
        </asp:Panel>
        
        <div class="header">
        <asp:Button ID="btLimpiar" runat="server" Text="Limpiar" OnClick="btLimpiar_Click" CssClass="btLimpiar" />
        Consultas
        </div>
        <div style="height: 150px; overflow-y: scroll; border: solid 1px black;">     
        
        <asp:UpdatePanel ID="updGridConsultas" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <asp:GridView ID="gridConsultas" runat="server" Width="100%" ShowHeader="false" 
            GridLines="None" AutoGenerateColumns="False" 
            onrowcommand="gridConsultas_RowCommand" 
            onrowdatabound="gridConsultas_RowDataBound" 
            onselectedindexchanging="gridConsultas_SelectedIndexChanging">
            <RowStyle CssClass="consultas_row" />
            <SelectedRowStyle CssClass="consultas_selectedrow" />
        <Columns>
        <asp:ButtonField ButtonType="Image"  ItemStyle-Width="30px" ImageUrl="~/images/magnifier.png"
                CommandName="Select" >
            <ItemStyle Width="20px" />
            </asp:ButtonField>
            <asp:TemplateField></asp:TemplateField>
        <asp:ButtonField ButtonType="Image" ItemStyle-Width="30px" ImageUrl="~/images/delete.png"
                CommandName="Eliminar" >
            <ItemStyle Width="16px" />
            </asp:ButtonField>
            
        </Columns>
        </asp:GridView>
        </ContentTemplate>
        </asp:UpdatePanel>
        </div>  
        <div class="header">Referencia Geografica</div>
        <asp:TextBox ID="txtCodigo" runat="server" Width="100%" CssClass="LogicTextbox"></asp:TextBox>
        <AjaxToolkit:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" TargetControlID="txtCodigo" WatermarkText="Codigo" WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
        <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoGeoRef" runat="server" Width="100%" AddAllItem="false" ></cwc:TipoReferenciaGeograficaDropDownList>
        <div style="text-align: right;">
        <asp:Button ID="btSaveGeoRef" runat="server" Text="Guardar como Referencia Geografica" CssClass="LogicButton" OnClick="btSaveGeoRef_Click" />
        </div>
        <asp:Label ID="lblSaveStatus" runat="server" />
    </ContentTemplate>
    
</asp:UpdatePanel>
</div>