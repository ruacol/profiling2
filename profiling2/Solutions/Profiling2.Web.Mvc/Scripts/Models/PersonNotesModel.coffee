Profiling.Models.PersonNotesModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Persons/#{@get('id')}/Notes/Json"

  parse: (response, opts) ->
    if response and response.Notes
      response.Notes = for i in response.Notes
        i.Note = i.Note.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />')
        i
    response