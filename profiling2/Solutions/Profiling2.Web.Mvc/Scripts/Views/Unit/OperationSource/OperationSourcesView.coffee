Profiling.Views.OperationSourcesView = Backbone.View.extend

  templates:
    operationSources: _.template """
      <table class="table">
        <% _.each(operationSources, function(operationSource) { %>
          <tr>
            <td>
              <span class="operation-source" data-id="<%= operationSource.Id %>"></span>
              <a href="<%= Profiling.applicationUrl %>Profiling/Sources#info/<%= operationSource.SourceId %>" target="_blank"><%= operationSource.SourceName %></a>
              /
              <% if (operationSource.Commentary) { %>
                <%= operationSource.Commentary %>
              <% } else { %>
                <span class='muted'>No commentary</span>
              <% } %>
              <%= $(new Profiling.Views.SourceThumbnailView({ model: operationSource }).render().el).html() %>
            </td>
            <td style="white-space: nowrap;">
              <div class="pull-right">
                <% if (operationSource.SourceIsRestricted === true) { %>
                  &nbsp;<span class="label label-important"><small>RESTRICTED</small></span>
                <% } %>
                <% if (operationSource.ReliabilityName) { %>
                  <span class="label" title="Reliability rating"><%= operationSource.ReliabilityName %></span>
                <% } %>
              </div>
            </td>
          </tr>
        <% }); %>
      </table>
    """
    noOperationSources: _.template """
      <div style="margin-bottom: 20px;">
        <span class="muted">There are no operation sources.</span>
      </div>
    """

  render: ->
    _.defer ->
      $("span.operation-source").each (i, el) ->
        operationSourceId = $(el).data 'id'
        editFormView = new Profiling.Views.OperationSourceEditFormView
          modalId: "modal-operation-source-edit-#{operationSourceId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/OperationSources/Edit/#{operationSourceId}"
          modalSaveButton: 'modal-operation-source-edit-button'
        $(el).before editFormView.render().el
        deleteButtonView = new Profiling.Views.OperationSourceDeleteButtonView
          title: "Delete Operation Source"
          confirm: "Are you sure you want to delete this operation source?"
          url: "#{Profiling.applicationUrl}Profiling/OperationSources/Delete/#{operationSourceId}"
        $(el).before deleteButtonView.render().el

    operationSourcesTemplate = if _.isEmpty(@model.get 'OperationSources')
      @templates.noOperationSources()
    else
      @templates.operationSources
        operationSources: @model.get 'OperationSources'

    @$el.html operationSourcesTemplate

    @