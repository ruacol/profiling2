Profiling.Views.OrgResponsibilityEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Organization Responsibility"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    new Profiling.BasicSelect
      el: "##{@options.modalId} #UnitId"
      placeholder: "Search by unit name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"