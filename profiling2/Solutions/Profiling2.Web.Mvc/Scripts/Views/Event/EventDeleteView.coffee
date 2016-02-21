Profiling.Views.EventDeleteView = Backbone.View.extend

  id: "event-delete-button"
  tagName: "li"

  events:
    "click": "deleteEvent"

  render: ->
    @$el.append "<a><small>Delete Event</small></a>"
    @

  deleteEvent: ->
    bootbox.confirm "Are you sure you want to delete this event?", (response) =>
      if response is true
        $.ajax
          url: @options.url
          success: (data, textStatus, xhr) =>
            bootbox.alert data, =>
              window.location.href = "#{Profiling.applicationUrl}Profiling/Events"
          error: (xhr, textStatus) ->
            bootbox.alert xhr.responseText
