Profiling.Routers.RequestSubmitRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    unitSearchView = new Profiling.Views.UnitSearchView
      modalId: 'modal-unit-search'
      modalUrl: "#{Profiling.applicationUrl}Screening/Initiate/SearchUnitsModal"
    $("#attached-units").html unitSearchView.render().el

    personSearchView = new Profiling.Views.PersonSearchView
      modalId: 'modal-person-search'
      modalUrl: "#{Profiling.applicationUrl}Screening/Initiate/SearchPersonsModal"
    $("#attached-persons").html personSearchView.render().el

    $("#submit-button").click (e) ->
      bootbox.confirm "Are you ready to submit this request for validation?", (response) ->
        if response is true
          $("#main-form").submit()
        else
          bootbox.hideAll()
      false

    $("#secondary-sidebar")
      .addClass("well").css("bottom", "70px")
      .append("<button id='save-button' type='submit' class='btn'>Save Progress</button>")
      .attr("title", "Save this request, but do not submit for validation.")

    $("#save-button").click (e) ->
      $("#main-form").attr "action", "#{Profiling.applicationUrl}Screening/Initiate/Save"
      $("#main-form").submit()

    delView = new Profiling.Views.DeleteRequestButtonView
      requestId: $("#Id").val()
    $("#buttons-row").append delView.render().el

    $("#RespondBy").datepicker
      format: "yyyy-mm-dd"
      autoclose: true

    $("#RespondBy").change ->
      $("#RespondImmediately").prop("checked", false) if $("#RespondBy").val()

    $("#RespondImmediately").change ->
      $("#RespondBy").val "" if $("#RespondImmediately").is(":checked")
        