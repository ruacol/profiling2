Profiling.Views.PublicSummaryEditFormIconView = Profiling.Views.BasePersonFormView.extend

  templates:
    icon: "<i class='accordion-toggle icon-pencil' style='margin-right: 5px;' title='Edit Public Summary'></i>"

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  formSubmittedSuccessCallback: ->
    Backbone.history.fragment = null  # null fragment so we can reload the same URL
    Backbone.history.navigate "personal-information",
      trigger: true
    
    @options.publicSummaryView.hideModal()