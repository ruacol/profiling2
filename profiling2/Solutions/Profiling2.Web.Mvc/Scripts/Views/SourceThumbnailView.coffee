Profiling.Views.SourceThumbnailView = Backbone.View.extend

  templates: 
    thumbnail: _.template """
      <ul class="thumbnails">
        <li>
            <div class="thumbnail" style="margin-top: 5px;">
                <a data-toggle="modal" href="#lightbox-<%= source.SourceId %>">
                    <img src="<%= Profiling.applicationUrl %>Profiling/Sources/Preview/<%= source.SourceId %>" width="150" height="150">
                </a>
                <div id="lightbox-<%= source.SourceId %>" class="modal hide fade" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                        <h3><%= source.SourceName %></h3>
                    </div>
                    <div class='modal-body'>
                        <img src="<%= Profiling.applicationUrl %>Profiling/Sources/Preview/<%= source.SourceId %>">
                    </div>
                </div>
            </div>
        </li>
    </ul>
    """

  render: ->
    if @model.SourceName and _([ 'jpg', 'jpeg', 'gif', 'png', 'bmp', 'tif' ]).contains(@getFileExtension(@model.SourceName))
      @$el.html @templates.thumbnail
        source: @model
    @

  getFileExtension: (name) ->
    ext = null
    if name
      i = name.indexOf '.'
      if i >= 0 and name.length >= i + 1
        ext = name.substring(i + 1).toLowerCase()
    ext