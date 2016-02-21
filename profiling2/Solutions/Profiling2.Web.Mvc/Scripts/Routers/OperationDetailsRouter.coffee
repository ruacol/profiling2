Profiling.Routers.OperationDetailsRouter = Backbone.Router.extend

  initialize: (opts) ->
    @operationId = opts.operationId
    @permissions = opts.permissions

  routes:
    "": "index"

  index: ->
    if @permissions.canChangeUnits
      unitAddFormView = new Profiling.Views.UnitAddFormView
        modalId: "modal-unit-add"
        modalUrl: "#{Profiling.applicationUrl}Profiling/Operations/AddUnitModal/#{@operationId}"
        modalSaveButton: "modal-unit-add-button"
      $("#dropdown-list .divider").before unitAddFormView.render().el

      $("span.alias").each (i, el) =>
        aliasId = $(el).data 'id'
        editFormView = new Profiling.Views.OperationAliasEditFormView
          modalId: "modal-operation-alias-edit-#{aliasId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/Operations/#{@operationId}/Aliases/Edit/#{aliasId}"
          modalSaveButton: 'modal-operation-alias-edit-button'
        $(el).before editFormView.render().el

        deleteButtonView = new Profiling.Views.UnitAliasDeleteButtonView
          title: "Delete Alias"
          confirm: "Are you sure you want to delete this alias?"
          url: "#{Profiling.applicationUrl}Profiling/Operations/#{@operationId}/Aliases/Delete/#{aliasId}"
        $(el).before deleteButtonView.render().el

      $("span.unit-operation").each (i, el) ->
        uoId = $(el).data 'id'

        editFormView = new Profiling.Views.UnitOperationEditFormView
          modalId: "modal-unit-operation-edit-#{uoId}"
          modalUrl: "#{Profiling.applicationUrl}Profiling/UnitOperations/EditModal/#{uoId}"
          modalSaveButton: "modal-unit-operation-edit-button-#{uoId}"
        $(el).before editFormView.render().el

        removeButtonView = new Profiling.Views.OperationRemoveButtonView
          title: "Remove unit from this operation"
          confirm: "Are you sure you want to remove this unit from this operation (unit itself will not be deleted)?"
          url: "#{Profiling.applicationUrl}Profiling/UnitOperations/Remove/#{uoId}"
        $(el).before removeButtonView.render().el

      $("#delete-operation-button").click (e) =>
        bootbox.confirm "This operation will only be deleted if there are no units linked to it. Continue?", (response) =>
          if response is true
            window.location.href = "#{Profiling.applicationUrl}Profiling/Operations/Delete/#{@operationId}"

    if @permissions.canViewAndSearchSources
      opSourcesModel = new Profiling.Models.OperationSourcesModel
        id: @operationId
      opSourcesModel.fetch
        beforeSend: ->
          $("#operation-sources").html "<span class='muted'>Loading...</span>"
        success: ->
          opSourcesView = new Profiling.Views.OperationSourcesView
            model: opSourcesModel
          $("#operation-sources").html opSourcesView.render().el
        error: (model, xhr, options) ->
          $("#operation-sources").html xhr.responseText

      addOperationSourceView = new Profiling.Views.OperationSourceCreateFormView
        modalId: 'modal-operation-source-add'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Operations/#{@operationId}/Sources/Add"
        modalSaveButton: 'modal-operation-source-add-button'
      $("#dropdown-list .divider").before addOperationSourceView.render().el

    orgResponsibilitiesTable = new Profiling.DataTable "organization-responsibilities-table",
      bServerSide: false
      aoColumns: if @permissions.canViewAndSearchEvents then [ { sType: "title-numeric" }, null, null ] else [ null, null ]