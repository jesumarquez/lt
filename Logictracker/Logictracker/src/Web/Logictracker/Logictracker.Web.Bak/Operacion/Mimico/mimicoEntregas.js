var item_template = 'item_template';

var vehicle_template_name = '{{{VEHICLE}}}';
var completed_template_name = '{{{COMPLETED}}}';
var completed_text_template_name = '{{{COMPLETED_TEXT}}}';
var state_bars_template_name = '{{{STATE_BARS}}}';

var state_bar_template = '<div class="tramodiv {{{STATE}}}" style="left: {{{POSITION}}};width: {{{SIZE}}};" title="{{{TITLE}}}">{{{TEXT}}}</div>';

var tickets = {};

function mimico(parentDiv, options)
{
    this.parent = document.getElementById(parentDiv);
    this.item_template = item_template;
    this.states = new Array();
    this.apply(options);
}
mimico.prototype =
{
    parent: null,
    div: null,
    interno: '',
    completed: 100,
    states: null,
    render: function() {
        if (this.div == null) {
            this.div = document.createElement('div');
            this.parent.appendChild(this.div);
        }
                
        var state_bars = this.render_state_bars();

        var totalWidth = this.states.length * 40
        var completado = totalWidth * this.completed / 100

        var template = document.getElementById(this.item_template).innerHTML;
        template = template.replace(vehicle_template_name, this.interno)
                           .replace(completed_template_name, completado + 'px')
                           .replace(completed_text_template_name, this.completed + '%')                           
                           .replace(state_bars_template_name, state_bars)
                           .replace('{{{VEHICLE_STATE}}}', this.style)
                           .replace('{{{ICON}}}', this.icon)
                           .replace('{{{ID_RUTA}}}', this.id)
                           .replace('{{{COMPLETED_BAR}}}', completado + 'px');

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

            var color = 'azul';
            if (state.estado == 0) color = 'naranja';
            else if (state.estado == 1) color = 'azul';
            else if (state.estado == 2) color = 'amarillo';
            else if (state.estado == 3) color = 'naranja';
            else if (state.estado == 4) color = 'gris';
            else if (state.estado == 8) color = 'rojo';
            else if (state.estado == 9) color = 'verde';

            cur_template = cur_template.replace('{{{POSITION}}}', accum + 'px')
                                           .replace('{{{SIZE}}}', '40px')
                                           .replace('{{{TITLE}}}', state.name)
                                           .replace('{{{TEXT}}}', state.name)
                                           .replace('{{{STATE}}}', color);

            accum += 40;
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
            this.div.style.width = '200px';
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
