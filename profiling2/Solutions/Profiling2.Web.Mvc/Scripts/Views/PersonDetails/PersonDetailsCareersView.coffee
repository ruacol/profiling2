Profiling.Views.PersonDetailsCareersView = Backbone.View.extend

  templates: 
    table: _.template """
      <ul>
        <% _.each(rows, function(row) { %>
          <%= row %>
        <% }); %>
      </ul>
    """
    row: _.template """
      <li>
        <span class="career" data-id="<%= model.Id %>"></span>
        <% if (model.Commentary) { %>
          <i class="icon-comment accordion-toggle" 
            data-toggle="collapse" 
            data-target="#commentary-<%= model.Id %>"
            title="Toggle Commentary">
          </i>
        <% } %>

        <% if (model.StartDate.indexOf('No date') < 0) { %>
          From <%= Profiling.incompleteDateFormat(model.YearOfStart, model.MonthOfStart, model.DayOfStart) %><%= (model.EndDate.indexOf('No date') < 0 ? "" : ":") %>
          <% if (model.EndDate.indexOf('No date') < 0) { %>
            until <%= Profiling.incompleteDateFormat(model.YearOfEnd, model.MonthOfEnd, model.DayOfEnd) %>:
          <% } %>
        <% } else if (model.EndDate.indexOf('No date') < 0) { %>
          <% if (model.AsOfDate.indexOf('No date') < 0) { %>
            As of <%= Profiling.incompleteDateFormat(model.YearAsOf, model.MonthAsOf, model.DayAsOf) %> until
          <% } else { %>
            Until
          <% } %>
          <%= Profiling.incompleteDateFormat(model.YearOfEnd, model.MonthOfEnd, model.DayOfEnd) %>:
        <% } else if (model.AsOfDate.indexOf('No date') < 0) { %>
          As of <%= Profiling.incompleteDateFormat(model.YearAsOf, model.MonthAsOf, model.DayAsOf) %>:
        <% } %>
        
        <% if (model.FunctionUnitSummary) { %>
          <%
            var unitAtag = "<a href='" + Profiling.applicationUrl + "Profiling/Units/Details/" + model.UnitId + "' target='_blank'>" + model.UnitName + "</a>";
          %>
          <%= model.FunctionUnitSummary.replace(model.UnitName, unitAtag) %><%= (model.RankOrganizationLocationSummary ? " / " : ".") %>
        <% } %>
        <% if (model.RankOrganizationLocationSummary) { %>
          <%
            var locationAtag = "<a href='" + Profiling.applicationUrl + "Profiling/Locations/Details/" + model.LocationId + "' target='_blank'>" + model.LocationName + "</a>";
          %>
          <%= model.RankOrganizationLocationSummary.replace(model.LocationName, locationAtag) %>.
        <% } %>

        <% if (model.Defected) { %>
          <span class="label label-important"><small>DEFECTED</small></span>
        <% } %>

        <% if (model.Absent) { %>
          <span class="label label-success"><small>ABSENT</small></span>
        <% } %>
        
        <% if (model.Nominated) { %>
          <span class="label label-success"><small>NOMINATED</small></span>
        <% } %>

        <% if (model.Commentary) { %>
          <div id="commentary-<%= model.Id %>" class="collapse out">
            <p>
              <%= model.Commentary %>
            </p>
          </div>
        <% } %>
      </li>
    """
    empty: _.template """
      <span class="muted"><%= text %></span>
    """

  render: ->

    currentCareers = _(@model.get 'Careers').chain().filter (item) ->
        item.IsCurrentCareer is true
      .sortBy (item) ->
        if item.StartDate and (item.StartDate.indexOf('No date') < 0)
          item.StartDate
        else if item.EndDate and (item.EndDate.indexOf('No date') < 0)
          item.EndDate
        else
          item.AsOfDate
      .value()
    careers = _(@model.get 'Careers').chain().filter (item) ->
        item.IsCurrentCareer is false
      # since sortBy is a stable sorting algorithm, we can approximate sorting by multiple attributes by sorting the last attribute first, etc.
      .sortBy (item) ->
        if item.StartDate and (item.StartDate.indexOf('No date') < 0)
          3
        else if item.EndDate and (item.EndDate.indexOf('No date') < 0)
          1
        else
          2
      .sortBy (item) ->
        if item.StartDate and (item.StartDate.indexOf('No date') < 0)
          item.StartDate
        else if item.EndDate and (item.EndDate.indexOf('No date') < 0)
          item.EndDate
        else
          item.AsOfDate
      .value()

    currentCareerRows = for i in currentCareers
      @templates.row
        model: i
    currentCareerRows = currentCareerRows.reverse() if currentCareerRows

    rows = for i in careers
      @templates.row
        model: i
    rows = rows.reverse() if rows

    $(@el).html "<h4>Current Position</h4>"
    if _.isEmpty currentCareers
      $(@el).append @templates.empty { text: "No current position." }
    else
      $(@el).append @templates.table
        rows: currentCareerRows
    $(@el).append "<h4>Career Information</h4>"
    if _.isEmpty careers
      $(@el).append @templates.empty { text: "No careers." }
    else
      $(@el).append @templates.table
        rows: rows

    if @options.permissions.canChangePersons
      personId = @options.personId
      _.defer ->
        $("span.career").each (i, el) ->
          careerId = $(el).data 'id'
          editFormView = new Profiling.Views.CareerEditFormView
            careerId: careerId
            modalId: "modal-person-career-edit-#{careerId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Careers/Edit/#{careerId}"
            modalSaveButton: 'modal-person-career-edit-button'
          $(el).before editFormView.render().el
          deleteButtonView = new Profiling.Views.CareerDeleteButtonView
            title: "Delete Career"
            confirm: "Are you sure you want to delete this career?"
            url: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Careers/Delete/#{careerId}"
          $(el).before deleteButtonView.render().el

    @