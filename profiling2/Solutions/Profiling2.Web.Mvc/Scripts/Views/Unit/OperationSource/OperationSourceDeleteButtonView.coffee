Profiling.Views.OperationSourceDeleteButtonView = Profiling.Views.BaseDeleteButtonView.extend

  successfulDeleteCallback: ->
    window.location.reload()
