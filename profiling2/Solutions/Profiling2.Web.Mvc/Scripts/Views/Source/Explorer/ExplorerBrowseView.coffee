Profiling.Views.ExplorerBrowseView = Backbone.View.extend

  templates:
    table: _.template """
      <hr />
      <div class="pull-right">
        <label class="checkbox">
          <input type="checkbox" id="checkbox-show-all-sub-directories" /> Show all sub-directories
        </label>
      </div>
      <table id="browse-table" class="table table-bordered">
        <thead>
          <tr>
            <th>Name</th><th>Date Modified</th><th>File Size</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
    """

  initialize: (opts) ->
    @code = opts.code
    @pathsView = opts.pathsView

  events:
    "change #checkbox-show-all-sub-directories": "createOrReloadTable"

  render: ->
    @pathsView.on "change:paths", =>
      @createOrReloadTable()

    @$el.html @templates.table
    _.defer =>
      @createOrReloadTable()
        
    @

  createOrReloadTable: ->
    if @dataTable
      @dataTable.fnReloadAjax()
    else
      @dataTable = new Profiling.DataTable 'browse-table',
        sAjaxSource: "#{Profiling.applicationUrl}Sources/Explorer/DataTables",
        sDom: 'T<"clear">ltipr',
        aoColumns: [ { mDataProp: 'Name', bSortable: true }, 
          { mDataProp: 'FileDateTimeStamp', bSortable: true }, 
          { mDataProp: 'FileSize', bSortable: true }
        ],
        fnServerParams: (aoData) =>
          aoData.push
            name: "showAllSubDirectories"
            value: $('#checkbox-show-all-sub-directories').is(':checked')
        fnServerData: (sSource, aoData, fnCallback) ->
          for obj in aoData
            if obj.name is 'sSearch'
              obj.value = $("select#path-select").val()
          $.getJSON sSource, aoData, (json) ->
            fnCallback json
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          name = aData['Name']
          name += "<span class='label label-important pull-right'><small>RESTRICTED</small></span>" if aData['IsRestricted'] is true

          $("td:eq(0)", nRow).html name
          $("td:eq(0)", nRow).uitooltip
            show: false
            hide: false
            track: true

          previewView = new Profiling.Views.ExplorerPreviewModalView
            sourceId: aData['Id']
            sourceName: aData['Name']
            sourceFolder: aData['Path']
            sourcePath: aData['Path']
            isRestricted: aData['IsRestricted']
            isReadOnly: false
            date: aData['FileDateTimeStamp']
          $("td:eq(0)", nRow).append previewView.render().el

          $("td:eq(2)", nRow).html "#{(aData['FileSize'] / 1024).toFixed(0)} KB"