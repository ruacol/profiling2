Profiling.Views.MergeUnitSearchView = Backbone.View.extend

  templates:
    table: _.template """
      <p>Choose unit 
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
            <th>Name(s)</th>
            <th>Background Information</th>
            <th>Organization</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
      <div id="<%= col %>-result" class="clearfix" style="clear: both;"></div>
    """
    unit: _.template """
      <hr />
      <p>Selected unit 
        <% if (col == 'left') { %>
          to delete:
        <% } else if (col == 'right') { %>
          to keep:
        <% } %>
        <strong>
          <a href="<%= Profiling.applicationUrl %>Profiling/Units/Details/<%= data.Unit.Id %>" target="_blank"><%= data.Unit.UnitName %></a>
        </strong>
      </p>
      <dl class="dl-horizontal">
        <dt>Aliases</dt>
        <dd>
          <% if (_(data.UnitAliases).size() > 0) { %>
            <%= _(data.UnitAliases).join(", ") %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>Organization</dt>
        <dd>
          <% if (data.Unit.OrganizationName) { %>
            <%= data.Unit.OrganizationName %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>Background Information</dt>
        <dd>
          <% if (data.Unit.BackgroundInformation) { %>
            <%= data.Unit.BackgroundInformation %>
          <% } else { %>
            <span class="muted">(none)</span>
          <% } %>
        </dd>
        <dt>Responsibilties</dt>
        <dd><%= _(data.OrganizationResponsibilities).size() %></dd>
        <dt>Sources</dt>
        <dd><%= _(data.UnitSources).size() %></dd>
        <dt>Careers</dt>
        <dd><%= _(data.Careers).size() %></dd>
        <dt>Locations</dt>
        <dd><%= _(data.UnitLocations).size() %></dd>
        <dt>Hierarchies</dt>
        <dd><%= _(data.UnitHierarchies).size() %></dd>
        <dt>Hierarchy Children</dt>
        <dd><%= _(data.UnitHierarchyChildren).size() %></dd>
      </dl>
    """

  initialize: (opts) ->
    @col = opts.col

  events:
    "click button.person-span": "selectUnit"

  render: ->
    @$el.html @templates.table
      col: @col

    if @dataTable
      @dataTable.fnDraw()
    else
      _.defer =>
        @dataTable = new Profiling.DataTable "#{@col}-table",
          sAjaxSource: "#{Profiling.applicationUrl}Profiling/Units/DataTables"
          bStateSave: false
          sDom: 'T<"clear">ftipr'
          aoColumns: [ 
            { mDataProp: 'Id', bSortable: false }, 
            { mDataProp: 'Name', bSortable: false }, 
            { mDataProp: 'BackgroundInformation', bVisible: false }, 
            { mDataProp: 'Organization', bSortable: false }
          ]
          fnRowCallback: (nRow, aData, iDisplayIndex) ->
            $("td:eq(0)", nRow).html "<button class='btn btn-mini person-span' style='width: auto;' data-id='#{aData['Id']}'><i class='accordion-toggle icon-ok'></i></button>"

    @

  selectUnit: (e) ->
    button = if $(e.target).is("button")
      $(e.target)
    else
      $(e.target).parent()
    @unitId = $(button).data "id"

    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Units/Json/#{@unitId}"
      type: "GET"
      error: (xhr, textStatus) ->
        if Profiling.isEmptyString(xhr.responseText)
          bootbox.alert xhr.responseText
        else
          bootbox.alert xhr.statusText
      success: (data, textStatus, xhr) =>
        $("##{@col}-result").html @templates.unit
          col: @col
          data: data

  resetUnit: ->
    @unitId = 0
    $("##{@col}-result").html ""