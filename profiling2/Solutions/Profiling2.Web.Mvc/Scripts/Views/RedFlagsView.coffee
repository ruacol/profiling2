Profiling.Views.RedFlagsView = Backbone.View.extend

  id: "red-flags-button"
  className: "btn btn-mini"
  tagName: "button"

  events:
    "click": "displayModal"

  templates:
    modal: _.template """
      <div id="modal-red-flags-<%= id %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>
          <h4>Red Flags</h4>
        </div>
        <div class='modal-body' id="modal-body-red-flags-<%= id %>"></div>
        <div class="modal-footer">
          <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
        </div>
      </div>
    """
    flags: _.template """
      <h4><%= priority %> Priority</h4>
      <table id="flags-table-<%= priority %>" class="table table-bordered">
        <thead>
          <tr>
            <th>Type</th><th>Summary</th>
          </tr>
        </thead>
        <tbody>
          <% $(flags).each(function(i, el) { %>
            <tr>
              <td><%= el.Type %></td>
              <td>
                <strong><%= el.Summary %></strong>
                <p><%= el.Text %></p>
              </td>
            </tr>
          <% }); %>
        </tbody>
      </table>
    """

  render: ->
    @$el.text "Red Flags"
    $("h2").after @templates.modal
      id: @options.id
    @

  displayModal: ->
    $.ajax
      url: "#{@options.url}/#{@options.id}"
      beforeSend: =>
        $("#modal-body-red-flags-#{@options.id}").html "<span class='muted'>Loading red flags...</span>"
        $("#modal-red-flags-#{@options.id}").modal
          keyboard: true
          width: "75%"
      success: (data, textStatus, xhr) =>
        if data and _(data).size() > 0
          html = ''
          for p in [ 'High', 'Medium', 'Low' ]
            filtered = _(data).filter (el) -> el.Priority == p
            if filtered and _(filtered).size() > 0
              filtered = _(filtered).map (f) ->
                f.Text = f.Text.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />').replace(/TBC/ig, '<span style="background-color: yellow;">TBC</span>')
                f
              html += @templates.flags
                priority: p
                flags: filtered
          $("#modal-body-red-flags-#{@options.id}").html html
        else
          $("#modal-body-red-flags-#{@options.id}").html "<span class='muted'>No red flags.</span>"
