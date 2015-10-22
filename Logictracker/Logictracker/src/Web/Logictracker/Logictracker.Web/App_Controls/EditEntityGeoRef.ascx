<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="EditEntityGeoRef.ascx.cs" Inherits="Logictracker.App_Controls.EditEntityGeoRef" %>

<%@ Register Src="../App_Controls/EditGeoRef.ascx" TagName="EditGeoRef" TagPrefix="uc1" %>
<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="IconPicker" TagPrefix="uc" %>
<%@ Register Src="../App_Controls/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc" %>


<table style="width: 100%"><tr>
    <td>
        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI10"></cwc:ResourceLabel>
    </td><td>
        <asp:UpdatePanel ID="updTipoGeoRef" runat="server" UpdateMode="Conditional">
            <ContentTemplate>  
                <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoReferenciaGeografica" runat="server" Width="125px" ParentControls="owner" OnSelectedIndexChanged="cbTipoReferenciaGeografica_SelectedIndexChanged" FilterMode="false" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </td>
<td>
           <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="ICON"></cwc:ResourceLabel>
    </td>
    <td>
    <asp:UpdatePanel ID="updSelectIcon" runat="server" UpdateMode="Conditional">
        <ContentTemplate>  
            <uc:IconPicker ID="SelectIcon2" runat="server" OnSelectedIconChange="SelectIcon2_SelectedIconChange" />
        </ContentTemplate>
    </asp:UpdatePanel>    
    </td>
<td>
    
        <cwc:ResourceLabel ID="lblColor" runat="server" ResourceName="Labels" VariableName="COLOR" />
    </td><td>   
     
        <asp:UpdatePanel ID="updColor" runat="server" UpdateMode="Conditional">
            <ContentTemplate>  
                <uc:ColorPicker ID="cpColor" runat="server" AutoPostback="true" OnColorChanged="cpColor_ColorChanged" /> 
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="updHackColor" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbTipoReferenciaGeografica" />
                <asp:AsyncPostBackTrigger ControlID="btBorrarPunto" />
                <asp:AsyncPostBackTrigger ControlID="btBorrarPoly" />
            </Triggers>
        </asp:UpdatePanel>
   </td><td>
<div style="text-align: right;">
                Aplicar desde
                <cwc:DateTimePicker ID="txtVigenciaDesde" runat="server" Mode="DateTime" IsValidEmpty="true" PopupPosition="TopRight" />
                
                <asp:Button ID="btBorrarPunto" runat="server" Text="Borrar Direccion" OnClick="btBorrarPunto_Click" />
                <asp:Button ID="btBorrarPoly" runat="server" Text="Borrar Geocerca" OnClick="btBorrarPoly_Click" />
                </div>
</td></tr></table>
<uc1:EditGeoRef ID="EditGeoRef1" runat="server" />  
          