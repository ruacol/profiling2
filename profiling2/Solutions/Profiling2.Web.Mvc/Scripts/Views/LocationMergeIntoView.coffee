Profiling.Views.LocationMergeIntoView = Backbone.View.extend

  templates:
    heading: _.template """
      <p><strong>Location to delete</strong></p>
      <div class="form-inline">
        <input type="text" placeholder="Enter location ID..." id="toDeleteId" />
        <button type="button" class="btn" id="getLocationButton">Get location</button>
      </div>
      <div id="location-result"></div>
    """
    location: _.template """
      <dl class="dl-horizontal">
        <% if (data.Name) { %>
          <dt>Name</dt>
          <dd><%= data.Name %></dd>
        <% } %>
        <% if (data.Town) { %>
          <dt>Town</dt>
          <dd><%= data.Town %></dd>
        <% } %>
        <% if (data.Territory) { %>
          <dt>Territory</dt>
          <dd><%= data.Territory %></dd>
        <% } %>
        <% if (data.Region) { %>
          <dt>Pre-2015 Province</dt>
          <dd><%= data.Region %></dd>
        <% } %>
        <% if (data.Province) { %>
          <dt>Province</dt>
          <dd><%= data.Province %></dd>
        <% } %>
        <% if (data.Latitude) { %>
          <dt>Latitude</dt>
          <dd><%= data.Latitude %></dd>
        <% } %>
        <% if (data.Longitude) { %>
          <dt>Longitude</dt>
          <dd><%= data.Longitude %></dd>
        <% } %>
        <% if (data.Notes) { %>
          <dt>Notes</dt>
          <dd><%= data.Notes %></dd>
        <% } %>
        <% if (data.NumEvents) { %>
          <dt>Events</dt>
          <dd><%= data.NumEvents %></dd>
        <% } %>
        <% if (data.NumCareers) { %>
          <dt>Careers</dt>
          <dd><%= data.NumCareers %></dd>
        <% } %>
        <% if (data.NumUnitLocations) { %>
          <dt>Unit Locations</dt>
          <dd><%= data.NumUnitLocations %></dd>
        <% } %>
      </dl>
    """

  events:
    "click #getLocationButton": "getLocation"

  render: ->
    @$el.html @templates.heading

    @

  getLocation: ->
    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Locations/Json/#{$('#toDeleteId').val()}"
      type: "GET"
      beforeSend: ->
        $("#location-result").html "Getting location..."
      success: (data, textStatus, xhr) =>
        if data
          $("#location-result").html @templates.location
            data: data
          $("#ToDeleteLocationId").val data.Id
      error: (xhr, textStatus) ->
        bootbox.alert xhr.responseText