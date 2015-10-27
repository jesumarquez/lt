<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="DocumentList.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_DocumentList" %>

<style type="text/css">
    .button,
    .documentToolbar input
    {
        border: none;
        outline: none; 
        background-color: #CCCCCC;
        padding: 0px 5px 0px 5px;
        height: 26px;
        margin: 0px;
        margin-right: 1px;
        cursor: pointer;
    }
    .flatCombo
    {
        border: none;
        background-color: #CCCCCC;
    }
</style>


<asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:MultiView ID="multiDocumentos" runat="server" ActiveViewIndex="0">
        <asp:View ID="ViewList" runat="server">
            <div class="documentToolbar">
                <div class="button">
                    <cwc:ResourceButton ID="btNuevo" runat="server" ResourceName="Controls" VariableName="BUTTON_NEW" OnClick="BtNuevoClick" />
                    <cwc:TipoDocumentoDropDownList ID="cbTipoDocumento" runat="server" ParentControls="cbEmpresa, cbLinea" CssClass="flatCombo"/>
                </div>
            </div>
            
            <C1:C1GridView ID="grid" runat="server" SkinID="ListGrid" DataKeyNames="Id">
                <Columns>
                    <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI25" SortExpression="TipoDocumento"  AllowGroup="true" AllowMove="true" AllowSizing="false" />
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="FECHA" SortExpression="Fecha" AllowGroup="true" AllowMove="true" AllowSizing="false" />
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VENCIMIENTO" SortExpression="Vencimiento" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="CODE" SortExpression="Codigo" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DESCRIPCION" SortExpression="Descripcion" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                </Columns>
            </C1:C1GridView>            
            
        </asp:View>
        <asp:View ID="viewDocument" runat="server">
        
            <div class="documentToolbar">
                <cwc:ResourceButton ID="btGuardar" runat="server" ResourceName="Controls" VariableName="BUTTON_SAVE" OnClick="BtGuardarClick" />
                <cwc:ResourceButton ID="btCancel" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelClick" />
                <cwc:ResourceButton ID="btBorrar" runat="server" ResourceName="Controls" VariableName="BUTTON_DELETE" OnClick="BtBorrarClick" style="margin-left: 30px" />
             </div>   
                <asp:UpdatePanel ID="updError" runat="server" UpdateMode="Always">
<ContentTemplate>
<cwc:InfoLabel ID="lblDocError" runat="server"></cwc:InfoLabel>
</ContentTemplate>
</asp:UpdatePanel>
            <asp:Panel ID="panelContainer" runat="server">
            </asp:Panel>
        </asp:View>
        </asp:MultiView>

    </ContentTemplate>
</asp:UpdatePanel>