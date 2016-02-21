class Profiling.DataTable

  @defaultOptions =
    bDeferRender: true
    bProcessing: true
    bStateSave: true
    bServerSide: true
    bFilter: true
    sDom: 'T<"clear">lftipr'
    sPaginationType: 'full_numbers'
    aLengthMenu: [[10, 25, 50, 100, 200, 500, 1000], [10, 25, 50, 100, 200, 500, 1000]]
    bAutoWidth: false
    fnServerData: (sSource, aoData, fnCallback) ->
      $.ajax
        dataType: 'json'
        type: 'POST'
        url: sSource
        data: aoData
        success: fnCallback
    fnInitComplete: (oSettings, json) ->
      # Place 'processing' message in centre of window, not centre of table
      $(".dataTables_processing").css
        position: 'fixed'
        top: "#{$(window).height()/2}px"
        left: "#{$(window).width()/2}px"

  constructor: (tableId, opts) ->
    @dataTable = $("##{tableId}").dataTable($.extend({}, Profiling.DataTable.defaultOptions, opts)).fnFilterOnReturn()

  fnDraw: ->
    @dataTable.fnDraw()

  fnReloadAjax: ->
    @dataTable.fnReloadAjax()

  fnSettings: ->
    @dataTable.fnSettings()

  oScroller: ->
    @fnSettings().oScroller