Profiling.Views.PersonResponsibilityAddIconFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add Person Responsibility"></i>
    """

  events:
    "click i.icon-plus": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @
    
  modalLoadedCallback: ->
    @setupPersonSelect "#modal-person-responsibility-add-#{@options.eventId} #PersonId"

    new Profiling.MultiSelect
      el: "#modal-person-responsibility-add-#{@options.eventId} #ViolationIds"
      placeholder: 'Search by category name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Violations/#{@options.eventId}"
