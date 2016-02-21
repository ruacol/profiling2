Profiling.Models.UnitSourcesModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/UnitSources/UnitSources/#{@get('id')}"
