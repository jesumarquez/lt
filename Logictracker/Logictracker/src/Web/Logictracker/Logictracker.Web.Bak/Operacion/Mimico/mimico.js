var item_template = 'item_template';
//var state_time_template = 'state_time_template';
//var state_bar_template = 'state_bar_template';

var vehicle_template_name = '{{{VEHICLE}}}';
var completed_template_name = '{{{COMPLETED}}}';
var completed_text_template_name = '{{{COMPLETED_TEXT}}}';
var state_times_template_name = '{{{STATE_TIMES}}}';
var state_bars_template_name = '{{{STATE_BARS}}}';

var state_time_template = '<div class="horario" style="{{{POSITION_PROP}}}: {{{POSITION}}};width: {{{SIZE}}};" title="{{{TITLE}}}"><span class="programado">{{{PROG_TIME}}}</span> <span class="real">{{{REAL_TIME}}}</span></div>';
var state_bar_template = '<div class="tramodiv {{{STATE}}}" style="left: {{{POSITION}}};width: {{{SIZE}}};" title="{{{TITLE}}}">{{{TEXT}}}</div>';

var tickets = {};

function mimico(parentDiv, options)
{
    this.parent = document.getElementById(parentDiv);
    this.item_template = item_template;
    //this.state_time_template = state_time_template;
    //this.state_bar_template = state_bar_template;
    this.states = new Array();
    this.apply(options);
}
mimico.prototype =
{
    parent: null,
    div: null,
    interno: '',
    completed: 40,
    states: null,
    render: function() {
        if (this.div == null) {
            this.div = document.createElement('div');
            this.parent.appendChild(this.div);
        }

        var state_times = this.render_state_times();
        var state_bars = this.render_state_bars();


        var template = document.getElementById(this.item_template).innerHTML;
        template = template.replace(vehicle_template_name, this.interno)
                           .replace(completed_template_name, this.completed + '%')
                           .replace(completed_text_template_name, this.completed + '%')
                           .replace(state_times_template_name, state_times)
                           .replace(state_bars_template_name, state_bars)
                           .replace('{{{VEHICLE_STATE}}}', this.style)
                           .replace('{{{ICON}}}', this.icon)
                           .replace('{{{COMPLETED_BAR}}}', this.completed + '%');

        this.div.innerHTML = template;

        var tramos = getElementsByClassName('tramodiv', this.div);
        for (var i = 0; i < this.states.length; i++) {
            var tramo = tramos[i];
            var state = this.states[i];
            if (tramo == null || state == null) continue;
            state.div = tramo;
            Sys.UI.DomEvent.addHandler(tramo, "click", Type.createDelegate(state, state.OnClick));
            Sys.UI.DomEvent.addHandler(tramo, "mouseover", Type.createDelegate(state, state.OnMouseMove));
            Sys.UI.DomEvent.addHandler(tramo, "mouseout", Type.createDelegate(state, state.OnMouseOut));
        }


        this.fullmode(infullmode);
    },
    fullmode: function(state) {
        if (state) this.showbars();
        else this.hidebars();
    },
    hidebars: function() {
        getElementsByClassName('bars', this.div)[0].style.display = 'none';
        getElementsByClassName('marker_line', this.div)[0].style.display = 'none';
        getElementsByClassName('marker_icon', this.div)[0].style.display = 'none';

        var txt = getElementsByClassName('marker_text', this.div)[0].style.top = '30px';
        var bar = getElementsByClassName('completed_bar', this.div)[0];
        bar.style.height = '16px';
        bar.style.top = '12px';
    },
    showbars: function() {
        getElementsByClassName('bars', this.div)[0].style.display = '';
        getElementsByClassName('marker_line', this.div)[0].style.display = '';
        getElementsByClassName('marker_icon', this.div)[0].style.display = '';
        var txt = getElementsByClassName('marker_text', this.div)[0].style.top = null;
        var bar = getElementsByClassName('completed_bar', this.div)[0];
        bar.style.height = null;
        bar.style.top = null;
    },
    render_state_times: function() {
        var result = '';

        var total = this.getTotalTime();
        var roundup = 0;
        var template = state_time_template;
        var accum = 0;

        var lastTime = '';
        for (var i = 0; i < this.states.length; i++) {
            var state = this.states[i];
            var next = i + 1 < this.states.length ? this.states[i + 1] : null;
            var cur_template = template;

            var size = 'none';
            var pos_prop = 'right';

            if (next != null) {
                var time = this.getStateTime(i);
                var dsize = time * 100 / total;
                var resto = time * 100 % total;
                if (roundup > resto) {
                    size = Math.ceil(dsize);
                    roundup -= resto;
                }
                else {
                    size = Math.floor(dsize);
                    roundup += resto;
                }

                pos_prop = 'left';
            }
            else accum = 0;

            var prog = this.formatTime(state.programmed);
            if (prog == lastTime) prog = '';
            else lastTime = prog;

            var real = state.real ? ' - ' + this.formatTime(state.real) : '';
            var title = prog + real;
            var pos_prop = next != null ? 'left' : 'right';
            var pos = next != null ? accum + '%' : '0px';
            var show_size = size + '%';

            cur_template = cur_template.replace('{{{POSITION_PROP}}}', pos_prop)
                                           .replace('{{{POSITION}}}', pos)
                                           .replace('{{{SIZE}}}', show_size)
                                           .replace('{{{TITLE}}}', title)
                                           .replace('{{{PROG_TIME}}}', prog)
                                           .replace('{{{REAL_TIME}}}', real);

            accum += size;
            result += cur_template;
        }
        return result;
    },
    render_state_bars: function() {
        var result = '';

        var total = this.getTotalTime();
        if (total == 0) total = 1;
        var template = state_bar_template;
        var accum = 0;
        var roundup = 0;

        for (var i = 0; i < this.states.length; i++) {
            var state = this.states[i];
            var cur_template = template;

            var time = this.getStateTime(i);
            var dsize = time * 100 / total;
            var resto = time * 100 % total;
            var size = 1;

            if (roundup > resto) {
                size = Math.ceil(dsize);
                roundup -= resto;
            }
            else {
                size = Math.floor(dsize);
                roundup += resto;
            }

            if (i == this.states.length - 2) size = 100 - accum;

            var prog = state.programmed;
            var real = state.real ? ' - ' + state.real : '';
            var title = prog + real;

            var color = 'azul';
            if (state.real) {
                var atraso = (state.real.getTime() - state.programmed.getTime()) / 60000;
                if (atraso > 5 && atraso < 15) color = 'amarillo';
                else if (atraso > 15) color = 'rojo';
                else color = 'verde';
            }


            cur_template = cur_template.replace('{{{POSITION}}}', accum + '%')
                                           .replace('{{{SIZE}}}', size + '%')
                                           .replace('{{{TITLE}}}', state.name)
                                           .replace('{{{TEXT}}}', state.name)
                                           .replace('{{{STATE}}}', color);

            accum += size;
            result += cur_template;
        }
        return result;
    },
    apply: function(options) {
        if (!options) return;
        for (w in options) this[w] = options[w];
    },
    getStateTime: function(index) {
        var state = this.states[index];
        var next = index + 1 < this.states.length ? this.states[index + 1] : null;
        if (next != null) {
            var time = next.programmed.getTime() - state.programmed.getTime();
            if (time < 300000) time = 300000;
            return time;
        }
        return 0;
    },
    getTotalTime: function() {
        if (!this.states || this.states.length < 2) return 0;
        //var from = this.states[0];
        //var to = this.states[this.states.length - 1];
        //return to.programmed.getTime() - from.programmed.getTime();
        var totalTime = 0;
        for (var i = 0; i < this.states.length; i++) {
            totalTime += this.getStateTime(i);
        }
        return totalTime;
    },
    formatTime: function(date) {
        return this.padLeft(date.getHours(), 2) + ':' + this.padLeft(date.getMinutes(), 2);
    },
    padLeft: function(v, n) { var p = v + ''; while (p.length < n) p = '0' + p; return p; }
};

