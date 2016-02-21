Profiling.Views.PersonDataTablesLuceneView = Backbone.View.extend

  id: "persons-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
          <tr>
              <th>ID</th>
              <th>Name(s)</th>
              <th>ID Number(s)</th>
              <th>Rank</th>
              <th>Function</th>
          </tr>
      </thead>
      <tbody></tbody>
    """

  render: ->
    @$el.append @templates.table

    _.defer =>
      new Profiling.DataTable 'persons-table',
        sAjaxSource: "#{Profiling.applicationUrl}Profiling/Persons/DataTablesLucene",
        aoColumns: [ { mDataProp: 'Id', bSortable: false }, { mDataProp: 'Name', bSortable: false }, { mDataProp: 'MilitaryIDNumber', bSortable: false }, { mDataProp: 'Rank', bSortable: false }, { mDataProp: 'Function', bSortable: false } ]
        fnRowCallback: (nRow, aData, iDisplayIndex) ->
          $("td:eq(0)", nRow).html "<a href='#{Profiling.applicationUrl}Profiling/Persons/Details/#{aData['Id']}'>#{aData['Id']}</a>"
          
          for field, i in [ 'Name', 'MilitaryIDNumber' ]
            list = aData[field]
            if _(list).size() is 1
              $("td:eq(#{i+1})", nRow).html "#{_(list).first()}"
            else if _(list).size() > 1
              prefix = if field is 'Name' then '<br />(a.k.a.) ' else '<br />'
              $("td:eq(#{i+1})", nRow).html "#{_(list).first()} #{_(list).tail().map((x) -> prefix + x).join('')}"

    @

  