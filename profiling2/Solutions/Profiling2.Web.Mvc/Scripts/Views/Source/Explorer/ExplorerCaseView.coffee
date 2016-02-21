Profiling.Views.ExplorerCaseView = Backbone.View.extend

  templates:
    table: _.template """
      <hr />
      <table id="browse-table" class="table table-bordered">
        <thead>
          <tr>
            <th>Case Number<th>Name</th><th>Date Modified</th><th>File Size</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
    """

  render: ->
    @$el.append @templates.table
    _.defer ->
      @dataTable = new Profiling.DataTable 'browse-table',
        sAjaxSource: "#{Profiling.applicationUrl}Sources/All/DataTablesCases",
        aoColumns: [ { mDataProp: 'JhroCaseNumber', bSortable: true },
          { mDataProp: 'Name', bSortable: true }, 
          { mDataProp: 'FileDateTimeStamp', bSortable: true }, 
          { mDataProp: 'FileSize', bSortable: true }
        ],
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).html """
            <a href="#{Profiling.applicationUrl}/Profiling/Hrdb/Case?CaseNumber=#{aData['JhroCaseNumber']}" target="_blank">#{aData['JhroCaseNumber']}</a>
          """
          name = "<a href='#{Profiling.applicationUrl}Profiling/Sources#info/#{aData['Id']}' target='_blank'>#{aData['Name']}</a>"
          name += "<span class='label label-important pull-right'><small>RESTRICTED</small></span>" if aData['IsRestricted'] is true
          $("td:eq(1)", nRow).html name
          $("td:eq(2)", nRow).html moment(aData['FileDateTimeStamp']).format('YYYY-MM-DD HH:mm')
          $("td:eq(2)", nRow).css 'white-space', 'nowrap'
          $("td:eq(3)", nRow).html "#{(aData['FileSize'] / 1024).toFixed(0)} KB"
    @