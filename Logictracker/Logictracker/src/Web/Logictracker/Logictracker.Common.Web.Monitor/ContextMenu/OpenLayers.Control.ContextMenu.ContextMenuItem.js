OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes = 
{
        None: 0,
        JavaScript: 1,
        Callback: 2,
        Postback: 3,
        MarkAddress: 4,
        ZoomIn: 5,
        ZoomOut: 6,
        Center: 7,
        RouteFrom: 8,
        RouteTo: 9
};

OpenLayers.Control.ContextMenu.ContextMenuItem = OpenLayers.Class({
    CLASS_NAME: "OpenLayers.Control.ContextMenu.ContextMenuItem",
    behaviourType: OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.None,
    text: '',
    behaviourArguments: '',
    style: '',
    enabled: true,
    div: null,
    context: null,
    initialize: function(behaviourType, text, args, style, enabled)
    {
        this.behaviourType = behaviourType;
        this.text = text;
        this.behaviourArguments = args;
        this.style = style;
        this.enabled = enabled;
    },
    getElement: function(context)
    {
        this.context = context;
        if(this.div != null) return this.div;
        this.div = document.createElement('div');
        this.div.className = this.enabled?this.style:this.style+"_disabled";
        this.div.innerHTML = this.text;
        if(this.enabled && this.behaviourType != OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.None)
        {
            Event.observe(this.div, 'click', this.click.bindAsEventListener(this));
            Event.observe(this.div, 'mouseover', this.itemover.bindAsEventListener(this));
            Event.observe(this.div, 'mouseout', this.itemout.bindAsEventListener(this));
        }       
        return this.div;     
    },
    itemover: function(evt)
    {
        var elem = Event.element(evt);
        elem.className = this.style + '_over';    
    },
    itemout: function(evt)
    {
        var elem = Event.element(evt);
        elem.className = this.style;
    },
    click: function(evt)
    {
        switch(this.behaviourType)
        {
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.Callback:
                this.context.doCallback('ContextMenuCallback', this.behaviourArguments);
                break;
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.Postback:
                this.context.doPostback('ContextMenuPostback', this.behaviourArguments);
                break;  
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.MarkAddress:
                this.context.doCallback('MarkAddress', this.behaviourArguments);
                break;
                
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.JavaScript:
                this.context.doJavaScript(this.behaviourArguments);
                break;
                
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.ZoomIn:
                this.context.zoomIn();
                break;
                
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.ZoomOut:
                this.context.zoomOut();
                break;
                
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.Center:
                this.context.center();
                break;
                
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.RouteFrom:
                this.context.doCallback('RouteFrom', this.behaviourArguments);
                break;
                
            case OpenLayers.Control.ContextMenu.ContextMenuItemBehaviourTypes.RouteTo:
                this.context.doCallback('RouteTo', this.behaviourArguments);
                break;
        }
        this.itemout(evt);
    }
});