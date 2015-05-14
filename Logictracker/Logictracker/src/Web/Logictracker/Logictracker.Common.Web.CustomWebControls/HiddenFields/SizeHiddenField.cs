#region Usings

using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace Logictracker.Web.CustomWebControls.HiddenFields
{
    [ToolboxData("<{0}:SizeHiddenField id=\"SizeHiddenField1\" runat=\"server\"></{0}:SizeHiddenField>")]
    public class SizeHiddenField : HiddenField, IPostBackDataHandler
    {
        #region Public Properties

        /// <summary>
        /// Page default heigth.
        /// </summary>
        public int? DefaultHeight;

        /// <summary>
        /// Page default width.
        /// </summary>
        public int? DefaultWidth;

        /// <summary>
        /// Page margin heigth.
        /// </summary>
        public int MarginHeigth;

        /// <summary>
        /// Page margin width.
        /// </summary>
        public int MarginWidth;

        /// <summary>
        /// Tick event handler.
        /// </summary>
        public EventHandler Tick;

        /// <summary>
        /// Page width.
        /// </summary>
        public int Width
        {
            get
            {
                return (DefaultWidth.HasValue ? DefaultWidth.Value : !string.IsNullOrEmpty(Value) ? Convert.ToInt32(Value.Split(';')[0]) : 800) - MarginWidth;
            }
        }

        /// <summary>
        /// Page Heigth.
        /// </summary>
        public int Heigth
        {
            get
            {
                return (DefaultHeight.HasValue ? DefaultHeight.Value : !string.IsNullOrEmpty(Value) ? Convert.ToInt32(Value.Split(';')[1]) : 600) - MarginHeigth;
            }
        }

        /// <summary>
        /// Determines wither if the OnTick event is enabled.
        /// </summary>
        public bool EnableTick
        {
            get { return ViewState["EnableTick"] != null ? (bool) ViewState["EnableTick"] : false; }
            set { ViewState["EnableTick"] = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method for checking if the control triggered the post back.
        /// </summary>
        /// <param name="postDataKey"></param>
        /// <param name="postCollection"></param>
        /// <returns></returns>
        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            base.LoadPostData(postDataKey, postCollection);

            return postCollection["__EVENTTARGET"] == UniqueID;
        }

        /// <summary>
        /// Method that handles the post back trierred by the control.
        /// </summary>
        void IPostBackDataHandler.RaisePostDataChangedEvent() { if (Tick != null) Tick(this, EventArgs.Empty); }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Register the control as a post back required control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.RegisterRequiresPostBack(this);

            var scriptManager = ScriptManager.GetCurrent(Page);

            if (scriptManager != null) scriptManager.RegisterAsyncPostBackControl(this);
        }

        /// <summary>
        /// Render the control content and scripts.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            AddScripts();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds size and event handling scripts.
        /// </summary>
        private void AddScripts()
        {
            var script = string.Format(@"
                function getDimensions()
                {{
                    var winWidth, winHeight;
                    var d = document;
                    if (typeof window.innerWidth != 'undefined')
                    {{
                        winWidth = window.innerWidth;
                        winHeight = window.innerHeight;
                    }}
                    else
                    {{
                        if (d.documentElement &&typeof d.documentElement.clientWidth != 'undefined'
                            && d.documentElement.clientWidth != 0)
                        {{
                            winWidth = d.documentElement.clientWidth
                            winHeight = d.documentElement.clientHeight
                        }}
                        else
                        {{
                            if (d.body && typeof d.body.clientWidth != 'undefined')
                            {{
                                winWidth = d.body.clientWidth
                                winHeight = d.body.clientHeight
                            }}
                        }}
                    }}
                    return {{ width: winWidth, height: winHeight }};
                }}

                function setScreenValues()
                {{
                    var dimentions = getDimensions();

                    $get('{0}').value = dimentions.width + ';' + dimentions.height;
                }}
    
                setScreenValues();                

                ", ClientID);

            if (EnableTick && Tick != null && !Page.IsPostBack)
                script = string.Concat(script, string.Format("setTimeout(function() {{ {0}; }}, 0);",
                                                             Page.ClientScript.GetPostBackEventReference(this, string.Empty)));

            Page.ClientScript.RegisterStartupScript(typeof(string), "size", script, true);
        }

        #endregion
    }
}