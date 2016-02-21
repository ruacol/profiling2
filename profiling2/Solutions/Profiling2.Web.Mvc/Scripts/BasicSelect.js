(function() {

  Profiling.BasicSelect = (function() {

    function BasicSelect(opts) {
      if (!$(opts.el).val()) {
        $(opts.el).val('');
      }
      $(opts.el).select2({
        placeholder: opts.placeholder,
        allowClear: true,
        initSelection: function(element, callback) {
          var data, id;
          if (element.val()) {
            id = $(element).val();
            data = {
              id: id
            };
            $.ajax({
              async: false,
              url: "" + opts.nameUrl + id,
              success: function(response, textStatus, xhr) {
                if (response) {
                  return data.text = response.Name;
                }
              }
            });
            return callback(data);
          }
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
        },
        escapeMarkup: function(m) {
          return m;
        }
      });
    }

    return BasicSelect;

  })();

}).call(this);
