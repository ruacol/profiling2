Profiling.Views.EventCreateFormIconView = Profiling.Views.BaseEventFormView.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add Event"></i>
    """

  events:
    "click i.icon-plus": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  formSubmittedSuccessCallback: (data) ->
    if data
      $("#EventId").select2 "val", data.Id
      $("#EventId").select2 "data",
        id: data.Id
        text: data.Name
    