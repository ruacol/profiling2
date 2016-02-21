Profiling.Views.UnitAliasCreateFormView = Profiling.Views.BaseModalForm.extend

  id: "unit-alias-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a>Add Unit Alias</a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @