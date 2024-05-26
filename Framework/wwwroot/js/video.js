window.video = {
    objRef: null,
    init: init,
    video: null, // 
    // Here go all the functions to manage the videos
    placeVideo: function(x, y, height, width, src){ // Function to place a video on the screen
        document.getElementById("svg").appendChild(window.video.foreignObject);
        this.vidTag.style.display = "block";
        // this.vidTag.style.top = y+"px";
        // this.vidTag.style.right = x+"px";
        // this.vidTag.style.width = width+"px";
        // this.vidTag.style.height = height+"px";
        this.foreignObject.setAttribute("x", x);
        this.foreignObject.setAttribute("y", y);
        this.foreignObject.setAttribute("width", width);
        this.foreignObject.setAttribute("height", height);
        this.srcTag.src = src;
        this.vidTag.load()
    },
    playVideo: function(){
        this.vidTag.play();
    },
    pauseVideo: function(){
        this.vidTag.pause();
    },
    letFinish: function(){
        return new Promise((resolve, reject) => {
            console.log("letFinish called on js");
            this.vidTag.onended = () => {
                // When the video is finished, change back to VideoService.cs
                console.log("onended");
                resolve();
            }
        });
    }
    
}



function init(objRef) {
    window.video.objRef = objRef;
    // Make BalzoredVideo/src tag and add them to the Document
    window.video.vidTag = document.createElement('video'); 
    window.video.vidTag.id = "vidtag";
    // window.video.vidTag.controls = true; // Not necessary, only for tests
    window.video.vidTag.setAttribute("width", "100%");
    window.video.vidTag.setAttribute("height", "100%");
    window.video.vidTag.style.display = "none"; // Hide the video-tag 

    window.video.foreignObject = document.getElementById('VidforeignObject'); // Scale this instead the video tag
    window.video.foreignObject.setAttribute("x", "100");
    window.video.foreignObject.setAttribute("y", "100");
    window.video.foreignObject.setAttribute("width", "300");
    window.video.foreignObject.setAttribute("height", "300");
    
    window.video.srcTag = document.createElement('source');
    window.video.srcTag.type = "video/mp4";

    window.video.vidTag.appendChild(window.video.srcTag);
    window.video.foreignObject.appendChild(window.video.vidTag);
}