/*

highlight v4

Highlights arbitrary terms.

<http://johannburkard.de/blog/programming/javascript/highlight-javascript-text-higlighting-jquery-plugin.html>

MIT license.

Johann Burkard
<http://johannburkard.de>
<mailto:jb@eaio.com>

*/

jQuery.fn.highlight = function (pat) {
  function innerHighlight(node, pat) {
    var skip = 0;
    if (node.nodeType == 3) {
      var regex;
      var lastChar = pat[pat.length - 1]
      if (lastChar == '*') {
        // highlight only search terms with trailing asterisk
        pat = pat.replace(/\*/g, "");
        regex = new RegExp("\\b" + pat, "i");
      } else {
        // every other case is treated as word-only search
        pat = pat.replace(/\*/g, "");
        regex = new RegExp("\\b" + pat + "\\b", "i");
      }
      // search term doesn't cross dom elements
      var match = node.data.match(regex);
      if (match) {
        var spannode = document.createElement('span');
        spannode.className = 'highlight';
        var middlebit = node.splitText(match.index);
        var endbit = middlebit.splitText(pat.length);
        var middleclone = middlebit.cloneNode(true);
        spannode.appendChild(middleclone);
        middlebit.parentNode.replaceChild(spannode, middlebit);
        skip = 1;
      }
    }
    else if (node.nodeType == 1 && node.childNodes && !/(script|style)/i.test(node.tagName)) {
      for (var i = 0; i < node.childNodes.length; ++i) {
        i += innerHighlight(node.childNodes[i], pat);
      }
    }
    return skip;
  }
  return this.length && pat && pat.length ? this.each(function () {
    innerHighlight(this, pat.toUpperCase());
  }) : this;
};

jQuery.fn.removeHighlight = function (searchTerm) {
  return this.find("span.highlight").each(function () {
    if ($(this).text().toUpperCase() === searchTerm.toUpperCase()) {
      $(this).removeAttr('style tabindex');
      this.parentNode.firstChild.nodeName;
      with (this.parentNode) {
        replaceChild(this.firstChild, this);
        normalize();
      }
    }
  }).end();
};
