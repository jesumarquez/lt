<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="DateTimePicker.ascx.cs" Inherits="Logictracker.App_Controls.Pickers.App_Controls_DateTimePicker" %>

<style type="text/css">
    .ajax__calendar_container {width:130px;font-size:9px;}
    .ajax__calendar_body {height:100px;width:130px;}
    .ajax__calendar_days, .ajax__calendar_months, .ajax__calendar_years {height:100px;width:130px;}
    .ajax__calendar_container TABLE {font-size:9px;}
    .ajax__calendar_header {height:15px;}
    .ajax__calendar_footer {height:10px;}
    .ajax__calendar_dayname {height:12px;width:12px;}
    .ajax__calendar_day {height:12px;width:12px;}
    .ajax__calendar_month {height:25px;width:20px;}
    .ajax__calendar_year {height:25px;width:20px;}
    .ajax__calendar_prev {cursor:pointer;width:10px;height:10px;}
    .ajax__calendar_next {cursor:pointer;width:10px;height:10px;}
</style>

<asp:TextBox ID="txtFechaHidden" runat="server" style="visibility:hidden;" Width="1px" />
<asp:TextBox ID="txtFecha" runat="server" style="margin-left: -10px" />
<asp:Image runat="server" ID="imgFecha" ImageUrl="../../images/calend.png" style="margin-bottom: -6px;" />
<AjaxToolkit:CalendarExtender ID="ceFecha" runat="server" PopupButtonID="imgFecha" TargetControlID="txtFechaHidden" Format="dd/MM/yyyy HH:mm" />
<AjaxToolkit:MaskedEditExtender ID="meeFecha" runat="server" TargetControlID="txtFecha" MaskType="DateTime" Mask="99/99/9999 99:99"
    ClearMaskOnLostFocus="false" CultureName="es-AR" OnFocusCssClass="MaskedEditFocus" />
<AjaxToolkit:MaskedEditValidator ID="mevFecha" runat="server" ControlToValidate="txtFecha" ControlExtender="meeFecha" IsValidEmpty="false"
    EmptyValueMessage="Fecha requerida" TooltipMessage="Formato (dd/mm/aaaa HH:mm)" InvalidValueMessage="Fecha invalida" />
    
<script type="text/javascript">
    function DateToHidden(from, to, cal) {
        var f = $get(from);
        var t = $get(to);
        var c = $find(cal);

        var day = f.value.substring(0, 2);
        var month = f.value.substring(3, 5);
        var year = f.value.substring(6, 10);

        t.value = f.value;

        c.set_selectedDate(new Date(month + '/' + day + '/' + year));
    }
    
    function DateToVisible(from, to) {
        var f = $get(from);
        var t = $get(to);

        var day = f.value.substring(0, 2);
        var month = f.value.substring(3, 5);
        var year = f.value.substring(6, 10);

        t.value = day + '/' + month + '/' + year + t.value.substring(10);
    }

    function CheckDateTimeRange(iniDate, finDate, interval, onClick) {
        if (onClick) onClick;

        var from = $get(iniDate).value;
        var to = $get(finDate).value;
        var fromDate = StringToDate(from);
        var toDate = StringToDate(to);

        var rangeOK = fromDate <= toDate;

        var elapsedDays = RangeDays(fromDate, toDate);

        var intervalOK = elapsedDays <= interval;

        if (!rangeOK)
            alert('La fecha inicial debe ser menor o igual a la fecha final.');
        else
            if (!intervalOK)
            alert('El intervalo elegido de ' + elapsedDays + ' días supera al valor máximo permitido de '
                    + interval + ' días.');

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

    function RangeDays(iniDate, finDate) {
        //Set 1 day in milliseconds
        var one_day = 1000 * 60 * 60 * 24

        //Returns the difference in days between the two givenn dates.
        return Math.abs(Math.ceil((iniDate.getTime() - finDate.getTime()) / (one_day)));
    }

    function RangeHours(iniDate, finDate) {
        //Set 1 day in milliseconds
        var one_hour = 1000 * 60 * 60

        //Returns the difference in minutes between the two givenn dates.
        return Math.abs(Math.ceil((iniDate.getTime() - finDate.getTime()) / (one_hour)));
    }

    function CheckDateTimeHoursRange(iniDate, finDate, interval, onClick) {
        if (onClick) onClick;
           
        var from = $get(iniDate).value;
        var to = $get(finDate).value;

        var fromDate = StringToDate(from);
        var toDate = StringToDate(to);

        var rangeOK = fromDate <= toDate;

        var elapsedHours = RangeHours(fromDate, toDate);

        var intervalOK = elapsedHours <= interval;

        if (!rangeOK)
            alert('La fecha inicial debe ser menor o igual a la fecha final.');
        else
            if (!intervalOK)
                alert('El intervalo elegido de ' + elapsedHours + ' horas supera al valor máximo permitido de '
                    + interval + ' horas.');

        return rangeOK && intervalOK;
    }
</script>