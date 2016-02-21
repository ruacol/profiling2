Profiling.Routers.EventIndexRouter = Backbone.Router.extend

  initialize: (opts) ->
    @permissions = opts.permissions

  routes:
    "": "index"

  index: ->
    if @permissions.canChangeEvents
      createView = new Profiling.Views.EventCreateFormView
        modalId: 'modal-event-create'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Events/Create"
        modalSaveButton: 'modal-event-create-button'
      $("#event-menu").prepend createView.render().el

    tableView = new Profiling.Views.EventDataTablesView
    $("h2").after tableView.render().el
