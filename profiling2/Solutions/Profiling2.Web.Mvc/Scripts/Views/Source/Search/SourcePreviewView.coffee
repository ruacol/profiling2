Profiling.Views.SourcePreviewView = Backbone.View.extend

  initialize: ->
    $(window).resize () =>
      @resizeIframeAndFocus()

  resizeIframeAndFocus: ->
    accordionHeight = $("#accordion-preview-body").height()
    if accordionHeight > 0
      $("#previewIframe").height(accordionHeight - $("#previewNav").height())
    else
      @$el.height "100%"
      $("#previewIframe").height "100%"

  template:
    previewNav: _.template """
      <div id="previewNav" tabindex="0" class="span12" style="background-color: lightgoldenrodyellow;">
        <div style="margin: 7px 20px 0px 20px;">
          <% if (_.size(searchTerms) > 0) { %>
            <p class="pull-left muted" style="margin-right: 20px;">Highlighted terms:</p>
            <ul class="nav nav-pills pull-left" style="margin-right: 20px; margin-bottom: 7px;">
              <% _.each(searchTerms, function(searchTerm) { %>
                <li class="searchTerm active"><a href="#"><%= searchTerm %></a></li>
              <% }); %>
            </ul>
            <ul class="pager pull-left" style="margin-top: 0px; margin-bottom: 7px;">
              <li id="leftPager"><a href="#">&larr;</a></li>
              <li id="rightPager"><a href="#">&rarr;</a></li>
            </ul>
          <% } else { %>
            <p class="pull-left muted">There are no search terms to highlight.</p>
          <% } %>

          <% if (hasOcrText === true) { %>
            <p class="pull-right muted">This preview shows this file's OCR text.  Download the file to see the original.</p>
            <span class="pull-right label label-info" style="margin-right: 7px;">OCR</span>
          <% } %>
        </div>
      </div>
    """
    previewIframe: _.template """
      <iframe id="previewIframe" src="<%= Profiling.applicationUrl %>Profiling/Sources/Preview/<%= id %>/<%= adminSourceSearchId %>" style="position: relative; overflow: scroll; width: 100%; border: none 0;">
    """
    previewImg: _.template """
      <div class="clearfix"></div>
      <img src="<%= Profiling.applicationUrl %>Profiling/Sources/Preview/<%= id %>/<%= adminSourceSearchId %>" class="img-polaroid" />
    """

  events:
    "click li.searchTerm": "toggleSearchTerm"
    "click li#leftPager": "leftPager"
    "click li#rightPager": "rightPager"

  render: () ->
    mostRecentReview = @model.getMostRecentReview()

    searchTerms = if mostRecentReview and mostRecentReview.AndSearchTerms
      array = _.map mostRecentReview.AndSearchTerms, (t) -> $.trim(t.replace(/"/g, ''))
      if mostRecentReview.ExcludeSearchTerms
        array = array.concat (_.map mostRecentReview.ExcludeSearchTerms, (t) -> "-#{$.trim(t.replace(/"/g, ''))}")
      array
    else
      []

    $(@el).html @template.previewNav
      searchTerms: searchTerms
      hasOcrText: @model.get('HasOcrText')
    if _([ 'jpg', 'jpeg', 'gif', 'png', 'bmp', 'tif' ]).contains(@model.get 'FileExtension') and not @model.get('HasOcrText')
      $(@el).append @template.previewImg
        id: @model.get 'id'
        adminSourceSearchId: @model.get 'adminSourceSearchId'
      @resizeIframeAndFocus()
    else
      $(@el).append @template.previewIframe
        id: @model.get 'id'
        adminSourceSearchId: @model.get 'adminSourceSearchId'

    @$("#previewIframe").load () =>
      @resizeIframeAndFocus()
      for searchTerm in searchTerms
        @highlightSearchTerm searchTerm

      @disableTextSelection() if @model.get('IsReadOnly') is true

    @

  disableTextSelection: ->
    @$("#previewIframe").ready =>
      @$("#previewIframe").contents().find("body").css("-webkit-user-select", "none")
        .css("-webkit-touch-callout", "none")
        .css("-khtml-user-select", "none")
        .css("-moz-user-select", "none")
        .css("-ms-user-select", "none")
        .css("user-select", "none")

  highlightSearchTerm: (searchTerm) ->
    @$("#previewIframe").ready () =>
      if @isPowerpoint()
        results = @$("#previewIframe").contents().find("tspan:icontains('#{searchTerm}')").each (i, el) ->
          # @filltext($(el).text(), $(el).attr('x'), $(el).attr('y'), 'yellow', 'black')
          $(el).attr('fill', 'yellow').attr 'class', 'highlight'
      else
        # Johann Burkard's highlight doesn't cross DOM elements...
        # @$("#previewIframe").contents().find("body").highlight(searchTerm)
        
        # ...so use James Padolsey's findAndReplaceDOMText.
        # NOTE text that does cross DOM elements, when highlighted, won't be removed by removeHighlight below.
        spannode = document.createElement 'span'
        spannode.className = 'highlight'
        findAndReplaceDOMText @$("#previewIframe").contents().find("body")[0],
          find: new RegExp("\\b" + searchTerm + "\\b", "ig")
          wrap: spannode
        
        @$("#previewIframe").contents().find(".highlight").attr('tabindex', -1).css
          backgroundColor: 'yellow'

  removeHighlight: (searchTerm) ->
    @$("#previewIframe").ready () =>
      searchTerm = searchTerm.replace /\*/g, ''
      if @isPowerpoint()
        results = @$("#previewIframe").contents().find("tspan:icontains('#{searchTerm}')").each (i, el) ->
          $(el).attr('fill', null).attr 'class', null
      else
        @$("#previewIframe").contents().find(".highlight").each (i, el) ->
          $(el).parent().removeHighlight searchTerm

  toggleSearchTerm: (event) ->
    liTag = $(event.target).parent()
    if liTag.hasClass 'active'
      liTag.removeClass 'active'
      @removeHighlight liTag.text()
    else
      liTag.addClass 'active'
      @highlightSearchTerm liTag.text()
    false

  leftPager: () ->
    highlightedItems = @highlightedItems()
    @scrollIndex = 0 if (_.isNaN(@scrollIndex) or _.isNull(@scrollIndex) or _.isUndefined(@scrollIndex))
    @scrollIndex = @scrollIndex - 1
    @scrollIndex = highlightedItems.length - 1 if @scrollIndex < 0
    @scrollTo highlightedItems[@scrollIndex]
    false

  rightPager: () ->
    highlightedItems = @highlightedItems()
    @scrollIndex = 0 if (_.isNaN(@scrollIndex) or _.isNull(@scrollIndex) or _.isUndefined(@scrollIndex))
    @scrollIndex = @scrollIndex + 1
    @scrollIndex = 0 if @scrollIndex > highlightedItems.length - 1
    @scrollTo highlightedItems[@scrollIndex]
    false

  isPowerpoint: ->
    _([ 'ppt', 'pptx' ]).contains(@model.get 'FileExtension')

  highlightedItems: ->
    items = if @isPowerpoint()
      x = @$("#previewIframe").contents().find("*[class~=highlight]").parents "svg"
      # Firefox 21.0 doesn't support focus() on svg object
      x = if x and _(x).first() and typeof(_(x).first().focus) isnt 'function'
        $(x).parents "div.slide"
      else
        x
    else
      @$("#previewIframe").contents().find(".highlight")

  scrollTo: (item) ->
    if item
      if typeof(item.scrollIntoView) is 'function'
        item.scrollIntoView()
      else if typeof(item.focus) is 'function'
        item.focus()