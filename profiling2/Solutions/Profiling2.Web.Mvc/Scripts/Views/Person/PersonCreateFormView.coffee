Profiling.Views.PersonCreateFormView = Profiling.Views.BasePersonFormView.extend

  id: "person-create-button"
  className: "btn btn-mini"
  tagName: "button"

  events:
    "click": "displayModalForm"

  render: ->
    $(@el).text "Create Person"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  formSubmittedSuccessCallback: (data) ->
    window.location.href = "#{Profiling.applicationUrl}Profiling/Persons/Details/#{data.Id}"
