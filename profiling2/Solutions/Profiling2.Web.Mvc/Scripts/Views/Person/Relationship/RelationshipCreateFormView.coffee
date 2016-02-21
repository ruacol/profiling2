Profiling.Views.RelationshipCreateFormView = Profiling.Views.BasePersonRelationshipFormView.extend

  id: "person-relationship-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Person Relationship</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @