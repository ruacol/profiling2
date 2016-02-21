Profiling.Routers.UnitChartRouter = Backbone.Router.extend

  initialize: (opts) ->
    @unitId = opts.unitId

  routes:
    "": "index"

  index: ->
    for type in [ 'Unknown', 'DSRSG', 'Force', 'Operation', 'Hierarchy' ]
      view = new Profiling.Views.UnitChartDetailsView
        unitId: @unitId
        hierarchyType: type
      $("#chart_div").append view.render().el
