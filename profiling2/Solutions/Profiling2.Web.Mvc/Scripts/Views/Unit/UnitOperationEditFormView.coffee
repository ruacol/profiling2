Profiling.Views.UnitOperationEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Unit Operation"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    $("##{@options.modalId}").on "hidden", ->
      window.location.reload()

    if @options.operationId
      editOperationFormView = new Profiling.Views.OperationEditFormView
        modalId: "modal-operation-edit-#{@options.operationId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Operations/EditModal/#{@options.operationId}"
        modalSaveButton: "modal-operation-edit-button-#{@options.operationId}"
      $("##{@options.modalId} .operation-label").prepend editOperationFormView.render().el
