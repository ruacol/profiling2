Profiling.Views.DeleteRequestButtonView = Backbone.View.extend
  
  className: "btn btn-danger"
  tagName: "a"

  events:
    "click": "deleteRequest"

  render: ->
    @$el.attr "title", "You may still delete this request if it has not yet been submitted."
    @$el.text "Delete"
    @

  deleteRequest: ->
    bootbox.confirm "Are you sure you want to delete this request?", (response) =>
      if response is true
        $.ajax
          url: "#{Profiling.applicationUrl}Screening/Initiate/Delete/#{@options.requestId}"
          success: (data, textStatus, xhr) =>
            bootbox.alert data, =>
              window.location.href = "#{Profiling.applicationUrl}Screening/Initiate"
          error: (xhr, textStatus) ->
            bootbox.alert xhr.responseText
