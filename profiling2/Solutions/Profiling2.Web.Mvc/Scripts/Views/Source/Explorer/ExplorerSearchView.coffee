Profiling.Views.ExplorerSearchView = Backbone.View.extend

  templates:
    facetFilter: _.template """
      <div class="btn-group" style="display: inline-block; margin-left: 10px; margin-bottom: 10px; text-align: left;">
        <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">Filter results <span class="caret"></span></a>
        <ul class="dropdown-menu">
          <% $(data).each(function(i, el) { %>
            <% if (el.Count > 0) { %>
              <li>
                <a href="#search" class="facet-uploaded-by" data-uploaded-by="<%= _(el.Facets).first().Value %>"><small>
                  <% $(el.Facets).each(function(j, facet) { %>
                    <%= facet.Name %>: <%= facet.Value %> (<%= el.Count %>)
                    <br />
                  <% }) %>
                </small></a>
              </li>
            <% } %>
          <% }) %>
        </ul>
      </div>
    """
    table: _.template """
      <hr />
      <h3>Search</h3>
      <table id="search-table" class="table table-bordered">
        <thead>
          <tr>
            <th>Relevance</th><th>Path</th><th>Date Modified</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
    """
    tips: _.template """
      <div class="clearfix" style="clear: both;">
        <hr />
        <h4>Search tips</h4>

        <div>
          <p>
            Search terms are by default ORed together.  To narrow down search results (ANDing terms together), prefix each search term with a '+'.
            e.g. <em>+m23 +kivu</em> will return results containing both 'm23' and 'kivu' whereas <em>m23 kivu</em> will return results containing either
            'm23' or 'kivu'.
          </p>
          <p>Search is not case sensitive.</p>
          <p>Use the start and end dates to filter results by files' last modified dates.</p>
        </div>
      </div>
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
    dateFilter: _.template """
      <div id="date-filter" style="display: inline-block;">
        <div class="input-daterange" data-provide="datepicker" data-date-format="yyyy/mm/dd" data-date-autoclose="true" data-date-clear-btn="true" style="margin-left: 10px;">
          <input type="text" class="input-small" name="start-date" id="start-date" placeholder="Start date..." />
          <span class="add-on" style="margin-bottom: 10px;">to</span>
          <input type="text" class="input-small" name="end-date" id="end-date" placeholder="End date..." />
        </div>
      </div>
    """

  initialize: (opts) ->
    @code = opts.code
    @permissions = opts.permissions

  events:
    "click a.facet-uploaded-by": "clickFacetFilter"

  render: ->
    @$el.append @templates.table
    @$el.append @templates.tips

    _.defer =>
      @dataTable = new Profiling.DataTable 'search-table',
        bStateSave: false
        sAjaxSource: "#{Profiling.applicationUrl}Sources/Explorer/DataTablesSearch"
        aaSorting: [ [ 0, 'desc' ] ]
        aoColumns: [
          { mDataProp: 'Score', bVisible: false },
          { mDataProp: 'SourcePath' }, 
          { mDataProp: 'FileDateTimeStamp' }
        ]
        fnServerParams: (aoData) =>
          aoData.push
            name: "code"
            value: @code
          aoData.push 
            name: "start-date"
            value: $("#start-date").val()
          aoData.push
            name: "end-date"
            value: $("#end-date").val()
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          sourceId = aData['Id']
          sourceName = aData['SourceName']
          sourcePath = aData['SourcePath']
          isRestricted = aData['IsRestricted']
          isReadOnly = aData['IsReadOnly']
          date = "#{ if aData['FileDateTimeStamp'] then moment(aData['FileDateTimeStamp']).format('LLLL') else '' }"

          sourceFolder = if sourcePath and @permissions.canViewAndSearchSources
            sourcePath.substring 0, sourcePath.lastIndexOf('\\')

          name = sourceName
          name += "<i class='icon-lock pull-right' style='margin-left: 5px;'></i>" if isReadOnly is true
          name += "<span class='label label-important pull-right' style='margin-left: 5px;'><small>RESTRICTED</small></span>" if isRestricted is true
          modal = @templates.modal
            sourceId: sourceId
            sourceName: sourceName
            sourceFolder: sourceFolder
            sourcePath: sourcePath
            isRestricted: isRestricted
            isReadOnly: isReadOnly
            date: date
          $("td:eq(0)", nRow).html """
            #{name}
            <br />
            #{sourcePath}
            <br />
            <span class="muted">
              #{aData['HighlightFragment']}
            </span>
            #{modal}
          """
          $("td:eq(1)", nRow).html moment(aData['FileDateTimeStamp']).format('LLLL')
          $("td:eq(1)", nRow).css "white-space", "nowrap"

          # instantiate source preview via bootstrap modal + click trigger
          $(nRow).addClass("accordion-toggle").click ->
            $.ajax
              url: "#{Profiling.applicationUrl}Profiling/Sources/Preview/#{sourceId}"
              beforeSend: ->
                $("#modal-body-#{sourceId}").html "<span class='muted'>Loading preview...</span>"
                $("#modal-#{sourceId}").modal
                  keyboard: true
                  width: "90%"
                view = new Profiling.Views.MoreLikeThisView
                  sourceId: sourceId
                  sourceName: sourceName
                  sourcePath: sourcePath
                  isRestricted: isRestricted
                  isReadOnly: isReadOnly
                  date: date
                $("#modal-#{sourceId} #modal-footer-#{sourceId}").html view.render().el
              success: (data, textStatus, xhr) ->
                $("#modal-body-#{sourceId}").html data

                # close the preview modal when user clicks 'go to folder'
                _.defer ->
                  $("a.navigate-away").on "click", (e) ->
                    sourceId = $(e.target).data "source-id"
                    $("#modal-#{sourceId}").modal "hide"

                # highlight search terms in preview
                searchTerm = $($("#search-table_filter input")[0]).val()
                if searchTerm
                  terms = searchTerm.split ' '
                
                  spannode = document.createElement 'span'
                  spannode.className = 'highlight'

                  for term in terms
                    if term
                      term = term.replace '+', ''
                      findAndReplaceDOMText $("#modal-body-#{sourceId}")[0],
                        find: new RegExp("\\b" + term.trim() + "\\b", "ig")
                        wrap: spannode

                  $("#modal-body-#{sourceId} .highlight").css 'backgroundColor', 'yellow'
        fnServerData: (sSource, aoData, fnCallback) =>
          # call facet search
          $.ajax
            url: "#{Profiling.applicationUrl}Sources/Explorer/SearchGetFacets"
            data:
              term: _(aoData).findWhere({ name: 'sSearch' }).value
              code: _(aoData).findWhere({ name: 'code' }).value
              "start-date": _(aoData).findWhere({ name: 'start-date' }).value
              "end-date": _(aoData).findWhere({ name: 'end-date' }).value
            success: (data, textStatus, xhr) =>
              if data
                $("#facet-filter").html @templates.facetFilter
                  data: data

          $.getJSON sSource, aoData, (json) ->
            fnCallback json

      $("#search-table_filter").append @templates.dateFilter
      $("#search-table_filter").append "<div id='facet-filter' style='display: inline-block;'></div>"
      $("#search-table_filter label").css "display", "inline-block"

      $(".input-daterange input").change (e) =>
        @dataTable.fnReloadAjax()

    @

  clickFacetFilter: (e) ->
    uploadedBy = $(e.currentTarget).data "uploaded-by"