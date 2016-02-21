Profiling.Views.SourceView = Backbone.View.extend
  className: "span12"

  initialize: (opts) ->
    @permissions = opts.permissions

  template: 
    info: _.template """
      <p class="pull-left" style="margin-right: 20px;">
        <a id="preview-btn" class="btn" href="#preview/<%= model.get('Id') %>/<%= model.get('adminSourceSearchId') %>">Preview</a>
      </p>
      <% if (model.get('IsReadOnly') === false) { %>
        <p class="pull-left" style="margin-right: 20px;">
          <a id="download-btn" class="btn" href="<%= Profiling.applicationUrl %>Profiling/Sources/Download/<%= model.get('Id') %>/<%= model.get('adminSourceSearchId') %>">Download</a>
        </p>
      <% } %>
      <% if (CanChangeSources === true) { %>
        <div class="btn-group pull-left">
          <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">Actions <span class="caret"></span></a>
          <ul class="dropdown-menu" id="action-list">
            <% if (model.get('IsPublic') === false) { %>
              <li class="accordion-toggle"><a id="set-public-btn">Set Public</a></li>
            <% } else { %>
              <li class="accordion-toggle"><a id="unset-public-btn">Unset Public</a></li>
            <% } %>
            <% if (model.get('IsReadOnly') === false) { %>
              <li class="accordion-toggle"><a id="set-read-only-btn">Set Read Only</a></li>
            <% } else { %>
              <li class="accordion-toggle"><a id="unset-read-only-btn">Unset Read Only</a></li>
            <% } %>
            <li class="accordion-toggle"><a href="<%= Profiling.applicationUrl %>Sources/Previews/RemovePassword/<%= model.get('Id') %>" target="_blank">Remove Password</a></li>
            <li class="divider"></li>
            <% if (model.get('Archive') === false) { %>
              <li class="accordion-toggle"><a id="archive-btn">Archive</a></li>
            <% } else { %>
              <li class="accordion-toggle"><a id="unarchive-btn">Unarchive</a></li>
            <% } %>
          </ul>
        </div>
      <% } %>
      <table class="table">
          <tr>
              <th>ID</th>
              <td><%= model.get('Id') %></td>
          </tr>
          <tr>
              <th>Name</th>
              <td>
                  <strong><%= model.get('SourceName') %></strong>
                  <% if (model.get('IsReadOnly') === true) { %>
                      &nbsp;<i class="icon-lock" data-selector="#tooltip-readonly"></i>
                  <% } %>
                  <% if (model.get('Archive') === true) { %>
                      &nbsp;<span class="label"><small>ARCHIVED</small></span>
                  <% } %>
                  <% if (model.get('IsPublic') === true) { %>
                      &nbsp;<span class="label"><small>PUBLIC</small></span>
                  <% } %>
                  <% if (model.get('IsRestricted') === true) { %>
                      &nbsp;<span class="label label-important"><small>RESTRICTED</small></span>
                  <% } %>
                  <% if (model.get('HasOcrText') === true) { %>
                      &nbsp;<span class="label label-info" title="Contains optical character recognised text."><small>OCR</small></span>
                  <% } %>
              </td>
          </tr>
          <tr>
              <th style="white-space: nowrap;">Imported From</th>
              <td><%= model.get('SourcePath') %></td>
          </tr>
          <tr>
              <th>Imported On</th>
              <td>
                <% if (model.getImportDate()) { %>
                  <%= moment(model.getImportDate()).format(Profiling.DATE_FORMAT) %>
                <% } %>
              </td>
          </tr>
          <tr>
              <th>File Date</th>
              <td>
                <% if (model.get('FileDateTimeStamp')) { %>
                  <%= moment(model.get('FileDateTimeStamp')).format(Profiling.DATE_FORMAT) %>
                <% } %>
              </td>
          </tr>
          <% if (model.getCreateTime()) { %>
              <tr>
                  <th>Create Date</th>
                  <td><%= moment(model.getCreateTime()).format(Profiling.DATE_FORMAT) %></td> 
              </tr>
          <% } %>
          <tr>
              <th>File Size</th>
              <td><%= (model.get('FileSize') / 1024).toFixed(0) %> KB</td> 
          </tr>
          <% if (model.get('UploadedByUserID')) { %>
              <tr>
                  <th>Uploaded By</th>
                  <td><%= model.get('UploadedByUserID') %></td>
              </tr>
          <% } %>
          <% if (model.get('SourceAuthors') && _.size(model.get('SourceAuthors')) > 0) { %>
            <tr>
              <th>Authors</th>
              <td><%= _.map(model.get('SourceAuthors'), function(x) { return x.Author; }).join() %></td>
            </tr>
          <% } %>
          <% if (model.get('SourceOwners') && _.size(model.get('SourceOwners')) > 0) { %>
            <tr>
              <th>Owners</th>
              <td><%= _.map(model.get('SourceOwners'), function(x) { return x.Name; }).join() %></td>
            </tr>
          <% } %>
          <% if (model.get('JhroCaseNumber')) { %>
              <tr>
                  <th>Case Number</th>
                  <td><a href="<%= Profiling.applicationUrl %>Hrdb/Cases/Details/<%= model.get('JhroCaseID') %>" target="_blank"><%= model.get('JhroCaseNumber') %></a></td> 
              </tr>
          <% } %>
          <% if (model.get('ParentSource')) { %>
              <tr>
                  <th>Attached To</th>
                  <td>
                    <% if (model.get('ParentSource').Id) { %>
                      <a href="#info/<%= model.get('ParentSource').Id %>/<%= model.get('adminSourceSearchId') %>" style="color: #0088cc;"><%= model.get('ParentSource').SourcePath %></a>
                    <% } else { %>
                      <p>The file <%= model.get('ParentSource').SourcePath %> should exist, but the system couldn't find it.</p>
                    <% } %>
                  </td> 
              </tr>
          <% } %>
          <% if (model.get('ChildSources')) { %>
              <% _.each(model.get('ChildSources'), function(ChildSource) { %>
                <tr>
                  <th>Attachment</th>
                  <td><a href="#info/<%= ChildSource.Id %>/<%= model.get('adminSourceSearchId') %>" style="color: #0088cc;"><%= ChildSource.SourcePath %></a></td>
                </tr>
              <% }); %>
          <% } %>
          <% if (model.get('Notes')) { %>
              <tr>
                  <th>Notes</th>
                  <td><%= model.get('Notes') %></td>
              </tr>
          <% } %>
      </table>
    """
    reviewsHeading: "<h4>Reviews</h4>"
    reviewsTable: _.template """
      <table class="table table-hover">
        <tr>
            <th>Date</th>
            <th>Search Terms</th>
            <th>Marked Relevant</th>
            <th>Previewed</th>
            <th>Downloaded</th>
            <th>User</th>
        </tr>

        <% _.each(rows, function(row) { %>
          <%= row %>
        <% }); %>
      </table>
    """
    reviewRow: _.template """
      <tr>
          <td>
            <% if (ReviewedDateTime) { %>
              <%= moment(ReviewedDateTime).format(Profiling.DATE_FORMAT) %>
            <% } %>
          </td>
          <td><%= SearchTerms %></td>
          <td>
            <% if (IsRelevant === true) { %>
              <i class="icon-star"></i>
            <% } else if (IsRelevant === false) { %>
              <i class="icon-remove"></i>
            <% } else { %>
              <i class="icon-question-sign"></i>
            <% } %>
          </td>
          <td>
            <% if (WasPreviewed === true) { %>
              <i class="icon-ok"></i>
            <% } else if (WasPreviewed === false) { %>
              <i class="icon-remove"></i>
            <% } %>
          </td>
          <td>
            <% if (WasDownloaded === true) { %>
              <i class="icon-ok"></i>
            <% } else if (WasDownloaded === false) { %>
              <i class="icon-remove"></i>
            <% } %>
          </td>
          <td><%= User %></td>
      </tr>
    """
    noReviewsRow: _.template """
      <tr>
        <td colspan="2"><span class="muted">No reviews for this source.</span></td>
      </tr>
    """
    attachedHeading: _.template """
      <h4>Attached <%= object %></h4>
    """
    attachedTable: _.template """
      <table class="table table-hover">
        <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Reliability</th>
            <th>Commentary</th>
        </tr>

        <% _.each(rows, function(row) { %>
          <%= row %>
        <% }); %>
      </table>
    """
    attachedRow: _.template """
      <tr>
        <td><%= Id %></td>
        <td><%= Name %></td>
        <td><%= Reliability %></td>
        <td><%= Commentary %></td>
      </tr>
    """
    noAttachedRow: _.template """
      <tr>
        <td colspan="2"><span class="muted">No <%= object %> attached to this source.</span></td>
      </tr>
    """

  events:
    "click #archive-btn": "archive"
    "click #unarchive-btn": "unarchive"
    "click #set-read-only-btn": "setReadOnly"
    "click #unset-read-only-btn": "unsetReadOnly"
    "click #set-public-btn": "setPublic"
    "click #unset-public-btn": "unsetPublic"

  render: ->
    $(@el).html @template.info
      model: @model
      CanChangeSources: @permissions.canChangeSources

    _.defer =>
      Profiling.setupUITooltips ".icon-lock"
      
      view = new Profiling.Views.SourceEditFormView
        modalId: 'modal-source-edit-form'
        modalUrl: "#{Profiling.applicationUrl}Profiling/Sources/Edit/#{@model.get 'Id'}"
        modalSaveButton: 'modal-source-edit-button'
        sourceId: @model.get 'Id'
      $("#action-list").prepend view.render().el

    $(@el).append @template.reviewsHeading
    if @model.get('AdminReviewedSources') and _.size(@model.get('AdminReviewedSources')) > 0
      reviewRows = for i in @model.get 'AdminReviewedSources'
        searchTerms = if i.AndSearchTerms
          array = i.AndSearchTerms
          if i.ExcludeSearchTerms
            array = array.concat (_.map i.ExcludeSearchTerms, (t) -> "-#{t}")
          array.join ', '
        @template.reviewRow
          ReviewedDateTime: i.ReviewedDateTime
          SearchTerms: searchTerms
          IsRelevant: i.IsRelevant
          WasPreviewed: i.WasPreviewed
          WasDownloaded: i.WasDownloaded
          User: i.User

      $(@el).append @template.reviewsTable
        rows: reviewRows
    else
      $(@el).append @template.noReviewsRow()

    for type in [ "PersonSources", "EventSources", "UnitSources", "OperationSources" ]
      object = type.replace 'Source', ''
      $(@el).append @template.attachedHeading
        object: object
      if @model.get(type) and _.size(@model.get type) > 0
        rows = for p in @model.get type
          @template.attachedRow
            Id: p.Id
            Name: "<a href='#{Profiling.applicationUrl}Profiling/#{object}/Details/#{p.Id}' class='source-links' target='_blank' title='Opens in new window...' style='color: #0088cc;'>#{p.Name}</a>"
            Reliability: p.Reliability
            Commentary: p.Commentary
        $(@el).append @template.attachedTable
          rows: rows
      else
        $(@el).append @template.noAttachedRow
          object: object.toLowerCase()

    @

  archive: ->
    bootbox.confirm "This will archive this source, meaning it will no longer appear in search results.  Are you sure?", (result) =>
      if result is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Sources/Archive"
          type: "post"
          data: 
            id: @model.get 'Id'
          error: (xhr, textStatus) ->
            alert xhr.responseText
          success: =>
            # null fragment so we can reload the same URL
            fragment = Backbone.history.fragment
            Backbone.history.fragment = null
            Backbone.history.navigate fragment, true

  unarchive: ->
    bootbox.confirm "This will unarchive this source, meaning it will be included in search results.  Are you sure?", (result) =>
      if result is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Sources/Unarchive"
          type: "post"
          data:
            id: @model.get 'Id'
          error: (xhr, textStatus) ->
            alert xhr.responseText
          success: =>
            # null fragment so we can reload the same URL
            fragment = Backbone.history.fragment
            Backbone.history.fragment = null
            Backbone.history.navigate fragment, true

  setReadOnly: ->
    bootbox.confirm "This will set this source as read only, meaning it will not be downloadable.  Are you sure?", (result) =>
      if result is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Sources/SetReadOnly"
          type: "post"
          data:
            id: @model.get 'Id'
          error: (xhr, textStatus) ->
            alert xhr.responseText
          success: =>
            # null fragment so we can reload the same URL
            fragment = Backbone.history.fragment
            Backbone.history.fragment = null
            Backbone.history.navigate fragment, true
            
  unsetReadOnly: ->
    bootbox.confirm "This will unset the read only flag for this source, meaning it become downloadable.  Are you sure?", (result) =>
      if result is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Sources/UnsetReadOnly"
          type: "post"
          data:
            id: @model.get 'Id'
          error: (xhr, textStatus) ->
            alert xhr.responseText
          success: =>
            # null fragment so we can reload the same URL
            fragment = Backbone.history.fragment
            Backbone.history.fragment = null
            Backbone.history.navigate fragment, true

  setPublic: ->
    bootbox.confirm "This will make this source public.  Are you sure?", (result) =>
      if result is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Sources/SetPublic"
          type: "post"
          data: 
            id: @model.get 'Id'
          error: (xhr, textStatus) ->
            alert xhr.responseText
          success: =>
            # null fragment so we can reload the same URL
            fragment = Backbone.history.fragment
            Backbone.history.fragment = null
            Backbone.history.navigate fragment, true

  unsetPublic: ->
    bootbox.confirm "This will unset the public flag for this source.  Are you sure?", (result) =>
      if result is true
        $.ajax
          url: "#{Profiling.applicationUrl}Profiling/Sources/UnsetPublic"
          type: "post"
          data:
            id: @model.get 'Id'
          error: (xhr, textStatus) ->
            alert xhr.responseText
          success: =>
            # null fragment so we can reload the same URL
            fragment = Backbone.history.fragment
            Backbone.history.fragment = null
            Backbone.history.navigate fragment, true