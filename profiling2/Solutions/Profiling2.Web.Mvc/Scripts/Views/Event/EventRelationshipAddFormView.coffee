Profiling.Views.EventRelationshipAddFormView = Profiling.Views.BaseEventRelationshipFormView.extend

  id: "add-event-relationship-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Event Relationship</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @
