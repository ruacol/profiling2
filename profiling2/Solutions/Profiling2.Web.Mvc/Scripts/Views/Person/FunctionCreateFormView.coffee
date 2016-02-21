Profiling.Views.FunctionCreateFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add Function"></i>
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
      $("#RoleId").select2 "val", data.Id
      $("#RoleId").select2 "data",
        id: data.Id
        text: data.Name