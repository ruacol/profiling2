Profiling.Views.PersonResponsibilityEditFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Person Responsibility"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    eventId = $("##{@options.modalId} #EventId").val()
    new Profiling.MultiSelect
      el: "##{@options.modalId} #ViolationIds"
      placeholder: 'Search by category name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Violations/#{eventId}"
