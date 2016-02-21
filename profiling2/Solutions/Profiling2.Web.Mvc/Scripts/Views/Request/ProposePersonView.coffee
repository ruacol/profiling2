Profiling.Views.ProposePersonView = Profiling.Views.BaseModalForm.extend

  templates:
    proposePerson: _.template """
      <p>
        <a id="person-propose-link">Can't find a person?</a>
      </p>
    """

  events:
    "click #person-propose-link": "displayModalForm"

  render: ->
    @$el.append @templates.proposePerson
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments

    @

  formSubmittedSuccessCallback: (data) ->
    if data and data.WasSuccessful # and @options.attachedPersonsView
      # @options.attachedPersonsView.trigger "attached-persons:redraw"
      window.location.reload()