<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DatosParte.ascx.cs" Inherits="Logictracker.Documentos.Controls.DatosParte" %>
<style type="text/css">
.parte_tit
{
    font-size: 14px;
    font-weight: bold;
    border-bottom: dotted 1px black;
    padding-bottom: 5px;
    margin: 10px 0px 10px 0px;
}
.parte_detalle
{
    text-align: left;
    padding: 0px 20px 0px 20px;
}
.parte_big
{
    font-size: 18px;    
    width: 20%;
    font-weight: bold;
    text-align: center;
}
.parte_mid
{
    font-size: 16px;
    width: 20%;
    font-weight: bold;
    text-align: center;
}
.parte_small_right
{
    font-size: 12px;
    text-align: right;
    font-weight: bold;
    text-transform: uppercase;
    width: 20%
}
.parte_small_left
{
    font-size: 12px;
    text-align: left;
    font-weight: bold;
    text-transform: uppercase;
    width: 20%
}

.parte_minitit
{
    font-weight: normal;
    text-transform: uppercase;
    font-size: 8px;
}
.parte_turnos th
{
    text-align: center;
    font-weight: bold;
    background-color: #C6C0B1;
    padding-top: 3px;
    padding-bottom: 3px;
}
.parte_turnos
{
    width: 100%;
    background-color: #E7E7E7;
}
.parte_turnos td
{
    text-align: center;
    border-bottom: solid 1px #CCCCCC;
}
.parte_txt_mod
{
    background-color: #FFDDDD;
}
</style>

