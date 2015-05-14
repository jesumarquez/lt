var StockDiario = function() {
    this.dias = {};
};

StockDiario.prototype =
{
    addDia: function(dia) {
        this.dias[dia] = new StockDia(dia, this);
    },
    getDia: function(dia) {
        if (!this.dias[dia]) this.addDia(dia);
        return this.dias[dia];
    },
    addInicial: function(dia, txt) {
        this.getDia(dia).addInicial(txt);
    },
    addIngreso: function(dia, txt) {
        this.getDia(dia).addIngreso(txt);
    },
    addEgreso: function(dia, txt) {
        this.getDia(dia).addEgreso(txt);
    },
    addStock: function(dia, txt) {
        this.getDia(dia).addStock(txt);
    }
};

var StockDia = function(dia, parent) {
    this.dia = dia;
    this.parent = parent;
    this.inicial = null;
    this.ingresos = new Array();
    this.egresos = new Array();
    this.stock = null;
};

StockDia.prototype =
{
    addInicial: function(txt) {
        this.inicial = txt;
        if (this.inicial && $get(this.inicial)) $addHandler($get(this.inicial), 'change', Type.createDelegate(this, this.changed));
        this.changed(this, null);
    },
    addStock: function(txt) {
    this.stock = txt;
    this.changed(this, null);
    },
    addIngreso: function(txt) {
        var ing = txt;
        if (ing && $get(ing)) {
            this.ingresos.push(ing);
            $addHandler($get(ing), 'change', Type.createDelegate(this, this.changed));
        }
        this.changed(this, null);
    },
    addEgreso: function(txt) {
        var egr = txt;
        if (egr && $get(egr)) {
            this.egresos.push(egr);
            $addHandler($get(egr), 'change', Type.createDelegate(this, this.changed));
        }
        this.changed(this, null);
    },
    changed: function(sender, eventArgs) {
        var stock = 0;
        if (this.inicial && $get(this.inicial)) {
            var val = parseInt($get(this.inicial).nodeName.toLowerCase() == 'input' ? $get(this.inicial).value : $get(this.inicial).innerHTML, 10);
            stock = isNaN(val) ? 0 : val;
        }
        for (var i = 0; i < this.ingresos.length; i++)
            if (this.ingresos[i] && $get(this.ingresos[i])) {
            var val = parseInt($get(this.ingresos[i]).value, 10);
            stock += isNaN(val) ? 0 : val;
        }

        for (var i = 0; i < this.egresos.length; i++)
            if (this.egresos[i] && $get(this.egresos[i])) {
            var val = parseInt($get(this.egresos[i]).value, 10);
            stock -= isNaN(val) ? 0 : val;
        }
        if (this.stock && $get(this.stock)) {
            $get(this.stock).innerHTML = stock;
            var next = this.parent.dias[(parseInt(this.dia, 10) + 1) + ''];
            if (next) next.changed(this, null);
        }
    }
};