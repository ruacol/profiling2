Profiling.Routers.UnitDetailsRouter = Backbone.Router.extend

  initialize: (opts) ->
    @unitId = opts.unitId
    @permissions = opts.permissions
    @parentHierarchyModels = opts.parentHierarchyModels
    @childHierarchyModels = opts.childHierarchyModels

  routes:
    "": "index"

  index: ->
    _.defer =>
      view = new Profiling.Views.UnitImmediateChartView
        unitId: @unitId
        permissions: @permissions
        parentHierarchyModels: @parentHierarchyModels
        childHierarchyModels: @childHierarchyModels
      view.render()

    if @permissions.canViewAndSearchSources
      unitSourcesModel = new Profiling.Models.UnitSourcesModel
        id: @unitId
      unitSourcesModel.fetch
        beforeSend: ->
          $("#unit-sources").html "<span class='muted'>Loading...</span>"
        success: ->
          unitSourcesView = new Profiling.Views.UnitSourcesView
            model: unitSourcesModel
          $("#unit-sources").html unitSourcesView.render().el
        error: (model, xhr, options) ->
          $("#unit-sources").html xhr.responseText

      addUnitSourceView = new Profiling.Views.UnitSourceCreateFormView
        modalId: 'modal-unit-source-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Units/#{@unitId}/Sources/Add"
        modalSaveButton: 'modal-unit-source-add-button'
      $("#write-buttons .divider").before addUnitSourceView.render().el

    if @permissions.canChangeUnits
      createAliasView = new Profiling.Views.UnitAliasCreateFormView
        modalId: 'modal-unit-alias-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Units/#{@unitId}/Aliases/Create"
        modalSaveButton: 'modal-unit-alias-create-save-button'
      $("#write-buttons .divider").before createAliasView.render().el

      _.defer ->
        $("span.unit-alias").each (i, el) ->
          aliasId = $(el).data 'id'
          editFormView = new Profiling.Views.UnitAliasEditFormView
            modalId: "modal-unit-alias-edit-#{aliasId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/UnitAliases/Edit/#{aliasId}"
            modalSaveButton: 'modal-unit-alias-edit-button'
          $(el).before editFormView.render().el
          deleteButtonView = new Profiling.Views.UnitAliasDeleteButtonView
            title: "Delete Alias"
            confirm: "Are you sure you want to delete this alias?"
            url: "#{Profiling.applicationUrl}Profiling/UnitAliases/Delete/#{aliasId}"
          $(el).before deleteButtonView.render().el

      operationAddFormView = new Profiling.Views.OperationAddFormView
        modalId: "modal-operation-add"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Units/AddOperationModal/#{@unitId}"
        modalSaveButton: "modal-operation-add-button"
      $("#write-buttons .divider").before operationAddFormView.render().el

      _.defer =>
        $("span.unit-operation").each (i, el) =>
          uoId = $(el).data 'id'
          oId = $(el).data 'operation-id'

          editFormView = new Profiling.Views.UnitOperationEditFormView
            modalId: "modal-unit-operation-edit-#{uoId}"
            modalUrl: "#{Profiling.applicationUrl}Profiling/UnitOperations/EditModal/#{uoId}"
            modalSaveButton: "modal-unit-operation-edit-button-#{uoId}"
            operationId: oId
          $(el).before editFormView.render().el

          removeButtonView = new Profiling.Views.OperationRemoveButtonView
            title: "Remove operation from this unit"
            confirm: "Are you sure you want to remove this operation from this unit (operation itself will not be deleted)?"
            url: "#{Profiling.applicationUrl}Profiling/UnitOperations/Remove/#{uoId}"
          $(el).before removeButtonView.render().el

    orgResponsibilitiesTable = new Profiling.DataTable "organization-responsibilities-table",
      bServerSide: false
      aoColumns: if @permissions.canViewAndSearchEvents then [ { sType: "title-numeric" }, null, null ] else [ null, null ]