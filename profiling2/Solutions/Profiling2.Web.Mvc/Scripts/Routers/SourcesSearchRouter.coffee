Profiling.Routers.SourcesSearchRouter = Backbone.Router.extend

  initialize: (opts) ->
    $("#source-search").css "height", "#{$(window).height()/2}px"
    $("#source-preview").css "height", "#{$(window).height()/2}px"
    @permissions = opts.permissions

  routes:
    "": "index"
    "info/:id": "info"
    "info/:id/": "info"
    "info/:id/:adminSourceSearchId": "info"
    "preview/:id": "preview"
    "preview/:id/": "preview"
    "preview/:id/:adminSourceSearchId": "preview"

  index: ->
    if not $("#source-search").html()
      searchView = new Profiling.Views.SourceSearchFormView
      $("#source-search").html searchView.render().el

  preview: (id, adminSourceSearchId) ->
    sourceModel = new Profiling.Models.SourceModel
      id: id
      adminSourceSearchId: adminSourceSearchId
    sourceModel.fetch
      success: ->
        sourcePreviewView = new Profiling.Views.SourcePreviewView
          model: sourceModel
        $("#source-preview").html sourcePreviewView.render().el
      error: (model, xhr, options) =>
        $("#source-preview").html xhr.responseText

  info: (id, adminSourceSearchId) ->
    sourceModel = new Profiling.Models.SourceModel
      id: id
      adminSourceSearchId: adminSourceSearchId
    sourceModel.fetch
      success: =>
        sourceView = new Profiling.Views.SourceView
          model: sourceModel
          permissions: @permissions
        $("#source-preview").html sourceView.render().el
      error: (model, xhr, options) =>
        $("#source-preview").html xhr.responseText