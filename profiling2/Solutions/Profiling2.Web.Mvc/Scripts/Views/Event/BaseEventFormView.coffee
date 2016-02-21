Profiling.Views.BaseEventFormView = Profiling.Views.BaseModalForm.extend

  parentDivId: ->
    id = if @options.eventId
      "#modal-event-edit-#{@options.eventId}"
    else
      "#modal-event-create"

  instantiateEditLocationView: ->
    locationId = $("#{@parentDivId()} #LocationId").val()
    if locationId
      editLocationView = new Profiling.Views.LocationEditFormView
        parentDivId: @parentDivId()
        modalId: "modal-location-edit-#{locationId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Locations/EditModal/#{locationId}"
        modalSaveButton: "modal-location-edit-button-#{locationId}"
      $("#{@parentDivId()} #edit-location-span").html editLocationView.render().el

  modalLoadedCallback: ->
    new Profiling.MultiSelect
      el: "#{@parentDivId()} #ViolationIds"
      placeholder: 'Search by category name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Violations/0"

    new Profiling.MultiSelect
      el: "#{@parentDivId()} #TagIds"
      placeholder: 'Search by tag name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Tags/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Tags/Get/"

    @setupSelect
      el: "#{@parentDivId()} #LocationId"
      placeholder: "Search by location name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Locations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Locations/Get"

    @instantiateEditLocationView()

    createLocationView = new Profiling.Views.LocationCreateFormView
      modalId: 'modal-location-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Locations/CreateModal"
      modalSaveButton: 'modal-location-create-button'
    $("#{@parentDivId()} #LocationId").parent().prev().prepend createLocationView.render().el
    
    $("#{@parentDivId()} #LocationId").change =>
      @instantiateEditLocationView()

    new Profiling.MultiSelect
      el: "#{@parentDivId()} #JhroCaseIds"
      placeholder: 'Search by case code...'
      nameUrl: "#{Profiling.applicationUrl}Hrdb/Cases/Name/"
      getUrl: "#{Profiling.applicationUrl}Hrdb/Cases/Get/"

    createJhroCaseView = new Profiling.Views.JhroCaseCreateFormView
      parentDivId: @parentDivId()
      modalId: 'modal-jhro-case-create'
      modalUrl: "#{Profiling.applicationUrl}Hrdb/Cases/CreateModal"
      modalSaveButton: 'modal-jhro-case-create-button'
    $("#{@parentDivId()} #JhroCaseIds").parent().prev().prepend createJhroCaseView.render().el

