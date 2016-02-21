Profiling.Views.OperationSourceCreateFormView = Profiling.Views.BaseModalForm.extend

  id: "operation-source-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a>Add Operation Source</a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    @setupSelect
      el: "#modal-operation-source-add #SourceId"
      placeholder: "Search by source ID or file name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Sources/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Sources/Get"
