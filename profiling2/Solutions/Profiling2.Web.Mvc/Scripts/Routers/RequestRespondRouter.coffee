Profiling.Routers.RequestRespondRouter = Backbone.Router.extend

  initialize: (opts) ->
    @requestId = opts.requestId

  routes:
    "": "index"

  index: ->
    $("#SubmitResponse").val(false)

    $("select.screening-result").each (i, el) ->
      Profiling.setInputColor el
    .change () ->
      Profiling.setInputColor @

    $("#secondary-sidebar")
      .addClass("well")
      .css("bottom", "70px")
      .append("<button type='submit' class='btn'>Save Progress</button>")
      .click () ->
        $("#main-form").submit()
        false

    $("#btn-final").click () ->
      bootbox.confirm "Are you ready to submit this response?", (response) ->
        if response is true
          $("#SubmitResponse").val(true)
          $("#main-form").submit()
        else
          bootbox.hideAll()
      false
        
    $("button.save-individual").click (el) =>
      if $(el.target).prop("disabled") == false
        $(el.target).prop "disabled", true
        requestPersonId = $(el.target).data "id"
        $.ajax
          url: "#{Profiling.applicationUrl}Screening/Inputs/RespondSingle/#{@requestId}"
          type: "POST"
          data:
            Id: $("input[name='Responses[#{requestPersonId}].value.Id']").val()
            RequestPersonID: $("input[name='Responses[#{requestPersonId}].value.RequestPersonID']").val()
            Version: $("input[name='Responses[#{requestPersonId}].value.Version']").val()
            ScreeningResultID: $("select[name='Responses[#{requestPersonId}].value.ScreeningResultID']").val()
            Reason: $("textarea[name='Responses[#{requestPersonId}].value.Reason']").val()
            Commentary: $("textarea[name='Responses[#{requestPersonId}].value.Commentary']").val()
          complete: () ->
            $(el.target).prop "disabled", false
          error: (xhr, textStatus) ->
            html = "<p>Save was unsuccessful:</p><div class='alert alert-error'><ul>"
            _(eval("(" + xhr.responseText + ")")).each (val, key) ->
              html += "<li>#{val}</li>"
            html += "</ul></div>"
            bootbox.alert html
          success: () ->
            name = $("#person-name-#{requestPersonId}").text()
            bootbox.alert "Color coding, reason, and commentary for <strong>#{name}</strong> successfully saved.", () ->
              window.location.reload()

      false