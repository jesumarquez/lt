<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ConsumosList.ascx.cs" Inherits="Logictracker.App_Controls.AppControlsConsumosList" %>

<asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <C1:C1GridView ID="grid" runat="server" SkinID="ListGridNoGroup" DataKeyNames="Id" OnSorting="Grid_SortingCommand" 
            OnRowDataBound="GridConsumos_ItemDataBound" OnSelectedIndexChanging="GridConsumos_SelectedIndexChanging" 
            OnGroupColumnMoved="GridConsumos_GroupColumnMoved" OnColumnMoved="GridConsumos_ColumnMoved" OnPageIndexChanging="GridConsumos_PageIndexChanging">
            <Columns>
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="FECHA" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="NRO_FACTURA" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="KM_DECLARADOS" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI59" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI87" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI58" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="UNIDAD_MEDIDA" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="CANTIDAD" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="IMPORTE_UNITARIO" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="IMPORTE_TOTAL" AllowGroup="false" AllowMove="false" AllowSizing="false" />
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ESTADO" AllowGroup="false" AllowMove="false" AllowSizing="false" />
            </Columns>
        </C1:C1GridView>
    </ContentTemplate>
</asp:UpdatePanel>
