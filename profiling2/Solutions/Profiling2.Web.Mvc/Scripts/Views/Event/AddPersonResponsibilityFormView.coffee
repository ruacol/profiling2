Profiling.Views.AddPersonResponsibilityFormView = Profiling.Views.BaseModalForm.extend

  id: "add-person-responsibility-button"
  tagName: "li"

  initialize: (opts) ->
    @eventId = opts.eventId

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Person Responsibility</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    @setupPersonSelect "#modal-person-responsibility-add #PersonId"

    new Profiling.MultiSelect
      el: "#modal-person-responsibility-add #ViolationIds"
      placeholder: 'Search by category name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Violations/#{@eventId}"
