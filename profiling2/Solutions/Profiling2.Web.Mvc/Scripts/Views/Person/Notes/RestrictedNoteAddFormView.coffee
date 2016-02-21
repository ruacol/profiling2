Profiling.Views.RestrictedNoteAddFormView = Profiling.Views.BaseModalForm.extend

  id: "add--restricted-note-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Restricted Note</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @
