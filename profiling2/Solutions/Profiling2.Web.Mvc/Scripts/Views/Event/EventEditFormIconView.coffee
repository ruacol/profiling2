Profiling.Views.EventEditFormIconView = Profiling.Views.BaseEventFormView.extend

  templates:
    icon: _.template """
      <i class="icon-pencil" style="margin-right: 5px;" title="Edit Event"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @
