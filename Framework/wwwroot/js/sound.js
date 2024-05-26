window.sound = {
    objRef: null,
    init: init,
    sound: null,
    music: null,
    currentLoop: null,
    playSound: function (path) { // Function to play a sound via path
        this.sfx.src = path; 
        this.sfx.play();
    },
    playMusic: function (path) { // Function to play backgroundmusic via path
        this.music.src = path;
        var that = this; // Save a reference to 'this'
        loopify(this.music.src, function(err,loop) {
            // If something went wrong, `err` is supplied
            if (err) {
                return console.err(err);
            }
            // If there's a current loop, stop it
            if (that.currentLoop) {
                that.currentLoop.stop();
            }
            // Play the loop, stop it with loop.stop()
            loop.play();
            that.currentLoop = loop; // Set the current loop to the new loop
        });
    },
    stopMusic: function(){
        if (this.currentLoop) {
            this.currentLoop.stop();
        }
    }
    
};


function init(objRef) {
    window.sound.objRef = objRef;
    // create two audio tags, sfx for audio, and music for the background track
    window.sound.sfx = document.createElement('audio');
    window.sound.music = document.createElement('audio');
    window.sound.music.src = "/audio/ambient-piano-loop-85bpm.wav";
    document.body.appendChild(window.sound.sfx);
    document.body.appendChild(window.sound.music);

}


(function() { // From https://github.com/veltman/loopify?tab=readme-ov-file

    function loopify(uri,cb) {

      var context = new (window.AudioContext || window.webkitAudioContext)(),
          request = new XMLHttpRequest();

      request.responseType = "arraybuffer";
      request.open("GET", uri, true);

      // XHR failed
      request.onerror = function() {
        cb(new Error("Couldn't load audio from " + uri));
      };

      // XHR complete
      request.onload = function() {
        context.decodeAudioData(request.response,success,function(err){
          // Audio was bad
          cb(new Error("Couldn't decode audio from " + uri));
        });
      };

      request.send();

      function success(buffer) {

        var source;

        function play() {

          // Stop if it's already playing
          stop();

          // Create a new source (can't replay an existing source)
          source = context.createBufferSource();
          source.connect(context.destination);

          // Set the buffer
          source.buffer = buffer;
          source.loop = true;

          // Play it
          source.start(0);

        }

        function stop() {

          // Stop and clear if it's playing
          if (source) {
            source.stop();
            source = null;
          }

        }

        cb(null,{
          play: play,
          stop: stop
        });

      }

    }

    loopify.version = "0.1";

    if (typeof define === "function" && define.amd) {
      define(function() { return loopify; });
    } else if (typeof module === "object" && module.exports) {
      module.exports = loopify;
    } else {
      this.loopify = loopify; 
    }

})();

