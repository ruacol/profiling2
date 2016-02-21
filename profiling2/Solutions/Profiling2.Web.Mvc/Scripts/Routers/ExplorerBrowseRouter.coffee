Profiling.Routers.ExplorerBrowseRouter = Backbone.Router.extend

  initialize: (opts) ->
    @code = opts.code
    @permissions = opts.permissions

  routes:
    "": "index"
    "path/:path": "path"
    "search": "search"

  index: ->
    @path('')

  path: (path) ->
    if @pathsView
      $("div#paths").show()
      @pathsView.updateSelectOptions(path) if path
      @pathsView.trigger "change:paths"
    else
      @pathsView = new Profiling.Views.ExplorerPathsView
        path: path
        code: @code
        permissions: @permissions
      $("div#paths").html @pathsView.render().el

    if not @browseView
      @browseView = new Profiling.Views.ExplorerBrowseView
        code: @code
        pathsView: @pathsView
      $("div#content").html @browseView.render().el

  search: ->
    $("div#paths").hide()

    view = new Profiling.Views.ExplorerSearchView
      code: @code
      permissions: @permissions
    $("div#content").html view.render().el