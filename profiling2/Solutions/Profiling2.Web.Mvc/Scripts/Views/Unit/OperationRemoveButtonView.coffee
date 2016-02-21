Profiling.Views.OperationRemoveButtonView = Profiling.Views.BaseDeleteButtonView.extend

  successfulDeleteCallback: ->
    window.location.reload()
