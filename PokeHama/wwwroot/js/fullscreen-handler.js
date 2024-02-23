var elem = document.documentElement;

function openFullscreen() {
    if (elem.requestFullscreen) {
        elem.requestFullscreen();
        return true;
    } else if (elem.mozRequestFullScreen) { /* Firefox */
        elem.mozRequestFullScreen();
        return true;
    } else if (elem.webkitRequestFullscreen) { /* Chrome, Safari and Opera */
        elem.webkitRequestFullscreen();
        return true;
    } else if (elem.msRequestFullscreen) { /* IE/Edge */
        elem.msRequestFullscreen();
        return true;
    }
    
    return false;
}

function closeFullscreen() {
    if (document.exitFullscreen) {
        document.exitFullscreen();
        return true;
    } else if (document.mozCancelFullScreen) { /* Firefox */
        document.mozCancelFullScreen();
        return true;
    } else if (document.webkitExitFullscreen) { /* Chrome, Safari and Opera */
        document.webkitExitFullscreen();
        return true;
    } else if (document.msExitFullscreen) { /* IE/Edge */
        document.msExitFullscreen();
        return true;
    }
}