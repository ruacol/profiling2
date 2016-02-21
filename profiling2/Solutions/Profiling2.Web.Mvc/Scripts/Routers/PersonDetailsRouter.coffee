Profiling.Routers.PersonDetailsRouter = Backbone.Router.extend

  initialize: (opts) ->
    @personId = opts.id
    @permissions = opts.permissions

    @on 'route', ->
      Profiling.setInStorage
        key: Profiling.MOST_RECENT_PERSONS 
        id: @personId
        name: (if @personModel and @personModel.get('Person') then @personModel.get('Person').Name else opts.personName)

    if @permissions.canDeletePersons
      deleteProfileView = new Profiling.Views.DeleteProfileButtonView
        personId: @personId
      $("#change-person-menu").append deleteProfileView.render().el
      $("#change-person-menu").prepend "<li class='divider'></li>"

    if @permissions.canChangePersons and @permissions.canPerformScreeningInput
      addActiveScreeningView = new Profiling.Views.ActiveScreeningAddFormView
        modalId: 'modal-active-screening-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/ActiveScreenings/CreateModal"
        modalSaveButton: 'modal-active-screening-add-button'
      $("#change-person-menu").prepend addActiveScreeningView.render().el

    if @permissions.canChangePersonResponsibilities
      addHumanRightsView = new Profiling.Views.HumanRightsAddFormView
        personId: @personId
        modalId: 'modal-person-responsibility-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/PersonResponsibilities/AddForPerson"
        modalSaveButton: 'modal-person-responsibility-add-button'
      $("#change-person-menu").prepend addHumanRightsView.render().el

    if @permissions.canChangePersons
      addCareerView = new Profiling.Views.CareerCreateFormView
        modalId: 'modal-person-career-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/Careers/Add"
        modalSaveButton: 'modal-person-career-add-button'
      $("#change-person-menu").prepend addCareerView.render().el

      addPersonRelationshipView = new Profiling.Views.RelationshipCreateFormView
        modalId: 'modal-person-relationship-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/Relationships/Add"
        modalSaveButton: 'modal-person-relationship-add-button'
      $("#change-person-menu").prepend addPersonRelationshipView.render().el
      
    if @permissions.canViewAndChangePersonRestrictedNotes
      addRestrictedNoteView = new Profiling.Views.RestrictedNoteAddFormView
        modalId: 'modal-restricted-note-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/Notes/CreateModal"
        modalSaveButton: 'modal-person-note-add-button'
      $("#change-person-menu").prepend addRestrictedNoteView.render().el

    if @permissions.canChangePersons
      addPersonPhotoView = new Profiling.Views.PhotoCreateFormView
        modalId: 'modal-person-photo-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/Photos/Add"
        modalSaveButton: 'modal-person-photo-add-button'
      $("#change-person-menu").prepend addPersonPhotoView.render().el

    if @permissions.canViewAndSearchSources and @permissions.canChangePersons
      addPersonSourceView = new Profiling.Views.PersonSourceCreateFormView
        modalId: 'modal-person-source-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/Sources/Add"
        modalSaveButton: 'modal-person-source-add-button'
      $("#change-person-menu").prepend addPersonSourceView.render().el

    if @permissions.canChangePersons
      addPersonAliasView = new Profiling.Views.AliasCreateFormView
        modalId: 'modal-person-alias-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/#{@personId}/Aliases/Create"
        modalSaveButton: 'modal-person-alias-create-save-button'
      $("#change-person-menu").prepend addPersonAliasView.render().el

      editView = new Profiling.Views.PersonEditFormView
        title: 'Edit Person'
        personId: @personId
        modalId: 'modal-person-edit'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/Edit/#{@personId}"
        modalSaveButton: 'modal-person-edit-save-button'
      $("#change-person-menu").prepend editView.render().el

    tabView = new Profiling.Views.PersonDetailsTabView
      permissions: @permissions
      id: @personId
    $("#person-details-content").html tabView.render().el

    personId = @personId
    if @permissions.canExportPersons
      $("#save-as-word-button").click (e) ->
        href = "#{Profiling.applicationUrl}Profiling/Persons/Export/#{personId}?includeBackground="
        includeBackground = $("#backgroundInformationCheckbox").is ":checked"
        $(@).attr 'href', "#{href}#{includeBackground}"

    if @permissions.canViewPersonReports
      flagView = new Profiling.Views.RedFlagsView
        id: @personId
        url: "#{Profiling.applicationUrl}Profiling/Persons/RedFlags"
      $("#button-group-right").append flagView.render().el

  routes:
    "": "information"
    "personal-information": "information"
    "restricted-information": "restricted"
    "relationships": "relationships"
    "career": "career"
    "human-rights-record": "humanRightsRecord"
    "screening-information": "screeningInformation"

  information: ->
    @personModel = new Profiling.Models.PersonModel
      id: @personId
    @personModel.fetch
      beforeSend: ->
        $("#personal-information").html "<span class='muted'>Loading personal information...</span>"
      success: =>
        informationView = new Profiling.Views.PersonDetailsInformationView
          model: @personModel
          permissions: @permissions
        $("#personal-information").html informationView.render().el
        $("#tab-list a[href='#personal-information']").tab 'show'

        # TODO not displayed outside Profiling team yet
        if @permissions.canChangePersonPublicSummaries
          publicSummaryView = new Profiling.Views.PublicSummaryView
            id: @personId
            publicSummary: @personModel.get('Person').PublicSummary
          $("#public-summary-container").html publicSummaryView.render().el

        @scroll()
      error: (model, xhr, options) =>
        $("#personal-information").tab 'show'
        $("#personal-information").html xhr.responseText

  restricted: ->
    @personNotesModel = new Profiling.Models.PersonNotesModel
      id: @personId
    @personNotesModel.fetch
      beforeSend: ->
        $("#restricted-information").html "<span class='muted'>Loading restricted information...</span>"
      success: =>
        notesView = new Profiling.Views.NotesView
          model: @personNotesModel
          permissions: @permissions
        $("#restricted-information").html notesView.render().el
        $("#tab-list a[href='#restricted-information']").tab 'show'
      error: (model, xhr, options) =>
        $("#restricted-information").tab 'show'
        $("#restricted-information").html xhr.responseText

  relationships: ->
    @personRelationshipsModel = new Profiling.Models.PersonRelationshipsModel
      id: @personId
    @personRelationshipsModel.fetch
      beforeSend: ->
        $("#relationships").html "<span class='muted'>Loading relationships...</span>"
      success: =>
        relationshipsView = new Profiling.Views.PersonDetailsRelationshipsView
          model: @personRelationshipsModel
          personId: @personId
          permissions: @permissions
        $("#relationships").html relationshipsView.render().el
        $("#tab-list a[href='#relationships']").tab 'show'
        @scroll()
      error: (model, xhr, options) =>
        $("#relationships").tab 'show'
        $("#relationships").html xhr.responseText

  career: ->
    @careersModel = new Profiling.Models.PersonCareersModel
      id: @personId
    @careersModel.fetch
      beforeSend: ->
        $("#career").html "<span class='muted'>Loading careers...</span>"
      success: =>
        careersView = new Profiling.Views.PersonDetailsCareersView
          model: @careersModel
          personId: @personId
          permissions: @permissions
        $("#career").html careersView.render().el
        $("#tab-list a[href='#career']").tab 'show'
        @scroll()
      error: (model, xhr, options) =>
        $("#career").tab 'show'
        $("#career").html xhr.responseText

  humanRightsRecord: ->
    @personResponsibilitiesModel = new Profiling.Models.PersonResponsibilitiesModel
      id: @personId
    @personResponsibilitiesModel.fetch
      beforeSend: ->
        $("#human-rights-record").html "<span class='muted'>Loading human rights record...</span>"
      success: =>
        responsibilitiesView = new Profiling.Views.PersonDetailsResponsibilitiesNewView
          model: @personResponsibilitiesModel
          personId: @personId
          permissions: @permissions
        $("#human-rights-record").html responsibilitiesView.render().el
        $("#tab-list a[href='#human-rights-record']").tab 'show'
        @scroll()
      error: (model, xhr, options) =>
        $("#human-rights-record").tab 'show'
        $("#human-rights-record").html xhr.responseText

  screeningInformation: ->
    @personScreeningInformationModel = new Profiling.Models.PersonScreeningInformationModel
      id: @personId
    @personScreeningInformationModel.fetch
      beforeSend: ->
        $("#screening-information").html "<span class='muted'>Loading screening information...</span>"
      success: =>
        view = new Profiling.Views.PersonDetailsScreeningInformationView
          model: @personScreeningInformationModel
          personId: @personId
        $("#screening-information").html view.render().el
        $("#tab-list a[href='#screening-information']").tab 'show'
        @scroll()
      error: (model, xhr, options) =>
        $("#screening-information").tab 'show'
        $("#screening-information").html xhr.responseText

  scroll: ->
    window.scrollBy 0, window.Profiling.storedPageYOffset if window.Profiling.storedPageYOffset