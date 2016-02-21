Profiling.Views.PersonSearchView = Profiling.Views.BaseModalForm.extend

  modalWidth: '85%'

  templates:
    personSearch: _.template """
      <p>
        Add new person: 
        <input type="text" id="person-search" class="input-xlarge" placeholder="Search by name or ID number..." />
        <button class="btn" type="button" id="btn-person-search">Search</button>
      </p>
    """

  events:
    "click #btn-person-search": "displayModalForm"
    "keydown #person-search": "openModalOnEnter"

  render: ->
    @$el.append @templates.personSearch
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments

    @attachedPersonsView = new Profiling.Views.AttachedPersonsView

    # TODO if the User has the ProfilingLimitedEdit role, they can create users straight away via /Profiling2/Profiling/Persons/QuickCreate...
    @proposePersonView = new Profiling.Views.ProposePersonView
      modalId: "modal-propose-person"
      modalUrl: "#{Profiling.applicationUrl}Screening/Initiate/ProposePersonModal/#{$("#Id").val()}"
      modalSaveButton: "modal-propose-person-button"
      attachedPersonsView: @attachedPersonsView
    @$el.append @proposePersonView.render().el
    @$el.append @attachedPersonsView.render().el

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

      dataTable = new Profiling.DataTable 'person-search-table',
        sAjaxSource: "#{Profiling.applicationUrl}Screening/Initiate/PersonDataTables"
        sDom: 'T<"clear">lftipr'
        bFilter: false
        bStateSave: false
        aoColumns: [ { mDataProp: 'Id', bSortable: false }, 
          { mDataProp: 'Name', bSortable: false }, 
          { mDataProp: 'MilitaryIDNumber', bSortable: false }, 
          { mDataProp: 'Rank', bSortable: false },
          { mDataProp: 'Function', bSortable: false } ]
        fnServerParams: (aoData) =>
          aoData.push
            name: "sSearch"
            value: $("#person-search").val()
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).html """
            <button id="attach-#{aData['Id']}" class="btn btn-mini" title="Attach"><i class="icon-ok"></i></button>
          """

          for field, i in [ 'Name', 'MilitaryIDNumber' ]
            list = aData[field]
            if _(list).size() is 1
              $("td:eq(#{i+1})", nRow).html "#{_(list).first()}"
            else if _(list).size() > 1
              prefix = if field is 'Name' then '<br />(a.k.a.) ' else '<br />'
              $("td:eq(#{i+1})", nRow).html "#{_(list).first()} #{_(list).tail().map((x) -> prefix + x).join('')}"

          $("td:eq(1)", nRow).html """
            <a href="#{Profiling.applicationUrl}Profiling/Persons/Details/#{aData['Id']}" target="_blank">#{$("td:eq(1)", nRow).html()}</a>
          """

          # Handle click of tick box
          _.defer () =>
            $("#attach-#{aData['Id']}").click (e) =>
              $.ajax
                url: "#{Profiling.applicationUrl}Screening/Initiate/#{$("#Id").val()}/AttachPerson/#{aData['Id']}" 
                success: (data, textStatus, xhr) =>
                  $("##{@options.modalId}").modal 'hide'
                  # update page display with added person
                  @attachedPersonsView.trigger "attached-persons:redraw"
                error: (xhr, textStatus) ->
                  bootbox.alert xhr.responseText