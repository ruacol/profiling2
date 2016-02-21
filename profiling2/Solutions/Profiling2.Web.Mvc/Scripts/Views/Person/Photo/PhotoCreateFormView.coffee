Profiling.Views.PhotoCreateFormView = Profiling.Views.BaseModalForm.extend

  id: "person-photo-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Person Photo</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    $("##{@options.modalId} #FileData").fileupload
      url: "#{Profiling.applicationUrl}Profiling/PersonPhotos/Upload"
      add: (e, data) =>
        $("##{@options.modalId} ##{@options.modalSaveButton}").addClass "disabled"
        data.submit()
        $("##{@options.modalId} #validation-errors").html "<div class='alert alert-info'><ul><li>Uploading...</li></ul></div>"
      done: (e, data) =>
        $("##{@options.modalId} ##{@options.modalSaveButton}").removeClass "disabled"
        $("##{@options.modalId} #Id").val data.result.Id
        $("##{@options.modalId} #validation-errors").empty()
        $("##{@options.modalId} #FileData").parent().after "<div>#{data.result.FileName}</div>"
      fail: (e, data) =>
        $("##{@options.modalId} ##{@options.modalSaveButton}").removeClass "disabled"
        $("##{@options.modalId} #validation-errors").html "<div class='alert alert-error'><ul><li>Upload failed.</li></ul></div>"