Profiling.Views.SourceResultsView = Backbone.View.extend

  initialize: (opts) ->
    @sourceSearchFormView = opts.sourceSearchFormView

  templates:
    table: _.template """
      <table id="sources-table" class="table table-bordered table-condensed">
        <thead>
          <tr>
            <th>Rank</th>
            <th>Rating</th>
            <th>Attached</th>
            <th>ID</th>
            <th>Source Name</th>
            <th>File Date</th>
            <th>Restricted</th>
            <th title="Last marked relevant, downloaded, or previewed by you">Reviewed</th>
          </tr>
        </thead>
        <tbody></tbody>
      </table>
    """
    modal: _.template """
      <div id="modal-div" class="modal hide fade"></div>
    """

  events:
    "click .icon-search,.icon-check": "preview"
    "click .info-link": "info"
    "click .icon-star": "markIrrelevant"
    "click .icon-remove": "markUnknown"
    "click .icon-question-sign": "markRelevant"
    "click .icon-tag,.icon-ok": "attachSource"
    "hidden .modal": "hiddenModalDiv"

  render: ->
    @$el.html @templates.table
    _.defer =>
      @initDataTable()
    @$el.append @templates.modal
    @

  redrawTable: ->
    @dataTable.fnDraw() if @dataTable

  initDataTable: ->
    @dataTable = new Profiling.DataTable 'sources-table',
      bStateSave: false
      sAjaxSource: "#{Profiling.applicationUrl}Profiling/Sources/DataTables"
      sDom: 'T<"clear">tipr'
      iDisplayLength: 1000
      bFilter: false
      aaSorting: [ [ 1, 'desc' ], [ 5, 'desc' ] ]
      aoColumns: [ { mDataProp: 'Rank' }, { mDataProp: 'IsRelevant' }, { mDataProp: 'IsAttached', bSortable: (if @sourceSearchFormView.selectAttachTargetView.getTargetId() then true else false) }, { mDataProp: 'Id' }, { mDataProp: 'SourceName' }, { mDataProp: 'FileDateTimeStamp' }, { mDataProp: 'IsRestricted' }, { mDataProp: 'ReviewedDateTime' }]
      fnRowCallback: (nRow, aData, iDisplayIndex) =>
        $("td:eq(1)", nRow).each (i, el) ->
          iconClass = switch $(el).text()
            when '2' then 'icon-star'
            when '1' then 'icon-question-sign'
            when '0' then 'icon-remove'
          $(el).html "<i class='accordion-toggle #{iconClass}'><span class='invisible'>#{aData['Id']}</span></i>" if iconClass
          $(el).css("text-align", "center")
        $("td:eq(2)", nRow).each (i, el) =>
          if @sourceSearchFormView.selectAttachTargetView.getTargetId() and $(el).text()
            if $(el).text() > 0
              $(el).html "<i class='accordion-toggle icon-ok' title='Click to detach' id='attach-icon-#{aData['Id']}'><span class='invisible'>#{aData['Id']}</span></i>"
            else
              $(el).html "<i class='accordion-toggle icon-tag' title='Click to attach' id='attach-icon-#{aData['Id']}'><span class='invisible'>#{aData['Id']}</span></i>"
            $(el).css("text-align", "center")
        $("td:eq(4)", nRow).html """
          <i class='accordion-toggle icon-#{if aData['ReviewedDateTime'] then 'check' else 'search'}' title='Preview'>
            <span class='invisible'>#{aData['Id']}</span>
          </i>&nbsp;
          <a href='#' data-source-id="#{aData['Id']}" title='View information for this source' class='info-link'>
            #{aData['SourceName']}
          </a>
        """
        $("td:eq(4)", nRow).click =>
          # Remember row in order to scroll back to it (Firefox)
          @currentRow = iDisplayIndex + @dataTable.fnSettings()._iDisplayStart
          # Highlight clicked row
          $("td").removeClass "highlighted"
          $("td:eq(4)", nRow).addClass "highlighted"
          @selectedSourceId = aData['Id']

        # Highlight selected source
        if @selectedSourceId is aData['Id']
          if not $("td:eq(4)", nRow).hasClass "highlighted"
            $("td:eq(4)", nRow).addClass "highlighted"

        $("td:eq(5)", nRow).each (i, el) ->
          if $(el).text()
            $(el).html moment($(el).text()).format(Profiling.NUMERIC_DATE_FORMAT)
            $(el).css "white-space", "nowrap"
        $("td:eq(6)", nRow).each (i, el) ->
          $(el).html "<i class='icon-exclamation-sign' title='Restricted'></i>" if $(el).text() is 'true'
          $(el).html "" if $(el).text() is 'false'
          $(el).css("text-align", "center")
        $("td:eq(7)", nRow).each (i, el) ->
          if $(el).text()
            $(el).html moment($(el).text()).format(Profiling.NUMERIC_DATE_FORMAT)
            $(el).css "white-space", "nowrap"
      fnServerData: (sSource, aoData, fnCallback) =>
        aoData.push
          name: "searchText"
          value: $("#search-text").val()
        aoData.push
          name: "searchId"
          value: $("#search-id").val()
        aoData.push
          name: "searchName"
          value: $("#search-name").val()
        aoData.push
          name: "searchExtension"
          value: $("#search-extension").val()
        aoData.push 
          name: "startDate"
          value: $("#start-date").val()
        aoData.push
          name: "endDate"
          value: $("#end-date").val()
        aoData.push
          name: "searchAdminSourceSearchId"
          value: $("#search-adminSourceSearchId").val()
        aoData.push
          name: "authorSearchText"
          value: $("#author-search-text").val()
        sourceAttachTargetType = @sourceSearchFormView.selectAttachTargetView.getTargetType()
        if sourceAttachTargetType
          aoData.push
            name: if sourceAttachTargetType is 'Person' then 'personId' else if sourceAttachTargetType is 'Event' then 'eventId' else ''
            value: @sourceSearchFormView.selectAttachTargetView.getTargetId()
        $.getJSON sSource, aoData, (json) ->
          $("#search-adminSourceSearchId").val(json.adminSourceSearchId);
          fnCallback json

  preview: (e) ->
    sourceId = $.trim $(e.target).text()
    $(e.target).removeClass "icon-search"
    $(e.target).addClass "icon-check"
    $(e.target).parent().next().next().next().text moment().format(Profiling.NUMERIC_DATE_FORMAT)

    Backbone.history.navigate "preview/#{sourceId}/#{$("#search-adminSourceSearchId").val()}", true

  info: (e) ->
    sourceId = $(e.target).data 'source-id'

    Backbone.history.navigate "info/#{sourceId}/#{$("#search-adminSourceSearchId").val()}", true

  markIrrelevant: (e) ->
    sourceId = $(e.target).text()
    adminSourceSearchId = $("#search-adminSourceSearchId").val()
    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Sources/IsIrrelevant/#{sourceId}/#{adminSourceSearchId}"
      method: 'get'
      beforeSend: ->
        $(e.target).removeClass "icon-star"
        $(e.target).addClass "icon-refresh"
      complete: ->
        $(e.target).removeClass "icon-refresh"
      success: ->
        $(e.target).addClass "icon-remove"
      error: ->
        $(e.target).addClass "icon-star"

  markUnknown: (e) ->
    sourceId = $(e.target).text()
    adminSourceSearchId = $("#search-adminSourceSearchId").val()
    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Sources/IsUnknown/#{sourceId}/#{adminSourceSearchId}"
      method: 'get'
      beforeSend: ->
        $(e.target).removeClass "icon-remove"
        $(e.target).addClass "icon-refresh"
      complete: ->
        $(e.target).removeClass "icon-refresh"
      success: ->
        $(e.target).addClass "icon-question-sign"
      error: ->
        $(e.target).addClass "icon-remove"

  markRelevant: (e) ->
    sourceId = $(e.target).text()
    adminSourceSearchId = $("#search-adminSourceSearchId").val()
    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Sources/IsRelevant/#{sourceId}/#{adminSourceSearchId}"
      method: 'get'
      beforeSend: ->
        $(e.target).removeClass "icon-question-sign"
        $(e.target).addClass "icon-refresh"
      complete: ->
        $(e.target).removeClass "icon-refresh"
      success: ->
        $(e.target).addClass "icon-star"
      error: ->
        $(e.target).addClass "icon-question-sign"

  attachSource: (e) ->
    @sourceId = $.trim $(e.target).text()
    targetId = @sourceSearchFormView.selectAttachTargetView.getTargetId()
    @url = if @sourceSearchFormView.selectAttachTargetView.getTargetType() is 'Person'
      "#{Profiling.applicationUrl}Profiling/Sources/#{@sourceId}/Attach/Person/#{targetId}"
    else if @sourceSearchFormView.selectAttachTargetView.getTargetType() is 'Event'
      "#{Profiling.applicationUrl}Profiling/Sources/#{@sourceId}/Attach/Event/#{targetId}"
    if @url and targetId
      $("#modal-div").load @url, '', =>
        $("#modal-div").modal
          keyboard: true
        $("#modal-button").click () =>
          @submitModalForm()
    else
      bootbox.alert "No person or event has been selected."

  submitModalForm: ->
    $.ajax
      url: @url
      type: 'post'
      data: $("#modal-div input,#modal-div select,#modal-div textarea").serialize()
      complete: ->
        $("#modal-div").modal 'hide'
      success: (data, textStatus, xhr) =>
        if $("#attach-icon-#{@sourceId}").hasClass 'icon-tag'
          $("#attach-icon-#{@sourceId}").removeClass('icon-tag').addClass('icon-ok').attr('title', 'Click to detach')
        else if $("#attach-icon-#{@sourceId}").hasClass 'icon-ok'
          $("#attach-icon-#{@sourceId}").removeClass('icon-ok').addClass('icon-tag').attr('title', 'Click to attach')
        bootbox.alert data
      error: (xhr, textStatus) ->
        bootbox.alert xhr.responseText
    false

  hiddenModalDiv: ->
    $("#modal-div").data 'modal', null