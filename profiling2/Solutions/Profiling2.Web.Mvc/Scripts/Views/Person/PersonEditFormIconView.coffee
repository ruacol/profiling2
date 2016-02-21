Profiling.Views.PersonEditFormIconView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="<%= title %>"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
      title: @options.title
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @