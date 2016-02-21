Profiling.Views.PotentialMatchingEventsView = Backbone.View.extend
  
  templates:
    title: _.template """
      <h4 style="margin: 10px 0;">Potential matching events</h4>
    """
    contents: _.template """
        <table class="table table-bordered table-condensed">
          <thead>
            <tr>
              <th colspan="7"><%= subtitle %></th>
            </tr>
            <tr>
              <th>ID</th>
              <th>Case Code</th>
              <th>Categories</th>
              <th>Start Date</th>
              <th>End Date</th>
              <th>Location</th>
            </tr>
          </thead>
          <tbody>
            <% _(candidates).each(function(candidate) { %>
              <tr>
                <td><a href="<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= candidate.Id %>" target="_blank"><%= candidate.Id %></a></td>
                <td><%= candidate.JhroCaseNumber %></td>
                <td><%= candidate.Violations %></td>
                <td><%= candidate.StartDateDisplay %></td>
                <td><%= candidate.EndDateDisplay %></td>
                <td><%= candidate.Location %></td>
              </tr>
            <% }); %>
          </tbody>
        </table>
    """
    list: _.template """
      <p><%= subtitle %></p>
      <ul>
        <% _(candidates).each(function(candidate) { %>
          <li>
            <a href="<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= candidate.Id %>" target="_blank"><%= candidate.Id %></a>
            /
            <%= candidate.Headline %>
            <% if (candidate.CaseCode) { %>
              /
              <%= candidate.CaseCode %>
            <% } %>
          </li>
        <% }); %>
      </ul>
    """
    none: _.template """
      <p>No potential matching events detected.</p>
    """

  render: ->
    @$el.html ""

    if _.flatten(_(@model.get 'Events').values()).length is 0
      @$el.append @templates.none()
    else
      for key in _(@model.get 'Events').keys()
        if _(@model.get('Events')[key]).size() > 0
          @$el.append @templates.list
            candidates: @model.get('Events')[key]
            subtitle: "... by #{key}"
    @
