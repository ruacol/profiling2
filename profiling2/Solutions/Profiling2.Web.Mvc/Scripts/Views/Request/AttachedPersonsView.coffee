Profiling.Views.AttachedPersonsView = Backbone.View.extend

  id: "attached-persons-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  initialize: ->
    @.on "attached-persons:redraw", @redrawTable

  templates:
    table: _.template """
      <thead>
        <tr>
          <th></th>
          <th>Name</th>
          <th>ID Number</th>
          <th>Notes</th>
          <th>Function</th>
          <th>Rank</th>
          <th>Last Screening Support Status</th>
          <th></th>
        </tr>
      </thead>
      <tbody></tbody>
    """

  render: ->
    @$el.html @templates.table
    
    thisView = @
    _.defer =>
      @table = new Profiling.DataTable 'attached-persons-table',
        sAjaxSource: "#{Profiling.applicationUrl}Screening/Initiate/AttachedPersonsDataTables/#{$('#Id').val()}"
        sDom: 'ftr'
        iDisplayLength: 200
        aaSorting: [ [7, 'asc'] ]
        aoColumnDefs: [ { aTargets: [ 0, 1, 2, 3, 4, 5, 6, 7 ], bSortable: false } ]
        fnPreDrawCallback: ->
          # We enable the search filter ('f' character in sDom parameter), because in Firefox 21.0 without it, pressing enter on the text input
          # triggers the ajax data call
          $("#attached-persons-table_filter").hide()
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).text(iDisplayIndex + 1).css "text-align", "center"
          if aData[0] and aData[0] > 0
            # Row is a RequestPerson
            $("td:eq(1)", nRow).html """
              <a href="#{Profiling.applicationUrl}Profiling/Persons/Details/#{aData[0]}">#{aData[1]}</a>
            """
            $("td:eq(7)", nRow).html """
              <a id="remove-#{aData[0]}" class="btn btn-mini" title="Remove"><i class="icon-remove"></i></a>
            """
            editRequestPersonView = new Profiling.Views.EditRequestPersonView
              modalId: "modal-edit-request-person-#{aData[7]}"
              modalUrl: "#{Profiling.applicationUrl}Screening/RequestPerson/RequestPersonModal/#{aData[7]}"
              modalSaveButton: "modal-edit-request-person-button"
              attachedPersonsView: thisView
            $("td:eq(7)", nRow).prepend editRequestPersonView.render().el
          else
            # Row is a RequestProposedPerson
            if aData[8] and aData[8] > 0
              $("td:eq(7)", nRow).html """
                <a id="remove-proposed-#{aData[8]}" class="btn btn-mini" title="Remove"><i class="icon-remove"></i></a>
              """
              editProposedPersonView = new Profiling.Views.EditProposedPersonView
                modalId: "modal-edit-proposed-person-#{aData[8]}"
                modalUrl: "#{Profiling.applicationUrl}Screening/RequestPerson/ProposedPersonModal/#{aData[8]}"
                modalSaveButton: "modal-edit-proposed-person-button"
                attachedPersonsView: thisView
              $("td:eq(7)", nRow).prepend editProposedPersonView.render().el

          # Handle click of buttons
          _.defer () =>
            if aData[0] and aData[0] > 0
              $("#remove-#{aData[0]}").click (e) =>
                $.ajax
                  url: "#{Profiling.applicationUrl}Screening/Initiate/#{$("#Id").val()}/RemovePerson/#{aData[0]}" 
                  success: (data, textStatus, xhr) =>
                    @redrawTable()
                  error: (xhr, textStatus) ->
                    bootbox.alert xhr.responseText

            if aData[8] and aData[8] > 0
              $("#remove-proposed-#{aData[8]}").click (e) =>
                $.ajax
                  url: "#{Profiling.applicationUrl}Screening/Initiate/#{$("#Id").val()}/RemoveProposedPerson/#{aData[8]}" 
                  success: (data, textStatus, xhr) =>
                    @redrawTable()
                  error: (xhr, textStatus) ->
                    bootbox.alert xhr.responseText
    @

  redrawTable: ->
    @table.fnDraw() if @table