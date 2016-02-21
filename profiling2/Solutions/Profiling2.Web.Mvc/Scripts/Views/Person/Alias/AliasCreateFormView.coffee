Profiling.Views.AliasCreateFormView = Profiling.Views.BaseModalForm.extend

  id: "person-alias-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Person Alias</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @