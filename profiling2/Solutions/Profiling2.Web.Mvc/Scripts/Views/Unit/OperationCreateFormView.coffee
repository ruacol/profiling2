Profiling.Views.OperationCreateFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Create new operation"></i>
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
      $("#OperationId").select2 "val", data.Id
      $("#OperationId").select2 "data",
        id: data.Id
        text: data.Name