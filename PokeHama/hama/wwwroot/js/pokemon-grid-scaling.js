$("img").hover(function() {
    $(this).toggleClass('scale-up');
    $("img").not(this).toggleClass('scale-down');
})
