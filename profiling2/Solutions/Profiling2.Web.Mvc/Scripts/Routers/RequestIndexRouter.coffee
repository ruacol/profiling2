Profiling.Routers.RequestIndexRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    tableView = new Profiling.Views.RequestDataTablesLuceneView()
    $("#requests-table-div").html tableView.render().el
