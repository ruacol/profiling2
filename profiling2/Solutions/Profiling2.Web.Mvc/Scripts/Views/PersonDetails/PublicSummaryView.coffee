Profiling.Views.PublicSummaryView = Backbone.View.extend

  className: "btn btn-mini"
  tagName: "a"

  events:
    "click": "displayModal"

  templates:
    modal: _.template """
      <div id="modal-public-summary-<%= id %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-header">
          <button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>
          <h4>Public Summary</h4>
        </div>
        <div class='modal-body' id="modal-body-public-summary-<%= id %>">
          <p>
            <% if (publicSummary) { %>
              <%= publicSummary %>
            <% } else { %>
              <span class="muted">There is no public summary.</span>
            <% } %>
          </p>
        </div>
        <div class="modal-footer">
          <button class="btn" data-dismiss="modal" aria-hidden="true">Close</button>
        </div>
      </div>
    """

  render: ->
    @$el.text "Public Summary"
    $("body").prepend @templates.modal
      id: @options.id
      publicSummary: @options.publicSummary
    @

  displayModal: ->
    $("#modal-public-summary-#{@options.id}").modal
      keyboard: true
      width: "75%"

    _.defer =>
      editFormView = new Profiling.Views.PublicSummaryEditFormIconView
        title: 'Edit public summary'
        modalId:  'modal-public-summary-edit'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/EditPublicSummaryModal/#{@options.id}"
        modalSaveButton: 'modal-public-summary-save-button'
        publicSummaryView: @
      $("#modal-public-summary-#{@options.id} h4").html(editFormView.render().el).append "Public Summary"

   hideModal: ->
     $("#modal-public-summary-#{@options.id}").modal 'hide'
