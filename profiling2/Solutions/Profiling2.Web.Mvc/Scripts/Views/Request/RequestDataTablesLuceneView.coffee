Profiling.Views.RequestDataTablesLuceneView = Backbone.View.extend

  id: "requests-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
        <tr>
          <th style="white-space: nowrap;" title="Reference Number">Ref. Number</th>
          <th>Name</th>
          <th>Entity</th>
          <th>Type</th>
          <th style="white-space: nowrap;">Status</th>
          <th style="white-space: nowrap;">Status Date</th>
          <th>Persons</th>
        </tr>
      </thead>
      <tbody></tbody>
    """

  render: ->
    @$el.append @templates.table

    _.defer =>
      new Profiling.DataTable 'requests-table',
        sAjaxSource: "#{Profiling.applicationUrl}Screening/Requests/DataTablesLucene"
        bStateSave: false
        aaSorting: [ [0, 'desc'] ]
        aoColumns: [ 
          { mDataProp: 'ReferenceNumber' }, 
          { mDataProp: 'RequestName' }, 
          { mDataProp: 'RequestEntity' }, 
          { mDataProp: 'RequestType' }, 
          { mDataProp: 'CurrentStatus' },
          { mDataProp: 'CurrentStatusDate' },
          { mDataProp: 'Persons' }
        ]
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).html "<a href='#{Profiling.applicationUrl}Screening/Requests/Details/#{aData['Id']}'>#{aData['ReferenceNumber']}</a>"

          if aData['Description']
            $("td:eq(1)", nRow).attr "data-selector", "#description-#{aData['Id']}"
            $("td:eq(1)", nRow).attr "id", "name-#{aData['Id']}"
            $("td:eq(1)", nRow).html """
              #{aData['RequestName']}
              <div id="description-#{aData['Id']}" style="display: none;">
                <strong>Description</strong>
                <p>#{aData['Description'].replace('\r\n', '').replace('\n', '')}</p>
              </div>
            """
            _.defer ->
              Profiling.setupUITooltips "#name-#{aData['Id']}"

          $("td:eq(5)", nRow).css("white-space", "nowrap").html "#{moment(aData['CurrentStatusDate']).format('YYYY-MM-DD HH:mm')}"

    @

  