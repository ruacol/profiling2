Profiling.Routers.SearchResponsesRouter = Backbone.Router.extend

  initialize: (opts) ->
    @screeningEntityName = opts.screeningEntityName

  routes:
    "": "index"

  index: ->
    tableView = new Profiling.Views.SearchResponsesDataTablesView
      screeningEntityName: @screeningEntityName
    $("#responses-table-div").html tableView.render().el
