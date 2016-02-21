Profiling.Models.PotentialMatchingEventsModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Hrdb/Cases/FindMatchingEventCandidates/#{@get('id')}"
