Profiling.Views.BasePersonRelationshipFormView = Profiling.Views.BaseModalForm.extend

  parentDivId: ->
    id = if @options.relationshipId
      "#modal-person-relationship-edit-#{@options.relationshipId}"
    else
      "#modal-person-relationship-add"

  modalLoadedCallback: ->
    @setupPersonSelect '#SubjectPersonId,#ObjectPersonId'
    @setupRelationshipTypeSelect()

    addSubjectView = new Profiling.Views.PersonQuickCreateFormView
      targetSelector: "#{@parentDivId()} #SubjectPersonId"
      modalId: 'modal-subject-person-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/QuickCreate"
      modalSaveButton: 'modal-person-create-save-button'
    $("#{@parentDivId()} #SubjectPersonId").parent().prev().prepend addSubjectView.render().el

    addObjectView = new Profiling.Views.PersonQuickCreateFormView
      targetSelector: "#{@parentDivId()} #ObjectPersonId"
      modalId: 'modal-object-person-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Persons/QuickCreate"
      modalSaveButton: 'modal-person-create-save-button'
    $("#{@parentDivId()} #ObjectPersonId").parent().prev().prepend addObjectView.render().el

  setupRelationshipTypeSelect: ->
    el = $("#{@parentDivId()} #PersonRelationshipTypeId")

    $(el).val '' if not $(el).val()
    $(el).select2
      placeholder: "Search by relationship..."
      allowClear: true
      initSelection: (element, callback) ->
        if element.val()
          relationshipTypeId = $(element).val()
          data =
            id: relationshipTypeId
          $.ajax
            async: false
            url: "#{Profiling.applicationUrl}Profiling/PersonRelationships/Name/#{relationshipTypeId}"
            success: (response, textStatus, xhr) ->
              if response
                data.text = response.Name
          callback data
      ajax:
        url: "#{Profiling.applicationUrl}Profiling/PersonRelationships/Get"
        dataType: 'json'
        quietMillis: 1000
        data: (term, page) ->
          term: term
        results: (data, page) ->
          results: data