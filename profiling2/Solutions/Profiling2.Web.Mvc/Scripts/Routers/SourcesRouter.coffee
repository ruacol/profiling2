Profiling.Routers.SourcesRouter = Backbone.Router.extend

  initialize: (opts) ->
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
    @setSourceSearchView()

  setSourceSearchView: ->
    @sourceSearchView = new Profiling.Views.SourceSearchView()
    $("#source-search").html @sourceSearchView.render().el

  info: (id, adminSourceSearchId) ->
    if not @sourceSearchView
      @setSourceSearchView()
      _.defer () =>
        $(@sourceSearchView.el).trigger 'activateAccordionPreview'
    @sourceModel = new Profiling.Models.SourceModel
      id: id
      adminSourceSearchId: adminSourceSearchId
    @sourceModel.fetch
      success: =>
        sourceView = new Profiling.Views.SourceView
          model: @sourceModel
          permissions: @permissions
        $("#accordion-preview-body").html sourceView.render().el
      error: (model, xhr, options) =>
        $("#accordion-preview-body").html xhr.responseText

  preview: (id, adminSourceSearchId) ->
    if not @sourceSearchView
      @setSourceSearchView()
      _.defer () =>
        $(@sourceSearchView.el).trigger 'activateAccordionPreview'
    @sourceModel = new Profiling.Models.SourceModel
      id: id
      adminSourceSearchId: adminSourceSearchId
    @sourceModel.fetch
      success: =>
        sourcePreviewView = new Profiling.Views.SourcePreviewView
          model: @sourceModel
        $("#accordion-preview-body").html sourcePreviewView.render().el
      error: (model, xhr, options) =>
        $("#accordion-preview-body").html xhr.responseText