Profiling.Views.OperationAliasEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Operation Alias"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @