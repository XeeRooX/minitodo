function main() {
    $(".star-toggle").click(StarClick);
}
function StarClick() {
    if ($(this).hasClass("favorited") === true) {
        $(this).removeClass("favorited");
        $(this).children().attr("src", "/png/unStar.png");
        console.log("unStar", $(this).children()[0].tagName);
    }
    else {
        $(this).addClass("favorited");
        $(this).children().attr("src", "/png/Star.png");
        
    }
}
main();
