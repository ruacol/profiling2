Profiling.Views.ExplorerPathsView = Backbone.View.extend

  templates:
    pathDropdown: _.template """
      <span class="help-inline">Choose a path: </span>
      <select id="path-select" class="span9">
        <%= paths %>  
      </select>
    """
    pathDropdownOptions: _.template """
      <% if (_.isString(parent)) { %>
        <option value="<%= parent %>">..</option>
      <% } %>
      <option value="<%= _.first(paths).Folder %>" selected="selected"><%= _.first(paths).Folder %></option> 
      <% _.each(_.rest(paths), function(path) { %>
        <option value="<%= path.Folder %>"><%= path.Folder %></option>
      <% }); %>
    """

  events:
    "change select#path-select": "pathSelected"

  initialize: (opts) ->
    @path = opts.path
    @code = opts.code
    @permissions = opts.permissions

  render: ->
    @$el.html "<span class='muted'>Loading folder paths...</span>"

    url = if @permissions.canViewAndSearchSources and not @code
      "#{Profiling.applicationUrl}Sources/All/Paths"
    else
      "#{Profiling.applicationUrl}Sources/Explorer/Paths/#{@code}"

    $.ajax
      url: "#{url}"
      type: "GET"
      error: (xhr, textStatus) ->
        bootbox.alert xhr.responseText
      success: (data, textStatus, xhr) =>
        @paths = data

        # first folder is the shortest, therefore the root folder
        @root = _(data).first()

        if @root and @root.Folder
          # initial render of dropdown
          @$el.html @templates.pathDropdown
            paths: @templates.pathDropdownOptions
              paths: _(data).filter (x) => 
                (x.ParentFolder and x.ParentFolder.toLowerCase() is @root.Folder.toLowerCase()) or x is @root
        else
          @$el.html "<p>No files found.</p>"
      complete: =>
        @updateSelectOptions(@path) if @path
        @.trigger "change:paths"

    @

  pathSelected: ->
    @.trigger "change:paths"
    @updateSelectOptions $("select#path-select").val()

  updateSelectOptions: (path) ->
    # get matching paths in cached collection of paths from server
    currentPathDtos = _(@paths).select (dto) -> 
      dto.Folder and dto.Folder.toLowerCase() is path.toLowerCase()
    
    if currentPathDtos and _(currentPathDtos).size() > 0
      dto = _(currentPathDtos).first()

      if dto
        parent = dto.ParentFolder

        # repopulate select dropdown list with new path's children and parent.
        $("select#path-select").html @templates.pathDropdownOptions
          paths: _(@paths).select (x) => 
            (x.ParentFolder and x.ParentFolder.toLowerCase() is path.toLowerCase()) or (x.Folder and x.Folder.toLowerCase() is path.toLowerCase())
          parent: parent
    else
      bootbox.alert "Can't find given path (#{path})."

    
