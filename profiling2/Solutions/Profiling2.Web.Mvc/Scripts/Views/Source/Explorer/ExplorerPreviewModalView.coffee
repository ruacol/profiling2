Profiling.Views.ExplorerPreviewModalView = Backbone.View.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-search" style="margin-left: 5px;" title="Preview <%= sourceName %>"></i>
    """
    modal: _.template """
      <div id="modal-<%= sourceId %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>
          <h4>
            <%= sourceName %>
            <% if (isReadOnly === true) { %>
              <i class='icon-lock' style='margin-left: 5px;' title='File is read only.'></i>
            <% } %>
            <% if (isRestricted === true) { %>
              <span class='label label-important' style='margin-left: 5px; font-size: x-small;'>RESTRICTED</span>
            <% } %>
            <span class='pull-right' style='margin-right: 10px;'><span class='muted'>Source ID:</span> <%= sourceId %></span>
          </h4>
          <div class='row-fluid'>
            <p class='pull-left'>
              <span class='muted'>Path:</span> <%= sourcePath %>
            </p>
            <p class='pull-right'><span class='muted'>Date last modified:</span> <%= date %></p>
          </div>
        </div>
        <div class='modal-body' id="modal-body-<%= sourceId %>"></div>
        <div class="modal-footer">
          <div id="modal-footer-<%= sourceId %>" style="display: inline-block; margin-right: 5px;"></div>
          <a class="btn" href="<%= Profiling.applicationUrl %>Sources/Explorer/Download/<%= sourceId %>" target="_blank">Download</a>
          <% if (sourceFolder) { %>
            <a class="btn navigate-away" data-source-id="<%= sourceId %>" href="#path/<%= sourceFolder %>">Go to Folder</a>
          <% } %>
          <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
        </div>
      </div>
    """
    previewImg: _.template """
      <div class="clearfix"></div>
      <img src="<%= Profiling.applicationUrl %>Sources/Explorer/Preview/<%= sourceId %>" class="img-polaroid" />
    """

  events:
    "click": "getPreview"

  initialize: (opts) ->
    @opts = opts

  render: ->
    @$el.css 'display', 'inline'

    @$el.append @templates.icon
      sourceName: @opts.sourceName

    @$el.append @templates.modal @opts

    @
    
  # instantiate source preview via bootstrap modal + click trigger
  getPreview: ->
    $("#modal-body-#{@opts.sourceId}").html "<span class='muted'>Loading preview...</span>"
    $("#modal-#{@opts.sourceId}").modal
      keyboard: true
      width: "90%"
      view = new Profiling.Views.MoreLikeThisView @opts
    $("#modal-#{@opts.sourceId} #modal-footer-#{@opts.sourceId}").html view.render().el

    $.ajax
      url: "#{Profiling.applicationUrl}Sources/Explorer/Preview/#{@opts.sourceId}"
      success: (data, textStatus, xhr) =>
        if xhr and xhr.getResponseHeader("Content-Type").indexOf("image") >= 0
          $("#modal-body-#{@opts.sourceId}").html @templates.previewImg @opts
        else
          $("#modal-body-#{@opts.sourceId}").html data.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if data

          # close the preview modal when user clicks 'go to folder'
          _.defer =>
            $("a.navigate-away").on "click", (e) =>
              sourceId = $(e.target).data "source-id"
              $("#modal-#{@opts.sourceId}").modal "hide"

          # highlight search terms in preview
          if @opts.searchTerm
            terms = @opts.searchTerm.split ' '
                
            spannode = document.createElement 'span'
            spannode.className = 'highlight'

            for term in terms
              if term
                term = term.replace '+', ''
                findAndReplaceDOMText $("#modal-body-#{@opts.sourceId}")[0],
                  find: new RegExp("\\b" + term.trim() + "\\b", "ig")
                  wrap: spannode

            $("#modal-body-#{@opts.sourceId} .highlight").css 'backgroundColor', 'yellow'
