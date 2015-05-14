/*
if (typeof (Sys.Browser.WebKit) == "undefined") {
    Sys.Browser.WebKit = {};
}
if (navigator.userAgent.indexOf("WebKit/") > -1) {
    Sys.Browser.agent = Sys.Browser.WebKit;
    Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
    Sys.Browser.name = "WebKit";
}
*/

 (function() {
     /* patch MS AJAX to support webkit browsers */
     function patchMicrosoftAjax() {
         Sys.Browser = {};
         Sys.Browser.InternetExplorer = {};
         Sys.Browser.Firefox = {};
         Sys.Browser.WebKit = {}; //Safari 3 is considered WebKit 
         Sys.Browser.Safari = {};
         Sys.Browser.Opera = {};
         Sys.Browser.agent = null;
         Sys.Browser.hasDebuggerStatement = false;
         Sys.Browser.name = navigator.appName;
         Sys.Browser.version = parseFloat(navigator.appVersion);
         if (navigator.userAgent.indexOf(' MSIE ') > -1) {
             Sys.Browser.agent = Sys.Browser.InternetExplorer;
             Sys.Browser.version = parseFloat(navigator.userAgent.match(/MSIE (\d+\.\d+)/)[1]);
             Sys.Browser.hasDebuggerStatement = true;
         } else if (navigator.userAgent.indexOf(' Firefox/') > -1) {
             Sys.Browser.agent = Sys.Browser.Firefox;
             Sys.Browser.version = parseFloat(navigator.userAgent.match(/ Firefox\/(\d+\.\d+)/)[1]);
             Sys.Browser.name = 'Firefox';
             Sys.Browser.hasDebuggerStatement = true;
         } else if (navigator.userAgent.indexOf('WebKit/') > -1) {
             Sys.Browser.agent = Sys.Browser.WebKit;
             Sys.Browser.version = parseFloat(navigator.userAgent.match(/WebKit\/(\d+(\.\d+)?)/)[1]);
             Sys.Browser.name = 'WebKit';
         } else if (navigator.userAgent.indexOf(' Safari/') > -1) {
             Sys.Browser.agent = Sys.Browser.Safari;
             Sys.Browser.version = parseFloat(navigator.userAgent.match(/ Safari\/(\d+(\.\d+)?)/)[1]);
             Sys.Browser.name = 'Safari';
         } else if (navigator.userAgent.indexOf('Opera/') > -1) {
             Sys.Browser.agent = Sys.Browser.Opera;
         }
     }
     patchMicrosoftAjax();
 })();