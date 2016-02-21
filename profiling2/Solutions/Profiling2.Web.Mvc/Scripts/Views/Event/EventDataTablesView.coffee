Profiling.Views.EventDataTablesView = Backbone.View.extend

  id: "events-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
        <tr>
          <th>ID</th>
          <th>Case Code</th>
          <th>Categories</th>
          <th>Start Date</th>
          <th>End Date</th>
          <th>Location</th>
        </tr>
      </thead>
      <tbody>
      </tbody>
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

  render: ->
    @$el.append @templates.table

    _.defer =>
      @dtable = new Profiling.DataTable "events-table"
        bStateSave: false
        sAjaxSource: "#{Profiling.applicationUrl}Profiling/Events/DataTables"
        aaSorting: [ ]
        aoColumns: [ { mDataProp: 'Id', bSortable: true },
          { mDataProp: 'JhroCaseNumber', bSortable: true },
          { mDataProp: 'Violations', bSortable: false }, 
          { mDataProp: 'StartDateDisplay', bSortable: true }, 
          { mDataProp: 'EndDateDisplay', bSortable: true }, 
          { mDataProp: 'Location', bSortable: true } 
        ]
        fnRowCallback: (nRow, aData, iDisplayIndex) ->
          $("td:eq(0)", nRow).html "<a href='#{Profiling.applicationUrl}Profiling/Events/Details/#{aData['Id']}'>#{aData['Id']}</a>"
          $("td:eq(2)", nRow).css "white-space", "nowrap"
        fnServerData: (sSource, aoData, fnCallback) ->
          aoData.push 
            name: "start-date"
            value: $("#start-date").val()
          aoData.push
            name: "end-date"
            value: $("#end-date").val()
          $.getJSON sSource, aoData, (json) ->
            fnCallback(json)

      $("#events-table_filter").append @templates.dateFilter
      $("#events-table_filter label").css "display", "inline-block"

      $(".input-daterange input").change (e) =>
        @dtable.fnReloadAjax()

    @

  