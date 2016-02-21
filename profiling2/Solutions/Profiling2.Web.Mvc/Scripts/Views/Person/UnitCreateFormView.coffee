Profiling.Views.UnitCreateFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add Unit"></i>
    """

  events:
    "click i.icon-plus": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    @setupSelect
      el: "#modal-unit-create #OrganizationId"
      placeholder: "Search by organization ID or name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Get"

  formSubmittedSuccessCallback: (data) ->
    if data
      $("#UnitId").select2 "val", data.Id
      $("#UnitId").select2 "data",
        id: data.Id
        text: data.Name