Profiling.Views.OperationAddFormView = Profiling.Views.BaseModalForm.extend

  id: "operation-add-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a>Add Operation</a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    new Profiling.BasicSelect
      el: "#OperationId"
      placeholder: "Search by operation name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Operations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Operations/Json"

    createOperationView = new Profiling.Views.OperationCreateFormView
      modalId: 'modal-operation-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Operations/CreateModal"
      modalSaveButton: 'modal-operation-create-button'
    $("#modal-operation-add #OperationId").parent().prev().prepend createOperationView.render().el