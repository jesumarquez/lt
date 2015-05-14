OpenLayers.Marker.Labeled = OpenLayers.Class({
    
    /** 
     * Property: icon 
     * {<OpenLayers.Icon>} The icon used by this marker.
     */
    icon: null,

    /** 
     * Property: lonlat 
     * {<OpenLayers.LonLat>} location of object
     */
    lonlat: null,
    
    /** 
     * Property: events 
     * {<OpenLayers.Events>} the event handler.
     */
    events: null,
    
    /** 
     * Property: map 
     * {<OpenLayers.Map>} the map this marker is attached to
     */
    map: null,
    cssStyle: 'ol_marker_labeled',
    label: null,
    /** 
     * Constructor: OpenLayers.Marker
     * Paraemeters:
     * icon - {<OpenLayers.Icon>}  the icon for this marker
     * lonlat - {<OpenLayers.LonLat>} the position of this marker
     */
    initialize: function(lonlat, icon, label, cssStyle) {
        this.lonlat = lonlat;
        
        var newIcon = (icon) ? icon : OpenLayers.Marker.defaultIcon();
        if (this.icon == null) {
            this.icon = newIcon;
        } else {
            this.icon.url = newIcon.url;
            this.icon.size = newIcon.size;
            this.icon.offset = newIcon.offset;
            this.icon.calculateOffset = newIcon.calculateOffset;
        }
        this.label = label;
        if(cssStyle) this.cssStyle = cssStyle;
        this.events = new OpenLayers.Events(this, this.icon.imageDiv, null);
    },
    
    /**
     * APIMethod: destroy
     * Destroy the marker. You must first remove the marker from any 
     * layer which it has been added to, or you will get buggy behavior.
     * (This can not be done within the marker since the marker does not
     * know which layer it is attached to.)
     */
    destroy: function() {
        // erase any drawn features
        this.erase();

        this.map = null;

        this.events.destroy();
        this.events = null;

        if (this.icon != null) {
            this.icon.destroy();
            this.icon = null;
        }
    },
    
    /** 
    * Method: draw
    * Calls draw on the icon, and returns that output.
    * 
    * Parameters:
    * px - {<OpenLayers.Pixel>}
    * 
    * Returns:
    * {DOMElement} A new DOM Image with this marker's icon set at the 
    * location passed-in
    */
    doneLabel: false,
    draw: function(px) {
        var icn = this.icon.draw(px);
        if(this.label)
        {
            if(this.doneLabel)
                icn.removeChild(icn.firstChild);
                        
            this.div = document.createElement('div');
            this.div.className = this.cssStyle;
            this.div.innerHTML = '<acronym title="'+this.label+'">'+this.label.replace(' ', '&nbsp;')+'</acronym>';
            this.div.style.cursor = 'pointer';
            //this.div.style.width = this.icon.size.w + 'px';
            this.div.style.width = 'auto';
            //this.div.style.left = '-1px';
            //this.div.style.overflow = 'hidden';
            this.div.style.position = 'absolute';
            //this.div.style.top = '-14px';
            this.div.style['white-space'] = 'nowrap';
            this.div.style.height = 'auto';
            icn.style.width = 'auto';
            icn.insertBefore(this.div,icn.firstChild);
            
            this.div.style.left = (-(this.div.offsetWidth - this.icon.size.w) / 2 ) + 'px';
            this.div.style.top = (-this.div.offsetHeight - 3 ) + 'px';
            
            this.doneLabel = true;       
        }
        return icn;
    },  
    /** 
    * Method: erase
    * Erases any drawn elements for this marker.
    */
    erase: function() {
        if (this.icon != null) {
            this.icon.erase();
        }
    }, 
    /**
    * Method: moveTo
    * Move the marker to the new location.
    *
    * Parameters:
    * px - {<OpenLayers.Pixel>} the pixel position to move to
    */
    moveTo: function (px) {
        if ((px != null) && (this.icon != null)) {
            this.icon.moveTo(px);
        }           
        this.lonlat = this.map.getLonLatFromLayerPx(px);
    },
    /**
     * APIMethod: isDrawn
     * 
     * Returns:
     * {Boolean} Whether or not the marker is drawn.
     */
    isDrawn: function() {
        var isDrawn = (this.icon && this.icon.isDrawn());
        return isDrawn;   
    },
    /**
     * Method: onScreen
     *
     * Returns:
     * {Boolean} Whether or not the marker is currently visible on screen.
     */
    onScreen:function() {
        
        var onScreen = false;
        if (this.map) {
            var screenBounds = this.map.getExtent();
            onScreen = screenBounds.containsLonLat(this.lonlat);
        }    
        return onScreen;
    },
    
    /**
     * Method: inflate
     * Englarges the markers icon by the specified ratio.
     *
     * Parameters:
     * inflate - {float} the ratio to enlarge the marker by (passing 2
     *                   will double the size).
     */
    inflate: function(inflate) {
        if (this.icon) {
            var newSize = new OpenLayers.Size(this.icon.size.w * inflate,
                                              this.icon.size.h * inflate);
            this.icon.setSize(newSize);
        }        
    },
    
    /** 
     * Method: setOpacity
     * Change the opacity of the marker by changin the opacity of 
     *   its icon
     * 
     * Parameters:
     * opacity - {float}  Specified as fraction (0.4, etc)
     */
    setOpacity: function(opacity) {
        this.icon.setOpacity(opacity);
    },

    /**
     * Method: setUrl
     * Change URL of the Icon Image.
     * 
     * url - {String} 
     */
    setUrl: function(url) {
        this.icon.setUrl(url);
    },    

    /** 
     * Method: display
     * Hide or show the icon
     * 
     * display - {Boolean} 
     */
    display: function(display) {
        this.icon.display(display);
    },

    CLASS_NAME: "OpenLayers.Marker.Labeled"
});