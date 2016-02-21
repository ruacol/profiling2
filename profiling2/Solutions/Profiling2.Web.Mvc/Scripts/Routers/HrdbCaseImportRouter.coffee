Profiling.Routers.HrdbCaseImportRouter = Backbone.Router.extend

  initialize: (opts) ->
    @id = opts.id
    @numPerpetrators = opts.numPerpetrators

    # existing event
    new Profiling.BasicSelect
      el: "#EventId"
      placeholder: 'Search event...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Events/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Get"
    
    # new event
    new Profiling.MultiSelect
      el: "#Event_ViolationIds"
      placeholder: 'Search by category name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Events/Violations/0"

    new Profiling.MultiSelect
      el: "#Event_TagIds"
      placeholder: 'Search by tag name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Tags/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Tags/Get/"

    new Profiling.BasicSelect
      el: "#Event_LocationId"
      placeholder: "Search by location name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Locations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Locations/Get"

    new Profiling.MultiSelect
      el: "#Event_JhroCaseIds"
      placeholder: 'Search by case code...'
      nameUrl: "#{Profiling.applicationUrl}Hrdb/Cases/Name/"
      getUrl: "#{Profiling.applicationUrl}Hrdb/Cases/Get/"

    # perpetrator responsibilities
    for i in [0..@numPerpetrators]
      new Profiling.BasicSelect
        el: "#HrdbPerpetrators_#{i}__PersonId"
        placeholder: 'Search persons...'
        nameUrl: "#{Profiling.applicationUrl}Profiling/Persons/Name/"
        getUrl: "#{Profiling.applicationUrl}Profiling/Persons/Get"
      new Profiling.BasicSelect
        el: "#HrdbPerpetrators_#{i}__OrganizationId"
        placeholder: 'Search organizations...'
        nameUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Name/"
        getUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Get"
      new Profiling.MultiSelect
        el: "#HrdbPerpetrators_#{i}__ViolationIds"
        placeholder: 'Search violations...'
        nameUrl: "#{Profiling.applicationUrl}Profiling/Violations/Name/"
        getUrl: "#{Profiling.applicationUrl}Profiling/Events/Violations/0"

    model = new Profiling.Models.PotentialMatchingEventsModel
      id: @id
    model.fetch
      beforeSend: ->
        $("#matches").html "<span class='muted'>Loading potential matching events...</span>"
      success: ->
        view = new Profiling.Views.PotentialMatchingEventsView
          model: model
        $("#matches").html view.render().el
      error: (model, xhr, options) ->
        bootbox.alert xhr.responseText
        $("#matches").html "<span class='help-inline'>No potential matching events detected.</span>"

  routes:
    "": "index"

  index: ->
