<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TicketsMantenimientoList.ascx.cs" Inherits="Logictracker.App_Controls.TicketsMantenimientoList" %>

<asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <div class="documentToolbar">
            <div class="button">
                <cwc:ResourceButton ID="btnNuevo" runat="server" ResourceName="Controls" VariableName="BUTTON_NEW" />
            </div>
        </div>
        
        <C1:C1GridView ID="grid" runat="server" SkinID="ListGridNoGroup" DataKeyNames="Id" OnSorting="GridSortingCommand" 
            OnRowDataBound="GridItemDataBound" OnSelectedIndexChanging="GridSelectedIndexChanging" 
            OnGroupColumnMoved="GridGroupColumnMoved" OnColumnMoved="GridColumnMoved" OnPageIndexChanging="GridPageIndexChanging">
            <Columns>
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ID" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DATE" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="CODE" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI35" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ESTADO" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DESCRIPCION" AllowGroup="false" AllowMove="false" AllowSizing="false" />
            </Columns>
        </C1:C1GridView>
    </ContentTemplate>
</asp:UpdatePanel>
