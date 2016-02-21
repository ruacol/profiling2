Profiling.Views.LocationEditFormView = Profiling.Views.BaseModalForm.extend

  modalWidth: '90%'

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-pencil" style="margin-right: 5px;" title="Edit Location"></i>
    """

  events:
    "click i.icon-pencil": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  displayModalForm: ->
    if $("#{@options.parentDivId} #LocationId").val()
      Profiling.Views.BaseModalForm.prototype.displayModalForm.call @, arguments

  modalLoadedCallback: ->
    mapView = new Profiling.Views.MapView
      height: 400
      latitude: $("#Latitude").val()
      longitude: $("#Longitude").val()
    $("#location-map-container").html mapView.render().el

    _.defer ->
      #var marker = L.marker([@Model.Latitude.Value, @Model.Longitude.Value]).addTo(mapView.map);

      new L.Control.GeoSearch
        provider: new L.GeoSearch.Provider.OpenStreetMap()
        showMarker: true
      .addTo mapView.map

      mapView.map.on 'click', (e) ->
        popup = L.popup()
          .setLatLng(e.latlng)
          .setContent("<strong>Latitude:</strong> #{e.latlng.lat}<br /><strong>Longitude:</strong> #{e.latlng.lng}")
          .openOn(mapView.map)

  formSubmittedSuccessCallback: ->
    $("#{@options.parentDivId} #LocationId").select2 "val", $("#{@options.parentDivId} #LocationId").select2("val")