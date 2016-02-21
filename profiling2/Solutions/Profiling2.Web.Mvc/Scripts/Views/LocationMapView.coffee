Profiling.Views.LocationMapView = Backbone.View.extend

  templates:
    popupTable: _.template """
      <table class="table table-condensed">
        <thead>
          <tr><th colspan="2"><a href='<%= Profiling.applicationUrl %>Profiling/Locations/Details/<%= loc.Id %>' target='_blank'><%= loc.Name %></a></th></tr>
        </thead>
        <tbody>
          <tr><th>Town</th><td><%= loc.Town %></td></tr>
          <tr><th>Territory</th><td><%= loc.Territory %></td></tr>
          <tr><th>Pre-2015 Province</th><td><%= loc.Region %></td></tr>
          <tr><th>Province</th><td><%= loc.Province %></td></tr>
          <tr><th>Latitude</th><td><%= loc.Latitude %></td></tr>
          <tr><th>Longitude</th><td><%= loc.Longitude %></td></tr>
          <tr><th>Notes</th><td><%= loc.Notes %></td></tr>
        </tbody>
      </table>
    """
    total: _.template """
      <p>There are <%= total %> locations with coordinates.</p>
      <hr />
    """

  render: ->
    mapView = new Profiling.Views.MapView
      height: 600
      className: "span12"
    @$el.html mapView.render().el

    _.defer =>
      $.ajax
        url: "#{Profiling.applicationUrl}Profiling/Locations/GetLocationsWithCoords"
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
                title: loc.LocationName
                riseOnHover: true
              marker.addTo mapView.map

              marker.bindPopup @templates.popupTable
                loc: loc

            mapView.map.fitBounds latlngs
        error: (xhr, textStatus) ->
          bootbox.alert xhr.responseText

    @