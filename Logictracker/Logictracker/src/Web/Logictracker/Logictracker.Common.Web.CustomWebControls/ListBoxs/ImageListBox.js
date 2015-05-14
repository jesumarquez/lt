var ImageListBox = function(divid)
{
    this.div = $get(divid);
    this.trlist = this.div.getElementsByTagName('tr');
    for(var i=0; i< this.trlist.length;i++) this.trlist[i].index = i;
    this.addEvents();
}
ImageListBox.prototype = 
{
    selected: [],
    firstSelected: function() { return this.selected.length == 0 ? -1 : this.getIndex(this.selected[0]); },
    clickIndex: -1,
    overIndex: -1,
    addEvents: function()
    {        
        this.onmousedownHandler = Function.createDelegate(this, this.onmousedown);
        this.onmouseupHandler = Function.createDelegate(this, this.onmouseup);
        this.onmouseoutHandler = Function.createDelegate(this, this.onmouseout);
        this.onmouseoverHandler = Function.createDelegate(this, this.onmouseover);
        this.onkeydownHandler = Function.createDelegate(this, this.onkeydown);
        
        for(var i = 0; i < this.trlist.length; i++)
        {
            $addHandlers(this.trlist[i],
            {'mousedown':this.onmousedownHandler,
            'mouseup': this.onmouseupHandler,
            'mouseout': this.onmouseoutHandler,
            'mouseover': this.onmouseoverHandler,
            'keydown': this.onkeydownHandler
            }, this);
        }
        $addHandlers(this.div,
            {
            'keydown': this.onkeydownHandler,
            'keypress': this.onkeydownHandler
            }, this);
    },
    getIndex: function(tr) { return parseInt(tr.index); },
    getItemFromEvent: function(evt)
    {
        var tr = evt.target;
        while(tr.nodeName.toLowerCase() != 'tr') tr = tr.parentNode;
        return tr;
    },
    onmousedown: function(evt)
    {
        evt.preventDefault();
        var tr = this.getItemFromEvent(evt);
        
        if(!evt.ctrlKey && !evt.shiftKey) 
        {
            this.clearSelected();
            this.toggle(tr);
        }
        else if(evt.ctrlKey && !evt.shiftKey)
        {            
            this.toggle(tr);
        }
        else if(evt.shiftKey && !evt.ctrlKey && this.clickIndex < 0)
        {
            this.toggle(tr);
        }
        else if(evt.shiftKey && !evt.ctrlKey && this.clickIndex > -1)
        {
            var first = this.clickIndex;
            this.clearSelected();
            this.toggleRange(first, this.getIndex(tr));
        }
        else if(evt.shiftKey && evt.ctrlKey)
        {
            var first = this.clickIndex;
            this.toggleRange(first, this.getIndex(tr));
        }
     
        this.down = true;
        this.clickIndex = this.getIndex(tr);
        this.overIndex = this.clickIndex;
        this.selectedIndex = this.clickIndex;
        this.div.focus();
    },
    onmouseup: function(evt)
    {
        evt.preventDefault();
        this.down = false;
    },
    onmouseout: function(evt) { evt.preventDefault(); },
    onmouseover: function(evt)
    {
        evt.preventDefault();
        if(this.down){
            var tr = this.getItemFromEvent(evt);
            if(this.overIndex == this.getIndex(tr)) return;
            else if(this.overIndex > 0) this.clearRange(this.clickIndex, this.overIndex);
            this.overIndex = this.getIndex(tr);
            if(!evt.ctrlKey) this.clearSelected();
            this.toggleRange(this.clickIndex, this.overIndex);
            this.selectedIndex = this.overIndex;
        }
    },
    onkeydown: function(evt)
    {
        if(!evt.shiftKey && evt.charCode == Sys.UI.Key.down)
        {
            evt.preventDefault();
            this.clearSelected();
            if(this.selectedIndex + 1 < this.trlist.length) this.selectedIndex++;
            var tr = this.trlist[this.selectedIndex];
            this.select(tr);
        }
        else if(!evt.shiftKey && evt.charCode == Sys.UI.Key.up)
        {
            evt.preventDefault();
            this.clearSelected();
            if(this.selectedIndex > 0) this.selectedIndex--;
            var tr = this.trlist[this.selectedIndex];
            this.select(tr);
        }
        /*if(!evt.shiftKey) return;
        //$get('debug').innerHTML += evt.charCode +"<br/>";
        if (evt.charCode == Sys.UI.Key.down) {
            evt.preventDefault();
            if(this.overIndex + 1 >= this.trlist.length) return;
            this.overIndex++;
            var tr = this.trlist[this.overIndex];
            this.toggle(tr);
        }
        if (evt.charCode == Sys.UI.Key.up) {
            evt.preventDefault();
            if(this.overIndex - 1 < 0) return;
            this.overIndex--;
            var tr = this.trlist[this.overIndex];
            this.toggle(tr);
        }*/
    },
    toggle: function(tr)
    {
        if(tr.selected) this.unselect(tr);
        else this.select(tr);
    },
    toggleRange: function(startIndex, endIndex)
    {
        var ini = Math.min(startIndex, endIndex);
        var fin = Math.max(startIndex, endIndex);
        for(var i = ini; i <= fin; i++) 
            this.toggle(this.trlist[i]);
    },
    selectRange: function(startIndex, endIndex)
    {
        var ini = Math.min(startIndex, endIndex);
        var fin = Math.max(startIndex, endIndex);
        for(var i = ini; i <= fin; i++) 
            this.select(this.trlist[i]);
    },
    select: function(tr)
    {
        this.addSelected(tr);        
    },
    unselect: function(tr)
    {
        this.removeSelected(tr);
    },
    addSelected: function(tr)
    {
        this.selected.push(tr);
        tr.style.backgroundColor= 'Red';
        tr["selected"] = true;
    },
    removeSelected: function(tr)
    {
        for(var i = 0; i < this.selected.length; i++) 
        {
            if(this.selected[i] == tr) 
            {
                this.selected[i].style.backgroundColor= '';
                this.selected[i]["selected"] = false;
                this.selected.splice(i, 1);                
                break;
            }
        }
    },
    clearRange: function(startIndex, endIndex)
    {
        var ini = Math.min(startIndex, endIndex);
        var fin = Math.max(startIndex, endIndex);
        for(var i = ini; i <= fin; i++) this.removeSelected(this.trlist[i]);
    },
    clearSelected: function()
    {
        for(var i = 0; i < this.selected.length; i++) 
        {
            this.selected[i].style.backgroundColor= '';
            this.selected[i]["selected"] = false;              
        }
        this.selected.splice(0); 
    }
}