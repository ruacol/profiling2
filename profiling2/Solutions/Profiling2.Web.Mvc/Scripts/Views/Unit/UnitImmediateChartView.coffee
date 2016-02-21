Profiling.Views.UnitImmediateChartView = Backbone.View.extend

  render: ->
    data = @getData() if google?
    if data and data.getNumberOfRows() > 0
      _.defer () =>
        if google?
          chart = new google.visualization.OrgChart document.getElementById("chart-hierarchy")
          chart.draw data,
            allowHtml: true
            allowCollapse: true
        
        $("i.icon-trash.delete-relationship").click (e) =>
          bootbox.confirm "This unit relationship will be deleted.  Continue?", (response) =>
            if response is true
              window.location.href = "#{Profiling.applicationUrl}Profiling/Units/#{@options.unitId}/Hierarchies/Delete/#{$(e.target).data "id"}"
    @

  getData: ->
    data = new google.visualization.DataTable()
    data.addColumn 'string', 'Unit'
    data.addColumn 'string', 'Parent'

    dataArray = []

    for i in @options.parentHierarchyModels
      dataArray.push [ { v: "#{i.parentUnitId}", f: @displayUnit(i.id, i.parentUnitId, i.parentUnitName) }, null ]
      dataArray.push [ { v: "#{i.unitId}", f: "<strong>#{i.unitName}</strong>" }, "#{i.parentUnitId}" ]
    for j in @options.childHierarchyModels
      dataArray.push [ { v: "#{j.unitId}", f: @displayUnit(j.id, j.unitId, j.unitName) }, "#{j.parentUnitId}" ]

    thisUnit = _(dataArray).find((x) => x[0].v is "#{@options.unitId}")
    if not thisUnit
      dataArray.push [ { v: "#{@options.unitId}", f: "<strong>#{$("h2").text().replace($("h2 small").text(), "")}</strong>" }, null ]

    data.addRows dataArray
    data

  displayUnit: (uhId, unitId, unitName) ->
    html = if unitId and unitName
      x = """
        <a href="#{unitId}" target="_blank">#{unitName}</a>
      """
      if @options.permissions.canChangeUnits
        x += """
          <br />
          <a href="#{Profiling.applicationUrl}Profiling/Units/#{@options.unitId}/Hierarchies/Edit/#{uhId}"><i class="accordion-toggle icon-pencil" title="Edit Relationship" style="margin-right: 5px;"></i></a> 
          <i class="accordion-toggle icon-trash delete-relationship" title="Delete Relationship" style="margin-right: 5px;" data-id="#{uhId}"></i>
        """
      x
    else
      ""
