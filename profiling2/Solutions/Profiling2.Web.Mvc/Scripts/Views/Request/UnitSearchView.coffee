Profiling.Views.UnitSearchView = Profiling.Views.BaseModalForm.extend

  modalWidth: '85%'

  templates:
    unitSearch: _.template """
      <p>
        Add new unit: 
        <input type="text" id="unit-search" class="input-xlarge" placeholder="Search by name or ID number..." />
        <button class="btn" type="button" id="btn-unit-search">Search</button>
      </p>
    """

  events:
    "click #btn-unit-search": "displayModalForm"
    "keydown #unit-search": "openModalOnEnter"

  render: ->
    @$el.append @templates.unitSearch
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments

    @attachedUnitsView = new Profiling.Views.AttachedUnitsView
    @$el.append @attachedUnitsView.render().el

    @

  openModalOnEnter: (e) ->
    if e.keyCode == 13
      @displayModalForm()
      false

  displayModalForm: ->
    $("##{@options.modalId}").load @options.modalUrl, '', =>
      $("##{@options.modalId}").modal
        keyboard: true
        width: @modalWidth

      dataTable = new Profiling.DataTable 'unit-search-table',
        sAjaxSource: "#{Profiling.applicationUrl}Screening/Initiate/UnitDataTables"
        sDom: 'T<"clear">lftipr'
        bFilter: false
        bStateSave: false
        aoColumns: [ { mDataProp: 'Id', bSortable: false }, 
          { mDataProp: 'Name', bSortable: false }, 
          { mDataProp: 'BackgroundInformation', bSortable: false }, 
          { mDataProp: 'Organization', bSortable: false } ]
        fnServerParams: (aoData) =>
          aoData.push
            name: "sSearch"
            value: $("#unit-search").val()
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).html """
            <button id="attach-unit-#{aData['Id']}" class="btn btn-mini" title="Attach"><i class="icon-ok"></i></button>
          """
          $("td:eq(1)", nRow).html """
            <a href="#{Profiling.applicationUrl}Profiling/Units/Details/#{aData['Id']}" target="_blank">#{aData['Name']}</a>
          """
          $("td:eq(1)", nRow).css "white-space", "nowrap"

          # Handle click of tick box
          _.defer () =>
            $("#attach-unit-#{aData['Id']}").click (e) =>
              $.ajax
                url: "#{Profiling.applicationUrl}Screening/Initiate/#{$("#Id").val()}/AttachUnit/#{aData['Id']}" 
                success: (data, textStatus, xhr) =>
                  $("##{@options.modalId}").modal 'hide'
                  # update page display with added person
                  @attachedUnitsView.trigger "attached-units:redraw"
                error: (xhr, textStatus) ->
                  bootbox.alert xhr.responseText