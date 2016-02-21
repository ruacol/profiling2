Profiling.Views.PersonDetailsResponsibilitiesSummaryView = Backbone.View.extend

  templates: 
    row: _.template """
      <div class="row-fluid">
        <p class="span12">Breakdown of violations by those categorized as HRDDP-relevant on the left, and the rest on the right.</p>
      </div>
      <div class="row-fluid">
        <%= contents %>
      </div>
    """
    top: _.template """
      <div class="accordion <%= spanClass %>">
        <% _(accordions).each(function(accordion, i) { %>
          <div class="accordion-group">
            <%= accordion %>
          </div>
        <% }); %>
      </div>
    """
    accordion: _.template """
      <div class="accordion-heading">
        <div class="accordion-toggle">
          <h5>
            <%= title %>
            <span class="badge pull-right<%= (conditionalityInterest === true ? " badge-warning" : "") %>">
              <%= _(contents).reduce(function(memo, num) { return memo + num; }) || "0" %>
            </span>
          </h5>
        </div>
      </div>
      <div class="accordion-body">
        <table class="table accordion-inner">
          <% _(_.keys(contents)).each(function(key, i) { %>
            <tr>
              <td><%= key %></td>
              <td><span class="badge pull-right<%= (conditionalityInterest === true ? " badge-warning" : "") %>"><%= contents[key] %></span></td>
            </tr>
          <% }); %>
        </table>
      </div>
    """

  render: ->
    eventTable = @templates.accordion
      title: "Events"
      contents: _(@model.getResponsibilitiesByType()).mapObject (val, key) ->
        _.chain(val).pluck("EventId").uniq().size().value()
      conditionalityInterest: false

    @$el.html @templates.top
      spanClass: "span4"
      accordions: [ eventTable ]

    violationsTable = @templates.accordion
      title: "Violations"
      contents: _(@model.getResponsibilitiesByType()).mapObject (val, key) ->
        _.chain(val).pluck("Violations").reduce((memo, violations) ->
          memo + _(violations).size()
        , 0).value()
      conditionalityInterest: false

    @$el.append @templates.top
      spanClass: "span4"
      accordions: [ violationsTable ]

    actionsTable = @templates.accordion
      title: "Actions"
      contents:
        "Remedial": _.chain(@model.getActionsTaken()).filter((item) -> item.ActionTakenType.IsRemedial is true).size().value()
        "Disciplinary": _.chain(@model.getActionsTaken()).filter((item) -> item.ActionTakenType.IsDisciplinary is true).size().value() 
        "Other": _.chain(@model.getActionsTaken()).filter((item) -> item.ActionTakenType.IsDisciplinary is false and item.ActionTakenType.IsRemedial is false).size().value()
      conditionalityInterest: false

    @$el.append @templates.top
      spanClass: "span4"
      accordions: [ actionsTable ]

    columns = for x in [ true, false ]
      violationCounts = @model.getViolationCounts x
      tables = for type in _(violationCounts).keys()
        total = _(violationCounts[type]).reduce (memo, num) -> 
          memo + num
        , 0
        @templates.accordion
          title: type
          contents: violationCounts[type]
          conditionalityInterest: x

      @templates.top
        spanClass: "span6"
        accordions: tables

    @$el.append @templates.row
      contents: columns.join ""

    @