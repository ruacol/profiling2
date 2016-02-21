Profiling.Views.NotesView = Backbone.View.extend

  className: "span9"

  templates: 
    table: _.template """
      <p>These notes are only visible to <em>CanViewAndChangePersonRestrictedNotes</em> permission holders.</p>
      <table class="table table-condensed table-bordered" style="width: auto;">
        <thead><tr><th></th><th>Note</th></tr></thead>
        <tbody>
          <% $(rows).each(function(i, row) { %>
            <%= row %>
          <% }); %>
        </tbody>
      </table>
    """
    row: _.template """
      <tr>
        <td class="restricted-note" data-id="<%= note.Id %>"></td>
        <td><%= note.Note %></td>
      </tr>
    """
    none: "<span class='muted'>No restricted information.</span>"

  render: ->
    notes = @model.get 'Notes'

    if notes and _(notes).size() > 0
      rows = for i in notes
        @templates.row
          note: i

      @$el.html @templates.table
        rows: rows
    else
      @$el.html @templates.none

    personId = @model.get 'id'
    if @options.permissions.canViewAndChangePersonRestrictedNotes
      _.defer ->
        $("td.restricted-note").each (i, el) ->
          noteId = $(el).data 'id'
          editFormView = new Profiling.Views.RestrictedNoteEditFormView
            modalId: "modal-restricted-note-edit-#{noteId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Notes/EditModal/#{noteId}"
            modalSaveButton: "modal-restricted-note-edit-button-#{noteId}"
          $(el).append editFormView.render().el
          deleteButtonView = new Profiling.Views.BaseDeleteButtonView
            title: "Delete Restricted Note"
            confirm: "Are you sure you want to delete this restricted note?"
            url: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Notes/Delete/#{noteId}"
          $(el).append deleteButtonView.render().el

    @