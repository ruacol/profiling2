Profiling.Views.SourceTabsView = Backbone.View.extend
  template: _.template """
    <hr />
    <ul class="nav nav-tabs">
      <li
        <% if (tab == "preview") { %>
          class="active"
        <% } %>
      >
        <a id="previewTabLink" href="#preview/<%= Id %>/<%= adminSourceSearchId %>">Preview <%= SourceName %></a>
      </li>
      <li
        <% if (tab == "view") { %>
          class="active"
        <% } %>
      >
        <a id="infoTabLink" href="#info/<%= Id %>/<%= adminSourceSearchId %>">
          Source Information
        </a>
      </li>
    </ul>
  """

  initialize: (opts) ->
    @tab = opts.tab

  render: () ->
    $(@el).html @template
      Id: @model.get('Id')
      adminSourceSearchId: @model.get 'adminSourceSearchId'
      SourceName: @model.get('SourceName') or "(no file name)"
      tab: @tab
    @