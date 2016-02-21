Profiling.Views.SelectMostRecentlyViewedEventsView = Backbone.View.extend

  templates:
    dropdown: _.template """
      <select 
        class="input-xxlarge" 
        name="most-recent-event" 
        id="most-recent-event"
        title="(optional) This list consists of most recently viewed events.  Search results will show whether each source is already attached to the selected event."
      >
        <option></option>
        <% _(events).each(function(event) { %>
          <option value="<%= event.id %>"><%= event.name %></option>
        <% }); %>
      </select>
    """

  initialize: ->
    $(window).on "storage", (e) =>
      @render() if e and e.originalEvent and e.originalEvent.key is Profiling.MOST_RECENT_EVENTS

  render: ->
    events = Profiling.getFromStorage(Profiling.MOST_RECENT_EVENTS).reverse()
    @$el.html @templates.dropdown
      events: events
        
    first = _(events).first()
    selectedEventId = first.id if first
    $("#most-recent-event").val(selectedEventId).change()

    @
