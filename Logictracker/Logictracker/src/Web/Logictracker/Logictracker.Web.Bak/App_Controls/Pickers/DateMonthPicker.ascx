<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateMonthPicker.ascx.cs" Inherits="Logictracker.App_Controls.Pickers.App_Controls_DateMonthPicker" %>

<asp:TextBox ID="txtFecha" runat="server" />
<asp:Image runat="server" ID="imgFecha" ImageUrl="../images/calend.png" style="margin-bottom: -6px;" />
<AjaxToolkit:CalendarExtender ID="ceFecha" runat="server" PopupButtonID="imgFecha" TargetControlID="txtFecha"
    Format="MM/yyyy" />
<AjaxToolkit:MaskedEditExtender ID="meeFecha" runat="server" TargetControlID="txtFecha" MaskType="None" CultureName="es-AR"
    Mask="99/9999" ClearMaskOnLostFocus="false" OnFocusCssClass="MaskedEditFocus" />
<AjaxToolkit:MaskedEditValidator ID="mevFecha" runat="server" ControlToValidate="txtFecha" ControlExtender="meeFecha"
    IsValidEmpty="false" EmptyValueMessage="Fecha requerida" TooltipMessage="Formato (mm/aaaa)"
    InvalidValueMessage="Fecha invalida" />
    
<script type="text/javascript">
    function CheckMonthRange(iniDate, finDate, interval, onClick) {
        if (onClick) onClick;
           
        var from = $get(iniDate).value;
        var to = $get(finDate).value;

        var fromDate = StringToDate(from);
        var toDate = StringToDate(to);

        var rangeOK = fromDate <= toDate;

        var elapsedMonths = RangeMonths(fromDate, toDate);

        var intervalOK = elapsedMonths <= interval;

        if (!rangeOK)
            alert('La fecha inicial debe ser menor o igual a la fecha final.');
        else
            if (!intervalOK)
                alert('El intervalo elegido de ' + elapsedMonths + ' meses supera al valor máximo permitido de '
                    + interval + ' meses.');

        return rangeOK && intervalOK;
    }

    function StringToDate(date) {
        //Parse the givenn string.
        var day = 1;
        var month = date.substring(3, 5);
        var year = date.substring(6, 10);

        //Generates a javascript format date.
        return new Date(month + '/' + day + '/' + year);
    }

    function RangeMonths(iniDate, finDate) {
        var iniMonth = iniDate.getMonth();
        var finMonth = finDate.getMonth();

        //Returns the number of months included in the selected period.
        return Math.abs(finMonth - iniMonth);
    }
</script>