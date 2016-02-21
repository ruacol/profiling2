Profiling.Views.MergePersonSearchView = Backbone.View.extend

  templates:
    table: _.template """
      <p>Choose person 
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
            <th>ID Number(s)</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
      <div id="<%= col %>-result" class="clearfix" style="clear: both;"></div>
    """
    person: _.template """
      <hr />
      <p>Selected person 
        <% if (col == 'left') { %>
          to delete:
        <% } else if (col == 'right') { %>
          to keep:
        <% } %>
        <strong>
          <a href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= person.Id %>" target="_blank"><%= person.Name %></a>
          <% if (person.IsRestrictedProfile === true) { %>
            &nbsp;<span class="label label-important"><small>RESTRICTED</small><span>
          <% } %>
        </strong>
      </p>
      <dl class="dl-horizontal">
        <% if (person.MilitaryIDNumber) { %>
          <dt>ID Number(s)</dt>
          <dd><%= person.MilitaryIDNumber %></dd>
        <% } %>
        <% if (person.DateOfBirth) { %>
          <dt>DOB</dt>
          <dd><%= person.DateOfBirth %></dd>
        <% } %>
        <% if (person.ProfileStatusName) { %>
          <dt>Profile Status</dt>
          <dd><%= person.ProfileStatusName %></dd>
        <% } %>
        <% if (person.ProfileLastModified) { %>
          <dt>Last Modified</dt>
          <dd><%= person.ProfileLastModified %></dd>
        <% } %>
      </dl>
    """

  initialize: (opts) ->
    @col = opts.col

  events:
    "click button.person-span": "selectPerson"

  render: ->
    @$el.html @templates.table
      col: @col

    if @dataTable
      @dataTable.fnDraw()
    else
      _.defer =>
        @dataTable = new Profiling.DataTable "#{@col}-table",
          sAjaxSource: "#{Profiling.applicationUrl}Profiling/Persons/DataTablesLucene"
          bStateSave: false
          sDom: 'T<"clear">ftipr'
          aoColumns: [ 
            { mDataProp: 'Id', bSortable: false }, 
            { mDataProp: 'Name', bSortable: false }, 
            { mDataProp: 'MilitaryIDNumber', bSortable: false }, 
            { mDataProp: 'Rank', bSortable: false, bVisible: false }, 
            { mDataProp: 'Function', bSortable: false, bVisible: false } 
          ]
          fnRowCallback: (nRow, aData, iDisplayIndex) ->
            $("td:eq(0)", nRow).html "<button class='btn btn-mini person-span' style='width: auto;' data-id='#{aData['Id']}'><i class='accordion-toggle icon-ok'></i></button>"

            for field, i in [ 'Name', 'MilitaryIDNumber' ]
              list = aData[field]
              if _(list).size() is 1
                $("td:eq(#{i+1})", nRow).html "#{_(list).first()}"
              else if _(list).size() > 1
                prefix = if field is 'Name' then '<br />(a.k.a.) ' else '<br />'
                $("td:eq(#{i+1})", nRow).html "#{_(list).first()} #{_(list).tail().map((x) -> prefix + x).join('')}"

    @

  selectPerson: (e) ->
    button = if $(e.target).is("button")
      $(e.target)
    else
      $(e.target).parent()
    @personId = $(button).data "id"

    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Persons/Json/#{@personId}"
      type: "GET"
      error: (xhr, textStatus) ->
        if Profiling.isEmptyString(xhr.responseText)
          bootbox.alert xhr.responseText
        else
          bootbox.alert xhr.statusText
      success: (data, textStatus, xhr) =>
        $("##{@col}-result").html @templates.person
          col: @col
          person: data.Person

  resetPerson: ->
    @personId = 0
    $("##{@col}-result").html ""