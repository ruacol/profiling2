Profiling.Routers.FeedingIndexRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    view = new Profiling.Views.FeedingIndexDataTablesView
    $("#feeding-sources").html view.render().el
