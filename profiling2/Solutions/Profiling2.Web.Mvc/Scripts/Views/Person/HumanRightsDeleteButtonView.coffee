Profiling.Views.HumanRightsDeleteButtonView = Profiling.Views.BaseDeleteButtonView.extend

  successfulDeleteCallback: ->
    # null fragment so we can reload the same URL
    fragment = Backbone.history.fragment
    Backbone.history.fragment = null
    Backbone.history.navigate fragment, true