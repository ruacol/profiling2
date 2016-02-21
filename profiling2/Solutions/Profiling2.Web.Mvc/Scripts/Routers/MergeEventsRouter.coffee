Profiling.Routers.MergeEventsRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    leftView = new Profiling.Views.MergeEventSearchView
      col: 'left'
    $("div#left-column").html leftView.render().el
    
    rightView = new Profiling.Views.MergeEventSearchView
      col: 'right'
    $("div#right-column").html rightView.render().el

    $("#merge-button").click ->
      $.ajax
        url: "#{Profiling.applicationUrl}Profiling/Events/Merge"
        type: "POST"
        data:
          ToKeepId: rightView.eventId
          ToDeleteId: leftView.eventId
        error: (xhr, textStatus) ->
          text = "<p>There were some errors with the request.</p><ul class='text-error'>"
          _($.parseJSON(xhr.responseText)).each (val, key) ->
            text += "<li>#{val}</li>"
          text += "</ul>"
          bootbox.alert text
        success: (data, textStatus, xhr) =>
          bootbox.alert "Events successfully merged."
        complete: (xhr, textStatus) =>
