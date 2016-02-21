Profiling.Views.UserEditFormView = Backbone.View.extend

  render: ->
    new Profiling.MultiSelect
      el: "#AdminRoleIds"
      placeholder: 'Search by role name...'
      nameUrl: "#{Profiling.applicationUrl}System/Roles/Name/"
      getUrl: "#{Profiling.applicationUrl}System/Roles/All"
    new Profiling.MultiSelect
      el: "#SourceOwningEntityIds"
      placeholder: 'Search by owner name...'
      nameUrl: "#{Profiling.applicationUrl}Sources/Owners/Name/"
      getUrl: "#{Profiling.applicationUrl}Sources/Owners/All"
    @

