Profiling.Views.DeleteProfileButtonView = Backbone.View.extend
  
  tagName: "li"

  events:
    "click": "delete"

  render: ->
    @$el.append "<a><small>Delete Profile</small></a>"
    @

  delete: ->
    bootbox.confirm "Are you sure you want to delete this profile?", (response) =>
      if response is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Persons/Delete/#{@options.personId}"
          success: (data, textStatus, xhr) =>
            bootbox.alert data, =>
              window.location.href = "#{Profiling.applicationUrl}Profiling/Persons"
          error: (xhr, textStatus) ->
            bootbox.alert xhr.responseText
