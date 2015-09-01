<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TimePicker.ascx.cs" Inherits="Logictracker.App_Controls.Pickers.App_Controls_TimePicker" %>

<asp:TextBox ID="txtTime" runat="server" />
<AjaxToolkit:MaskedEditExtender ID="meeTime" runat="server" MaskType="Time" UserTimeFormat="TwentyFourHour"
    TargetControlID="txtTime" Mask="99:99" CultureName="es-AR" OnFocusCssClass="MaskedEditFocus" />
<AjaxToolkit:MaskedEditValidator ID="mevTime" runat="server" ControlToValidate="txtTime" ControlExtender="meeTime"
    IsValidEmpty="false" EmptyValueMessage="Valor requerido" InvalidValueMessage="Valor invalido"
    TooltipMessage="Formato (hh:mm)" />
    
<script type="text/javascript">
    function CheckTimeRange(iniDate, finDate, interval, onClick) {
        if (onClick) onClick;
           
        var from = $get(iniDate).value;
        var to = $get(finDate).value;

        var fromDate = StringToDate(from);
        var toDate = StringToDate(to);

        var rangeOK = fromDate <= toDate;

        var elapsedMinutes = RangeMinutes(fromDate, toDate);

        var intervalOK = elapsedMinutes <= interval;

        if (!rangeOK)
            alert('La fecha inicial debe ser menor o igual a la fecha final.');
        else
            if (!intervalOK)
                alert('El intervalo elegido de ' + elapsedMinutes + ' minutos supera al valor máximo permitido de '
                    + interval + ' minutos.');

        return rangeOK && intervalOK;
    }

    function StringToDate(date) {
        //Parse the givenn string.
        var day = date.substring(0, 2);
        var month = parseInt(date.substring(3, 5), 10) - 1;
        var year = date.substring(6, 10);
        var hours = date.substring(11, 13);
        var minutes = date.substring(14, 16);
        var seconds = 0;

        //Generates a javascript format date.
        return new Date(year, month, day, hours, minutes, seconds);
    }
    
    function RangeMinutes(iniDate, finDate) {
        //Set 1 day in milliseconds
        var one_minute=1000*60

        //Returns the difference in minutes between the two givenn dates.
        return Math.abs(Math.ceil((iniDate.getTime() - finDate.getTime()) / (one_minute)));
    }
</script>