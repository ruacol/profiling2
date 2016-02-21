Profiling.Views.BaseModalForm = Backbone.View.extend

  modalWidth: '65%'

  baseTemplates:
    modal: _.template """
      <div id="<%= modalId %>" class="modal hide fade"></div>
    """

  render: ->
    @$el.append @baseTemplates.modal
      modalId: @options.modalId
    @

  displayModalForm: ->
    $("##{@options.modalId}").load @options.modalUrl, '', =>
      $("##{@options.modalId}").modal
        keyboard: true
        width: @modalWidth

      @modalLoadedCallback()
      Profiling.setupUITooltips "i.icon-info-sign"
      Profiling.setupUITooltips ".has-help-text"

      $("##{@options.modalId} ##{@options.modalSaveButton}").click (e) =>
        if window.Profiling.formSubmitting isnt true
          $.ajax
            url: @options.modalUrl
            type: 'post'
            data: $("##{@options.modalId} input,##{@options.modalId} select,##{@options.modalId} textarea").serialize()
            beforeSend: =>
              window.Profiling.formSubmitting = true
              $(e.target).attr "disabled", "disabled"
            success: (data, textStatus, xhr) =>
              if data and not data.WasSuccessful
                _($("##{@options.modalId} .control-group")).each (el) -> $(el).removeClass 'error'
                text = "<ul>"
                _(data).each (val, key) ->
                  text += "<li>#{val}</li>"
                  $("##{key}").parent().parent().addClass 'error'
                text += "</ul>"

                validationElement = $("##{@options.modalId} #validation-errors")
                validationElement.attr 'tabindex', -1
                validationElement.html "<div class='alert alert-error'>#{text}</div>"
                if typeof(validationElement.scrollIntoView) is 'function'
                  validationElement.scrollIntoView()
                else if typeof(validationElement.focus) is 'function'
                  validationElement.focus()
              else
                $("##{@options.modalId}").modal 'hide'
                @formSubmittedSuccessCallback data
            complete: ->
              $(e.target).removeAttr "disabled"
              window.Profiling.formSubmitting = false
            error: (xhr, textStatus) ->
              bootbox.alert xhr.responseText

  modalLoadedCallback: ->

  formSubmittedSuccessCallback: ->
    if Backbone.history.fragment
      # remember scroll position
      window.Profiling.storedPageYOffset = window.pageYOffset
      # null fragment so we can reload the same URL
      fragment = Backbone.history.fragment
      Backbone.history.fragment = null
      Backbone.history.navigate fragment, true
    else
      window.location.reload()

  setupPersonSelect: (selector) ->
    $(selector).each (i, el) ->
      $(el).val '' if not $(el).val()
      $(el).select2
        placeholder: "Search for person..."
        allowClear: true
        initSelection: (element, callback) ->
          if element.val()
            personId = $(element).val()
            data =
              id: personId
            $.ajax
              async: false
              url: "#{Profiling.applicationUrl}Profiling/Persons/Name/#{personId}"
              success: (response, textStatus, xhr) ->
                if response
                  data.text = response.Name
            callback data
        ajax:
          url: "#{Profiling.applicationUrl}Profiling/Persons/Get"
          dataType: 'json'
          quietMillis: 1000
          data: (term, page) ->
            term: term
          results: (data, page) ->
            results: data

  setupSelect: (opts) ->
    new Profiling.BasicSelect opts
