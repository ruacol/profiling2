Profiling.Views.CombinedLocationsMapView = Backbone.View.extend

  templates:
    popupTable: _.template """
      <h5>
        <a href='<%= Profiling.applicationUrl %>Profiling/Locations/Details/<%= loc.Id %>' target='_blank'><%= loc.Name %></a>
      </h5>
      <ul>
        <% _(loc.Dates).each(function(el) { %>
          <li>
            <% if (el.StartDateString) { %>
              <%= el.StartDateString %>
            <% } else if (el.AsOfDateString) { %>
              <%= el.AsOfDateString %>
            <% } %>
            <% if (el.EndDateString) { %>
              to <%= el.EndDateString %>
            <% } %>
          </li>
        <% }); %>
      </ul>
    """
    total: _.template """
      <p>There are <%= total %> locations with coordinates.</p>
      <hr />
    """

  initialize: (opts) ->
    @unitId = opts.unitId

  render: ->
    mapView = new Profiling.Views.MapView
      height: 600
      className: "span12"
    @$el.html mapView.render().el

    _.defer =>
      $.ajax
        url: "#{Profiling.applicationUrl}Profiling/Units/GetCombinedLocations/#{@unitId}"
        type: "GET"
        success: (data, textStatus, xhr) =>
          if data
            latlngs = []

            locs = _(data).select (x) -> x.Latitude isnt 0 and x.Longitude isnt 0
            @$el.before @templates.total
              total: _(locs).size()

            for loc in locs
              latlng = L.latLng loc.Latitude, loc.Longitude
              latlngs.push latlng
              marker = L.marker latlng,
                title: loc.Name
                riseOnHover: true
              marker.addTo mapView.map

              loc.Dates = _(loc.Dates).sortBy (x) ->
                if x.StartDateString
                  x.StartDateString
                else if x.AsOfDateString
                  x.AsOfDateString
                else
                  x.EndDateString
              .reverse()

              marker.bindPopup @templates.popupTable
                loc: loc

            mapView.map.fitBounds latlngs
        error: (xhr, textStatus) ->
          bootbox.alert xhr.responseText

    @