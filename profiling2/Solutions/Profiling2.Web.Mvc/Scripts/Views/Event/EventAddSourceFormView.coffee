Profiling.Views.EventAddSourceFormView = Profiling.Views.BaseModalForm.extend

  id: "event-add-source-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Event Source</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  modalLoadedCallback: ->
    @setupSourceSelect()

  setupSourceSelect: ->
    $("#modal-event-source-add #SourceId").val ''
    $("#modal-event-source-add #SourceId").select2
      placeholder: "Search by Source ID or file name..."
      allowClear: true
      ajax:
        url: "#{Profiling.applicationUrl}Profiling/Sources/Get"
        dataType: 'json'
        quietMillis: 1000
        data: (term, page) ->
          term: term
        results: (data, page) ->
          results: data