Profiling.Views.PersonDetailsTabView = Backbone.View.extend

  templates:
    tabList: _.template """
      <ul class="nav nav-tabs" id="tab-list">
        <li class="active"><a href="#personal-information">Personal Information</a><li>
        <% if (permissions.canViewAndChangePersonRestrictedNotes) { %>
          <li><a href="#restricted-information">Restricted Information</a></li>
        <% } %>
        <% if (permissions.canViewPersonRelationships) { %>
          <li><a href="#relationships">Relationships</a></li>
        <% } %>
        <li><a href="#career">Career</a></li>
        <% if (permissions.canViewPersonResponsibilities) { %>
          <li><a href="#human-rights-record">Human Rights Record</a></li>
        <% } %>
        <% if (permissions.hasScreeningRole) { %>
          <li><a href="#screening-information">Screening Information</a></li>
        <% } %>
        <% if (permissions.canViewPersonResponsibilities && permissions.canViewPersonRelationships) { %>
          <li><a href="<%= Profiling.applicationUrl %>Profiling/Persons/Timeline/<%= id %>">Timeline</a></li>
        <% } %>
      </ul>
    """
    tabContent: _.template """
      <div class="tab-content">
        <div class="tab-pane active" id="personal-information"></div>
        <div class="tab-pane active" id="restricted-information"></div>
        <div class="tab-pane" id="relationships"></div>
        <div class="tab-pane" id="career"></div>
        <div class="tab-pane" id="human-rights-record"></div>
        <div class="tab-pane" id="screening-information"></div>
      </div>
    """

  events:
    "click #tab-list a": "clickTab"

  render: ->
    @$el.html @templates.tabList
      permissions: @options.permissions
      id: @options.id
    @$el.append @templates.tabContent
    @

  clickTab: (e) ->
    $(e.target).tab 'show'