<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="DatePicker.ascx.cs" Inherits="Logictracker.App_Controls.Pickers.App_Controls_DatePicker" %>

<asp:TextBox ID="txtFecha" runat="server" />
<asp:Image runat="server" ID="imgFecha" ImageUrl="../../images/calend.png" style="margin-bottom: -6px;" />
<AjaxToolkit:CalendarExtender ID="ceFecha" runat="server" PopupButtonID="imgFecha" TargetControlID="txtFecha" Format="dd/MM/yyyy" />
<AjaxToolkit:MaskedEditExtender ID="meeFecha" runat="server" TargetControlID="txtFecha" MaskType="Date" CultureName="es-AR" Mask="99/99/9999"
    OnFocusCssClass="MaskedEditFocus" />
<AjaxToolkit:MaskedEditValidator ID="mevFecha" runat="server" ControlToValidate="txtFecha" ControlExtender="meeFecha" IsValidEmpty="false"
    EmptyValueMessage="Fecha requerida" InvalidValueMessage="Fecha invalida" TooltipMessage="Formato(dd/MM/yyyy)" />

<script type="text/javascript">
    function CheckDateRange(iniDate, finDate, interval, onClick) {
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
        var month = date.substring(3, 5);
        var year = date.substring(6, 10);

        //Generates a javascript format date.
        return new Date(month + '/' + day + '/' + year);
    }
    
    function RangeDays(iniDate, finDate) {
        //Set 1 day in milliseconds
        var one_day=1000*60*60*24

        //Returns the difference in days between the two givenn dates.
        return Math.abs(Math.ceil((iniDate.getTime() - finDate.getTime()) / (one_day)));
    }
</script>