Profiling.Views.PersonEditFormView = Profiling.Views.BasePersonFormView.extend

  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>#{@options.title}</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  formSubmittedSuccessCallback: ->
    Backbone.history.fragment = null  # null fragment so we can reload the same URL
    Backbone.history.navigate "personal-information",
      trigger: true
    