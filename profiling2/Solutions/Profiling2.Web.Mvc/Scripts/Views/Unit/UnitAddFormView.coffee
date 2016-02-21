Profiling.Views.UnitAddFormView = Profiling.Views.BaseModalForm.extend

  id: "unit-add-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a>Add Unit</a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    new Profiling.BasicSelect
      el: "#UnitId"
      placeholder: "Search by unit name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"
