Profiling.Views.PersonDetailsScreeningInformationView = Backbone.View.extend

  templates:
    table: _.template """
      <table class="table table-condensed table-bordered">
        <tbody>
          <%= rows %>
        </tbody>
      </table>
    """
    requestRows: _.template """
      <tr>
        <th colspan="<%= _(model.EntityResults).size() + 2 %>">
          <a href="<%= Profiling.applicationUrl %>Screening/Requests/Details/<%= model.RequestId %>">
            <%= model.RequestName %>
          </a>
          &nbsp;
          <button type="button" class="btn btn-mini" data-toggle="collapse" data-target=".request-<%= model.RequestId %>-commentary">
            Toggle commentary
          </button>
          <span class="pull-right">
            <%= model.FinalSupportStatus %>
          </span>
        </th>
      </tr>
      <% if (model.Notes) { %>
        <tr>
          <td colspan="<%= _(model.EntityResults).size() + 2 %>">
            <%= model.Notes %>
          </td>
        </tr>
      <% } %>
      <tr>
        <% _([ 'JHRO', 'JMAC', 'CPS', 'SWA' ]).each(function(name) { %>
          <% var entityResult = _(model.EntityResults).findWhere({ Name: name }); %>
          <% if (entityResult) { %>
            <td class="screening-result <%= (entityResult ? entityResult.Result : '') %>">
              <%= name %><%= (entityResult && entityResult.Result ? ": " + entityResult.Result : '') %>
            </td>
          <% } %>
        <% }) %>
        <td class="screening-result <%= (model.RecommendationResult ? model.RecommendationResult.Result : '') %>">
          PWG Recommendation<%= (model.RecommendationResult ? ": " + model.RecommendationResult.Result : '') %>
        </td>
        <td class="screening-result <%= (model.FinalResult ? model.FinalResult.Result : '') %>">
          Final Decision<%= (model.FinalResult ? ": " + model.FinalResult.Result : '') %>
        </td>
      </tr>
      <tr>
        <% _([ 'JHRO', 'JMAC', 'CPS', 'SWA' ]).each(function(name) { %>
          <% var entityResult = _(model.EntityResults).findWhere({ Name: name }); %>
          <% if (entityResult) { %>
            <td>
              <% if (entityResult.Result) { %>
                <dl>
                  <dt>Date</dt>
                  <dd><%= moment(entityResult.Date).format(Profiling.DATE_FORMAT) %></dd>
                  <dt class="request-<%= model.RequestId %>-commentary collapse out">Reason</dt>
                  <dd class="request-<%= model.RequestId %>-commentary collapse out">
                    <% if (entityResult.Reason) { %>
                      <%= entityResult.Reason %>
                    <% } else { %>
                      <span class="muted">&nbsp;</span>
                    <% } %>
                  </dd>
                  <dt class="request-<%= model.RequestId %>-commentary collapse out">Commentary</dt>
                  <dd class="request-<%= model.RequestId %>-commentary collapse out">
                    <% if (entityResult.Commentary) { %>
                      <%= entityResult.Commentary %>
                    <% } else { %>
                      <span class="muted">&nbsp;</span>
                    <% } %>
                  </dd>
                </dl>
              <% } %>
            </td>
          <% } %>
        <% }) %>
        <td>
          <% if (model.RecommendationResult) { %>
            <dl>
              <dt>Date</dt>
              <dd><%= moment(model.RecommendationResult.Date).format(Profiling.DATE_FORMAT) %></dd>
              <dt class="request-<%= model.RequestId %>-commentary collapse out">Commentary</dt>
              <dd class="request-<%= model.RequestId %>-commentary collapse out">
                <% if (model.RecommendationResult.Commentary) { %>
                  <%= model.RecommendationResult.Commentary %>
                <% } else { %>
                  <span class="muted">&nbsp;</span>
                <% } %>
              </dd>
            </dl>
          <% } %>
        </td>
        <td>
          <% if (model.FinalResult) { %>
            <dl>
              <dt>Date</dt>
              <dd><%= moment(model.FinalResult.Date).format(Profiling.DATE_FORMAT) %></dd>
              <dt class="request-<%= model.RequestId %>-commentary collapse out">Commentary</dt>
              <dd class="request-<%= model.RequestId %>-commentary collapse out">
                <% if (model.FinalResult.Commentary) { %>
                  <%= model.FinalResult.Commentary %>
                <% } else { %>
                  <span class="muted">&nbsp;</span>
                <% } %>
              </dd>
            </dl>
          <% } %>
        </td>
      </tr>
    """
    noRequests: _.template """
      <span class="muted">No screening information.</span>
    """

  render: ->
    requests = _(@model.get 'ScreeningInformation').sortBy (item) -> item.RequestId

    if requests and _(requests).size() > 0
      requests = requests.reverse()
      for i in requests
        @$el.append @templates.table
          rows: @templates.requestRows
            model: i

      _.defer ->
        $("td.screening-result").each (i, el) ->
          $(el).width '16%'
          if $(el).hasClass('Red')
            $(el).css 'backgroundColor', 'red'
          else if $(el).hasClass('Yellow')
            $(el).css 'backgroundColor', 'yellow'
          else if $(el).hasClass('Green')
            $(el).css 'backgroundColor', 'green'
            $(el).css 'color', 'white'
    else
      @$el.append @templates.noRequests

    @