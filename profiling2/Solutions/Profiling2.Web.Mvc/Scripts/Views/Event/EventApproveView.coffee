Profiling.Views.EventApproveView = Backbone.View.extend

  id: "event-approve-button"
  className: "btn btn-mini"
  tagName: "button"

  events:
    "click": "approveEvent"

  render: ->
    @$el.text "Approve Event"
    @

  approveEvent: ->
    bootbox.confirm "Are you sure you want to approve this event?", (response) =>
      if response is true
        $.ajax
          url: @options.url
          success: (data, textStatus, xhr) =>
            bootbox.alert data, =>
              window.location.href = "#{Profiling.applicationUrl}Profiling/Events/Details/#{@options.eventId}"
          error: (xhr, textStatus) ->
            bootbox.alert xhr.responseText
