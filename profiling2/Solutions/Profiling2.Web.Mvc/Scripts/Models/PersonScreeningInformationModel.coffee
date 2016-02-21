Profiling.Models.PersonScreeningInformationModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Persons/Screening/#{@get('id')}"

  parse: (response, opts) ->
    for request in response.ScreeningInformation
      for entityResult in request.EntityResults
        entityResult.Reason = entityResult.Reason.replace /\n/g, '<br />' if entityResult.Reason
        entityResult.Commentary = entityResult.Commentary.replace /\n/g, '<br />' if entityResult.Commentary
      if request.RecommendationResult
        request.RecommendationResult.Commentary = request.RecommendationResult.Commentary.replace /\n/g, '<br />' if request.RecommendationResult.Commentary
      if request.FinalResult
        request.FinalResult.Commentary = request.FinalResult.Commentary.replace /\n/g, '<br />' if request.FinalResult.Commentary

    response