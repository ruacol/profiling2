Profiling.Routers.RequestValidateRouter = Backbone.Router.extend

  routes:
    "": "index"

  index: ->
    $("#ForwardRequest").val false
    $("#RejectRequest").val false

    unitSearchView = new Profiling.Views.UnitSearchView
      modalId: 'modal-unit-search'
      modalUrl: "#{Profiling.applicationUrl}Screening/Initiate/SearchUnitsModal"
    $("#attached-units").html unitSearchView.render().el

    personSearchView = new Profiling.Views.PersonSearchView
      modalId: 'modal-person-search'
      modalUrl: "#{Profiling.applicationUrl}Screening/Initiate/SearchPersonsModal"
    $("#attached-persons").html personSearchView.render().el

    $("#submit-button").click (e) ->
      bootbox.confirm "Are you ready to forward this request to the conditionality participants?", (response) ->
        if response is true
          $("#ForwardRequest").val true
          $("#main-form").submit()
        else
          bootbox.hideAll()
      false

    $("#secondary-sidebar")
      .addClass("well").css("bottom", "70px")
      .append("<button id='save-button' type='submit' class='btn'>Save Progress</button>")
      .attr("title", "Save this request, but do not forward for screening.")

    $("#save-button").click (e) ->
      $("#main-form").submit()

    $("#reject-button").click (e) ->
      bootbox.prompt "Please enter the reason this request is rejected.  This will be emailed automatically to those who created and submitted the request.", (response) ->
        if response isnt null
          $("#RejectRequest").val true
          $("#RejectReason").val response
          $("#main-form").submit()
        else
          bootbox.hideAll()
      false

    $("#RespondBy").datepicker
      format: "yyyy-mm-dd"
      autoclose: true

    $("#RespondBy").change ->
      $("#RespondImmediately").prop("checked", false) if $("#RespondBy").val()

    $("#RespondImmediately").change ->
      $("#RespondBy").val "" if $("#RespondImmediately").is(":checked")
        