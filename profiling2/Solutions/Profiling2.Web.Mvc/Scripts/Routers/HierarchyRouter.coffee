Profiling.Routers.HierarchyRouter = Backbone.Router.extend

  initialize: (opts) ->
    @addParent = opts.addParent
    @addChild = opts.addChild

  routes:
    "": "index"

  index: ->
    if @addParent
      new Profiling.BasicSelect
        el: "#ParentUnitId"
        placeholder: "Search by unit ID or name..."
        nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
        getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"
    if @addChild
      new Profiling.BasicSelect
        el: "#UnitId"
        placeholder: "Search by unit ID or name..."
        nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
        getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"
