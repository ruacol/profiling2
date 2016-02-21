Profiling.Views.RequestDataTablesView = Backbone.View.extend

  id: "requests-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
        <tr>
          <th></th>
          <th style="white-space: nowrap;" title="Reference Number">Ref. Number</th>
          <th>Name</th>
          <th>Entity</th>
          <th>Type</th>
          <th style="white-space: nowrap;">Respond By</th>
          <th style="white-space: nowrap;">Status</th>
          <th style="white-space: nowrap;">Status Date</th>
          <th>Persons</th>
        </tr>
      </thead>
      <tbody></tbody>
    """

  render: ->
    @$el.append @templates.table

    invisibleColumns = if @options.showLastUpdate
      [ 0, 5 ]
    else
      [ 0, 7 ]

    _.defer =>
      new Profiling.DataTable 'requests-table',
        sAjaxSource: @options.dataTablesAjaxSource
        bStateSave: false
        aaSorting: [ [1, 'desc'] ]
        aoColumnDefs: [ { aTargets: invisibleColumns, bVisible: false }, { aTargets: [ 0 ], bSortable: false }, { aTargets: [ 5 ], sType: 'title-numeric' } ]
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).html "<a href='#{@options.dataTablesActionUrl}#{aData[0]}'>#{aData[1]}</a>"

          if aData[9]
            $("td:eq(1)", nRow).attr "title", "Description: #{aData[9]}"
            $("td:eq(1)", nRow).uitooltip
              show: false
              hide: false
              position:
                my: "left+15 center"
                at: "right center"

          if @options.showLastUpdate
            $("td:eq(5)", nRow).css("white-space", "nowrap").html "#{moment(aData[7]).format(Profiling.NUMERIC_DATE_FORMAT)}"

    @

  