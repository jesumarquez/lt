<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TicketsList.ascx.cs" Inherits="Logictracker.App_Controls.TicketsList" %>

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
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="OPETICK01" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DATE" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="INCIDENCE" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DESCRIPCION" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="CONTACTO" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="MDN" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="MAIL" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="STATE" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI01" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI03" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI08" AllowGroup="false" AllowMove="false" AllowSizing="false" />
            </Columns>
        </C1:C1GridView>
    </ContentTemplate>
</asp:UpdatePanel>
