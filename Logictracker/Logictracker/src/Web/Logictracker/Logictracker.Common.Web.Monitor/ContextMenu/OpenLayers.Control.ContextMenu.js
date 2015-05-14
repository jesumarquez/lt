// JScript File
OpenLayers.Control.ContextMenu = OpenLayers.Class(OpenLayers.Control, {
    CLASS_NAME: "OpenLayers.Control.ContextMenu",
    consultas: null,
    style: 'olPopupMenu',
    initialize: function()
    {
        this.isOpera = navigator.appName.indexOf('Opera') > -1;
        OpenLayers.Control.prototype.initialize.apply(this, arguments);
        this.events = new OpenLayers.Events(this, null, this.EVENT_TYPES, false);
    },
    /** 
     * Method: setMap
     * Set the map property for the control. This is done through an accessor
     * so that subclasses can override this and take special action once 
     * they have their map variable set. 
     *
     * Parameters:
     * map - {<OpenLayers.Map>} 
     */
    setMap: function(map) {
        OpenLayers.Control.prototype.setMap.apply(this,arguments);
        this.map.events.register('contextmenu', this, this.oncontext);
        if(this.isOpera)
        {
            Event.observe(this.map.div, 'mousedown', this.onoperamousedown.bindAsEventListener(this));
            Event.observe(this.map.div, 'mousemove', this.onoperamousemove.bindAsEventListener(this));
            Event.observe(this.map.div, 'mouseup', this.onoperamouseup.bindAsEventListener(this));
        }
    },
    /**
     * Method: createPopup
     * Creates a popup object created from the 'lonlat', 'popupSize',
     *     and 'popupContentHTML' properties set in this.data. It uses
     *     this.marker.icon as default anchor. 
     *  
     *  If no 'lonlat' is set, returns null. 
     *  If no this.marker has been created, no anchor is sent.
     *
     *  Note - the returned popup object is 'owned' by the feature, so you
     *      cannot use the popup's destroy method to discard the popup.
     *      Instead, you must use the feature's destroyPopup
     * 
     *  Note - this.popup is set to return value
     * 
     * Parameters: 
     * closeBox - {Boolean} create popup with closebox or not
     * 
     * Returns:
     * {<OpenLayers.Popup>} Returns the created popup, which is also set
     *     as 'popup' property of this feature. Will be of whatever type
     *     specified by this feature's 'popupClass' property, but must be
     *     of type <OpenLayers.Popup>.
     * 
     */
    createPopup: function(closeBox) {

        if (this.lonlat != null) {
            
            var id = this.id + "_popup";
            var anchor = (this.marker) ? this.marker.icon : null;
            this.idcontent = id + '_content';
            if (!this.popup) {
                this.popup = new OpenLayers.Popup.Anchored(id, 
                                                 this.lonlat,
                                                 new OpenLayers.Size(200,200),
                                                 '',
                                                 anchor, 
                                                 closeBox);
                //this.popup.autoSize = true;
            }    
            this.popup.contentDiv.className = this.style;    
            Event.observe(this.popup.contentDiv,'mouseout', this.onmouseout.bindAsEventListener(this));
            Event.observe(this.popup.contentDiv,'mouseover', this.onmouseover.bindAsEventListener(this));
        }        
        return this.popup;
    },
    onoperamousedown: function(evt)
    {
        OpenLayers.Event.stop(evt);
        this.curPx = evt.xy.clone();
        this.operaTime = new Date().getTime();
        setTimeout(this.ontimeout.bind(this), 500);
    },
    ontimeout: function()
    {
        if(!this.operaTime) return;

        this.showContext(this.curPx);

        this.operaTime = null;
        this.curPx = null;
    },
    onoperamousemove: function(evt)
    {
        OpenLayers.Event.stop(evt);
        if(!this.curPx)
            return;
            
        var curPx = evt.xy.clone();
        if((Math.abs(curPx.x - this.curPx.x) > 2) || (Math.abs(curPx.y - this.curPx.y) > 2))
        {
            this.operaTime = null;
        }
    },
    onoperamouseup: function(evt)
    {
        OpenLayers.Event.stop(evt);
        this.operaTime = null;
    },
    oncontext: function(evt)
    {
        OpenLayers.Event.stop(evt);
        var px = evt.xy.clone();
        px.x -=1;
        px.y -=1;
        this.showContext(px);
    },
    showContext: function(px)
    {
        this.lonlat = this.map.getLonLatFromPixel(px);
        if(this.map.currentPopup && this.map.currentPopup != this.popup)
            this.map.currentPopup.hide();
        else 
            this.deletePopup();
        if (this.popup == null) 
        {
            this.popup = this.createPopup(false);
            this.popup.calculateRelativePosition = function(px) {return "br";};
            this.popup.panMapIfOutOfView = true;
            this.map.addPopup(this.popup);
            
            for(var i = 0; i < this.items.length; i++)
                this.popup.contentDiv.appendChild(this.items[i].getElement(this));
            
            this.popup.show();
            this.popup.setBackgroundColor("");
            this.popup.contentDiv.style.height = "";
            this.popup.contentDiv.style.width = "";
        } 
        else
        {
            this.popup.moveTo(this.lonlat);
            this.popup.show();
            this.popup.setBackgroundColor("");
            this.popup.contentDiv.style.height = "";
            this.popup.contentDiv.style.width = "";
        }
        this.map.currentPopup = this.popup; 
    },
    deletePopup: function()
    {
        if(!this.deletepop)
            return;
        if(this.map.currentPopup && this.map.currentPopup == this.popup)
        {
            this.map.removePopup(this.popup);
            delete(this.popup);
            this.popup = null;
        }
    },
    onmouseout: function()
    {
        this.deletepop = true;
        this.timer = setTimeout(this.deletePopup.bind(this),200);  
    },
    onmouseover: function()
    {
        this.deletepop = false;
    },
    
    
    center: function()
    {
        this.forceDeletePopup();
        this.map.setCenter(this.lonlat);
    },
    
    zoomIn: function()
    {
        this.forceDeletePopup();
        this.map.setCenter(this.lonlat);
        this.map.zoomIn();
    },
    zoomOut: function()
    {
        this.forceDeletePopup();
        this.map.setCenter(this.lonlat);
        this.map.zoomOut();
    },
    doJavaScript: function(args)
    {
        this.forceDeletePopup();
        eval(args);
    },
    doCallback: function(commandName, commandArgs)
    {
        this.forceDeletePopup();
        var ll = this.lonlat.clone().transform(this.map.projection, this.map.displayProjection);
        CallServer(commandName + ':' + ll.lon + ',' + ll.lat + ',' + commandArgs );   
    },
    doPostback: function(commandName, commandArgs)
    {
        this.forceDeletePopup();
        var ll = this.lonlat.clone().transform(this.map.projection, this.map.displayProjection);
        CallServerPB(commandName + ':' + ll.lon + ',' + ll.lat + ',' + commandArgs );   
    },
    forceDeletePopup: function()
    {
        this.deletepop = true;
        this.deletePopup();
    }
});
