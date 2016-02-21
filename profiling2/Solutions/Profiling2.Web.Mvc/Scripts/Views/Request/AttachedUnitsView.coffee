Profiling.Views.AttachedUnitsView = Backbone.View.extend

  id: "attached-units-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  initialize: ->
    @.on "attached-units:redraw", @redrawTable

  templates:
    table: _.template """
      <thead>
        <tr>
          <th></th>
          <th>Name</th>
          <th>Organization</th>
          <th></th>
        </tr>
      </thead>
      <tbody></tbody>
    """

  render: ->
    @$el.html @templates.table
    
    thisView = @
    _.defer =>
      @table = new Profiling.DataTable 'attached-units-table',
        sAjaxSource: "#{Profiling.applicationUrl}Screening/Initiate/AttachedUnitsDataTables/#{$('#Id').val()}"
        sDom: 'ftr'
        iDisplayLength: 200
        aoColumnDefs: [ { aTargets: [ 0, 1, 2, 3 ], bSortable: false } ]
        fnPreDrawCallback: ->
          # We enable the search filter ('f' character in sDom parameter), because in Firefox 21.0 without it, pressing enter on the text input
          # triggers the ajax data call
          $("#attached-units-table_filter").hide()
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).text(iDisplayIndex + 1).css "text-align", "center"
          $("td:eq(1)", nRow).css "white-space", "nowrap"
          $("td:eq(2)", nRow).html aData[3]
          $("td:eq(3)", nRow).html """
            <a id="remove-#{aData[0]}" class="btn btn-mini" title="Remove"><i class="icon-remove"></i></a>
          """

          _.defer () =>
            $("#remove-#{aData[0]}").click (e) =>
              $.ajax
                url: "#{Profiling.applicationUrl}Screening/Initiate/#{$("#Id").val()}/RemoveUnit/#{aData[0]}" 
                success: (data, textStatus, xhr) =>
                  @redrawTable()
                error: (xhr, textStatus) ->
                  bootbox.alert xhr.responseText
          
    @

  redrawTable: ->
    @table.fnDraw() if @table