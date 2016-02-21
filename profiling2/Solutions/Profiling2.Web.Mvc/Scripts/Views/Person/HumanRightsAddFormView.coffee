Profiling.Views.HumanRightsAddFormView = Profiling.Views.BaseModalForm.extend

  id: "add-person-responsibility-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Human Rights</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    createEventView = new Profiling.Views.EventCreateFormIconView
      modalId: "modal-event-create"
      modalUrl: "#{Profiling.applicationUrl}Profiling/Events/Create"
      modalSaveButton: 'modal-event-create-button'
    $("#modal-person-responsibility-add #EventId").parent().prev().prepend createEventView.render().el

    suggestEventView = new Profiling.Views.EventSuggestFormView
      personId: @options.personId
      modalId: "modal-event-suggest"
      modalUrl: "#{Profiling.applicationUrl}Profiling/Events/SuggestModal"
    $("#modal-person-responsibility-add #EventId").parent().prev().prepend suggestEventView.render().el

    @setupSelect
      el: "#modal-person-responsibility-add #EventId"
      placeholder: "Search for event..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Events/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Get"

    $("#modal-person-responsibility-add #EventId").change (e) =>
      @changeCategoryGetUrl e

    @categoryMultiSelect = @reinitCategoryMultiSelect "#{Profiling.applicationUrl}Profiling/Events/Violations/0"
    $("#modal-person-responsibility-add #ViolationIds").prop "disabled", true

  reinitCategoryMultiSelect: (getUrl) ->
    if @categoryMultiSelect
        $("#modal-person-responsibility-add #ViolationIds").select2 "destroy"
    if getUrl
      @categoryMultiSelect = new Profiling.MultiSelect
        el: "#modal-person-responsibility-add #ViolationIds"
        placeholder: 'Search by category name...'
        nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
        getUrl: getUrl

  changeCategoryGetUrl: (e) ->
    eventId = $(e.target).val()
    if eventId
      getUrl = "#{Profiling.applicationUrl}Profiling/Events/Violations/#{eventId}"
      $.ajax
        url: getUrl
        dataType: 'json'
        async: false
        success: (data, textStatus, xhr) ->
          if data
            ids = _(data).map (el) -> el.id
            $("#modal-person-responsibility-add #ViolationIds").val ids.join()
        complete: =>
          @reinitCategoryMultiSelect getUrl
          $("#modal-person-responsibility-add #ViolationIds").prop "disabled", false