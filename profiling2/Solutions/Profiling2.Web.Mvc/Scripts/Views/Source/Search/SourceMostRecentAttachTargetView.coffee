Profiling.Views.SourceMostRecentAttachTargetView = Backbone.View.extend

  tagName: "table"
  className: "table table-bordered"

  templates:
    table: _.template """
      <tr>
        <th class="span3">Selected person</th>
        <td class="span9" id="persons-cell"></td>
      </tr>
      <tr>
        <th>Selected event</th>
        <td id="events-cell"></td>
      </tr>
    """

  initialize: ->
    @personsView = new Profiling.Views.SelectMostRecentlyViewedPersonsView()
    @eventsView = new Profiling.Views.SelectMostRecentlyViewedEventsView()

  render: ->
    @$el.attr "style", "width: auto;"
    @$el.append @templates.table()

    _.defer =>
      $("#persons-cell").append @personsView.render().el
      $("#events-cell").append @eventsView.render().el

    @
    
  getTargetType: ->
    if $("#most-recent-person").val()
      return 'Person'
    if $("#most-recent-event").val()
      return 'Event'
    null

  getTargetId: ->
    personId = $("#most-recent-person").val()
    return personId if personId
    
    eventId = $("#most-recent-event").val()
    return eventId if eventId 

    null