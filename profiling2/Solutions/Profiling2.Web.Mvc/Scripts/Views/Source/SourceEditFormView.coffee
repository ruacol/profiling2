Profiling.Views.SourceEditFormView = Profiling.Views.BaseModalForm.extend

  modalWidth: '90%'

  templates:
    link: _.template """
      <li class="accordion-toggle">
        <a>Edit</a>
      </li>
      <li class="divider"></li>
    """

  events:
    "click": "displayModalForm"

  render: ->
    @$el.html @templates.link
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments

    @

  modalLoadedCallback: ->
    new Profiling.MultiSelect
      el: "#SourceAuthorIds"
      placeholder: 'Search by author name...'
      nameUrl: "#{Profiling.applicationUrl}Sources/Authors/Name/"
      getUrl: "#{Profiling.applicationUrl}Sources/Authors/Get"
    new Profiling.MultiSelect
      el: "#SourceOwningEntityIds"
      placeholder: 'Search by owner name...'
      nameUrl: "#{Profiling.applicationUrl}Sources/Owners/Name/"
      getUrl: "#{Profiling.applicationUrl}Sources/Owners/All"

  formSubmittedSuccessCallback: ->
    Backbone.history.fragment = null  # null fragment so we can reload the same URL
    Backbone.history.navigate "info/#{@options.sourceId}",
      trigger: true