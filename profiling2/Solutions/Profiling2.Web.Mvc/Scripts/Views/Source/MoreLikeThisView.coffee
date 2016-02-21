Profiling.Views.MoreLikeThisView = Backbone.View.extend

  tagName: "button"
  
  className: "btn"

  templates:
    modal: _.template """
      <div id="modal-morelike-<%= sourceId %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>
          <h4>
            More Sources Like:
            <%= sourceName %>
            <% if (isReadOnly === true) { %>
              <i class='icon-lock' style='margin-left: 5px;' title='File is read only.'></i>
            <% } %>
            <% if (isRestricted === true) { %>
              <span class='label label-important' style='margin-left: 5px; font-size: x-small;'>RESTRICTED</span>
            <% } %>
          </h4>
          <div class='row-fluid'>
            <p class='pull-left'><span class='muted'>Path:</span> <%= sourcePath %></p>
            <p class='pull-right'><span class='muted'>Date last modified:</span> <%= date %></p>
          </div>
        </div>
        <div class='modal-body' id="modal-morelike-body-<%= sourceId %>">
          <span class='muted'>Loading...</span>
        </div>
        <div class="modal-footer">
          <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
        </div>
      </div>
    """
    table: _.template """
      <table class="table table-bordered">
        <thead>
          <tr>
            <th>Path</th><th>Date Modified</th>
          </tr>
        </thead>
        <tbody>
        </tbody>
      </table>
    """
    row: _.template """
      <tr>
        <td id="cell-<%= originatingId %>-<%= source.Id %>"><%= source.SourcePath %>\<%= source.SourceName %></td>
        <td><%= source.FileDateTimeStamp %></td>
      </tr>
    """

  events:
    "click": "getMoreLikeThis"

  render: ->
    @$el.text "More Like This"
    @

  getMoreLikeThis: ->
    $.ajax
      url: "#{Profiling.applicationUrl}Sources/Explorer/MoreLikeThis/#{@options.sourceId}"
      beforeSend: =>
        @$el.after @templates.modal
          sourceId: @options.sourceId
          sourceName: @options.sourceName
          sourcePath: @options.sourcePath
          isRestricted: @options.isRestricted
          isReadOnly: @options.isReadOnly
          date: @options.date

        _.defer =>
          $("#modal-morelike-#{@options.sourceId}").modal
            keyboard: true
            width: "80%"
      success: (data, textStatus, xhr) =>
        if data
          $("#modal-morelike-body-#{@options.sourceId}").html @templates.table()
          for source in data
            $("#modal-morelike-body-#{@options.sourceId} tbody").append @templates.row
              originatingId: @options.sourceId
              source: source

            previewView = new Profiling.Views.ExplorerPreviewModalView
              sourceId: source.Id
              sourceName: source.SourceName
              sourceFolder: source.SourcePath
              sourcePath: source.SourcePath
              isRestricted: source.IsRestricted
              isReadOnly: source.IsReadOnly
              date: source.FileDateTimeStamp

            $("#cell-#{@options.sourceId}-#{source.Id}").append previewView.render().el
        else
          $("#modal-morelike-body-#{@options.sourceId}").html "No data received."