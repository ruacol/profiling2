Profiling.Views.SourceAddFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add new source"></i>
    """

  events:
    "click i.icon-plus": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    $("##{@options.modalId} #FileData").fileupload
      url: "#{Profiling.applicationUrl}Profiling/Sources/Upload"
      add: (e, data) =>
        # TODO this doesn't actually disable the button
        $("##{@options.modalId} ##{@options.modalSaveButton}").addClass "disabled"
        data.submit()
        $("##{@options.modalId} #validation-errors").html "<div class='alert alert-info'><ul><li>Uploading...</li></ul></div>"
      done: (e, data) =>
        $("##{@options.modalId} ##{@options.modalSaveButton}").removeClass "disabled"
        if data and data.result and data.result.Id
          $("##{@options.modalId} #Id").val data.result.Id
          $("##{@options.modalId} #validation-errors").empty()
          $("##{@options.modalId} #FileData").parent().after "<div id='source-name'>#{data.result.SourceName}</div>"
        else
          $("##{@options.modalId} #validation-errors").html "<div class='alert alert-error'><ul><li>Upload succeeded, but failed to receive source Id number.</li></ul></div>"
      fail: (e, data) =>
        $("##{@options.modalId} ##{@options.modalSaveButton}").removeClass "disabled"
        $("##{@options.modalId} #validation-errors").html "<div class='alert alert-error'><ul><li>Upload failed.</li></ul></div>"

  formSubmittedSuccessCallback: ->
    $("#SourceId").select2 "data",
      id: $("##{@options.modalId} #Id").val()
      text: $("##{@options.modalId} #source-name").text()
