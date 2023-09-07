window.triggerClick = (elt) => elt.click();
window.JsFunctions = {
    addKeyboardListenerEvent: function (foo) {
        let serializeEvent = function (e) {
            if (e) {
                return {
                    key: e.key,
                    code: e.keyCode.toString(),
                    location: e.location,
                    repeat: e.repeat,
                    ctrlKey: e.ctrlKey,
                    shiftKey: e.shiftKey,
                    altKey: e.altKey,
                    metaKey: e.metaKey,
                    type: e.type
                };
            }
        };

        window.document.addEventListener('keydown', function (e) {
            DotNet.invokeMethodAsync('SupasharpTodo.BlazorWASM', 'JsKeyDown', serializeEvent(e))
        });
    }
};