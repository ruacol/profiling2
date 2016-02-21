Profiling.Views.FunctionEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Function"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  displayModalForm: ->
    if $("#{@options.careerDivId} #RoleId").val()
      Profiling.Views.BaseModalForm.prototype.displayModalForm.call @, arguments

  formSubmittedSuccessCallback: ->
    $("#{@options.careerDivId} #RoleId").select2 "val", $("#{@options.careerDivId} #RoleId").select2("val")
