Profiling.Models.PersonModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Persons/Json/#{@get('id')}"

  parse: (response, opts) ->
    response.Person.PublicSummary = response.Person.PublicSummary.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if response.Person.PublicSummary
    response.Person.BackgroundInformation = response.Person.BackgroundInformation.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if response.Person.BackgroundInformation
    response.Person.Notes = response.Person.Notes.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if response.Person.Notes
    response