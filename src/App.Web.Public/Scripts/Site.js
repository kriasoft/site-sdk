/// <reference path="jquery-1.8.3.js" />

// Enable Google Analytics tracking if Google Analytics property ID is present in Web.config\appSettings\GoogleAnalyticsPID
if ($('body').data().ga) {
    var _gaq = [['_setAccount', $('body').data().ga], ['_trackPageview']];

    (function (d, t) {
        var g = d.createElement(t), s = d.getElementsByTagName(t)[0];
        g.src = ('https:' == location.protocol ? '//ssl' : '//www') + '.google-analytics.com/ga.js';
        s.parentNode.insertBefore(g, s)
    }(document, 'script'));
}