function getElementsByClassName(clsName, obj, arr) 
{
    if(!arr) arr = new Array();
    if(!obj) obj = document;
    if (obj.className == clsName) arr.push(obj);
    else if(obj.className && obj.className.indexOf(' ') >= 0)
    {
        var classes = obj.className.split(' ');
        for(var i = 0; i < classes.length; i++)
        {
            if(classes[i] == clsName) 
            {
                arr.push(obj);
                break;
            }
        }
    }
    for (var i = 0; i < obj.childNodes.length; i++) getElementsByClassName(clsName, obj.childNodes[i], arr);        
    return arr;
}

function estado(mimico, options)
{
    this.mimico = mimico;
    this.apply(options);
}
estado.prototype =
{
    id: 0,
    name: '',
    programmed: new Date(),
    real: null,
    apply: function(options) {
        if (!options) return;
        for (w in options) this[w] = options[w];
    },
    OnClick: function(e) {
        this.showBox();
    },
    OnMouseOut: function(e) {
        this.close = true;
        setTimeout(Type.createDelegate(this, this.closeBox), 500);
    },
    OnMouseMove: function(e) {
        this.close = false;
    },
    showBox: function() {
        var show = !this.opened;
        this.div.style.height = show ? 'auto' : null;
        this.div.style.zIndex = show ? '9999999' : null;

        if (show) {
            this.w = this.div.style.width;
            if (parseInt(this.div.style.width, 10) * zoomlevel < 20) this.div.style.width = '200px';
            this.div.innerHTML = this.details;
            this.mouseoutdelegate = Type.createDelegate(this, this.OnMouseOut);
            this.mousemovedelegate = Type.createDelegate(this, this.OnMouseMove);
            this.close = false;
        }
        else {
            this.div.style.width = this.w;
            this.div.innerHTML = this.name;
            this.mouseoutdelegate = null;
            this.mousemovedelegate = null;
            this.close = false;
        }
        this.opened = show;
    },
    closeBox: function() {
        if (!this.close) return;
        if (this.opened) this.showBox();
    }
};

function status(txt, time)
{
    document.getElementById("div_mensajes").innerHTML = txt; 
    if(time <= 0) return;
    setTimeout(function(){ status('',0); }, time);
}

function clearMimicos()
{
    tickets = {};
    document.getElementById('content').innerHTML = '';
}
function addMimico(name, options, states)
{                   
    var m = tickets[name];
    
    if(!m) m = new mimico('content', options);
    else m.apply(options);
    
    m.states = new Array();    
    
    for( var i = 0; i< states.length; i++)
    {
        var state = states[i];
        var e = new estado(m, state);
        m.states.push(e);
    }
     
    m.render();
    tickets[name] = m;
}
var infullmode = true;
function FullMode(state)
{
    for(w in tickets) tickets[w].fullmode(state);
    document.getElementById('fullmode').className = state ? 'fullmodeselected' : 'fullmodeunselected';
    document.getElementById('simplemode').className = state ? 'simplemodeunselected' : 'simplemodeselected';
    infullmode = state;
}

var maxzoomlevel = 4;
var zoomlevel = 1;
function Zoom() {
    zoomlevel = zoomlevel == maxzoomlevel ? 1 : zoomlevel * 2;
    document.getElementById('changezoom').className = 'zoomx' + zoomlevel;
    
    document.getElementById('content').style.width = (zoomlevel * 100) + "%";
    
}
