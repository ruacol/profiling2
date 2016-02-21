Profiling.Views.UnitSourceDeleteButtonView = Profiling.Views.BaseDeleteButtonView.extend

  successfulDeleteCallback: ->
    window.location.reload()
