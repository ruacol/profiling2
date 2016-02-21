Profiling.Views.UnitSourcesView = Backbone.View.extend

  templates:
    unitSources: _.template """
      <table class="table">
        <% _.each(unitSources, function(unitSource) { %>
          <tr>
            <td>
              <span class="unit-source" data-id="<%= unitSource.Id %>"></span>
              <a href="<%= Profiling.applicationUrl %>Profiling/Sources#info/<%= unitSource.SourceId %>" target="_blank"><%= unitSource.SourceName %></a>
              /
              <% if (unitSource.Commentary) { %>
                <%= unitSource.Commentary %>
              <% } else { %>
                <span class='muted'>No commentary</span>
              <% } %>
              <%= $(new Profiling.Views.SourceThumbnailView({ model: unitSource }).render().el).html() %>
            </td>
            <td style="white-space: nowrap;">
              <div class="pull-right">
                <% if (unitSource.SourceIsRestricted === true) { %>
                  &nbsp;<span class="label label-important"><small>RESTRICTED</small></span>
                <% } %>
                <% if (unitSource.ReliabilityName) { %>
                  <span class="label" title="Reliability rating"><%= unitSource.ReliabilityName %></span>
                <% } %>
              </div>
            </td>
          </tr>
        <% }); %>
      </table>
    """
    noUnitSources: _.template """
      <div style="margin-bottom: 20px;">
        <span class="muted">There are no unit sources.</span>
      </div>
    """

  render: ->
    _.defer ->
      $("span.unit-source").each (i, el) ->
        unitSourceId = $(el).data 'id'
        editFormView = new Profiling.Views.UnitSourceEditFormView
          modalId: "modal-unit-source-edit-#{unitSourceId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/UnitSources/Edit/#{unitSourceId}"
          modalSaveButton: 'modal-unit-source-edit-button'
        $(el).before editFormView.render().el
        deleteButtonView = new Profiling.Views.UnitSourceDeleteButtonView
          title: "Delete Unit Source"
          confirm: "Are you sure you want to delete this unit source?"
          url: "#{Profiling.applicationUrl}Profiling/UnitSources/Delete/#{unitSourceId}"
        $(el).before deleteButtonView.render().el

    unitSourcesTemplate = if _.isEmpty(@model.get 'UnitSources')
      @templates.noUnitSources()
    else
      @templates.unitSources
        unitSources: @model.get 'UnitSources'

    @$el.html unitSourcesTemplate

    @