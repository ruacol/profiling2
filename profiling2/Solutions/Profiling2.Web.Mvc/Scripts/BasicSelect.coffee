class Profiling.BasicSelect

  constructor: (opts) ->
    $(opts.el).val '' if not $(opts.el).val()
    $(opts.el).select2
      placeholder: opts.placeholder
      allowClear: true
      initSelection: (element, callback) ->
        if element.val()
          id = $(element).val()
          data =
            id: id
          $.ajax
            async: false
            url: "#{opts.nameUrl}#{id}"
            success: (response, textStatus, xhr) ->
              if response
                data.text = response.Name
          callback data
      ajax:
        url: opts.getUrl
        dataType: 'json'
        quietMillis: 1000
        data: (term, page) ->
          term: term
        results: (data, page) ->
          results: data
      escapeMarkup: (m) -> 
        m
