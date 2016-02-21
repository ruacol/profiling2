Profiling.Routers.MatchPersonsRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    $(".to-be-matched").each (i, el) ->
      view = new Profiling.Views.MatchPersonView
        row: el
        rowNumber: i
      $("td:last-child", el).html view.render().el