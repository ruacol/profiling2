Profiling.Views.EventSuggestFormView = Profiling.Views.BaseModalForm.extend

  modalWidth: '85%'

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-certificate" style="margin-right: 5px;" title="Suggest Event"></i>
    """

  events:
    "click i.icon-certificate": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @

  displayModalForm: ->
    $("##{@options.modalId}").load @options.modalUrl, '', =>
      $("##{@options.modalId}").modal
        keyboard: true
        width: @modalWidth

      dataTable = new Profiling.DataTable 'suggestions-table',
        sAjaxSource: "#{Profiling.applicationUrl}Profiling/Events/Suggest/#{@options.personId}"
        bServerSide: false
        sDom: 'T<"clear">lftipr'
        bFilter: false
        aoColumns: [ { mDataProp: 'Features', bSortable: false}, { mDataProp: 'Score', bSortable: false}, { mDataProp: 'EventID', bSortable: false }, { mDataProp: 'EventName', bSortable: false }, { mDataProp: 'SuggestionReason', bSortable: false } ]
        fnServerData: (sSource, aoData, fnCallback) ->
          $('input[name="enabledIds[]"]:checkbox:checked').each (i) ->
            aoData.push
              name: "enabledIds"
              value: $(@).val()
          $.getJSON sSource, aoData, (json) ->
            fnCallback json
        fnRowCallback: (nRow, aData, iDisplayIndex) =>
          $("td:eq(0)", nRow).html """
            <button id="accept-#{aData['EventID']}" class="btn btn-mini" title="Accept"><i class="icon-ok"></i></button>
          """
          $("td:eq(1)", nRow).html """
            <button id="decline-#{aData['EventID']}" class="btn btn-mini" title="Decline"><i class="icon-remove"></i></button>
          """
          $("td:eq(2)", nRow).html """
            <a href="#{Profiling.applicationUrl}Profiling/Events/Details/#{aData['EventID']}" target="_blank">
              #{aData['EventID']}
            </a>
          """
          suggestionReasons = for r in aData['SuggestionReasons']
            "<li>#{r}</li>"
          $("td:eq(4)", nRow).html """
            <ul>#{suggestionReasons.join ' '}</ul>
          """

          # Click row text to popup event narrative in alert box
          $("td:gt(1)", nRow).css "cursor", "pointer"
          $("td:gt(1)", nRow).click () =>
            content = ""
            $.ajax
              url: "#{Profiling.applicationUrl}Profiling/Events/Get"
              data:
                term: "#{aData['EventID']}"
              async: false
              success: (data, textStatus, xhr) =>
                if data and _(data).size() > 0
                  event = _(data).first()
                  if event.narrativeEn
                    content += "<h5>#{aData['EventID']} - #{aData['EventName']}</h5><p>#{event.narrativeEn}</p>"
                  if event.narrativeFr
                    content += "<p>#{event.narrativeFr}</p>"
            if content
              bootbox.dialog content,
                label: "OK",
                callback: null
              ,
                onEscape: true
                classes: "narrative"
                animate: false
              # Highlight person names in event narrative
              if window.Profiling.personDetailsRouter.personModel
                person = window.Profiling.personDetailsRouter.personModel.get 'Person'
                aliases = window.Profiling.personDetailsRouter.personModel.get 'Aliases'
                aliases.push
                  FirstName: person.FirstName
                  LastName: person.LastName
                for obj in aliases
                  for name in [ obj.FirstName, obj.LastName ]
                    if name
                      terms = name.split ' '
                      for term in terms
                        $(".modal.narrative").highlight term if term
                $(".modal.narrative .highlight").css
                  backgroundColor: 'yellow'

          # Handle click of tick/cross boxes
          _.defer () =>
            $("#accept-#{aData['EventID']},#decline-#{aData['EventID']}").click (e) =>
              button = if $(e.target).is('button')
                $(e.target)
              else
                $(e.target).parent()
              isAccepted = (button.attr('title') is 'Accept')
              $.ajax
                url: "#{Profiling.applicationUrl}Profiling/Events/SuggestSave"
                type: "post"
                data:
                  PersonId: @options.personId
                  EventId: aData['EventID']
                  IsAccepted: isAccepted
                  SuggestionFeatures: aData['Features']
                  Notes: aData['SuggestionReasons'].join ' '
                success: (data, textStatus, xhr) =>
                  if isAccepted
                    $("##{@options.modalId}").modal 'hide'
                    $("#EventId").select2 "data",
                      id: aData['EventID']
                      text: aData['EventName']
                  else
                    $(nRow).hide()
                error: (xhr, textStatus) ->
                  bootbox.alert xhr.responseText
            
      # handle click of refresh suggestions button
      _.defer () ->
        $("#refresh-suggestions-btn").click () ->
          dataTable.fnReloadAjax()