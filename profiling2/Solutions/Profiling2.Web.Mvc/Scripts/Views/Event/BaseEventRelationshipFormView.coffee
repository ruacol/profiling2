Profiling.Views.BaseEventRelationshipFormView = Profiling.Views.BaseModalForm.extend

  parentDivId: ->
    id = if @options.eventRelationshipId
      "#modal-event-relationship-edit-#{@options.eventRelationshipId}"
    else
      "#modal-event-relationship-add"

  modalLoadedCallback: ->
    @setupSelect
      el: "#{@parentDivId()} #SubjectEventId"
      placeholder: "Search for subject event..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Events/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Get"
      
    @setupSelect
      el: "#{@parentDivId()} #ObjectEventId"
      placeholder: "Search for object event..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Events/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Get"

    $("#{@parentDivId()} #icon-set-as-subject").click () =>
      $("#{@parentDivId()} #SubjectEventId").select2 "data",
        id: @options.eventId
        text: @options.eventHeadline

    $("#{@parentDivId()} #icon-set-as-object").click () =>
      $("#{@parentDivId()} #ObjectEventId").select2 "data",
        id: @options.eventId
        text: @options.eventHeadline

    Profiling.setupUITooltips "i.icon-arrow-left"