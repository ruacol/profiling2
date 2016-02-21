Profiling.Views.EventCreateFormView = Profiling.Views.BaseEventFormView.extend

  id: "event-create-button"
  className: "btn btn-mini"
  tagName: "button"

  events:
    "click": "displayModalForm"

  render: ->
    $(@el).text "Create Event"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  formSubmittedSuccessCallback: (data) ->
    window.location.href = "#{Profiling.applicationUrl}Profiling/Events/Details/#{data.Id}"
