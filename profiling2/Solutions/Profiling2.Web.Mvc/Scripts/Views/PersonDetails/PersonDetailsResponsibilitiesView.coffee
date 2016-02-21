Profiling.Views.PersonDetailsResponsibilitiesView = Backbone.View.extend
  className: "span12"

  templates: 
    table: _.template """
      <table class="table table-condensed">
        <thead>
          <tr>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Location</th>
            <th>Event</th>
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
        <td><%= _(model).first().EventStartDate %></td>
        <td><%= _(model).first().EventEndDate %></td>
        <td title="<%= _(model).first().EventLocationFullName %>">
          <a href='<%= Profiling.applicationUrl %>Profiling/Locations/Details/<%= _(model).first().EventLocationId %>' target='_blank'><%= _(model).first().EventLocationName %></a> 
        </td>
        <td>
          <span class="event" data-event-id="<%= _(model).first().EventId %>"></span>
          <a href='<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= _(model).first().EventId %>' target='_blank'><%= _(model).first().EventHeadline %></a>
          <% if (_(model).first().EventVerifiedStatusName) { %>
            <span class="label"><small><%= _(model).first().EventVerifiedStatusName %></small></span>
          <% } %>
          <% if (_(model).first().RelatedEvents.length > 0) { %>
            <% _(_(model).first().RelatedEvents).each(function(el) { %>
              <br />
              <div class="related-event" data-selector="#relationship-notes-<%= el.Id %>">
                <span class="icon-arrow-right" style="margin-left: 15px;"></span> 
                <% if (el.SubjectId) { %>
                  <a href='<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= el.SubjectId %>' target='_blank'><%= el.Headline %></a>
                  <% if (el.EventVerifiedStatusName) { %>
                    <span class="label"><small><%= el.EventVerifiedStatusName %></small></span>
                  <% } %>
                  <%= el.Type %>...
                <% } else { %>
                  ...<%= el.Type %>
                  <a href='<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= el.ObjectId %>' target='_blank'><%= el.Headline %></a>
                  <% if (el.EventVerifiedStatusName) { %>
                    <span class="label"><small><%= el.EventVerifiedStatusName %></small></span>
                  <% } %>
                <% } %>
              </div>
              <span id="relationship-notes-<%= el.Id %>" style="display: none;">
                <% if (el.Notes) { %>
                  <%= el.Notes %>
                <% } else { %>
                  <span class="muted">No notes on this event relationship.</span>
                <% } %>
              </span>
            <% }) %>
          <% } %>
        </td>
      </tr>
      <tr>
        <td colspan="4">
          <ul>
            <% if (_(model).first().EventNarrative) { %>
              <li><%= _(model).first().EventNarrative %></li>
            <% } %>
            <li>
              Function: 
              <% if (_(model).first().PersonFunctionUnitSummary) { %>
                <strong><%= _(model).first().PersonFunctionUnitSummary %></strong>
              <% } else { %>
                <span class="muted">No known function at the time</span>
              <% } %>
              <table class="table table-condensed">
                <thead>
                  <tr>
                    <td></td><td>Categories</td><td>Commentary</td><td>Responsibility</td>
                  </tr>
                </thead>
                <tbody>
                  <% _(model).each(function(pr, i) { %>
                    <tr>
                      <td>
                        <span class="person-responsibility" data-id="<%= pr.Id %>"></span>
                      </td>
                      <td>
                        <strong>
                          <%= _(pr.Violations).map(function(obj) { return obj.Name; }).join("; ") %>
                        </strong>
                      </td>
                      <td>
                        <% if (pr.Commentary) { %>
                          <%= pr.Commentary %>
                        <% } else { %>
                          <span class='muted'>No commentary</span>
                        <% } %>
                      </td>
                      <td><strong><%= pr.PersonResponsibilityTypeName %></strong></td>
                    </tr>
                  <% }); %>
                </tbody>
              </table>
            </li>
            <li>
              <span class="others-responsible" data-event-id="<%= _(model).first().EventId %>">Others responsible:</span>
              <% if (_.isEmpty(_(model).first().OthersResponsible)) { %>
                <span class="muted">No others responsible.</span>
              <% } else { %>
                <ul>
                  <% _(_(model).first().OthersResponsible).each(function(responsibility) { %>
                    <li>
                      <span class="person-responsible" data-id="<%= responsibility.Id %>">
                        <a href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= responsibility.PersonId %>" target="_blank" data-selector="#responsibility-tooltip-<%= responsibility.Id %>">
                          <%= responsibility.PersonName %>
                        </a>
                        <span id="responsibility-tooltip-<%= responsibility.Id %>" style="display: none;">
                          <p>
                            Function:
                            <% if (responsibility.PersonFunctionUnitSummary) { %>
                              <strong><%= responsibility.PersonFunctionUnitSummary %></strong>
                            <% } else { %>
                              <span class="muted">No known function at the time</span>
                            <% } %>
                          </p>
                          <p>
                            <%= _(responsibility.Violations).map(function(obj) { return obj.Name; }).join("; ") %>
                          </p>
                          <p>
                            Commentary:
                            <% if (responsibility.Commentary) { %>
                              <%= responsibility.Commentary %>
                            <% } else { %>
                              <span class="muted">No commentary</span>
                            <% } %>
                          </p>
                          <p><strong><%= responsibility.PersonResponsibilityTypeName %></strong> responsibility</p>
                        </span>
                      </span>
                    </li>
                  <% }); %>
                </ul>
              <% } %>
            </li>
            <li>
              <span class="actions-taken" data-event-id="<%= _(model).first().EventId %>">Actions taken:</span>
              <% if (_.isEmpty(_(model).first().ActionsTaken)) { %>
                <span class="muted">No information exists on actions taken.</span>
              <% } else { %>
                <ul>
                  <% _(_(model).first().ActionsTaken).each(function(action) { %>
                    <li>
                      <span class="action-taken" data-id="<%= action.Id %>">
                        <% if (action.StartDate != '-' && action.EndDate != '-') { %>
                          <%= action.StartDate %> to <%= action.EndDate %>:
                        <% } else if (action.StartDate != '-') { %>
                          <%= action.StartDate %>:
                        <% } else if (action.EndDate != '-') { %>
                          <%= action.EndDate %>:
                        <% } %>
                        <%= action.ActionTakenSummary %>
                      </span>
                      <% if (action.ActionTakenTypeIsRemedial) { %>
                        <span class="label"><small>REMEDIAL</small></span>
                      <% } %>
                      <% if (action.ActionTakenTypeIsDisciplinary) { %>
                        <span class="label"><small>DISCIPLINARY</small></span>
                      <% } %>
                    </li>
                  <% }); %>
                </ul>
              <% } %>
            </li>
            <li>
              <span class="event-sources" data-event-id="<%= _(model).first().EventId %>">Sources:</span>
              <% if (_.isEmpty(_(model).first().EventSources)) { %>
                <span class="muted">No sources.</span>
              <% } else { %>
                <table class="table table-condensed" style="width: auto;">
                  <thead>
                    <tr>
                      <td></td><td>Source ID</td><td>Reliability</td><td>Commentary</td>
                    </tr>
                  </thead>
                  <tbody>
                    <% _(_(model).first().EventSources).each(function(es) { %>
                      <tr>
                        <td>
                          <span class="event-source" data-id="<%= es.Id %>"></span>
                        </td>
                        <td>
                          <a href="<%= Profiling.applicationUrl %>Profiling/Sources#info/<%= es.SourceId %>" target="_blank"><%= es.SourceId %></a>
                          <% if (es.SourceArchive === true) { %>
                            &nbsp;<span class="label"><small>ARCHIVED</small></span>
                          <% } %>
                          <% if (es.SourceIsRestricted === true) { %>
                            &nbsp;<span class="label label-important"><small>RESTRICTED</small></span>
                          <% } %>
                        </td>
                        <td><%= es.ReliabilityName %></td>
                        <td><%= es.Commentary %></td>
                      </tr>
                    <% }); %>
                  </tbody>
                </table>
              <% } %>
            </li>
          </ul>
        </td>
      </tr>
    """
    empty: _.template """
      <span class="muted">No human rights records.</span>
    """
    numbersTable: _.template """
      <% _(counts).chain().keys().each(function(type, i) { %>
        <table class="table table-bordered table-condensed" style="width: auto;">
          <tr>
            <th colspan="2"><%= type %></th>
          </tr>
          <% _(counts[type]).chain().keys().sortBy(function(item) { return item; }).each(function(category, j) { %>
            <% if (counts[type][category].ConditionalityInterest === conditionalityInterest) { %>
              <tr>
                <td><%= category %></td>
                <td><%= counts[type][category].Count %></td>
              </tr>
            <% } %>
          <% }) %>
          <tr>
            <th>Total <%= type %></th>
            <th>
              <%= _(counts[type]).keys().reduce(function(memo, cat) { 
                  return (counts[type][cat].ConditionalityInterest === conditionalityInterest ? memo + counts[type][cat].Count : memo); 
                }, 0) %>
            </th>
          </tr>
        </table>
      <% }) %>
    """
    numbers: _.template """
      <p>
        Command responsibility for <%= numCommand %> events (<%= numCommandByViolation %> violations), 
        direct responsibility for <%= numDirect %> events (<%= numDirectByViolation %> violations),
        indirect command for <%= numIndirectCommand %> events (<%= numIndirectCommandByViolation %> violations).
        &nbsp;&nbsp;
        <a class="btn btn-mini" data-toggle="collapse" data-target="#resp-counts">Toggle</a> breakdown by violation.
      </p>
      <div id="resp-counts" class="collapse out">
        <p>Violations relevant for HRDDP purposes appear on the left.</p>
        <div class="row-fluid">
          <div class="span6">
            <%= conditionalityInterestTable %>
          </div>
          <div class="span6">
            <%= notInterestTable %>
          </div>
        </div>
      </div>
    """
    filter: _.template """
      <p>
        <a class="btn btn-mini" id="toggle-events-button">Toggle</a> only events with direct responsibility.
      </p>
    """

  events:
    "click #toggle-events-button": "toggleEvents"

  toggleEvents: ->
    @toggleEventsFlag = if @toggleEventsFlag is true then false else true
    @render()

  render: ->
    if @options.permissions.canPerformScreeningInput
      @$el.html @templates.filter
    else
      @$el.html ''

    indexedResponsibilities = @model.getIndexedResponsibilities()

    # send an array of PersonResponsibilityViewModel for templating
    rows = for i in @model.getSortedEventIds(@toggleEventsFlag)
      @templates.row
        model: indexedResponsibilities[i]

    counts = @model.countResponsibilitiesByTypeAndViolation()

    if _.isEmpty rows
      @$el.append @templates.empty
    else
      if @options.permissions.canPerformScreeningInput
        @$el.append @templates.numbers
          numCommand: @model.getTotalResponsibilitiesOfType 'Command'
          numDirect: @model.getTotalResponsibilitiesOfType 'Direct'
          numIndirectCommand: @model.getTotalResponsibilitiesOfType 'Indirect command'
          numCommandByViolation: @model.countTotalResponsibilitiesByViolation 'Command'
          numDirectByViolation: @model.countTotalResponsibilitiesByViolation 'Direct'
          numIndirectCommandByViolation: @model.countTotalResponsibilitiesByViolation 'Indirect command'
          conditionalityInterestTable: @templates.numbersTable
            counts: counts
            conditionalityInterest: true
          notInterestTable: @templates.numbersTable
            counts: counts
            conditionalityInterest: false

      @$el.append @templates.table
        rows: rows

    personId = @options.personId
    _.defer =>
      Profiling.setupUITooltips "span.person-responsible a"
      Profiling.setupUITooltips "div.related-event"
      $("span.person-responsibility").each (i, el) ->
        responsibilityId = $(el).data 'id'
        editFormView = new Profiling.Views.PersonResponsibilityEditFormView
          modalId: "modal-person-responsibility-edit-#{responsibilityId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Edit/#{responsibilityId}"
          modalSaveButton: 'modal-person-responsibility-edit-button'
        $(el).before editFormView.render().el
        deleteButtonView = new Profiling.Views.HumanRightsDeleteButtonView
          title: "Delete Human Rights Responsibility"
          confirm: "Are you sure you want to remove this person's responsibility for this event?"
          url: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Delete/#{responsibilityId}"
        $(el).before deleteButtonView.render().el

      if @options.permissions.canChangeEvents
        $("span.event").each (i, el) ->
          eventId = $(el).data 'event-id'
          editEventView = new Profiling.Views.EventEditFormIconView
            eventId: eventId
            modalId: "modal-event-edit-#{eventId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Events/Edit/#{eventId}"
            modalSaveButton: 'modal-event-edit-save-button'
          $(el).before editEventView.render().el

        $("span.action-taken").each (i, el) ->
          actionTakenId = $(el).data 'id'
          editFormView = new Profiling.Views.ActionTakenEditFormView
            actionTakenId: actionTakenId
            modalId: "modal-action-taken-edit-#{actionTakenId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/ActionsTaken/Edit/#{actionTakenId}"
            modalSaveButton: 'modal-action-taken-edit-button'
          $(el).before editFormView.render().el

          deleteButtonView = new Profiling.Views.BaseDeleteButtonView
            title: "Delete Action Taken"
            confirm: "Are you sure you want to remove this action taken for this event?"
            url: "#{Profiling.applicationUrl}Profiling/ActionsTaken/Delete/#{actionTakenId}"
          $(el).before deleteButtonView.render().el

        $("span.actions-taken").each (i, el) ->
          eventId = $(el).data 'event-id'
          addActionTakenView = new Profiling.Views.ActionTakenAddIconFormView
            eventId: eventId
            modalId: "modal-action-taken-add-#{eventId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{eventId}/ActionsTaken/Add"
            modalSaveButton: 'modal-action-taken-add-button'
          $(el).before addActionTakenView.render().el

      if @options.permissions.canChangePersonResponsibilities
        $("span.person-responsible").each (i, el) ->
          responsibilityId = $(el).data 'id'
          editFormView = new Profiling.Views.PersonResponsibilityEditFormView
            modalId: "modal-person-responsibility-edit-#{responsibilityId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Edit/#{responsibilityId}"
            modalSaveButton: 'modal-person-responsibility-edit-button'
          $(el).before editFormView.render().el

          deleteButtonView = new Profiling.Views.BaseDeleteButtonView
            title: "Delete Person Responsibility"
            confirm: "Are you sure you want to delete this person's responsibility for this event?"
            url: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Delete/#{responsibilityId}"
          $(el).before deleteButtonView.render().el

        $("span.others-responsible").each (i, el) ->
          eventId = $(el).data 'event-id'
          addPersonResponsibilityView = new Profiling.Views.PersonResponsibilityAddIconFormView
            eventId: eventId
            modalId: "modal-person-responsibility-add-#{eventId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{eventId}/PersonResponsibilities/Add"
            modalSaveButton: 'modal-person-responsibility-add-button'
          $(el).before addPersonResponsibilityView.render().el

      $("span.event-source").each (i, el) ->
        esId = $(el).data 'id'
        editFormView = new Profiling.Views.EventSourceEditFormView
          modalId: "modal-event-source-edit-#{esId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/EventSources/Edit/#{esId}"
          modalSaveButton: 'modal-event-source-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Detach Source"
          confirm: "Are you sure you want to detach this source from this event?"
          url: "#{Profiling.applicationUrl}Profiling/EventSources/Delete/#{esId}"
        $(el).before deleteButtonView.render().el

      $("span.event-sources").each (i, el) ->
        eventId = $(el).data 'event-id'
        addSourceView = new Profiling.Views.EventAddSourceIconFormView
          eventId: eventId
          modalId: "modal-event-source-add-#{eventId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{eventId}/Sources/Add"
          modalSaveButton: 'modal-event-source-add-button'
        $(el).before addSourceView.render().el

    @