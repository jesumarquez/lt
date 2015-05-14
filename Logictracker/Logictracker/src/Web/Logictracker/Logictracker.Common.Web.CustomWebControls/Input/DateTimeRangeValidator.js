function DateTimeRangeValidator(startControl, endControl, minRange, maxRange)
{
    this.start = startControl;
    this.end = endControl;
    this.minRange = minRange;
    this.maxRange = maxRange;
    this.start.on_blur = Type.createDelegate(this, this.startChanged);
    this.end.on_blur = Type.createDelegate(this, this.endChanged);
}
DateTimeRangeValidator.prototype = 
{
    validateRange: function()
    {
        var startDate = this.start.getDate();
        var endDate = this.end.getDate();
        if(startDate == null || endDate == null) return true;
        var ini = startDate.getTime() / 60000;
        var fin = endDate.getTime() /60000;
        if(ini > fin) return false;
        var range = fin - ini;
        if(range < this.minRange || range > this.maxRange) return false;
        return true;
    },
    startChanged: function()
    {
        var startDate = this.start.getDate();
        var endDate = this.end.getDate();     
        if(startDate == null || endDate == null) return;
        var ini = startDate.getTime() / 60000;
        var fin = endDate.getTime() /60000;
        var range = fin - ini;
        if(range < this.minRange)
        {
            var d = new Date();
            d.setTime((ini + this.minRange) * 60000);
            this.end.setDate(d);
        }        
        else if(this.maxRange > -1 && range > this.maxRange)
        {
            var d = new Date();
            d.setTime((ini + this.maxRange) * 60000);
            this.end.setDate(d);
        }
    },
    endChanged: function()
    {
        var startDate = this.start.getDate();
        var endDate = this.end.getDate();
        if(startDate == null || endDate == null) return;
        var ini = startDate.getTime() / 60000;
        var fin = endDate.getTime() /60000;
        var range = fin - ini;
        if(range < this.minRange)
        {
            var d = new Date();
            d.setTime((fin - this.minRange) * 60000);
            this.start.setDate(d);
        }        
        else if(this.maxRange > -1 && range > this.maxRange)
        {
            var d = new Date();
            d.setTime((fin - this.maxRange) * 60000);
            this.start.setDate(d);
        }
    }
}