Profiling.Routers.RequestTableRouter = Backbone.Router.extend

  initialize: (opts) ->
    @dataTablesAjaxSource = opts.dataTablesAjaxSource
    @dataTablesActionUrl = opts.dataTablesActionUrl
    @showLastUpdate = if opts.showLastUpdate
      opts.showLastUpdate
    else
      false

  routes:
    "": "index"

  index: ->
    tableView = new Profiling.Views.RequestDataTablesView
      dataTablesAjaxSource: @dataTablesAjaxSource
      dataTablesActionUrl: @dataTablesActionUrl
      showLastUpdate: @showLastUpdate
    $("#requests-table-div").html tableView.render().el