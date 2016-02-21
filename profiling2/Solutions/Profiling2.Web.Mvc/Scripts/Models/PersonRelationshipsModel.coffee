Profiling.Models.PersonRelationshipsModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Persons/Relationships/#{@get('id')}"
