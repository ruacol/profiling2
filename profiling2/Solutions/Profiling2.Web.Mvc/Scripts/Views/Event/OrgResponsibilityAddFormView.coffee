Profiling.Views.OrgResponsibilityAddFormView = Profiling.Views.BaseModalForm.extend

  id: "org-responsibility-add-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Organization Responsibility</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    new Profiling.BasicSelect
      el: "#modal-org-responsibility-add #OrganizationId"
      placeholder: "Search by organization name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Get"
    new Profiling.BasicSelect
      el: "#modal-org-responsibility-add #UnitId"
      placeholder: "Search by unit name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"