<div style="height: 300px;">
    <div class="parte_tit">PARTE DE TRANSPORTE DE PERSONAL</div>
    <div class="parte_detalle">
        <table style="width: 100%;">
            <tr>
                <td class="parte_small_left"><div class="parte_minitit">EMPRESA</div>
                    <asp:Label ID="lblEmpresa" runat="server" Text="Empresa"></asp:Label></td>
                <td class="parte_big"><div class="parte_minitit">PARTE</div><asp:Label ID="lblCodigo" runat="server" Text="-"></asp:Label></td>
                <td class="parte_big"><div class="parte_minitit">VEHICULO</div><asp:Label ID="lblInterno" runat="server" Text="-"></asp:Label></td>
                <td class="parte_big"><div class="parte_minitit">SERVICIO</div><asp:Label ID="lblTipoServicio" runat="server" Text="-"></asp:Label></td>
                <td class="parte_mid"><div class="parte_minitit">EQUIPO</div><asp:Label ID="lblEquipo" runat="server" Text="-"></asp:Label></td>
                <td class="parte_small_right"><div class="parte_minitit">FECHA</div><asp:Label ID="lblFecha" runat="server" Text="-"></asp:Label></td>
            </tr>
            <tr>
                <td class="parte_small_left" colspan="3"><div class="parte_minitit">LUGAR DE PARTIDA</div><asp:Label ID="lblSalida" runat="server" Text="-"></asp:Label></td>
                <td class="parte_small_right" colspan="3"><div class="parte_minitit">LUGAR DE LLEGADA</div><asp:Label ID="lblLlegada" runat="server" Text="-"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="6" style="border-bottom: dotted 1px black;"></td>
            </tr>
        </table>
        <div style="height: 180px; overflow: auto;margin-top: 10px; border: solid 1px #E1A500;background-color: #FCE091;">
            <asp:Repeater ID="Repeater1" runat="server" 
                onitemdatabound="Repeater1_ItemDataBound" 
                onitemcommand="Repeater1_ItemCommand">
            <HeaderTemplate>
                <table class="parte_turnos" cellspacing="0">
                <tr>
                    <th>Salida al Pozo</th>
                    <th>Llegada al Pozo</th>
                    <th>Salida del Pozo</th>
                    <th>Llegada del Pozo</th>
                    <th>Odometros</th>
                    <th>Kilometros</th>
                    <th>Responsable</th>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Label ID="lblSalidaAlPozo" runat="server" Text=""></asp:Label>
                        <asp:TextBox ID="txtSalidaAlPozo" runat="server" Width="60px"></asp:TextBox>
                        <AjaxToolkit:MaskedEditExtender ID="MaskSalidaAlPozo" runat="server" 
                            TargetControlID="txtSalidaAlPozo"
                            Mask="99:99"
                            MaskType="Time"
                            UserTimeFormat="TwentyFourHour"
                            AutoComplete="true"
                            AutoCompleteValue="0"
                            />
                    </td>
                    <td>
                        <asp:Label ID="lblLlegadaAlPozo" runat="server" Text=""></asp:Label>
                        <asp:TextBox ID="txtLlegadaAlPozo" runat="server" Width="60px"></asp:TextBox>
                        <AjaxToolkit:MaskedEditExtender ID="MaskLlegadaAlPozo" runat="server" 
                            TargetControlID="txtLlegadaAlPozo"
                            Mask="99:99"
                            MaskType="Time"
                            UserTimeFormat="TwentyFourHour"
                            AutoComplete="true"
                            AutoCompleteValue="0"
                            />
                    </td>
                    <td>
                        <asp:Label ID="lblSalidaDelPozo" runat="server" Text=""></asp:Label>
                        <asp:TextBox ID="txtSalidaDelPozo" runat="server" Width="60px"></asp:TextBox>
                        <AjaxToolkit:MaskedEditExtender ID="MaskSalidaDelPozo" runat="server" 
                            TargetControlID="txtSalidaDelPozo"
                            Mask="99:99"
                            MaskType="Time"
                            UserTimeFormat="TwentyFourHour"
                            AutoComplete="true"
                            AutoCompleteValue="0"
                            />
                    </td>
                    <td>
                        <asp:Label ID="lblLlegadaDelPozo" runat="server" Text=""></asp:Label>
                        <asp:TextBox ID="txtLlegadaDelPozo" runat="server" Width="60px"></asp:TextBox>
                        <AjaxToolkit:MaskedEditExtender ID="MaskLlegadaDelPozo" runat="server" 
                            TargetControlID="txtLlegadaDelPozo"
                            Mask="99:99"
                            MaskType="Time"
                            UserTimeFormat="TwentyFourHour"
                            AutoComplete="true"
                            AutoCompleteValue="0"
                            />
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Ini:"></asp:Label>
                        <asp:Label ID="lblOdometroInicio" runat="server"></asp:Label>
                        <br />
                        <asp:Label ID="Label2" runat="server" Text="Fin:"></asp:Label>
                        <asp:Label ID="lblOdometroFin" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblKilometraje" runat="server" Text=""></asp:Label>
                        <asp:TextBox ID="txtKilometraje" runat="server" Width="60px"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="lblResponsable" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:LinkButton ID="lnkHistorico" runat="server">Mapa</asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
            <tr>
                    <td style="text-align: right; vertical-align: top;">
                    <b>Horas: </b>
                    </td>
                    <td style="text-align: right; vertical-align: top;">
                        
                        
                        Reportado: <asp:Label ID="lblHorasRep" runat="server" Text=""></asp:Label>hs<br />
                        Real: <asp:Label ID="lblHorasReal" runat="server" Text=""></asp:Label>hs<br />
                        Diferencia: <asp:Label ID="lblHorasDif" runat="server" Text=""></asp:Label>hs
                        
                        
                        
                    </td>
                    <td style="text-align: right; vertical-align: top;">
                        <b>Kilometros: </b>
                    </td>
                    <td style="text-align: right; vertical-align: top;">
                        Reportado: <asp:Label ID="lblKmRep" runat="server" Text=""></asp:Label>km<br />
                        Real: <asp:Label ID="lblKmReal" runat="server" Text=""></asp:Label>km<br />
                        Diferencia: <asp:Label ID="lblKmDif" runat="server" Text=""></asp:Label>km
                    </td>
                    <td style="text-align: right; vertical-align: top;">
                        <b>Promedio km/h: </b>
                    </td>
                    <td style="text-align: right; vertical-align: top;">                        
                        <asp:Label ID="lblHsKm" runat="server" Text=""></asp:Label> km/h
                    </td>
                    <td></td>
                </tr>
                </table>
            </FooterTemplate>
            </asp:Repeater>

        </div>
    </div>
</div>
