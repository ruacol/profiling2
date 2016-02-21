Profiling.Routers.PersonsRouter = Backbone.Router.extend

  initialize: (opts) ->
    @permissions = opts.permissions

  routes:
    "": "index"
    "legacy": "legacy"
    "lucene": "index"

  index: ->
    tableView = new Profiling.Views.PersonDataTablesLuceneView
    $("div#search-table").html tableView.render().el

    if @permissions.canChangePersons
      @renderPersonCreate() 

    $("input[name=search-radio]").change () ->
      if $(@).val() is "legacy"
        Backbone.history.navigate "legacy", true
        $("#legacy-help").show()
        $("#lucene-help").hide()
      else if $(@).val() is "lucene"
        Backbone.history.navigate "lucene", true
        $("#legacy-help").hide()
        $("#lucene-help").show()

  legacy: ->
    tableView = new Profiling.Views.PersonDataTablesView
    $("div#search-table").html tableView.render().el

    if @permissions.canChangePersons
      @renderPersonCreate()

  renderPersonCreate: ->
    createView = new Profiling.Views.PersonCreateFormView
      modalId: 'modal-person-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/Create"
      modalSaveButton: 'modal-person-create-save-button'
    $("#person-menu").prepend createView.render().el