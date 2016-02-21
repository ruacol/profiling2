Profiling.Models.SimilarEventsModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Events/FindSimilarEvents/#{@get('id')}"
