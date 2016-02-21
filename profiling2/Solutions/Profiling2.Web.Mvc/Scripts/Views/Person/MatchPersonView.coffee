Profiling.Views.MatchPersonView = Backbone.View.extend

  templates:
    modal: _.template """
      <div id="modal-candidate-<%= rowNumber %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>
          <h4>
            Potential matches for <%= firstName %> <%= lastName %>, ID number
            <% if (idNumber) { %>
              <%= idNumber %>
            <% } else { %>
              (none)
            <% } %>
          </h4>
        </div>
        <div class='modal-body' id="modal-candidate-body-<%= rowNumber %>">
          <table class="table table-bordered">
            <tr>
                <th>ID number</th><th>Name</th>
            </tr>
            <% $(persons).each(function(i, el) { %>
              <tr>
                <td><%= el.MilitaryIDNumber %></td>
                <td><%= el.FirstName %> <%= el.LastName %></td>
              </tr>
            <% }) %>
          </table>
        </div>
        <div class="modal-footer">
          <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
        </div>
      </div>
    """
    name: _.template """
      <%= firstName %> <%= lastName %>
    """

  events:
    "click": "showModal"

  initialize: (opts) ->
    @tr = opts.row
    @rowNumber = opts.rowNumber

  render: ->
    idNumber = $("td:first-child", @tr).text()
    firstName = $("td:nth-child(2)", @tr).text()
    lastName = $("td:nth-child(3)", @tr).text()

    $.ajax
      url: "#{Profiling.applicationUrl}Profiling/Persons/MatchPerson"
      type: "POST"
      async: true
      data:
        MilitaryIDNumber: idNumber
        FirstName: firstName
        LastName: lastName
      success: (data, textStatus, xhr) =>
        if data
          first = _(data).first()
          if first
            @$el.html @templates.modal
              rowNumber: @rowNumber
              idNumber: idNumber
              firstName: firstName
              lastName: lastName
              persons: data

            @$el.append @templates.name
              idNumber: first.Id
              firstName: first.FirstName
              lastName: first.LastName

            @$el.addClass 'accordion-toggle'

    @

  showModal: ->
    _.defer =>
      $("#modal-candidate-#{@rowNumber}").modal
        keyboard: true
        width: "80%"