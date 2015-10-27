<%@ Control Language="C#" AutoEventWireup="true" Inherits="Logictracker.Operacion.Qtree.Operacion_Qtree_AutoGenConfig" Codebehind="AutoGenConfig.ascx.cs" %>
<%@ Register src="LevelSelector.ascx" tagname="LevelSelector" tagprefix="uc" %>


<table style="width: 100%;" class="Grid_Header">
<tr>
<td>Tipo de via</td>
<td style="width: 65px;">Pincel</td>
<td style="width: 10px;"></td>
<td style="width: 60px;">Nivel</td>
</tr>
</table>

<table style="width: 100%;" class="filterpanel">
<tr>
<td>Autopista</td>
<td style="width: 40px;"><asp:TextBox ID="txtSliderAutopista" runat="server" Width="40px" Text="2"></asp:TextBox> </td>
<td style="width: 25px;"><asp:TextBox ID="lblSliderAutopista" runat="server" Width="25px"></asp:TextBox></td>
<td style="width: 10px;"></td>
<td style="width: 60px;">
    <ajaxToolkit:SliderExtender ID="SliderExtender1" runat="server" TargetControlID="txtSliderAutopista" Minimum="1" Maximum="3" BoundControlID="lblSliderAutopista" Steps="3" Length="40" />
    <uc:LevelSelector ID="lvlAutopista" runat="server" BoxSize="20px" />
</td>
</tr>
</table>
<table style="width: 100%;" class="filterpanel">
<tr>
<td>Ruta</td>
<td style="width: 40px;"><asp:TextBox ID="txtSliderRuta" runat="server" Width="40px" Text="3"></asp:TextBox> </td>
<td style="width: 25px;"><asp:TextBox ID="lblSliderRuta" runat="server" Width="25px"></asp:TextBox></td>
<td style="width: 10px;"></td>
<td style="width: 60px;">
    <ajaxToolkit:SliderExtender ID="SliderExtender2" runat="server" TargetControlID="txtSliderRuta" Minimum="1" Maximum="3" BoundControlID="lblSliderRuta" Steps="3" Length="40" />
    <uc:LevelSelector ID="lvlRuta" runat="server" BoxSize="20px" />
</td>
</tr>
</table>
<table style="width: 100%;" class="filterpanel">
<tr>
<td>Avenida</td>
<td style="width: 40px;"><asp:TextBox ID="txtSliderAvenida" runat="server" Width="40px" Text="2"></asp:TextBox> </td>
<td style="width: 25px;"><asp:TextBox ID="lblSliderAvenida" runat="server" Width="25px"></asp:TextBox></td>
<td style="width: 10px;"></td>
<td style="width: 60px;">
    <ajaxToolkit:SliderExtender ID="SliderExtender3" runat="server" TargetControlID="txtSliderAvenida" Minimum="1" Maximum="3" BoundControlID="lblSliderAvenida" Steps="3" Length="40" />
    <uc:LevelSelector ID="lvlAvenida" runat="server" BoxSize="20px" />
</td>
</tr>
</table>
<table style="width: 100%;" class="filterpanel">
<tr>
<td>Calle</td>
<td style="width: 40px;"><asp:TextBox ID="txtSliderCalle" runat="server" Width="40px" Text="2"></asp:TextBox> </td>
<td style="width: 25px;"><asp:TextBox ID="lblSliderCalle" runat="server" Width="25px"></asp:TextBox></td>
<td style="width: 10px;"></td>
<td style="width: 60px;">
    <ajaxToolkit:SliderExtender ID="SliderExtender4" runat="server" TargetControlID="txtSliderCalle" Minimum="1" Maximum="3" BoundControlID="lblSliderCalle" Steps="3" Length="40" />
    <uc:LevelSelector ID="lvlCalle" runat="server" BoxSize="20px" />
</td>
</tr>
</table>