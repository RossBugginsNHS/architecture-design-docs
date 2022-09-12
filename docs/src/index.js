import { Canvg, presets} from 'canvg';

const preset = presets.offscreen()

var $ = require("jquery");
global.jQuery = $;
global.$ = $;
window.jQuery = $;
window.$ = $;

function UseMermaid(document)
{
    $(document).ready(function() {
        mermaid.initialize({
            logLevel: 1,
            startOnLoad: true,
            flowchart: { useMaxWidth: true, htmlLabels: false },
            mermaid: {
                callback: function(id) {
                    console.log(id);
                    addLinks(id);
                },
            },
        });
       
        window.mermaid.init(undefined, document.querySelectorAll('.language-mermaid'));
    });
}

 function addLinks(id)
{
    var svg = document.getElementById(id);
    var btn = document.createElement('button'); 
    btn.id = id +"_button";
    var pre = svg.parentNode.parentNode;
    pre.id = id + "_pre";
    btn.innerHTML = "View diagram as PNG (" + id + ")";

    //Remove to enable the png button.
    btn.style.display = "none";

    svg.parentNode.parentNode.before(btn);
   
    svg.addEventListener('mouseover', (event) => 
    { 
        event.target.style.cursor = "pointer";
    });

    svg.addEventListener('click', (event) => 
    { 
        console.log(event);
        var pngVersion = document.getElementById(id+"_png");
        if(pngVersion)
        {
            window.open(pngVersion.src)
        }
        else
        {
            drawCanvas(id, (img)=>
            {
                img.style.display = "none";
                var p = document.createElement('p'); 
                btn.after(p);
                p.appendChild(img);
                window.open(img.src); 
            });
        }
    });

    btn.addEventListener('click', function () 
    { 
        var pngVersion = document.getElementById(id+"_png");
        if(pngVersion)
        {
            if (pngVersion.style.display === "none") {
                pngVersion.style.display = "block";
                pre.style.display = "none";
                btn.innerHTML = "View diagram as SVG (" + id + ")";
              } 
              else {
                pngVersion.style.display = "none";
                pre.style.display = "block";
                btn.innerHTML = "View diagram as PNG (" + id + ")";
              }
        }
        else
        {
            btn.innerHTML = "View diagram as SVG (" + id + ")";
            drawCanvas(id, (img)=>
            {
                img.style.display = "block";
                var p = document.createElement('p'); 
                btn.after(p);
                p.appendChild(img);
                pre.style.display = "none";
            });
        }
    });
}

function drawCanvas(id, callback)
{

    var svg = document.getElementById(id);
    var { width, height } = svg.getBoundingClientRect();
    var pixelRatio = 2;//window.devicePixelRatio || 1;

    // lets scale the canvas and change its CSS width/height to make it high res.
      //  canvas.style.width = canvas.width +'px';
    var newWidth = width * pixelRatio;
    var newHeight = height * pixelRatio;

        
    var canvas =  canvas = new OffscreenCanvas(newWidth, newHeight);// document.createElement('canvas'); // Create a Canvas element.
    var ctx = canvas.getContext('2d'); // For Canvas returns 2D graphic.

   // ctx.fillStyle = 'white'; // background color for the canvas
   // ctx.fillRect(0, 0, width, height); // fill the color on the canvas


// Now that its high res we need to compensate so our images can be drawn as 
//normal, by scaling everything up by the pixelRatio.
  //  ctx.setTransform(pixelRatio,0,0,pixelRatio,0,0);

    var img =  document.createElement('img');
    var data = svg.outerHTML; // Get SVG element as HTML code.
    Canvg.from(ctx, data, preset)
        .then(result =>
            {
                result.resize(canvas.width, canvas.height, 'xMidYMid meet');
                result.render()
                    .then(function()
                    {
                       // toDataURL return DataURI as Base64 format.
                         img.id=id+"_png";
                         
                         canvas.convertToBlob()
                            .then(blobresult=>
                                {
                                    img.src = URL.createObjectURL(blobresult);
                                    callback(img);
                                });

                         //img.style.width = width +'px';
                       
                    });
        });
}


UseMermaid(document);