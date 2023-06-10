(function(win, mod){
    mod.preventReconnection = () => {
        win.Blazor.defaultReconnectionHandler.onConnectionDown = _ => {}
    }
})(window, window.MyHelpers = (window.MyHelpers || {}))