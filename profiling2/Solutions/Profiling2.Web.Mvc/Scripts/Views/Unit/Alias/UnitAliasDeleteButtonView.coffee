Profiling.Views.UnitAliasDeleteButtonView = Profiling.Views.BaseDeleteButtonView.extend

  successfulDeleteCallback: ->
    window.location.reload()
