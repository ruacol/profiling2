Profiling.Models.PersonCareersModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Persons/Careers/#{@get('id')}"
