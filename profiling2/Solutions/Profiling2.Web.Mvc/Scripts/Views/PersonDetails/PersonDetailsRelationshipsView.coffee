Profiling.Views.PersonDetailsRelationshipsView = Backbone.View.extend
  className: "span12"

  templates:
    table: _.template """
      <table class="table">
        <thead>
          <tr>
            <th></th>
            <th>From</th>
            <th>To</th>
            <th>Relationship</th>
            <th>Notes</th>
          </tr>
        </thead>
        <tbody>
          <% _.each(rows, function(row) { %>
            <%= row %>
          <% }); %>
        </tbody>
      </table>
    """
    row: _.template """
      <tr>
        <td style="white-space: nowrap;">
          <span class="person-relationship" data-id="<%= model.Id %>"></span>
        </td>
        <td style="white-space: nowrap;"><%= model.StartDate %></td>
        <td style="white-space: nowrap;"><%= model.EndDate %></td>
        <td style="white-space: nowrap;">
          <% if (model.SubjectPersonId == personId) { %>
            <%= model.SubjectPersonName %>
          <% } else { %>
            <a href='<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= model.SubjectPersonId %>'><%= model.SubjectPersonName %></a>
          <% } %>
          <%= model.PersonRelationshipTypeName %>
          <% if (model.ObjectPersonId == personId) { %>
            <%= model.ObjectPersonName %>
          <% } else { %>
            <a href='<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= model.ObjectPersonId %>'><%= model.ObjectPersonName %></a>.
          <% } %>
        </td>
        <td><%= model.Notes %></td>
      </tr>
    """
    empty: _.template """
      <span class="muted">No person relationships.</span>
    """
    sameEthnicityText: _.template """
      <hr />
      <h4 style="padding-top: 10px; padding-bottom: 10px;">List of people with the same ethnicity</h4>
    """
    sameEthnicityTable: _.template """
      <table class="table" id="ethnicity-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Ethnicity</th>
            <th>ID Number
            <th>Function</th>
            <th>Rank</th>
          </tr>
        </thead>
        <tbody>
          <% _.each(rows, function(row) { %>
            <tr>
              <td><a href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= row.Id %>"><%= row.Name %></a></td>
              <td><%= row.EthnicityName %></td>
              <td><%= row.MilitaryIDNumber %></td>
              <td>
                <% if (row.FunctionUnitSummary) { %>
                  <% if (row.UnitId  && row.UnitName) { %>
                    <% var unitAtag = "<a href='" + Profiling.applicationUrl + "Profiling/Units/Details/" + row.UnitId + "' target='_blank'>" + row.UnitName + "</a>"; %>
                    <%= row.FunctionUnitSummary.replace(row.UnitName, unitAtag) %>
                  <% } else { %>
                    <%= row.FunctionUnitSummary %>
                  <% } %>
                <% } %>
              </td>
              <td><%= row.Rank %></td>
            </tr>
          <% }); %>
        </tbody>
      </table>
    """

  render: ->
    personId = @options.personId

    personRelationships = $.merge @model.get('PersonRelationshipsAsSubject'), @model.get('PersonRelationshipsAsObject')
    personRelationships = _(personRelationships).sortBy (item) ->
      if item.StartDate and (item.StartDate.indexOf('No date') < 0)
        item.StartDate
      else
        item.EndDate
    personRelationships = personRelationships.reverse() if personRelationships

    rows = for i in personRelationships
      @templates.row
        model: i
        personId: @options.personId

    if _.isEmpty rows
      $(@el).html @templates.empty
    else
      $(@el).html @templates.table
        rows: rows

    if @options.permissions.canChangePersons
      _.defer ->
        $("span.person-relationship").each (i, el) ->
          relationshipId = $(el).data 'id'
          editFormView = new Profiling.Views.RelationshipEditFormView
            relationshipId: relationshipId
            modalId: "modal-person-relationship-edit-#{relationshipId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Relationships/Edit/#{relationshipId}"
            modalSaveButton: "modal-person-relationship-edit-button-#{relationshipId}"
          $(el).before editFormView.render().el
          deleteButtonView = new Profiling.Views.RelationshipDeleteButtonView
            title: "Delete Person Relationship"
            confirm: "Are you sure you want to delete this relationship?"
            url: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Relationships/Delete/#{relationshipId}"
          $(el).before deleteButtonView.render().el

    if _.size(@model.get('PersonsWithSameEthnicity')) > 0
      $(@el).append @templates.sameEthnicityText()
      $(@el).append @templates.sameEthnicityTable
        rows: @model.get('PersonsWithSameEthnicity')

      _.defer () ->
        new Profiling.DataTable "ethnicity-table",
          bServerSide: false
          aaSorting: [ [ 0, 'asc' ] ]
          sDom: 'T<"clear">tipr'

    @