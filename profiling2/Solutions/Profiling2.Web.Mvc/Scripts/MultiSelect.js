(function() {

  Profiling.MultiSelect = (function() {

    function MultiSelect(opts) {
      if (!$(opts.el).val()) {
        $(opts.el).val('');
      }
      $(opts.el).select2({
        placeholder: opts.placeholder,
        multiple: true,
        closeOnSelect: false,
        initSelection: function(element, callback) {
          var data;
          data = [];
          $(element.val().split(",")).each(function(i, el) {
            if (el) {
              return $.ajax({
                async: false,
                url: "" + opts.nameUrl + el,
                success: function(response, textStatus, xhr) {
                  if (response) {
                    return data.push({
                      id: el,
                      text: response.Name
                    });
                  }
                }
              });
            }
          });
          return callback(data);
        },
        ajax: {
          url: opts.getUrl,
          dataType: 'json',
          quietMillis: 1000,
          data: function(term, page) {
            return {
              term: term
            };
          },
          results: function(data, page) {
            return {
              results: data
            };
          }
        }
      });
    }

    return MultiSelect;

  })();

}).call(this);
