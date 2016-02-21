Profiling.Views.PersonDataTablesView = Backbone.View.extend

  id: "persons-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
          <tr>
              <th>ID</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Aliases</th>
              <th>Military ID</th>
          </tr>
      </thead>
      <tbody></tbody>
    """

  render: ->
    @$el.append @templates.table

    _.defer =>
      new Profiling.DataTable 'persons-table',
        sAjaxSource: "#{Profiling.applicationUrl}Profiling/Persons/DataTables",
        aoColumns: [ { mDataProp: 'Id', bSortable: false }, { mDataProp: 'FirstName', bSortable: false }, { mDataProp: 'LastName', bSortable: false }, { mDataProp: 'Aliases', bSortable: false }, { mDataProp: 'MilitaryIDNumber', bSortable: false } ]
        fnRowCallback: (nRow, aData, iDisplayIndex) ->
          $("td:eq(0)", nRow).html "<a href='#{Profiling.applicationUrl}Profiling/Persons/Details/#{aData['Id']}'>#{aData['Id']}</a>"
        fnServerData: (sSource, aoData, fnCallback) ->
          # Validate search term - stored proc can't handle 5 or more terms
          sSearch = _.chain(aoData)
            .filter((x) -> x.name is 'sSearch')
            .first()
            .value()
          if sSearch.value
            numTerms = _.chain(sSearch.value.split " ")
              .reject((x) -> $.trim(x).length is 0)
              .size()
              .value()
            if numTerms > 4
              bootbox.alert "Search must not have more than 4 terms."
              $("#persons-table_processing").css "visibility", "hidden"
              return

          $.getJSON sSource, aoData, (json) ->
            fnCallback json

    @

  