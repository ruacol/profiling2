Profiling.Views.MergeEventSearchView = Backbone.View.extend

  templates:
    table: _.template """
      <p>Choose event 
        <strong>
        <% if (col == 'left') { %>
          to delete
        <% } else if (col == 'right') { %>
          to keep
        <% } %>
        </strong>
      </p>
      <table id="<%= col %>-table" class="table table-bordered table-hover table-condensed">
        <thead>
          <tr>
            <th></th>
            <th>Case Code</th>
            <th>Categories</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Location</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
      <div id="<%= col %>-result" class="clearfix" style="clear: both;"></div>
    """
    event: _.template """
      <hr />
      <p>Selected event 
        <% if (col == 'left') { %>
          to delete:
        <% } else if (col == 'right') { %>
          to keep:
        <% } %>
        <strong>
          <a href="<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= data.Event.Id %>" target="_blank"><%= data.Event.EventName %></a>
        </strong>
      </p>
      <dl class="dl-horizontal">
        <dt>ID</dt>
        <dd><a href="<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= data.Event.Id %>" target='_blank'><%= data.Event.Id %></a></dd>
        <dt>Case Code</dt>
        <dd>
          <% if (data.Event.CaseCode) { %>
            <%= data.Event.CaseCode %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>Categories</dt>
        <dd><%= _(data.Categories).size() %></dd>
        <dt>Start Date</dt>
        <dd>
          <% if (data.Event.StartDate) { %>
            <%= data.Event.StartDate %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>End Date</dt>
        <dd>
          <% if (data.Event.EndDate) { %>
            <%= data.Event.EndDate %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>Location</dt>
        <dd>
          <% if (data.Event.Location.FullName) { %>
            <%= data.Event.Location.FullName %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>Organization Responsibilities</dt>
        <dd><%= _(data.OrganizationResponsibilities).size() %></dd>
        <dt>Person Responsibilities</dt>
        <dd><%= _(data.PersonResponsibilities).size() %></dd>
        <dt>Relationships</dt>
        <dd><%= _(data.EventRelationships).size() %></dd>
        <dt>Actions Taken</dt>
        <dd><%= _(data.ActionsTaken).size() %></dd>
        <dt>Sources</dt>
        <dd><%= _(data.EventSources).size() %></dd>
      </dl>
    """

  initialize: (opts) ->
    @col = opts.col

  events:
    "click button.event-span": "selectEvent"

  render: ->
    @$el.html @templates.table
      col: @col

    if @dataTable
      @dataTable.fnDraw()
    else
      _.defer =>
        @dataTable = new Profiling.DataTable "#{@col}-table",
          sAjaxSource: "#{Profiling.applicationUrl}Profiling/Events/DataTables"
          bStateSave: false
          sDom: 'T<"clear">ftipr'
          aoColumns: [ 
            { mDataProp: 'Id' }, 
            { mDataProp: 'JhroCaseNumber' },
            { mDataProp: 'Violations', bSortable: false }, 
            { mDataProp: 'StartDateDisplay' }, 
            { mDataProp: 'EndDateDisplay' },
            { mDataProp: 'Location' }
          ]
          fnRowCallback: (nRow, aData, iDisplayIndex) ->
            $("td:eq(0)", nRow).html "<button class='btn btn-mini event-span' style='width: auto;' data-id='#{aData['Id']}' title='ID: #{aData['Id']}'><i class='accordion-toggle icon-ok'></i></button>"
            $("td:eq(2)", nRow).html "<a href='#{Profiling.applicationUrl}Profiling/Events/Details/#{aData['Id']}' target='_blank'>#{aData['Violations']}</a>"

    @

  selectEvent: (e) ->
    button = if $(e.target).is("button")
      $(e.target)
    else
      $(e.target).parent()
    @eventId = $(button).data "id"

    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Events/Json/#{@eventId}"
      type: "GET"
      error: (xhr, textStatus) ->
        if Profiling.isEmptyString(xhr.responseText)
          bootbox.alert xhr.responseText
        else
          bootbox.alert xhr.statusText
      success: (data, textStatus, xhr) =>
        $("##{@col}-result").html @templates.event
          col: @col
          data: data

  resetEvent: ->
    @eventId = 0
    $("##{@col}-result").html ""