Profiling.Views.EventAddSourceIconFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add Event Source"></i>
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
      el: "#modal-event-source-add-#{@options.eventId} #SourceId"
      placeholder: "Search by source ID or file name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Sources/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Sources/Get"
