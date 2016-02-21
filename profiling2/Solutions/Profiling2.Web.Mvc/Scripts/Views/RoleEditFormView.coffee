Profiling.Views.RoleEditFormView = Backbone.View.extend

  render: ->
    @setupPermissionSelect()
    @

  setupPermissionSelect: ->
    $("#AdminPermissionIds").select2
      placeholder: 'Search by permission name...'
      multiple: true
      closeOnSelect: false
      initSelection: (element, callback) ->
        data = []
        $(element.val().split(",")).each (i, el) ->
          if el
            $.ajax
              async: false
              url: "#{Profiling.applicationUrl}System/Permissions/Name/#{el}"
              success: (response, textStatus, xhr) ->
                if response
                  data.push
                    id: el
                    text: response.Name
        callback data
      ajax:
        url: "#{Profiling.applicationUrl}System/Permissions/All"
        dataType: 'json'
        quietMillis: 1000
        data: (term, page) ->
          term: term
        results: (data, page) ->
          results: data
