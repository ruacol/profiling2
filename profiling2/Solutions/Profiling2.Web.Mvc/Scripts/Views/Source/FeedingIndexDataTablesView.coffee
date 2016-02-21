Profiling.Views.FeedingIndexDataTablesView = Backbone.View.extend

  id: "feeding-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
        <tr>
          <th>Id</th>
          <th>Name</th>
          <th>Restricted</th>
          <th>File Modified Date</th>
          <th>Uploaded By</th>
          <th>Upload Date</th>
          <th>Approved By</th>
          <th>Approve Date</th>
          <th>Rejected By</th>
          <th>Rejected Date</th>
          <th>Rejected Reason</th>
          <th>Upload Notes</th>
          <th style="text-align: center;">Status</th>
          <th style="text-align: center;">Last Update Date</th>
          <th>Last Update By</th>
          <th>Persons Attached</th>
          <th>Events Attached</th>
          <th>Units Attached</th>
          <th>Operations Attached</th>
          <th style="text-align: center;">Attached</th>
          <th style="text-align: center;">Actions</th>
        </tr>
      </thead>
      <tbody></tbody>
    """
    actions: _.template """
      <div class="btn-group">
        <button class="btn btn-mini dropdown-toggle" data-toggle="dropdown">
          Action
          <span class="caret"></span>
        </button>
        <ul class="dropdown-menu" style="text-align: left;">
          <li>
            <a href='<%= Profiling.applicationUrl %>Sources/Feeding/Details/<%= dto.id %>' style='margin-right: 5px;'>View</a>
          </li>
          <li>
            <a href='<%= Profiling.applicationUrl %>Sources/Feeding/Download/<%= dto.id %>' style='margin-right: 5px;'>Download</a>
          </li>
          <% if (dto.status == "Waiting") { %>
            <li>
              <a href='<%= Profiling.applicationUrl %>Sources/Feeding/Rename/<%= dto.id %>' style='margin-right: 5px;'>Rename</a>
            </li>
          <% } %>
          <li>
            <a href='<%= Profiling.applicationUrl %>Sources/Feeding/Delete/<%= dto.id %>' style='margin-right: 5px;'>Delete</a>
          </li>
        </ul>
      </div>
    """

  render: ->
    @$el.append @templates.table

    _.defer =>
      new Profiling.DataTable 'feeding-table',
        sAjaxSource: "#{Profiling.applicationUrl}Sources/Feeding/DataTables"
        aaSorting: [ [13, 'desc'] ]
        aoColumnDefs: [ 
          { aTargets: [ 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14, 15, 16, 17, 18 ], bVisible: false }, 
          { aTargets: [ 20 ], bSortable: false },
          { aTargets: [ 15, 16, 17, 18 ], sType: "numeric" }
        ]
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          dto =
            id: aData[0]
            name: aData[1]
            restricted: aData[2]
            fileModifiedDateTime: aData[3]
            uploadedBy: aData[4]
            uploadDate: aData[5]
            approvedBy: aData[6]
            approveDate: aData[7]
            rejectedBy: aData[8]
            rejectedDate: aData[9]
            rejectedReason: aData[10]
            uploadNotes: aData[11]
            status: aData[12]
            lastUpdateDate: aData[13]
            lastUpdatedBy: aData[14]
            numPersonSources: aData[15]
            numEventSources: aData[16]
            numUnitSources: aData[17]
            numOperationSources: aData[18]
            totalSources: aData[19]
            sourceId: aData[20]
            isReadOnly: aData[21]

          if dto.sourceId isnt "0"
            firstColumn = "<a href='#{Profiling.applicationUrl}Profiling/Sources#info/#{dto.sourceId}'>#{dto.name}</a>"
          else
            firstColumn = dto.name
          firstColumn += "<span class='label label-important pull-right' style='margin-left: 5px;'><small>RESTRICTED</small></span>" if dto.restricted is true
          firstColumn += "<i class='icon-lock pull-right' style='margin-left: 5px;' title='File is read only.'></i>" if dto.isReadOnly is true
          $("td:eq(0)", nRow).html firstColumn
          if dto.uploadNotes
            dto.uploadNotes = dto.uploadNotes.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />')
            $("td:eq(0)", nRow).attr 'title', "<p>Notes from uploader:</p> <p>#{dto.uploadNotes}</p>"

          if dto.status is 'Waiting'
            $("td:eq(1)", nRow).html """
              <div class="btn-group">
                <button class="btn btn-mini dropdown-toggle" data-toggle="dropdown">
                  #{dto.status}
                  <span class="caret"></span>
                </button>
                <ul class="dropdown-menu" style="text-align: left;">
                  <li>
                    <a href='#{Profiling.applicationUrl}Sources/Feeding/Approve/#{dto.id}' style='margin-right: 5px;'>Approve</a>
                  </li>
                  <li>
                    <a href='#{Profiling.applicationUrl}Sources/Feeding/Reject/#{dto.id}' style='margin-right: 5px;'>Reject</a>
                  </li>
                </ul>
              </div>
            """
          else
            $("td:eq(1)", nRow).attr 'title', "#{ if dto.rejectedReason then dto.rejectedReason else '(no reason given)' }" if dto.status is 'Rejected'
            $("td:eq(1)", nRow).html dto.status

          $("td:eq(2)", nRow).html moment(dto.lastUpdateDate).format(Profiling.NUMERIC_DATE_FORMAT)
          $("td:eq(2)", nRow).attr 'title', "#{if dto.status is 'Waiting' then 'Uploaded' else dto.status} by #{dto.lastUpdatedBy}"

          $("td:eq(3)", nRow).attr 'title', "Persons: #{dto.numPersonSources}, Events: #{dto.numEventSources}, Units: #{dto.numUnitSources}, Operations: #{dto.numOperationSources}"

          $("td:eq(4)", nRow).html @templates.actions
            dto: dto

          $("td:gt(0)", nRow).css "text-align", "center"
          _.defer ->
            Profiling.setupUITooltips $("td", nRow)

    @

  