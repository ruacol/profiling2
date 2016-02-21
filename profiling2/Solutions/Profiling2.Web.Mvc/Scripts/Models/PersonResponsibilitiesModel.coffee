Profiling.Models.PersonResponsibilitiesModel = Backbone.Model.extend

  url: ->
    "#{Profiling.applicationUrl}Profiling/Persons/Responsibilities/#{@get('id')}"

  parse: (response, opts) ->
    if response and response.Events
      response.Events = for e in response.Events
        e.Narrative = e.Narrative.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if e.Narrative
        e.Notes = e.Notes.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if e.Notes
        e.Tags = for t in e.Tags
          t.Events = _(t.Events).filter (x) -> x.Id isnt e.Id
          t
        e

    response

  getResponsibilityTypes: ->
    types = _(@get 'PersonResponsibilityTypes').pluck "Name"

  getVerifiedStatuses: ->
    statuses = _(@get 'VerifiedStatuses').pluck "Name"

  getResponsibilitiesByEvent: ->
    indexed = _(@get 'Responsibilities').groupBy (item) -> item.EventId
    
  getResponsibilitiesByFunction: ->
    indexed = _(@get 'Responsibilities').groupBy (item) -> item.PersonFunctionUnitSummary

  getResponsibilitiesByType: ->
    #indexed = _(@get 'Responsibilities').groupBy (item) -> item.PersonResponsibilityType.Name
    indexed = {}
    for type in @getResponsibilityTypes()
      indexed[type] = []
    for responsibility in @get('Responsibilities')
      indexed[responsibility.PersonResponsibilityType.Name].push responsibility
    indexed

  getActionsTaken: ->
    actions = _.chain(@get 'Events').pluck("ActionsTaken").flatten().value()

  getSortedEvents: ->
    events = _(@get 'Events').sortBy (item) ->
      startMoment = moment(if item.StartDate then item.StartDate.replace(/-/g, '1') else item.StartDate)
      endMoment = moment(if item.EndDate then item.EndDate.replace(/^-\//, '9999/').replace('/-/', '/12/').replace(/\/-$/, '/30') else item.EndDate)
      if startMoment.isValid()
        parseInt startMoment.format("X")
      else if endMoment.isValid()
        parseInt endMoment.format("X")
      else
        0
    
    events = events.reverse() if events
    events

  getViolationCounts: (conditionalityInterest) ->
    counts = { }
    responsibilities = @getResponsibilitiesByType()
    types = @getResponsibilityTypes()
    for type in types
      counts[type] = { }

    for type in types
      if responsibilities[type]
        for responsibility in responsibilities[type]
          for violation in responsibility.Violations
            if violation.ConditionalityInterest is conditionalityInterest
              if not counts[type][violation.Name]
                counts[type][violation.Name] = 0
              counts[type][violation.Name]++

    counts
