Profiling.Views.MapView = Backbone.View.extend

  id: "map"
  tagName: "div"

  templates:
    attributionMapQuest: _.template """
      Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>
      | Tiles Courtesy of <a href="http://www.mapquest.com/" target="_blank">MapQuest</a> <img src="http://developer.mapquest.com/content/osm/mq_logo.png">
    """
    attributionMapQuestOpenAerial: " | Portions Courtesy NASA/JPL-Caltech and U.S. Depart. of Agriculture, Farm Service Agency"

  render: ->
    @$el.css 'height', "#{@options.height}px"
    @$el.addClass @options.className if @options.className

    _.defer =>
      mapquest = L.tileLayer 'http://{s}.mqcdn.com/tiles/1.0.0/map/{z}/{x}/{y}.png',
        attribution: @templates.attributionMapQuest()
        maxZoom: 18
        subdomains: [ 'otile1', 'otile2', 'otile3', 'otile4' ]
      satellite = L.tileLayer 'http://{s}.mqcdn.com/tiles/1.0.0/sat/{z}/{x}/{y}.png',
        attribution: "#{@templates.attributionMapQuest()}#{@templates.attributionMapQuestOpenAerial}"
        maxZoom: 18
        subdomains: [ 'otile1', 'otile2', 'otile3', 'otile4' ]

      @map = L.map 'map'
      mapquest.addTo @map
      L.control.layers({ "MapQuest": mapquest, "MapQuest Open Aerial": satellite }, null).addTo @map

      L.control.scale().addTo @map

      if @options.latitude and @options.longitude
        @map.setView [ @options.latitude, @options.longitude ], 7
      else
        @map.setView [ -2.9814, 23.8221 ], 5  # Kinshasa

    @