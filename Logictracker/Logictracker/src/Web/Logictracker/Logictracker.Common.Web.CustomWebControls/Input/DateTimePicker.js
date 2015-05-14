
function DateTimePicker(textBoxId, hiddenTextBoxId, mode, maskExtenderId)
{
    this.init(textBoxId, hiddenTextBoxId, mode, maskExtenderId);
}
DateTimePicker.prototype =
{
    _ranges: { 'DateTime': [{ i: 0, f: 2, min: 1, max: 31 },
                            { i: 3, f: 5, min: 1, max: 12 },
                            { i: 6, f: 10, min: 1, max: 9999 },
                            { i: 11, f: 13, min: 0, max: 23 },
                            { i: 14, f: 16, min: 0, max: 59}],
        'Date': [{ i: 0, f: 2, min: 1, max: 31 },
                     { i: 3, f: 5, min: 1, max: 12 },
                     { i: 6, f: 10, min: 1, max: 9999}],
        'Time': [{ i: 0, f: 2, min: 0, max: 23 },
                     { i: 3, f: 5, min: 0, max: 59}],
        'Month': [{ i: 0, f: 2, min: 1, max: 12 },
                      { i: 3, f: 7, min: 1, max: 9999}]
    },
    emptyValue: { 'DateTime': '__/__/____ __:__',
        'Date': '__/__/____',
        'Time': '__:__',
        'Month': '__/____'
    },
    action: null,
    init: function(textBoxId, hiddenTextBoxId, mode, maskExtenderId) {
        this.mode = mode;
        this.ranges = this._ranges[mode];
        this.textboxId = textBoxId;
        this.hiddenId = hiddenTextBoxId;
        var textbox = this.getTextbox();
        //this.textbox = $get(textBoxId);
        this.maskExtenderId = maskExtenderId;
        try {
            Sys.UI.DomEvent.addHandler(textbox, "click", Type.createDelegate(this, this.OnClick));
            Sys.UI.DomEvent.addHandler(textbox, "keyup", Type.createDelegate(this, this.OnKeyUp));
            Sys.UI.DomEvent.addHandler(textbox, "keydown", Type.createDelegate(this, this.OnKeyDown));
            Sys.UI.DomEvent.addHandler(textbox, "focus", Type.createDelegate(this, this.OnFocus));
            Sys.UI.DomEvent.addHandler(textbox, "blur", Type.createDelegate(this, this.OnBlur));
            this._clear();
            this.lastValue = textbox.value;
            setTimeout(Type.createDelegate(this, this.FocusBlur), 100);
        } catch (e) { }
    },
    setEnabled: function(state) {
        this.enabled = state;
        if (this.getTextbox()) this.getTextbox().disabled = !state;
    },
    getTextbox: function() { return $get(this.textboxId); },
    getDate: function() {
        var textbox = this.getTextbox();
        if (this.mode == 'Time') {
            var hh = parseInt(textbox.value.substr(0, 2), 10);
            var mm = parseInt(textbox.value.substr(3, 2), 10);
            var now = new Date();
            var date = new Date(now.getFullYear(), now.getMonth(), now.getDate(), hh, mm, 00);
            if (!isNaN(date.getTime())) return date;
        }
        else if (this.mode == 'Month') {
            var mm = parseInt(textbox.value.substr(0, 2), 10);
            var yyyy = parseInt(textbox.value.substr(3, 4), 10);
            var date = new Date(yyyy, mm - 1, 1);
            if (!isNaN(date.getTime())) return date;
        }
        else {
            var dd = parseInt(textbox.value.substr(0, 2), 10);
            var mm = parseInt(textbox.value.substr(3, 2), 10);
            var yyyy = parseInt(textbox.value.substr(6, 4), 10);
            var hh = this.mode == 'DateTime' ? parseInt(textbox.value.substr(11, 2), 10) : 0;
            var mi = this.mode == 'DateTime' ? parseInt(textbox.value.substr(14, 2), 10) : 0;
            var date = new Date(yyyy, mm - 1, dd, hh, mi, 0);
            if (!isNaN(date.getTime())) return date;
        }
        return null;
    },
    setDate: function(date) {
        if (!date) return;

        var textbox = this.getTextbox();

        var dd = this.padLeft(date.getDate(), 2);
        var mm = this.padLeft(date.getMonth() + 1, 2);
        var yyyy = this.padLeft(date.getFullYear(), 4);
        var hh = this.padLeft(date.getHours(), 2);
        var mi = this.padLeft(date.getMinutes(), 2);

        if (this.mode == 'Time') textbox.value = hh + ':' + mi;
        else if (this.mode == 'Month') textbox.value = mm + '/' + yyyy;
        else if (this.mode == 'Date') textbox.value = dd + '/' + mm + '/' + yyyy;
        else if (this.mode == 'DateTime') textbox.value = dd + '/' + mm + '/' + yyyy + ' ' + hh + ':' + mi;
    },
    //getSelectionStart: function() { try { return this.getTextbox().selectionStart; } catch (e) { return 0; }; },
    //getSelectionEnd: function() { try { return this.getTextbox().selectionEnd; } catch (e) { return 0; } },
    getSelectionStart: function() { try { return this.getInputSelection(this.getTextbox()).start; } catch (e) { return 0; }; },
    getSelectionEnd: function() { try { return this.getInputSelection(this.getTextbox()).end; } catch (e) { return 0; } },
    getInputSelection: function(el) {
        var start = 0, end = 0, normalizedValue, range, textInputRange, len, endRange;
        if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
            start = el.selectionStart;
            end = el.selectionEnd;
        } else {
            range = document.selection.createRange();

            if (range && range.parentElement() == el) {
                len = el.value.length;
                normalizedValue = el.value.replace(/\r\n/g, "\n");

                // Create a working TextRange that lives only in the input
                textInputRange = el.createTextRange();
                textInputRange.moveToBookmark(range.getBookmark());

                // Check if the start and end of the selection are at the very end
                // of the input, since moveStart/moveEnd doesn't return what we want
                // in those cases
                endRange = el.createTextRange();
                endRange.collapse(false);

                if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                    start = end = len;
                } else {
                    start = -textInputRange.moveStart("character", -len);
                    start += normalizedValue.slice(0, start).split("\n").length - 1;

                    if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                        end = len;
                    } else {
                        end = -textInputRange.moveEnd("character", -len);
                        end += normalizedValue.slice(0, end).split("\n").length - 1;
                    }
                }
            }
        }
        return { start: start, end: end };
    },

    //setSelectionStart: function(value) { this.getTextbox().selectionStart = value; },
    //setSelectionEnd: function(value) { this.getTextbox().selectionEnd = value; },
    setSelection: function(start, end) {
        var ctrl = this.getTextbox();
        if (ctrl.setSelectionRange) // Standard way (Mozilla, Opera, ...)
        {
            ctrl.setSelectionRange(start, end);
        }
        else // MS IE
        {
            var range;
            try { range = ctrl.createTextRange(); }
            catch (e) {
                try {
                    range = document.body.createTextRange();
                    range.moveToElementText(ctrl);
                }
                catch (e) { range = null; }
            }

            if (!range) return;

            range.moveStart("character", start);
            range.collapse(true);
            range.moveEnd("character", end - start);
            range.select();
        }
    },
    selectFirstRange: function() {
        try {
            this.setSelection(this.ranges[0].i, this.ranges[0].f);
        } catch (e) { }
    },
    hasSelection: function() { return this.getSelectionStart() != this.getSelectionEnd(); },
    getRangeIndex: function() {
        if (!this.hasSelection) return -1;
        for (var i = 0; i < this.ranges.length; i++)
            if (this.getSelectionStart() == this.ranges[i].i && this.getSelectionEnd() == this.ranges[i].f) return i;
        return -1;
    },
    padLeft: function(v, n) { var p = v + ''; while (p.length < n) p = '0' + p; return p; },
    OnKeyDown: function(evt) {
        this.action = null;

        var selectedRangeIndex = this.getRangeIndex();

        if (selectedRangeIndex < 0) return true;

        var textbox = this.getTextbox();

        var range = this.ranges[selectedRangeIndex];
        var startPos = this.getSelectionStart();
        var endPos = this.getSelectionEnd();

        switch (evt.keyCode) {
            case 39: // Derecha
                if (selectedRangeIndex == this.ranges.length - 1) return false;
                this.action = this.ranges[selectedRangeIndex + 1];
                break;
            case 37: // Izquierda
                if (selectedRangeIndex == 0) return false;
                this.action = this.ranges[selectedRangeIndex - 1];
                break;
            case 38: // Arriba
                var rangeValue = parseInt(textbox.value.substring(startPos, endPos), 10);
                if (isNaN(rangeValue)) rangeValue = range.min - 1;
                if (rangeValue >= range.max) rangeValue = range.min - 1;
                textbox.value = textbox.value.substring(0, startPos)
                                    + this.padLeft(rangeValue + 1, range.f - range.i)
                                    + textbox.value.substring(endPos);
                this.action = range;
                break;
            case 40: //Abajo
                var rangeValue = parseInt(textbox.value.substring(startPos, endPos), 10);
                if (isNaN(rangeValue)) rangeValue = range.min + 1;
                if (rangeValue <= range.min) rangeValue = range.max + 1;
                textbox.value = textbox.value.substring(0, startPos)
                                    + this.padLeft(rangeValue - 1, range.f - range.i)
                                    + textbox.value.substring(endPos);
                this.action = range;
                break;
            default:
                return true;
        }
        evt.preventDefault();
        return false;
    },
    OnKeyUp: function(evt) {
        evt.preventDefault();
        if (this.action) {
            this.setSelection(this.action.i, this.action.f);
            this.action = null;
            return false;
        }
        else if (evt.keyCode == 38 || evt.keyCode == 40) return false;

        var startPos = this.getSelectionStart();

        for (var i = 0; i < this.ranges.length; i++)
            if (startPos == this.ranges[i].i) {
            this.setSelection(this.ranges[i].i, this.ranges[i].f);
            break;
        }
        evt.preventDefault();
        return false;
    },
    OnClick: function(evt) {
        evt.preventDefault();
        var startPos = this.getSelectionStart();
        for (var i = 0; i < this.ranges.length; i++)
            if (startPos <= this.ranges[i].f) {
            this.setSelection(this.ranges[i].i, this.ranges[i].f);
            break;
        }
    },
    OnFocus: function(evt) {
        evt.preventDefault();
        if (this.ignoreAll) return;
        this.selectFirstRange();
    },
    OnBlur: function(evt) {
        evt.preventDefault();
        if (this.ignoreAll) return;
        this._clear();
        if (this.on_blur) this.on_blur();
        var textbox = this.getTextbox();
        if (this.lastValue != textbox.value) {
            this.lastValue = textbox.value;
            if (this.onchange) this.onchange();
        }
    },
    _clear: function() {
        var textbox = this.getTextbox();
        if (textbox.value == this.emptyValue[this.mode]) textbox.value = '';
    },
    FocusBlur: function() {
        var textbox = this.getTextbox();
        textbox.disabled = false;
        try {
            this.ignoreAll = true;
            var ext = $find(this.maskExtenderId);
            try {
                ext._onFocus();
            } catch (e) { }
            this._clear();
            ext._onBlur(true);
            this.ignoreAll = false;
        } catch (e) { }
        textbox.disabled = !this.enabled;
    },
    CalendarChanged: function(sender, args) {
        var selectedDate = sender.get_selectedDate();
        var textbox = this.getTextbox();

        var hour = this.mode == 'DateTime' ? textbox.value.substr(10) : '';
        if (this.mode == 'DateTime' && (hour.length < 6 || hour == ' __:__')) hour = ' 00:00';

        textbox.value = (this.mode == 'Month' ? '' : this.padLeft(selectedDate.getDate(), 2) + '/')
                    + this.padLeft(selectedDate.getMonth() + 1, 2) + '/'
                    + this.padLeft(selectedDate.getFullYear(), 4) + hour;
        textbox.focus();
    },
    CalendarShowing: function(sender, args) {
        if (!this.enabled) sender.hide();
        var date = this.getDate();
        if (date == null) return;
        sender.set_selectedDate(date);
    },
    CalendarHidden: function(sender, args) {
        if (this.mode != 'Month') return;
        this.calendar = sender;
        //Iterate every month Item and remove click event from it
        if (sender._monthsBody) {
            for (var i = 0; i < sender._monthsBody.rows.length; i++) {
                var row = sender._monthsBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    Sys.UI.DomEvent.removeHandler(row.cells[j].firstChild, "click", Type.createDelegate(this, this.MonthCalendar));
                }
            }
        }
    },
    CalendarShown: function(sender, args) {
        if (this.mode != 'Month') return;
        this.calendar = sender;
        sender._switchMode("months", true, false);

        //Iterate every month Item and attach click event to it
        if (sender._monthsBody) {
            for (var i = 0; i < sender._monthsBody.rows.length; i++) {
                var row = sender._monthsBody.rows[i];
                for (var j = 0; j < row.cells.length; j++) {
                    Sys.UI.DomEvent.addHandler(row.cells[j].firstChild, "click", Type.createDelegate(this, this.MonthCalendar));
                }
            }
        }
    },
    MonthCalendar: function(eventElement) {
        var target = eventElement.target;

        switch (target.mode) {
            case "month":
                this.calendar._visibleDate = target.date;
                this.calendar.set_selectedDate(target.date);
                this.calendar._switchMonth(target.date);
                this.calendar._blur.post(true);
                this.calendar.raiseDateSelectionChanged();
                break;
        }
    }
}

    
    
   
   
   