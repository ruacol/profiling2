Profiling.Views.EditProposedPersonView = Profiling.Views.BaseModalForm.extend

  templates:
    button: _.template """
      <a class="btn btn-mini" title="Edit proposed person">
        <i class="icon-pencil"></i>
      </a>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.css 'margin-right', '4px'
    @$el.html @templates.button
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  formSubmittedSuccessCallback: ->
    @options.attachedPersonsView.trigger "attached-persons:redraw"