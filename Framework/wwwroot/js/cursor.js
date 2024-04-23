// define variables for all the pos-presets
// most likely to be null at initialization, but who knows
let leftElement = document.getElementById("pos-preset-left");
let rightElement = document.getElementById("pos-preset-right");
let topElement = document.getElementById("pos-preset-top");
let bottomElement = document.getElementById("pos-preset-bottom");

// define consts for images
// can't be null as they're in index.html
const leftImg = document.getElementById("left-arrow-img");
const rightImg = document.getElementById("right-arrow-img");
const topImg = document.getElementById("up-arrow-img");
const bottomImg = document.getElementById("down-arrow-img");

// app div containing the entire generated blazor stuff
// listen for mutations in that div
const appContainer = document.getElementById("app");

// if mutation occurs, call reassign
let observer = new MutationObserver(function (mutations) {
    reassign();
});

// config for mutation observer
// observes attribute changes, node deletions/insertions in the entire subtree of the app div
let config = { attributes: true, childList: true, subtree: true };

// get the pos presets again, if they're not null, attach event listeners
function reassign() {
    leftElement = document.getElementById("pos-preset-left");

    if (leftElement != null) {
        leftElement.addEventListener("mouseenter", () => {
            // start showing the image
            show("left");
            // start tracking the image to mouse cursor
            track("left");
        });
        leftElement.addEventListener("mouseleave", () => {
            // stop showing image
            show("left", false);
            // remove mouse tracking
            track("left", false);
        });
    }

    rightElement = document.getElementById("pos-preset-right");

    if (rightElement != null) {
        rightElement.addEventListener("mouseenter", () => {
            show("right");
            track("right");
        });
        rightElement.addEventListener("mouseleave", () => {
            show("right", false);
            track("right", false);
        });
    }

    topElement = document.getElementById("pos-preset-top");

    if (topElement != null) {
        topElement.addEventListener("mouseenter", () => {
            show("top");
            track("top");
        });
        topElement.addEventListener("mouseleave", () => {
            show("top", false);
            track("top", false);
        });
    }

    bottomElement = document.getElementById("pos-preset-bottom");

    if (bottomElement != null) {
        bottomElement.addEventListener("mouseenter", () => {
            show("bottom");
            track("bottom");
        });
        bottomElement.addEventListener("mouseleave", () => {
            show("bottom", false);
            track("bottom", false);
        });
    }
}

// if start=true, show, if false, stop showing
function show(dir, start = true) {
    switch (dir) {
        case "left":
            start
                ? (leftImg.style.display = "block")
                : (leftImg.style.display = "none");
            break;
        case "right":
            start
                ? (rightImg.style.display = "block")
                : (rightImg.style.display = "none");
            break;
        case "top":
            start
                ? (topImg.style.display = "block")
                : (topImg.style.display = "none");
            break;
        case "bottom":
            start
                ? (bottomImg.style.display = "block")
                : (bottomImg.style.display = "none");
            break;
        default:
            break;
    }
}

// "template" function for the mouse tracker event listener callbacks
function trackHelper(event, imgObject) {
    let width = window.getComputedStyle(imgObject).width;
    imgObject.style.top = `calc(${event.y}px - calc(${width} / 2))`;
    imgObject.style.left = `calc(${event.x}px - calc(${width} / 2))`;
}

// create named callback for every image
const trackLeft = (event) => {
    trackHelper(event, leftImg);
};
const trackRight = (event) => {
    trackHelper(event, rightImg);
};
const trackTop = (event) => {
    trackHelper(event, topImg);
};
const trackBottom = (event) => {
    trackHelper(event, bottomImg);
};

// if start=true, track, if false, stop tracking
//? maybe combine this with show into one function?
function track(dir, start = true) {
    switch (dir) {
        case "left":
            start
                ? window.addEventListener("mousemove", trackLeft)
                : window.removeEventListener("mousemove", trackLeft);
            break;
        case "right":
            start
                ? window.addEventListener("mousemove", trackRight)
                : window.removeEventListener("mousemove", trackRight);
            break;
        case "top":
            start
                ? window.addEventListener("mousemove", trackTop)
                : window.removeEventListener("mousemove", trackTop);
            break;
        case "bottom":
            start
                ? window.addEventListener("mousemove", trackBottom)
                : window.removeEventListener("mousemove", trackBottom);
            break;

        default:
            break;
    }
}

// start looking for mutations
observer.observe(appContainer, config);
