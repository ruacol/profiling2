Profiling.Views.BasePersonFormView = Profiling.Views.BaseModalForm.extend

  modalLoadedCallback: ->
    new Profiling.BasicSelect
      el: "#BirthRegionId"
      placeholder: "Search by region name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Regions/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Regions/Json"

    new Profiling.BasicSelect
      el: "#EthnicityId"
      placeholder: "Search by ethnicity name..."
      nameUrl: "#{Profiling.applicationUrl}Profiling/Ethnicities/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Ethnicities/Json"
