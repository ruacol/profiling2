Profiling.Views.UnitSourceEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Unit Source"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @
