Profiling.Views.SelectMostRecentlyViewedPersonsView = Backbone.View.extend

  templates:
    dropdown: _.template """
      <select 
        class="input-xlarge" 
        name="most-recent-person" 
        id="most-recent-person"
        title="(optional) This list consists of most recently viewed persons.  Search results will show whether each source is already attached to the selected person."
      >
        <option></option>
        <% _(persons).each(function(person) { %>
          <option value="<%= person.id %>"><%= person.name %></option>
        <% }); %>
      </select>
    """

  initialize: ->
    $(window).on "storage", (e) =>
      @render() if e and e.originalEvent and e.originalEvent.key is Profiling.MOST_RECENT_PERSONS

  render: ->
    persons = Profiling.getFromStorage(Profiling.MOST_RECENT_PERSONS).reverse()
    @$el.html @templates.dropdown
      persons: persons
        
    first = _(persons).first()
    selectedPersonId = first.id if first
    $("#most-recent-person").val(selectedPersonId).change()

    @
