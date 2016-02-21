Profiling.Views.EventEditFormView = Profiling.Views.BaseEventFormView.extend

  id: "event-edit-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Edit Event</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

