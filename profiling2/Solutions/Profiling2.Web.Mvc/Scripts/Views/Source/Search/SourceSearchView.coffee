Profiling.Views.SourceSearchView = Backbone.View.extend

  templates:
    bootstrapAccordion: _.template """
      <div class="accordion" id="accordion">
        <div class="accordion-group">
          <div class="accordion-heading">
            <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#accordion-search">
              Source Search
            </a>
          </div>
          <div class="accordion-body collapse in" id="accordion-search">
            <div class="accordion-inner">
              <%= searchForm %>
            </div>
          </div>
        </div>

        <div class="accordion-group">
          <div class="accordion-heading">
            <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#accordion-search-results">
              Search Results
            </a>
          </div>
          <div class="accordion-body collapse out" id="accordion-search-results">
            <div class="accordion-inner">
              <%= resultsTable %>
            </div>
          </div>
        </div>

        <div class="accordion-group">
          <div class="accordion-heading">
            <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#accordion-preview">
              Preview
            </a>
          </div>
          <div class="accordion-body collapse out" id="accordion-preview">
            <div class="accordion-inner" id="accordion-preview-body">
              
            </div>
          </div>
        </div>
      </div>
    """
    accordion: _.template """
      <div id="accordion">
        <h4>Source Search</h4>
        <div><%= searchForm %></div>
        <h4>
          Search Results
          <button type="button" class="btn btn-mini" data-toggle="collapse" data-target="#results-table-help" title="Click to toggle help">Help</button>  
        </h4>
        <div><%= resultsTable %></div>
        <h4>Preview</h4>
        <div id="accordion-preview-body"></div>
      </div>
    """
    searchForm: _.template """
      <form>
        <input type="hidden" id="search-adminSourceSearchId">
        <table class="table table-bordered" style="width: auto;" id="source-search-table">
          <tr>
            <th>Source ID</th>
            <td><input type="text" id="search-id" placeholder="Source ID" title="Search by source ID number." class="input-small"></td>
          </tr>
          <tr>
            <th>File name</th>
            <td><input type="text" id="search-name" placeholder="File name" title="Search by file name or file path, e.g. '\\\\kindata01\\jmac'" class="input-xxlarge"></td>
          </tr>
          <tr>
            <th>File extension</th>
            <td><input type="text" id="search-extension" placeholder="File extension" maxlength="4" title="Search by file extension, e.g. 'doc', 'jpg', 'pdf'." class="input-small"></td>
          </tr>
          <tr>
            <th class="span3">Search text inside files...</th>
            <td class="span9">
              <input type="text" 
                id="search-text" 
                placeholder="Search text inside files..." 
                class="input-xxlarge"
                title="Search text surrounded by quotations, e.g. 'This is my phrase'. Exclude terms by prefixing with a minus sign, e.g. '-excluded'. Add a trailing wildcard star (*) for flexible term searching, e.g. 'tree*' will match 'tree', 'trees', and 'treehouse'."
              >
            </td>
          </tr>
          <tr>
            <th>File modified date</th>
            <td>
              <div class="input-daterange" 
                data-provide="datepicker" 
                data-date-format="yyyy/mm/dd" 
                data-date-autoclose="true" 
                data-date-clear-btn="true"
                title="Filter results based on the last modified date (if available) of each file.  Files with no dates are not returned.">
                <input type="text" class="input-small" name="start-date" id="start-date" placeholder="Start date..." />
                <span class="add-on" style="margin-bottom: 10px;">to</span>
                <input type="text" class="input-small" name="end-date" id="end-date" placeholder="End date..." />
              </div>
            </td>
          </tr>
          <tr>
            <th class="span3">Authors</th>
            <td class="span9">
              <input type="text" 
                id="author-search-text" 
                placeholder="Authors" 
                class="input-xxlarge"
                title="Filter results based on the author name/s attached to the source."
              >
            </td>
          </tr>
          <tr>
            <td colspan="2" style="text-align: right;"><button class="btn" type="button" id="btn-search-text">Search</button></td>
          </tr>
        </table>
      </form>
    """
    modal: _.template """
      <div id="modal-div" class="modal hide fade"></div>
    """
    resultsTable: _.template """
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
      <div id="tooltip-readonly" style="display: none;">
        <p>This is a read only source.</p>
        <p>Read only sources should not be downloaded, shared, or copied.</p>
      </div>
    """
    resultsTableHelp: _.template """
      <div id="results-table-help" class="collapse out">
        <p>On this screen you may rate, attach/detach, preview, download, and archive/unarchive sources.</p>
        <p>Rating sources is important for building our knowledge base.  When a source is previewed or downloaded, the user should specify whether 
          this document is relevant (marked with <i class="icon-star"></i>) or not (marked with <i class="icon-remove"></i>) to the current search context.
          Please note that the current search context can be independent of the current profile that is opened.</p>
        <p>Sources may be attached to the current profile or event by clicking the attach icon (marked with <i class="icon-tag"></i>).  Conversely a source's
          attachment may be removed by clicking the attached icon (marked with <i class="icon-ok"></i>).</p>
        <p>Sources may be previewed by clicking the preview icon (marked with <i class="icon-search"></i> or <i class="icon-check"></i> if previously previewed).  Other information and actions may be accessed by clicking
          the source's name.</p>
        <p>The search results table column headings may be clicked to toggle sorting of that column. Shift-clicking multiple columns will sort results by those columns in that order.</p>
      </div>
    """

  events:
    "click #btn-search-text": "submitSearch"
    "keyup #search-id,#search-name,#search-extension,#search-text,#author-search-text": "keyupSubmitSearch"
    "change #search-text": "changeSearchText"
    "click .icon-star": "clickIconStar"
    "click .icon-remove": "clickIconRemove"
    "click .icon-question-sign": "clickIconQuestionSign"
    "click .icon-search,.icon-check": "clickIconSearch"
    "click .icon-tag,.icon-ok": "clickIconTag"
    "hidden .modal": "hiddenModalDiv"
    "click .info-link": "activateAccordionPreview"
    "activateAccordionPreview": "activateAccordionPreview"

  render: ->
    $(@el).css 'height', $(window).height() - 40

    $(@el).append @templates.accordion
      searchForm: "#{@templates.searchForm()}"
      resultsTable: "#{@templates.resultsTableHelp()} #{@templates.resultsTable()}"

    _.defer =>
      # add 'selected person' dropdown to search form table
      @selectAttachTargetView = new Profiling.Views.SourceMostRecentAttachTargetView()
      $("#source-search-table").before @selectAttachTargetView.render().el
      
      # set tooltip help
      _.delay ->
        $("#source-search-table input,select").uitooltip
          show: false
          hide: false
          position:
            my: "left+15 center"
            at: "right center"
        , 1000

      $("#accordion").accordion
        heightStyle: 'fill'
        activate: (event, ui) =>
          if @dataTable and @currentRow
            @dataTable.oScroller().fnScrollToRow @currentRow
          
    $(@el).append @templates.modal
    @
   
  keyupSubmitSearch: (e) ->
    document.title = "Source Search: #{$("#search-text").val()}"
    @submitSearch() if e.keyCode == 13

  submitSearch: ->
    @activateAccordionResults()
    if @dataTable
      @dataTable.fnDraw()
    else
      @initDataTable()

  activateAccordionResults: =>
    $("#accordion").accordion 'option', 'active', 1

  activateAccordionPreview: ->
    $("#accordion").accordion 'option', 'active', 2

  changeSearchText: ->
    $("#search-adminSourceSearchId").val("")

  clickIconStar: (e) ->
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

  clickIconRemove: (e) ->
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

  clickIconQuestionSign: (e) ->
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

  clickIconSearch: (e) ->
    sourceId = $.trim $(e.target).text()
    $(e.target).removeClass "icon-search"
    $(e.target).addClass "icon-check"
    $(e.target).parent().next().next().next().text moment().format(Profiling.DATE_FORMAT)
    @activateAccordionPreview()
    window.location.hash = "preview/#{sourceId}/#{$("#search-adminSourceSearchId").val()}"

  clickIconTag: (e) ->
    @sourceId = $.trim $(e.target).text()
    targetId = @selectAttachTargetView.getTargetId()
    @url = if @selectAttachTargetView.getTargetType() is 'Person'
      "#{Profiling.applicationUrl}Profiling/Sources/#{@sourceId}/Attach/Person/#{targetId}"
    else if @selectAttachTargetView.getTargetType() is 'Event'
      "#{Profiling.applicationUrl}Profiling/Sources/#{@sourceId}/Attach/Event/#{targetId}"
    if @url and targetId
      $("#modal-div").load @url, '', =>
        $("#modal-div").modal
          keyboard: true
        $("#modal-button").click () =>
          @submitModalForm()
    else
      bootbox.alert "No person or event has been selected."

  initDataTable: ->
    @dataTable = new Profiling.DataTable 'sources-table',
      bStateSave: false
      sScrollY: '400px'
      sAjaxSource: "#{Profiling.applicationUrl}Profiling/Sources/DataTables"
      sDom: 'T<"clear">tirS'
      bFilter: false
      aaSorting: [ [ 1, 'desc' ], [ 5, 'desc' ] ]
      aoColumns: [ { mDataProp: 'Rank' }, { mDataProp: 'IsRelevant' }, { mDataProp: 'IsAttached', bSortable: (if @selectAttachTargetView.getTargetId() then true else false) }, { mDataProp: 'Id' }, { mDataProp: 'SourceName' }, { mDataProp: 'FileDateTimeStamp' }, { mDataProp: 'IsRestricted' }, { mDataProp: 'ReviewedDateTime' }]
      fnRowCallback: (nRow, aData, iDisplayIndex) =>
        $("td:eq(1)", nRow).each (i, el) ->
          iconClass = switch $(el).text()
            when '2' then 'icon-star'
            when '1' then 'icon-question-sign'
            when '0' then 'icon-remove'
          $(el).html "<i class='accordion-toggle #{iconClass}'><span class='invisible'>#{aData['Id']}</span></i>" if iconClass
          $(el).css("text-align", "center")
        $("td:eq(2)", nRow).each (i, el) =>
          if @selectAttachTargetView.getTargetId() and $(el).text()
            if $(el).text() > 0
              $(el).html "<i class='accordion-toggle icon-ok' title='Click to detach' id='attach-icon-#{aData['Id']}'><span class='invisible'>#{aData['Id']}</span></i>"
            else
              $(el).html "<i class='accordion-toggle icon-tag' title='Click to attach' id='attach-icon-#{aData['Id']}'><span class='invisible'>#{aData['Id']}</span></i>"
            $(el).css("text-align", "center")
        sourceNameRow = """
          <i class='accordion-toggle icon-#{if aData['ReviewedDateTime'] then 'check' else 'search'}' title='Preview'>
            <span class='invisible'>#{aData['Id']}</span>
          </i>&nbsp;
          <a href='#info/#{aData['Id']}/#{$("#search-adminSourceSearchId").val()}' title='View information for this source' class='info-link'>
            #{aData['SourceName']}
          </a>
        """
        if aData['IsReadOnly'] is true
          sourceNameRow += "&nbsp;<i id='icon-lock-#{aData['Id']}' class='icon-lock' data-selector='#tooltip-readonly'></i>"
          _.defer ->
            Profiling.setupUITooltips "#icon-lock-#{aData['Id']}"
        $("td:eq(4)", nRow).html sourceNameRow
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
          $(el).html moment($(el).text()).format(Profiling.DATE_FORMAT) if $(el).text()
        $("td:eq(6)", nRow).each (i, el) ->
          $(el).html "<i class='icon-exclamation-sign' title='Restricted'></i>" if $(el).text() is 'true'
          $(el).html "" if $(el).text() is 'false'
          $(el).css("text-align", "center")
        $("td:eq(7)", nRow).each (i, el) ->
          $(el).html moment($(el).text()).format(Profiling.DATE_FORMAT) if $(el).text()
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
        sourceAttachTargetType = @selectAttachTargetView.getTargetType()
        if sourceAttachTargetType
          aoData.push
            name: if sourceAttachTargetType is 'Person' then 'personId' else if sourceAttachTargetType is 'Event' then 'eventId' else ''
            value: @selectAttachTargetView.getTargetId()
        $.getJSON sSource, aoData, (json) ->
          $("#search-adminSourceSearchId").val(json.adminSourceSearchId)
          fnCallback json

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
