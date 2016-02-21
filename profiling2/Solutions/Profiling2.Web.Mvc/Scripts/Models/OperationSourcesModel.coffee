Profiling.Models.OperationSourcesModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/OperationSources/Json/#{@get('id')}"
