Profiling.Views.ActiveScreeningAddFormView = Profiling.Views.BaseModalForm.extend

  id: "add-active-screening-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Active Screening</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    # We reload the page when the modal closes; jqueryui's datepicker uses
    # the input's id internally as it's selector which causes problems when we have multiple
    # #DateActivelyScreened elements created by modals.
    $("##{@options.modalId}").on "hidden", ->
      window.location.reload()
    $("##{@options.modalId} #DateActivelyScreened").datepicker
      format: "yyyy-mm-dd"
