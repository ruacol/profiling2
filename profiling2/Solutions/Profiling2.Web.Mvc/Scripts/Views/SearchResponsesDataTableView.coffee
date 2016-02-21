Profiling.Views.SearchResponsesDataTablesView = Backbone.View.extend

  id: "responses-table"
  className: "table table-bordered table-hover table-condensed"
  tagName: "table"

  templates:
    table: _.template """
      <thead>
        <tr>
          <th>Name</th>
          <th style="text-align: center;">Last <%= screeningEntityName %> Color Coding</th>
          <th style="text-align: center;">Last Screening Result</th>
          <th style="text-align: center;">Last Screening Date</th>
        </tr>
      </thead>
      <tbody>
      </tbody>
    """
    modal: _.template """
      <%= personName %>
      <div id="modal-<%= personId %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true" data-width="65%">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>
          <h4><%= personName %></h4>
        </div>
        <div class='modal-body'>
          <p>
            Color coded&nbsp;&nbsp;<%= lastColourCoding %>&nbsp;&nbsp;by <%= screeningEntityName %> on <em><%= lastScreeningDate %></em>.
            &nbsp;&nbsp;Screening result was <em><%= lastScreeningResult %></em>.
          </p>
          <% if (reason) { %>
            <strong>Reason</strong>
            <p><%= reason %></p>
          <% } %>
          <% if (commentary) { %>
            <strong>Commentary</strong>
            <p><%= commentary %></p>
          <% } %>
        </div>
        <div class="modal-footer">
          <a class="btn" href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= personId %>" target="_blank">Open Profile</a>
          <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
        </div>
      </div>
    """
    colourCoding: _.template """
      <span
        <% if (colour == 'Green') { %>
          class="label label-success"
        <% } else if (colour == 'Yellow') { %>
          class="label label-warning"
        <% } else if (colour == 'Red') { %>
          class="label label-important"
        <% } %>
      >
        <%= colour %>
      </span>
    """

  initialize: (opts) ->
    @screeningEntityName = opts.screeningEntityName

  render: ->
    @$el.append @templates.table
      screeningEntityName: @screeningEntityName

    _.defer =>
      new Profiling.DataTable 'responses-table',
        sAjaxSource: "#{Profiling.applicationUrl}Screening/ScreeningEntity/DataTables"
        bStateSave: false
        aoColumns: [ { mDataProp: 'PersonName', bSortable: false }, { mDataProp: 'LastColourCoding', bSortable: false }, { mDataProp: 'LastScreeningResult', bSortable: false }, { mDataProp: 'LastScreeningDate', bSortable: false } ]
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          lastScreeningDate = moment(aData['LastScreeningDate']).format(Profiling.NUMERIC_DATE_FORMAT)

          $("td:eq(0)", nRow).html @templates.modal
            personId: aData['PersonId']
            personName: aData['PersonName']
            screeningEntityName: @screeningEntityName
            lastColourCoding: @templates.colourCoding
              colour: aData['LastColourCoding']
            lastScreeningResult: aData['LastScreeningResult']
            lastScreeningDate: lastScreeningDate
            reason: "#{if aData['Reason'] then aData['Reason'].replace(/\r\n/g, "<br />").replace(/\n/g, "<br />") else ""}"
            commentary: "#{if aData['Commentary'] then aData['Commentary'].replace(/\r\n/g, "<br />").replace(/\n/g, "<br />") else ""}"

          $("td:gt(0)", nRow).css("text-align", "center")
          $("td:eq(1)", nRow).html @templates.colourCoding
            colour: aData['LastColourCoding']
          $("td:eq(3)", nRow).html lastScreeningDate
          
          $(nRow).addClass("accordion-toggle").attr("data-toggle", "modal").attr("data-target", "#modal-#{aData['PersonId']}")

    @
