var fs = require('fs');
let jsURL = '/build/js/si-chunk-include.js'

fs.readFile(__dirname+'/build/index.html','utf8', (err, html)=>{

    let getSlicedScript = (newStr)=>{
        return newStr.slice(newStr.indexOf('</script>')+9, newStr.lastIndexOf('</script>')+9)
    },
    getScript = (newStr)=>{
        return newStr.substring(newStr.indexOf('<script src=')+12, newStr.indexOf('></script>'))
    },
    script1 = html.substring(html.indexOf('<script>')+8 , html.indexOf('</script>')).trim(),
    newStr = getSlicedScript(html),
    scriptSrc1 = getScript(newStr)

    newStr = getSlicedScript(newStr)
    
    let scriptSrc2 = getScript(newStr),
    loadJsScript = `\n var loadJS = function (path) {
        var script = document.createElement('script');
        script.src = path;
        script.type = 'text/javascript';
        document.body.appendChild(script);
    }
    loadJS(${scriptSrc1})
    loadJS(${scriptSrc2})
    if (typeof Object.assign != 'function') {
        // Must be writable: true, enumerable: false, configurable: true
        Object.defineProperty(Object, "assign", {
          value: function assign(target, varArgs) { // .length of function is 2
            'use strict';
            if (target == null) { // TypeError if undefined or null
              throw new TypeError('Cannot convert undefined or null to object');
            }
      
            var to = Object(target);
      
            for (var index = 1; index < arguments.length; index++) {
              var nextSource = arguments[index];
      
              if (nextSource != null) { // Skip over if undefined or null
                for (var nextKey in nextSource) {
                  // Avoid bugs when hasOwnProperty is shadowed
                  if (Object.prototype.hasOwnProperty.call(nextSource, nextKey)) {
                    to[nextKey] = nextSource[nextKey];
                  }
                }
              }
            }
            return to;
          },
          writable: true,
          configurable: true
        });
      }`
    
    fs.writeFile(__dirname + jsURL,script1 + loadJsScript,()=>{
        console.log('success')
    })
})