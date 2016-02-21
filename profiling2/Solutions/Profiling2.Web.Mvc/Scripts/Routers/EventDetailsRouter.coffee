Profiling.Routers.EventDetailsRouter = Backbone.Router.extend

  initialize: (opts) ->
    @eventId = opts.id
    @eventHeadline = opts.headline
    @permissions = opts.permissions

    @on 'route', ->
      Profiling.setInStorage
        key: Profiling.MOST_RECENT_EVENTS 
        id: @eventId
        name: @eventHeadline

    $("#save-as-word-button").click (e) ->
      $(@).attr 'href', "#{Profiling.applicationUrl}Profiling/Events/Export/#{opts.id}"

  routes:
    "": "index"

  index: ->
    if @permissions.canApproveEvents
      eventApproveView = new Profiling.Views.EventApproveView
        eventId: @eventId
        url: "#{Profiling.applicationUrl}Profiling/Events/Approve/#{@eventId}"
      $("#approve-event-group").append eventApproveView.render().el

    if @permissions.canChangeEvents
      eventDeleteView = new Profiling.Views.EventDeleteView
        eventId: @eventId
        url: "#{Profiling.applicationUrl}Profiling/Events/Delete/#{@eventId}"
      $("#change-event-menu").append eventDeleteView.render().el
      $("#change-event-menu").prepend "<li class='divider'></li>"

    flagView = new Profiling.Views.RedFlagsView
      id: @eventId
      url: "#{Profiling.applicationUrl}Profiling/Events/RedFlags"
    $("#button-group-right").append flagView.render().el

    if @permissions.canChangeActionsTaken
      addActionTakenView = new Profiling.Views.ActionTakenAddFormView
        eventId: @eventId
        modalId: 'modal-action-taken-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{@eventId}/ActionsTaken/Add"
        modalSaveButton: 'modal-action-taken-add-button'
      $("#change-event-menu").prepend addActionTakenView.render().el

    if @permissions.canChangePersonResponsibilities
      addPersonResponsibilityView = new Profiling.Views.AddPersonResponsibilityFormView
        eventId: @eventId
        modalId: 'modal-person-responsibility-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{@eventId}/PersonResponsibilities/Add"
        modalSaveButton: 'modal-person-responsibility-add-button'
      $("#change-event-menu").prepend addPersonResponsibilityView.render().el

      orgResponsibilityAddFormView = new Profiling.Views.OrgResponsibilityAddFormView
        eventId: @eventId
        modalId: 'modal-org-responsibility-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{@eventId}/OrgResponsibilities/Add"
        modalSaveButton: 'modal-org-responsibility-add-button'
      $("#change-event-menu").prepend orgResponsibilityAddFormView.render().el

    if @permissions.canLinkEvents
      addEventRelationshipView = new Profiling.Views.EventRelationshipAddFormView
        eventId: @eventId
        eventHeadline: @eventHeadline
        modalId: 'modal-event-relationship-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/EventRelationships/Add"
        modalSaveButton: 'modal-event-relationship-add-button'
      $("#change-event-menu").prepend addEventRelationshipView.render().el

    if @permissions.canLinkEventsAndSources
      addSourceView = new Profiling.Views.EventAddSourceFormView
        eventId: @eventId
        modalId: 'modal-event-source-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Events/#{@eventId}/Sources/Add"
        modalSaveButton: 'modal-event-source-add-button'
      $("#change-event-menu").prepend addSourceView.render().el

    if @permissions.canChangeEvents
      editView = new Profiling.Views.EventEditFormView
        eventId: @eventId
        modalId: "modal-event-edit-#{@eventId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Events/Edit/#{@eventId}"
        modalSaveButton: "modal-event-edit-save-button"
      $("#change-event-menu").prepend editView.render().el

    if @permissions.canLinkEventsAndSources
      $("span.event-source").each (i, el) =>
        id = $(el).data 'id'
        editFormView = new Profiling.Views.EventSourceEditFormView
          modalId: 'modal-event-source-edit'
          modalUrl: "#{Profiling.applicationUrl}Profiling/EventSources/Edit/#{id}"
          modalSaveButton: 'modal-event-source-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Detach Source"
          confirm: "Are you sure you want to detach this source from this event?"
          url: "#{Profiling.applicationUrl}Profiling/EventSources/Delete/#{id}"
        $(el).before deleteButtonView.render().el

    if @permissions.canChangePersonResponsibilities
      $("span.organization-responsibility").each (i, el) =>
        organisationResponsibilityId = $(el).data 'organization-responsibility-id'
        editFormView = new Profiling.Views.OrgResponsibilityEditFormView
          organisationResponsibilityId: organisationResponsibilityId
          modalId: "modal-org-responsibility-edit-#{organisationResponsibilityId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/OrgResponsibilities/Edit/#{organisationResponsibilityId}"
          modalSaveButton: 'modal-org-responsibility-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Delete Organization Responsibility"
          confirm: "Are you sure you want to remove this organization and/or unit's responsibility for this event?"
          url: "#{Profiling.applicationUrl}Profiling/OrgResponsibilities/Delete/#{organisationResponsibilityId}"
        $(el).before deleteButtonView.render().el

      $("span.person-responsibility").each (i, el) =>
        id = $(el).data 'id'
        editFormView = new Profiling.Views.PersonResponsibilityEditFormView
          modalId: 'modal-person-responsibility-edit'
          modalUrl: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Edit/#{id}"
          modalSaveButton: 'modal-person-responsibility-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Delete Person Responsibility"
          confirm: "Are you sure you want to delete this person's responsibility for this event?"
          url: "#{Profiling.applicationUrl}Profiling/PersonResponsibilities/Delete/#{id}"
        $(el).before deleteButtonView.render().el

    if @permissions.canChangeActionsTaken
      $("span.action-taken").each (i, el) ->
        actionTakenId = $(el).data 'action-taken-id'
        editFormView = new Profiling.Views.ActionTakenEditFormView
          actionTakenId: actionTakenId
          modalId: 'modal-action-taken-edit'
          modalUrl: "#{Profiling.applicationUrl}Profiling/ActionsTaken/Edit/#{actionTakenId}"
          modalSaveButton: 'modal-action-taken-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Delete Action Taken"
          confirm: "Are you sure you want to remove this action taken for this event?"
          url: "#{Profiling.applicationUrl}Profiling/ActionsTaken/Delete/#{actionTakenId}"
        $(el).before deleteButtonView.render().el

    if @permissions.canLinkEvents
      $("span.event-relationship").each (i, el) =>
        eventRelationshipId = $(el).data 'id'
        editFormView = new Profiling.Views.EventRelationshipEditFormView
          eventId: @eventId
          eventHeadline: @eventHeadline
          eventRelationshipId: eventRelationshipId
          modalId: "modal-event-relationship-edit-#{eventRelationshipId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/EventRelationships/Edit/#{eventRelationshipId}"
          modalSaveButton: 'modal-event-relationship-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.BaseDeleteButtonView
          title: "Delete Event Relationship"
          confirm: "Are you sure you want to remove this relationship for this event?"
          url: "#{Profiling.applicationUrl}Profiling/EventRelationships/Delete/#{eventRelationshipId}"
        $(el).before deleteButtonView.render().el

    Profiling.setupUITooltips "td.pr-cell"
    Profiling.setupUITooltips "td.or-cell"

    _.defer =>
      model = new Profiling.Models.SimilarEventsModel
        id: @eventId
      model.fetch
        success: ->
          if _(model.get 'Events').size() > 0
            view = new Profiling.Views.SimilarEventsView
              model: model
            $("#secondary-sidebar").css("bottom", "40px").html(view.render().el).fadeOut().fadeIn()