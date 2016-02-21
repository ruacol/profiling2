Profiling.Views.UnitChartDetailsView = Backbone.View.extend

  render: ->
    data = @getData() if google?
    if data and data.getNumberOfRows() > 0
      @$el.html """
        <h3>#{@options.hierarchyType}</h3>
        <div id="chart-#{@options.hierarchyType}"></div>
      """
      _.defer () =>
        if google?
          chart = new google.visualization.OrgChart document.getElementById("chart-#{@options.hierarchyType}")
          chart.draw data,
            allowHtml: true
            allowCollapse: true
    @

  getData: ->
    data = new google.visualization.DataTable()
    data.addColumn 'string', 'Name'
    data.addColumn 'string', 'ParentUnitId'
    data.addColumn 'string', 'Commanders'

    dataArray = []

    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Units/Hierarchy/#{@options.unitId}"
      data:
        hierarchyType: @options.hierarchyType
      async: false
      success: (data, textStatus, xhr) =>
        if data
          for i in data.UnitHierarchies
            dataArray.push [ { v: "#{i.ParentUnitId}", f: @displayParentUnit(i) }, null, null ]
            dataArray.push [ { v: "#{i.UnitId}", f: @displayUnit(i) }, "#{i.ParentUnitId}", @commandersTooltip(i.Commanders, i.DeputyCommanders) ]
          for j in data.UnitHierarchyChildren
            dataArray = @transformUnitHierarchyChild dataArray, j

          if _(data.UnitHierarchies).size() > 0 or _(data.UnitHierarchyChildren).size() > 0
            thisUnit = _(dataArray).find((x) => x[0].v is "#{@options.unitId}")
            if not thisUnit
              dataArray.push [ { v: "#{@options.unitId}", f: "<strong>#{$("h2").text()}</strong>" }, null, null ]
      error: (xhr, textStatus) ->
        bootbox.alert xhr.responseText

    data.addRows dataArray
    data

  displayParentUnit: (vm) ->
    html = if vm
      x = """
        <a href="#{vm.ParentUnitId}" target="_blank">#{vm.ParentUnitName}</a>
        <div style='white-space: nowrap;'>
          #{vm.NumCareers} careers
        </div>
      """
      x
    else
      ""

  displayUnit: (uhViewModel) ->
    html = if uhViewModel
      x = """
        <a href="#{uhViewModel.UnitId}" target="_blank">#{uhViewModel.UnitName}</a>
        <div style='white-space: nowrap;'>
          #{uhViewModel.NumCareers} careers
        </div>
      """
      #if uhViewModel.YearAsOf > 0
      #  x = x + """
      #    <div style='color: blue; white-space: nowrap;'>
      #      As Of Date: #{uhViewModel.AsOfDate}
      #    </div>
      #  """
      #if uhViewModel.YearOfStart > 0
      #  x = x + """
      #    <div style='color: blue; white-space: nowrap;'>
      #      Start Date: #{uhViewModel.StartDate}
      #    </div>
      #  """
      #if uhViewModel.YearOfEnd > 0
      #  x = x + """
      #    <div style='color: blue; white-space: nowrap;'>
      #      End Date: #{uhViewModel.EndDate}
      #    </div>
      #  """
      #if uhViewModel.LocationName
      #  x = x + """
      #    <div style='color: blue; white-space: nowrap;'>
      #      Location: #{uhViewModel.LocationName}
      #    </div>
      #  """
      x
    else
      ""

  transformUnitHierarchyChild: (collection, child) ->
    if child
      collection.push [ { v: "#{child.UnitId}", f: @displayUnit(child) }, "#{child.ParentUnitId}", @commandersTooltip(child.Commanders, child.DeputyCommanders) ]
      for i in child.Children
        @transformUnitHierarchyChild collection, i

    collection

  commandersTooltip: (commanders, deputies) ->
    items = []
    if commanders
      for c in commanders
        items.push "Commander: #{c}"
    if deputies
      for d in deputies
        items.push "Deputy: #{d}"
    items.join ", "