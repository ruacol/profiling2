Profiling.Views.BaseDeleteButtonView = Backbone.View.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-trash" style="margin-right: 5px;" title="<%= title %>"></i>
    """

  events:
    "click i.icon-trash": "clickTrashIcon"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
      title: @options.title
    @

  clickTrashIcon: ->
    bootbox.confirm @options.confirm, (response) =>
      if response is true
        $.ajax
          url: @options.url
          success: (data, textStatus, xhr) =>
            bootbox.alert data, =>
              @successfulDeleteCallback()
          error: (xhr, textStatus) ->
            bootbox.alert xhr.responseText

  successfulDeleteCallback: ->
    if Backbone.history.fragment
      # null fragment so we can reload the same URL
      fragment = Backbone.history.fragment
      Backbone.history.fragment = null
      Backbone.history.navigate fragment, true
    else
      window.location.reload()