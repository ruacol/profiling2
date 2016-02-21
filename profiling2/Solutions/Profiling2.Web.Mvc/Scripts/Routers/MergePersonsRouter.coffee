Profiling.Routers.MergePersonsRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    leftView = new Profiling.Views.MergePersonSearchView
      col: 'left'
    $("div#left-column").html leftView.render().el
    
    rightView = new Profiling.Views.MergePersonSearchView
      col: 'right'
    $("div#right-column").html rightView.render().el

    $("#merge-button").click ->
      $.ajax
        url: "#{Profiling.applicationUrl}Profiling/Persons/Merge"
        type: "POST"
        data:
          ToKeepId: rightView.personId
          ToDeleteId: leftView.personId
        error: (xhr, textStatus) ->
          text = "<p>There were some errors with the request.</p><ul class='text-error'>"
          _($.parseJSON(xhr.responseText)).each (val, key) ->
            text += "<li>#{val}</li>"
          text += "</ul>"
          bootbox.alert text
        success: (data, textStatus, xhr) =>
          bootbox.alert "Persons successfully merged."
        complete: (xhr, textStatus) =>
