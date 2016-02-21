Profiling.Views.ActionTakenAddFormView = Profiling.Views.BaseModalForm.extend

  id: "add-action-taken-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Action Taken</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    @setupPersonSelect "#SubjectPersonId,#ObjectPersonId"
