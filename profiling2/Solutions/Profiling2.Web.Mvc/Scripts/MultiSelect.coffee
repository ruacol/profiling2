class Profiling.MultiSelect

  constructor: (opts) ->
    $(opts.el).val '' if not $(opts.el).val()
    $(opts.el).select2
      placeholder: opts.placeholder
      multiple: true
      closeOnSelect: false
      initSelection: (element, callback) ->
        data = []
        $(element.val().split(",")).each (i, el) ->
          if el
            $.ajax
              async: false
              url: "#{opts.nameUrl}#{el}"
              success: (response, textStatus, xhr) ->
                if response
                  data.push
                    id: el
                    text: response.Name
        callback data
      ajax:
        url: opts.getUrl
        dataType: 'json'
        quietMillis: 1000
        data: (term, page) ->
          term: term
        results: (data, page) ->
          results: data