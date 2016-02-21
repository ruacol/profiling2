Profiling.Views.UnitEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Unit"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @
  
  modalLoadedCallback: ->
    @setupSelect
      el: "#modal-unit-edit-#{@options.unitId} #OrganizationId"
      placeholder: "Search by organization ID or name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Get"

  displayModalForm: ->
    if $("#{@options.careerDivId} #UnitId").val()
      Profiling.Views.BaseModalForm.prototype.displayModalForm.call @, arguments

  formSubmittedSuccessCallback: ->
    $("#{@options.careerDivId} #UnitId").select2 "val", $("#{@options.careerDivId} #UnitId").select2("val")
