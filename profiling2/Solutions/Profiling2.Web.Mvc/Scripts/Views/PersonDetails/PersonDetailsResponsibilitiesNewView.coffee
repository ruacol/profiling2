Profiling.Views.PersonDetailsResponsibilitiesNewView = Backbone.View.extend
  className: "span12"

  templates: 
    bottom: _.template """
      <div id="records"></div>
      <div class="navbar navbar-fixed-bottom">
        <div class="navbar-inner">
          <div class="navbar-form">
            <div id="bottom-row"></div>
            <div id="stats"></div>
            <div class="pull-right" style="padding-right: 10px;">
              <label class="radio inline"><input type="radio" name="control" value="summary" /> View summary</label>
              <label class="radio inline"><input type="radio" name="control" value="event" checked="checked" /> View by event</label>
            </div>
          </div>
        </div>
      </div>
    """
    button: _.template """
      <div class="pull-left btn-group" data-toggle="buttons-checkbox" style="padding-right: 20px;">
        <button id="show-detail-button" type="button" class="btn collapsed" data-toggle="collapse" data-target=".responsibility-body" style="margin-left: 10px;">
          Show detail
        </button>
      </div>
    """
    filter: _.template """
      <div class="pull-left" style="padding-left: 10px;">
        <span class="help-inline"><%= label %> </span>
        <select id="<%= select_id %>" class="<%= className %>">
          <option value="All">All</option>
          <% _(options).each(function(option, i) { %>
            <option value="<%= option %>"><%= option %></option>
          <% }); %>
        </select>
      </div>
    """
    stats: _.template """
      <div class="pull-left navbar-text" style="padding-left: 10px;">
        Events: <span class="badge"><%= numEvents %></span>
        Violations: <span class="badge"><%= numViolations %></span>
        Actions: <span class="badge"><%= numActions %></span>
      </div>
    """
    accordion: _.template """
      <div class="accordion">
        <% _(items).each(function(item, i) { %>
          <%= item %>
        <% }); %>
      </div>
    """
    eventHeadlineLink: _.template """
      <a href='<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= event.Id %>' target='_blank'>
        <% if (labelViolations === true) { %>
          <%= _(event.Violations).map(function(obj) { return "<span class='label label-" + (obj.ConditionalityInterest === true ? "warning" : "inverse") + "' style='margin-right: 5px;'>" + obj.Name + "</span>"; }).join("") %>
        <% } else { %>
          <%= _(event.Violations).map(function(obj) { return obj.Name; }).join(", ") %>;
        <% } %>
        <span style="white-space: nowrap;">
          <% if (event.Location && event.Location.Name) { %>
            <%= event.Location.Name %>;
          <% } %>
          <% if (event.StartDate) { %>
            <%= Profiling.prettyPrintIncompleteDate(event.StartDate) %>
          <% } %>
          <% if (event.EndDate) { %>
            to <%= Profiling.prettyPrintIncompleteDate(event.EndDate) %>
          <% } %>
        </span>
      </a>
      <% if (event.VerifiedStatus) { %>
        <span class="label pull-right" style="margin-left: 5px;"><%= event.VerifiedStatus %></span>
      <% } %>
      <% _(event.Tags).each(function(tag) { %>
        <a class="label pull-right" style="margin-left: 5px;" href="<%= Profiling.applicationUrl %>Profiling/Tags/Details/<%= tag.Id %>" target="_blank"><%= tag.TagName %></a>
      <% }); %>
    """
    event: _.template """
      <div class="accordion-group">
        <div class="accordion-heading">
          <div class="accordion-toggle">
            <%= index %>.&nbsp;
            <span class="event" data-event-id="<%= event.Id %>"></span>
            <%= eventHeadlineLinks[event.Id] %>
          </div>
          <% if (event.RelatedEvents.length > 0) { %>
            <div class="accordion-toggle" style="padding: 0px 15px 8px 15px;">
              <% _(event.RelatedEvents).each(function(el) { %>
                <div class="related-event" data-selector="#relationship-notes-<%= el.Id %>">
                  <span class="icon-arrow-right" style="margin-left: 35px;"></span> 
                  <% if (el.SubjectEvent.Id && el.SubjectEvent.Id != event.Id) { %>
                    <%= eventHeadlineLinks[el.SubjectEvent.Id] %>
                    <%= el.RelationshipType %>...
                  <% } else { %>
                    ...<%= el.RelationshipType %>
                    <%= eventHeadlineLinks[el.ObjectEvent.Id] %>
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
            </div>
          <% } %>
        </div>
        <div class="accordion-body">
          <div class="accordion-inner">
            <p class="pull-right" style="margin: 5px;"><span class="label"><%= event.Location.Label %></span></p>
            <p class="event-narrative" data-selector="#event-note-<%= event.Id %>">
              <% if (event.Narrative) { %>
                <%= event.Narrative %>
              <% } else { %>
                <span class="muted">This event's narrative is empty.</span>
              <% } %>
            </p>
            <span id="event-note-<%= event.Id %>" style="display: none;">
              <% if (event.Notes) { %>
                <%= event.Notes %>
              <% } else { %>
                <span class="muted">No notes on this event.</span>
              <% } %>
            </span>
            <div class="responsibility-body<%= showDetail === true ? "" : " collapse" %>">
              <div class="row-fluid">
                <div class="span4 well well-small"><%= responsibilities %></div>
                <div class="span4 well well-small"><%= othersResponsible %></div>
                <div class="span4 well well-small"><%= actionsTaken %></div>
              </div>
              <%= eventSources %>
            </div>
          </div>
        </div>
      </div>
    """
    responsibilities: _.template """
      <% _(prs).each(function(pr, i) { %>
        <span class="person-responsible" data-id="<%= pr.Id %>" data-selector="#person-responsibility-note-<%= pr.Id %>">
        Function: 
        <% if (pr.PersonFunctionUnitSummary) { %>
          <strong><%= pr.PersonFunctionUnitSummary %></strong>
          <% if (pr.Absent === true) { %>
            <span class="label label-success"><small>ABSENT</small></span>
          <% } %>
          <% if (pr.Nominated === true) { %>
            <span class="label label-success"><small>NOMINATED</small></span>
          <% } %>
          <% if (pr.Defected === true) { %>
            <span class="label label-important"><small>DEFECTED</small></span>
          <% } %>
        <% } else { %>
          <span class="muted">No known function at the time</span>
        <% } %>
        <ul>
          <li><%= _(pr.Violations).map(function(obj) { return "<span class='label label-" + (obj.ConditionalityInterest === true ? "warning" : "inverse") + "' style='margin-right: 5px;'>" + obj.Name + "</span>"; }).join("") %></li>
          <li>
            <% if (pr.Commentary) { %>
              <%= pr.Commentary %>
            <% } else { %>
              <span class='muted'>No commentary</span>
            <% } %>
          </li>
          <li>Responsibility: <%= pr.PersonResponsibilityType.Name %></li>
        </ul>
        </span>
        <span id="person-responsibility-note-<%= pr.Id %>" style="display: none;">
          <% if (pr.Notes) { %>
            <%= pr.Notes %>
          <% } else { %>
            <span class="muted">No notes on this responsibility.</span>
          <% } %>
        </span>
      <% }); %>
    """
    otherResponsibilities: _.template """
      Others responsible:
      <% if (_(prs).size() > 0) { %>
        <ul>
          <% _(prs).each(function(pr, i) { %>
            <li class="other-responsible" data-id="<%= pr.Id %>">
              <a href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= pr.Person.Id %>" target="_blank" data-selector="#responsibility-tooltip-<%= pr.Id %>">
                <%= pr.Person.Name %>
              </a>
              <span id="responsibility-tooltip-<%= pr.Id %>" style="display: none;">
                Function:
                <% if (pr.PersonFunctionUnitSummary) { %>
                  <strong><%= pr.PersonFunctionUnitSummary %></strong>
                <% } else { %>
                  <span class="muted">No known function at the time</span>
                <% } %>
                <ul>
                  <li>
                    <%= _(pr.Violations).map(function(obj) { return "<span class='label label-" + (obj.ConditionalityInterest === true ? "warning" : "inverse") + "' style='margin-right: 5px;'>" + obj.Name + "</span>"; }).join("") %>
                  </li>
                  <li>
                    Commentary:
                    <% if (pr.Commentary) { %>
                      <%= pr.Commentary %>
                    <% } else { %>
                      <span class="muted">No commentary</span>
                    <% } %>
                  </li>
                  <li>Responsibility: <%= pr.PersonResponsibilityType.Name %></li>
                </ul>
              </span>
            </li>
          <% }); %>
        </ul>
      <% } else { %>
        <span class='muted'>No information on others responsible.</span>
      <% } %>
    """
    actionsTaken: _.template """
      <span class="actions-taken" data-event-id="<%= eventId %>"></span>Actions taken:
      <% if (_(ats).size() > 0) { %>
        <ul>
          <% _(ats).each(function(at, i) { %>
            <li class="action-taken" data-id="<%= at.Id %>">
              <span class="action-taken" data-id="<%= at.Id %>"></span>
              <% if (at.StartDate && at.EndDate) { %>
                <%= at.StartDate %> to <%= at.EndDate %>:
              <% } else if (at.StartDate) { %>
                <%= at.StartDate %>:
              <% } else if (at.EndDate) { %>
                <%= at.EndDate %>:
              <% } %>
              <ul>
                <% if (at.SubjectPerson) { %>
                  <li>
                    <a href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= at.SubjectPerson.Id %>" target='_blank'>
                      <%= at.SubjectPerson.Name %>
                    </a>
                  </li>
                <% } %>
                <li>
                  <%= at.ActionTakenType.Name %>
                  <% if (at.ActionTakenType.IsRemedial) { %>
                    <span class="label"><small>REMEDIAL</small></span>
                  <% } %>
                  <% if (at.ActionTakenType.IsDisciplinary) { %>
                    <span class="label"><small>DISCIPLINARY</small></span>
                  <% } %>
                </li>
                <% if (at.ObjectPerson) { %>
                  <li>
                    <a href="<%= Profiling.applicationUrl %>Profiling/Persons/Details/<%= at.ObjectPerson.Id %>" target='_blank'>
                      <%= at.ObjectPerson.Name %>
                    </a>
                  </li>
                <% } %>
                <% if (at.Commentary) { %>
                  <li><%= at.Commentary %></li>
                <% } %>
              </ul>
            </li>
          <% }); %>
        </ul>
      <% } else { %>
        <span class='muted'>No information on actions taken.</span>
      <% } %>
    """
    eventSources: _.template """
      <span class="event-sources" data-event-id="<%= eventId %>"></span>Sources:
      <% if (_(eventSources).size() > 0) { %>
        <ul>
          <% _(eventSources).each(function(es, i) { %>
            <li class="event-source" data-id="<%= es.Id %>" data-selector="#event-source-notes-<%= es.Id %>">
              <a href="<%= Profiling.applicationUrl %>Profiling/Sources#info/<%= es.Source.Id %>" target="_blank"><%= es.Source.Name %></a>
              <% if (es.Source.Archive === true) { %>
                &nbsp;<span class="label"><small>ARCHIVED</small></span>
              <% } %>
              <% if (es.Source.IsRestricted === true) { %>
                &nbsp;<span class="label label-important"><small>RESTRICTED</small></span>
              <% } %>
              /
              <% if (es.Commentary) { %>
                <%= es.Commentary %>
              <% } else { %>
                <span class='muted'>No commentary</span>
              <% } %>
              /
              <% if (es.Reliability) { %>
                <%= es.Reliability.Name %>
              <% } else { %>
                <span class='muted'>No reliability rating</span>
              <% } %>
            </li>
            <span id="event-source-notes-<%= es.Id %>" style="display: none;">
              <% if (es.Notes) { %>
                <%= es.Notes %>
              <% } else { %>
                <span class="muted">No notes on this EventSource.</span>
              <% } %>
            </span>
          <% }); %>
        </ul>
      <% } else { %>
        <span class='muted'>No sources on this event.</span>
      <% } %>
    """

  events:
    "click label.radio": "renderData"
    "change select#responsibility-type-select,select#function-select,select#verified-select": "renderEvents"

  render: ->
    @$el.html @templates.bottom()
    
    _.defer =>
      $("#bottom-row").html @templates.filter
        label: "Filter by responsibility type: "
        select_id: "responsibility-type-select"
        className: "input-small"
        options: @model.getResponsibilityTypes()
      $("#bottom-row").append @templates.filter
        label: "Function: "
        select_id: "function-select"
        className: "input-large"
        options: _.chain(@model.getResponsibilitiesByFunction()).keys().sortBy((k) -> k).value()
      $("#bottom-row").append @templates.filter
        label: "Verified: "
        select_id: "verified-select"
        className: "input-medium"
        options: @model.getVerifiedStatuses()
      $("#bottom-row").append @templates.button()

      @renderData()
    
    @

  renderData: ->
    control = $("input[name=control]:checked").val()
    if control is 'event'
      $("#bottom-row").show()
      $("#stats").show()
      @renderEvents()
    else if control is 'summary'
      $("#bottom-row").hide()
      $("#stats").hide()
      view = new Profiling.Views.PersonDetailsResponsibilitiesSummaryView
        model: @model
      @.$("#records").html view.render().el

  renderEvents: ->
    if _(@model.get('Events')).isEmpty()
      @.$("#records").html "<span class='muted'>No known human rights records.</span>" 
      return

    responsibilitiesByEvent = @model.getResponsibilitiesByEvent()

    # filter by responsibility types
    responsibilityTypeFilter = $("select#responsibility-type-select").val()
    filteredEvents = if responsibilityTypeFilter is "All"
      @model.getSortedEvents()
    else
      _(@model.getSortedEvents()).filter (e) ->
        _(responsibilitiesByEvent[e.Id]).find (pr) ->
          pr.PersonResponsibilityType.Name is responsibilityTypeFilter

    # filter by function
    functionFilter = $("select#function-select").val()
    if functionFilter isnt "All"
      filteredEvents = _(filteredEvents).filter (e) ->
        _(responsibilitiesByEvent[e.Id]).find (pr) ->
          result = if pr.PersonFunctionUnitSummary
            pr.PersonFunctionUnitSummary is functionFilter
          else
            functionFilter is "" or _(functionFilter).isUndefined() or _(functionFilter).isNull()

    # filter by verified status
    verifiedFilter = $("select#verified-select").val()
    if verifiedFilter isnt "All"
      filteredEvents = _(filteredEvents).filter (e) ->
        result = if e.VerifiedStatus
          e.VerifiedStatus is verifiedFilter
        else
          verifiedFilter is "" or _(verifiedFilter).isUndefined() or _(verifiedFilter).isNull()

    # counts for footer
    filteredEventIds = _(filteredEvents).pluck "Id"
    $("#stats").html @templates.stats
      numEvents: _(filteredEvents).size()
      numViolations: _(@model.get('Responsibilities')).reduce (memo, pr) -> 
        memo + (if _(filteredEventIds).contains(pr.EventId) then _(pr.Violations).size() else 0)
      , 0
      numActions: _(filteredEvents).reduce (memo, e) ->
        memo + _(e.ActionsTaken).size()
      , 0

    # build event templates
    items = for event, i in filteredEvents
      eventHeadlineLinks = {}
      eventHeadlineLinks[event.Id] = @templates.eventHeadlineLink({ event: event, labelViolations: true })
      for r in event.RelatedEvents
        eventHeadlineLinks[r.SubjectEvent.Id] = @templates.eventHeadlineLink({ event: r.SubjectEvent, labelViolations: false }) if r.SubjectEvent and r.SubjectEvent.Id isnt event.Id
        eventHeadlineLinks[r.ObjectEvent.Id] = @templates.eventHeadlineLink({ event: r.ObjectEvent, labelViolations: false }) if r.ObjectEvent and r.ObjectEvent.Id isnt event.Id

      @templates.event
        index: i + 1
        showDetail: $("#show-detail-button").hasClass "active"
        event: event
        eventHeadlineLinks: eventHeadlineLinks
        responsibilities: @templates.responsibilities
          prs: responsibilitiesByEvent[event.Id]
        othersResponsible: @templates.otherResponsibilities
          prs: event.OthersResponsible
        actionsTaken: @templates.actionsTaken
          ats: event.ActionsTaken
          eventId: event.Id
        eventSources: @templates.eventSources
          eventSources: event.EventSources
          eventId: event.Id

    @.$("#records").html @templates.accordion
      items: items

    _.defer =>
      Profiling.setupUITooltips "li.other-responsible a"
      Profiling.setupUITooltips "div.related-event"
      Profiling.setupUITooltips "p.event-narrative"
      Profiling.setupUITooltips "span.person-responsible"
      Profiling.setupUITooltips "li.event-source"

      if @options.permissions.canChangeEvents
        $("span.event").each (i, el) ->
          eventId = $(el).data 'event-id'
          editEventView = new Profiling.Views.EventEditFormIconView
            eventId: eventId
            modalId: "modal-event-edit-#{eventId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Events/Edit/#{eventId}"
            modalSaveButton: 'modal-event-edit-save-button'
          $(el).prepend editEventView.render().el

        $("span.action-taken").each (i, el) ->
          actionTakenId = $(el).data 'id'
          deleteButtonView = new Profiling.Views.BaseDeleteButtonView
            title: "Delete Action Taken"
            confirm: "Are you sure you want to remove this action taken for this event?"
            url: "#{Profiling.applicationUrl}Profiling/ActionsTaken/Delete/#{actionTakenId}"
          $(el).prepend deleteButtonView.render().el
          editFormView = new Profiling.Views.ActionTakenEditFormView
            actionTakenId: actionTakenId
            modalId: "modal-action-taken-edit-#{actionTakenId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/ActionsTaken/Edit/#{actionTakenId}"
            modalSaveButton: 'modal-action-taken-edit-button'
          $(el).prepend editFormView.render().el

        $("span.actions-taken").each (i, el) ->
          eventId = $(el).data 'event-id'
          addActionTakenView = new Profiling.Views.ActionTakenAddIconFormView
            eventId: eventId
            modalId: "modal-action-taken-add-#{eventId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{eventId}/ActionsTaken/Add"
            modalSaveButton: 'modal-action-taken-add-button'
          $(el).prepend addActionTakenView.render().el

      if @options.permissions.canChangePersonResponsibilities
        $("span.person-responsible").each (i, el) ->
          prId = $(el).data 'id'
          deleteButtonView = new Profiling.Views.BaseDeleteButtonView
            title: "Delete Person Responsibility"
            confirm: "Are you sure you want to delete this person's responsibility for this event?"
            url: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Delete/#{prId}"
          $(el).prepend deleteButtonView.render().el
          editFormView = new Profiling.Views.PersonResponsibilityEditFormView
            modalId: "modal-person-responsibility-edit-#{prId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Edit/#{prId}"
            modalSaveButton: 'modal-person-responsibility-edit-button'
          $(el).prepend editFormView.render().el

        $("li.other-responsible").each (i, el) ->
          prId = $(el).data 'id'
          deleteButtonView = new Profiling.Views.HumanRightsDeleteButtonView
            title: "Delete Human Rights Responsibility"
            confirm: "Are you sure you want to remove this person's responsibility for this event?"
            url: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Delete/#{prId}"
          $(el).prepend deleteButtonView.render().el
          editFormView = new Profiling.Views.PersonResponsibilityEditFormView
            modalId: "modal-person-responsibility-edit-#{prId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Edit/#{prId}"
            modalSaveButton: 'modal-person-responsibility-edit-button'
          $(el).prepend editFormView.render().el

      $("li.event-source").each (i, el) ->
        esId = $(el).data 'id'
        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Detach Source"
          confirm: "Are you sure you want to detach this source from this event?"
          url: "#{Profiling.applicationUrl}Profiling/EventSources/Delete/#{esId}"
        $(el).prepend deleteButtonView.render().el
        editFormView = new Profiling.Views.EventSourceEditFormView
          modalId: "modal-event-source-edit-#{esId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/EventSources/Edit/#{esId}"
          modalSaveButton: 'modal-event-source-edit-button'
        $(el).prepend editFormView.render().el

      $("span.event-sources").each (i, el) ->
        eventId = $(el).data 'event-id'
        addSourceView = new Profiling.Views.EventAddSourceIconFormView
          eventId: eventId
          modalId: "modal-event-source-add-#{eventId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{eventId}/Sources/Add"
          modalSaveButton: 'modal-event-source-add-button'
        $(el).prepend addSourceView.render().el
