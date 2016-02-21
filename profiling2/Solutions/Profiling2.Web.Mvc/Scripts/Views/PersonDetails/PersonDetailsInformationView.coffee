Profiling.Views.PersonDetailsInformationView = Backbone.View.extend

  templates: 
    column: _.template """
      <div class="pull-left" style="margin-right: 30px; margin-top: 10px;">
        <%= contents %>
      </div>
    """
    clearfix: _.template """
      <div style="clear: both;">
        <%= contents %>
      </div>
    """
    aliases: _.template """
      (also known as
      <% _.each(aliases, function(alias, i) { %>
        <span class="person-alias" data-id="<%= alias.Id %>"></span>
        <span>
          <strong><%= alias.FirstName %> <%= alias.LastName %></strong>
          <% if (i != aliases.length - 1) { %>
            ,
          <% } %>
        </span>
      <% }); %>
      )
    """
    noAliases: _.template """
      <span class="muted">No person aliases.</span>
    """
    photo: _.template """
      <a data-toggle="modal" href="#lightbox-<%= model.PhotoId %>">
        <img src="<%= Profiling.applicationUrl %>Profiling/Persons/<%= model.PersonId %>/Photo/Get/<%= model.PhotoId %>" width="150" height="150">
      </a>
      <p style="margin-top: 10px;">
        <span class="person-photo" style="clear: both;" data-id="<%= model.PhotoId %>" data-person-id="<%= model.PersonId %>"></span>
        <div id="lightbox-<%= model.PhotoId %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
            <% if (model.PhotoName) { %>
              <h3><%= model.PhotoName %></h3>
            <% } else { %>
              <h3 class="muted">(no photo name)</h3>
            <% } %>
          </div>
          <div class='modal-body'>
              <img src="<%= Profiling.applicationUrl %>Profiling/Persons/<%= model.PersonId %>/Photo/Get/<%= model.PhotoId %>">
          </div>
        </div>
      </p>
    """
    photos: _.template """
      <ul class="thumbnails">
        <% _(photos).each(function(photo) { %>
          <li>
            <div class="thumbnail">
              <%= photo %>
            </div>
          </li>
        <% }); %>
      </ul>
    """
    noPhotos: _.template """
      <span class="muted">No photos.</span>
    """
    personalInfo: _.template """
      <table class="table" style="width: auto;">
        <% if (person.MilitaryIDNumber) { %>
          <tr>
              <th>ID Number</th>
              <td><%= person.MilitaryIDNumber %></td>
          </tr>
        <% } %>
        <% if (person.DateOfBirth) { %>
          <tr>
              <th>Date of Birth</th>
              <td>
                <%= person.DateOfBirth %>
              </td>
          </tr>
        <% } %>
        <% if (person.PlaceOfBirth) { %>
          <tr>
              <th>Place of Birth</th>
              <td><%= person.PlaceOfBirth %></td>
          </tr>
        <% } %>
        <% if (person.EthnicityName) { %>
          <tr>
              <th>Ethnicity</th>
              <td><%= person.EthnicityName %></td>
          </tr>
        <% } %>
        <% if (person.Height) { %>
          <tr>
              <th>Height</th>
              <td><%= person.Height %></td>
          </tr>
        <% } %>
        <% if (person.Weight) { %>
          <tr>
              <th>Weight</th>
              <td><%= person.Weight %></td>
          </tr>
        <% } %>
      </table>
    """
    backgroundInformation: _.template """
      <h4 id="background-information-heading">Background Information</h4>
      <div class="form-horizontal">
        <label class="checkbox">
          <input id="backgroundInformationCheckbox" type="checkbox" data-toggle="collapse" data-target="#backgroundInformationDiv" checked="checked">&nbsp;Include Background
        </label>
      </div>
      <div id="backgroundInformationDiv" class="collapse in">
        <% if (backgroundInformation) { %>
          <p><%= backgroundInformation %></p>
        <% } else { %>
          <span class="muted">No background information.</span>
        <% } %>
      </div>
    """
    notes: _.template """
      <div class="row-fluid">
        <p class="alert alert-info span12" style="width: auto;">
          <span id="notes-heading"><strong>Notes</strong></span> 
          <br />
          <% if (notes) { %>
            <%= notes %>
          <% } else { %>
            <span class="muted">There are no notes.</span>
          <% } %>
        </p>
      </div>
    """
    personSourceHeading: _.template """
      <h4 class='pull-left'>Person Sources</h4> 
      <span class='pull-left alert close' 
        style='padding: 0; margin-left: 5px; margin-top: 10px; line-height: 0.3; background-color: whitesmoke;' 
        data-toggle='collapse' 
        data-target='#person-sources-table'>
        &hellip;
      </span>
    """
    personSources: _.template """
      <div class="collapse in" id="person-sources-table" style="clear: both;">
        <table class="table">
          <% _.each(personSources, function(personSource) { %>
            <tr>
              <td>
                <span class="person-source" data-id="<%= personSource.Id %>"></span>
                <a href="<%= Profiling.applicationUrl %>Profiling/Sources#info/<%= personSource.SourceId %>" target="_blank"><%= personSource.SourceName %></a>
                /
                <% if (personSource.Commentary) { %>
                  <%= personSource.Commentary %>
                <% } else { %>
                  <span class='muted'>No commentary</span>
                <% } %>
                <%= $(new Profiling.Views.SourceThumbnailView({ model: personSource }).render().el).html() %>
              </td>
              <td style="white-space: nowrap;">
                <div class="pull-right">
                  <% if (personSource.SourceIsRestricted === true) { %>
                    &nbsp;<span class="label label-important"><small>RESTRICTED</small></span>
                  <% } %>
                  <% if (personSource.ReliabilityName) { %>
                    <span class="label" title="Reliability rating"><%= personSource.ReliabilityName %></span>
                  <% } %>
                </div>
              </td>
            </tr>
          <% }); %>
        </table>
      </div>
    """
    noPersonSources: _.template """
      <div class="collapse in" id="person-sources-table" style="margin-bottom: 20px; clear: both;">
        <span class="muted">There are no person sources.</span>
      </div>
    """
    activeScreenings: _.template """
      <blockquote>
        <p>For the purposes of implementing the 3 month rule on applying conditionality, 
        an active screening related to a request that has been finalized marks the date a person was last colour coded.</p>
        <small>Color coding update, <cite>Guidance Note on Applying Conditionality SOP - 30 Jul 2012</cite></small>
      </blockquote>
      <table class="table">
        <thead>
          <tr>
            <th></th><th>Date</th><th>Request</th><th>Screened By</th><th>Notes</th>
          </tr>
        </thead>
        <tbody>
          <% _.each(activeScreenings, function(as) { %>
            <tr>
              <td>
                <span class="active-screening" data-id="<%= as.Id %>"></span>
              </td>
              <td><%= as.DateActivelyScreened %></td>
              <td><%= as.RequestHeadline %></td>
              <td><%= as.ScreenedByName %></td>
              <td><%= as.Notes %></td>
            </tr>
          <% }); %>
        </tbody>
      </table>
    """
    footer: _.template """
      <span class="muted pull-right">Profile last modified: <%= profileLastModified %></span>
    """

  render: ->
    if @model.get('Person').Name
      $("span#name-header").text "#{@model.get('Person').Name}"
    else
      $("span#name-header").text "(no name)"

    if @model.get('Person').IsRestrictedProfile is true
      @$el.append @templates.clearfix
        contents: "<span class='label label-important' style='margin-bottom: 10px;'><small>RESTRICTED</small></span>"

    @$el.append @templates.clearfix
      contents: "#{@renderAliases()}"

    @$el.append @templates.column
      contents: "<h4>Personal Information</h4>#{@renderPersonalInformation()}"

    @$el.append @templates.column
      contents: "<h4>Photos</h4>#{@renderPhotos()}"

    if @options.permissions.canViewBackgroundInformation
      @$el.append @templates.clearfix
        contents: @templates.column
          contents: @renderBackgroundInformation()

    if @options.permissions.canViewAndSearchSources
      @$el.append @templates.clearfix
        contents: @templates.column
          contents: "#{@templates.personSourceHeading()} #{@renderPersonSources()}"
          
    if @options.permissions.canPerformScreeningInput
      if _(@model.get('ActiveScreenings')).size() > 0
        @$el.append @templates.clearfix
          contents: @templates.column
            contents: "<h4>Active Screenings</h4>#{@renderActiveScreenings()}"

    @$el.append @templates.clearfix
      contents: @templates.column
        contents: @renderNotes()

    @$el.append @templates.clearfix
      contents: @templates.footer
        profileLastModified: moment(@model.get('Person').ProfileLastModified).format(Profiling.DATE_FORMAT)

    @

  renderPersonalInformation: ->
    personalTemplate = @templates.personalInfo
      person: @model.get 'Person'

  renderAliases: ->
    if @options.permissions.canChangePersons
      _.defer ->
        $("span.person-alias").each (i, el) ->
          aliasId = $(el).data 'id'
          editFormView = new Profiling.Views.AliasEditFormView
            modalId: "modal-person-alias-edit-#{aliasId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/PersonAliases/Edit/#{aliasId}"
            modalSaveButton: 'modal-person-alias-edit-button'
          $(el).before editFormView.render().el
          deleteButtonView = new Profiling.Views.AliasDeleteButtonView
            title: "Delete Alias"
            confirm: "Are you sure you want to delete this alias?"
            url: "#{Profiling.applicationUrl}Profiling/PersonAliases/Delete/#{aliasId}"
          $(el).before deleteButtonView.render().el

    aliasesTemplate = if _.isEmpty(@model.get 'Aliases')
      ''
    else
      @templates.aliases
        aliases: @model.get 'Aliases'

  renderPhotos: ->
    if @options.permissions.canChangePersons
      _.defer ->
        $("span.person-photo").each (i, el) ->
          photoId = $(el).data 'id'
          personId = $(el).data 'person-id'
          editFormView = new Profiling.Views.PhotoEditFormView
            modalId: "modal-person-photo-edit-#{photoId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Photo/Edit/#{photoId}"
            modalSaveButton: "modal-person-photo-edit-button-#{photoId}"
          $(el).before editFormView.render().el
          deleteButtonView = new Profiling.Views.PhotoDeleteButtonView
            title: "Delete Person Photo"
            confirm: "Are you sure you want to delete this person photo?"
            url: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/Photo/Delete/#{photoId}"
          $(el).before deleteButtonView.render().el

    photosTemplate = if _.isEmpty(@model.get 'Photos')
      @templates.noPhotos()
    else
      photos = for i in @model.get 'Photos'
        @templates.photo
          model: i
      @templates.photos
        photos: photos

  renderBackgroundInformation: ->
    if @options.permissions.canChangePersonBackground
      _.defer =>
        editFormView = new Profiling.Views.PersonEditFormIconView
          title: 'Edit background information'
          modalId:  'modal-background-information-edit'
          modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/EditBackgroundInformationModal/#{@model.get('Person').Id}"
          modalSaveButton: 'modal-background-information-save-button'
        $("#background-information-heading").prepend editFormView.render().el

    backgroundInformationTemplate = @templates.backgroundInformation
      backgroundInformation: @model.get('Person').BackgroundInformation

  renderNotes: ->
    if @options.permissions.canChangePersons
      _.defer =>
        editFormView = new Profiling.Views.PersonEditFormIconView
          title: 'Edit notes'
          modalId:  'modal-notes-edit'
          modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/EditNotesModal/#{@model.get('Person').Id}"
          modalSaveButton: 'modal-notes-save-button'
        $("#notes-heading").prepend editFormView.render().el

    notesTemplate = @templates.notes
      notes: @model.get('Person').Notes

  renderPersonSources: ->
    _.defer ->
      $("span.person-source").each (i, el) ->
        personSourceId = $(el).data 'id'
        editFormView = new Profiling.Views.PersonSourceEditFormView
          modalId: "modal-person-source-edit-#{personSourceId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/PersonSources/Edit/#{personSourceId}"
          modalSaveButton: 'modal-person-source-edit-button'
        $(el).before editFormView.render().el
        deleteButtonView = new Profiling.Views.PersonSourceDeleteButtonView
          title: "Delete Person Source"
          confirm: "Are you sure you want to delete this person source?"
          url: "#{Profiling.applicationUrl}Profiling/PersonSources/Delete/#{personSourceId}"
        $(el).before deleteButtonView.render().el

    personSourcesTemplate = if _.isEmpty(@model.get 'PersonSources')
      @templates.noPersonSources()
    else
      @templates.personSources
        personSources: @model.get 'PersonSources'

  renderActiveScreenings: ->
    personId = @model.get('Person').Id
    _.defer ->
      $("span.active-screening").each (i, el) ->
        asId = $(el).data 'id'
        editFormView = new Profiling.Views.ActiveScreeningEditFormView
          modalId: "modal-active-screening-edit-#{asId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/ActiveScreenings/EditModal/#{asId}"
          modalSaveButton: 'modal-active-screening-edit-button'
        $(el).before editFormView.render().el
        deleteButtonView = new Profiling.Views.ActiveScreeningDeleteButtonView
          title: "Delete Active Screening"
          confirm: "Are you sure you want to delete this active screening?"
          url: "#{Profiling.applicationUrl}Profiling/Persons/#{personId}/ActiveScreenings/Delete/#{asId}"
        $(el).before deleteButtonView.render().el

    template = @templates.activeScreenings
      activeScreenings: _(@model.get 'ActiveScreenings').sortBy('DateActivelyScreened').reverse()