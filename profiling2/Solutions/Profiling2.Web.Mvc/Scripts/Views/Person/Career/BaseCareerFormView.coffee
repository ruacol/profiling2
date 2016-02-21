Profiling.Views.BaseCareerFormView = Profiling.Views.BaseModalForm.extend

  parentDivId: ->
    id = if @options.careerId
      "#modal-person-career-edit-#{@options.careerId}"
    else
      "#modal-person-career-add"

  modalLoadedCallback: ->
    @setupSelect
      el: "#{@parentDivId()} #OrganizationId"
      placeholder: "Search by organization ID or name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Organizations/Get"
    @setupSelect
      el: "#{@parentDivId()} #LocationId"
      placeholder: "Search by location name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Locations/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Locations/Get"
    @setupSelect
      el: "#{@parentDivId()} #RankId"
      placeholder: "Search by rank name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Ranks/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Ranks/Get"
    @setupSelect
      el: "#{@parentDivId()} #RoleId"
      placeholder: "Search by function name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Roles/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Roles/Get"
    @setupSelect
      el: "#{@parentDivId()} #UnitId"
      placeholder: "Search by unit ID or name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"

    locationId = $("#{@parentDivId()} #LocationId").val()
    if locationId
      editLocationView = new Profiling.Views.LocationEditFormView
        parentDivId: @parentDivId()
        modalId: "modal-location-edit-#{locationId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Locations/EditModal/#{locationId}"
        modalSaveButton: "modal-location-edit-button-#{locationId}"
      $("#{@parentDivId()} #LocationId").parent().prev().prepend editLocationView.render().el

    createLocationView = new Profiling.Views.LocationCreateFormView
      modalId: 'modal-location-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Locations/CreateModal"
      modalSaveButton: 'modal-location-create-button'
    $("#{@parentDivId()} #LocationId").parent().prev().prepend createLocationView.render().el

    roleId = $("#{@parentDivId()} #RoleId").val()
    if roleId
      editFunctionView = new Profiling.Views.FunctionEditFormView
        careerDivId: @parentDivId()
        modalId: "modal-role-edit-#{roleId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Roles/EditModal/#{roleId}"
        modalSaveButton: "modal-role-edit-button-#{roleId}"
      $("#{@parentDivId()} #RoleId").parent().prev().prepend editFunctionView.render().el

    createFunctionView = new Profiling.Views.FunctionCreateFormView
      modalId: 'modal-role-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Roles/CreateModal"
      modalSaveButton: 'modal-role-create-button'
    $("#{@parentDivId()} #RoleId").parent().prev().prepend createFunctionView.render().el

    unitId = $("#{@parentDivId()} #UnitId").val()
    if unitId
      editUnitView = new Profiling.Views.UnitEditFormView
        careerDivId: @parentDivId()
        unitId: unitId
        modalId: "modal-unit-edit-#{unitId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Units/EditModal/#{unitId}"
        modalSaveButton: "modal-unit-edit-button-#{unitId}"
      $("#{@parentDivId()} #UnitId").parent().prev().prepend editUnitView.render().el

    createUnitView = new Profiling.Views.UnitCreateFormView
      modalId: 'modal-unit-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Units/CreateModal"
      modalSaveButton: "modal-unit-create-button"
    $("#{@parentDivId()} #UnitId").parent().prev().prepend createUnitView.render().el

    rankId = $("#{@parentDivId()} #RankId").val()
    if rankId
      editRankView = new Profiling.Views.RankEditFormView
        careerDivId: @parentDivId()
        modalId: "modal-rank-edit-#{unitId}"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Ranks/EditModal/#{rankId}"
        modalSaveButton: "modal-rank-edit-button-#{rankId}"
      $("#{@parentDivId()} #RankId").parent().prev().prepend editRankView.render().el

    createRankView = new Profiling.Views.RankCreateFormView
      modalId: 'modal-rank-create'
      modalUrl: "#{Profiling.applicationUrl}Profiling/Ranks/CreateModal"
      modalSaveButton: "modal-rank-create-button"
    $("#{@parentDivId()} #RankId").parent().prev().prepend createRankView.render().el