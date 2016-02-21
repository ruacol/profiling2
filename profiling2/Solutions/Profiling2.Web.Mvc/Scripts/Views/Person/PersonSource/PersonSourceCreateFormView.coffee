Profiling.Views.PersonSourceCreateFormView = Profiling.Views.BaseModalForm.extend

  id: "person-source-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Person Source</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    @setupSelect
      el: "#modal-person-source-add #SourceId"
      placeholder: "Search by source ID or file name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Sources/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Sources/Get"
