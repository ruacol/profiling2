Profiling.Views.SimilarEventsView = Backbone.View.extend
  
  templates:
    list: _.template """
      <p class="alert alert-info">
        Inspect the events listed for possible duplicates.
      </p>
      <ul>
        <% _(events).each(function(e) { %>
          <li>
            <a href="<%= Profiling.applicationUrl %>Profiling/Events/Details/<%= e.Id %>" target="_blank">
              <%= e.Headline %>
            </a>
          </li>
        <% }); %>
      </ul>
    """
    some: _.template """
      <div class="alert">
        <button type="button" class="close" data-dismiss="alert">&times;</button>
        <button type="button" class="btn" id="similar-events-btn">
          <%= num %> similar events detected.
        </button>
      </div>
    """

  events:
    "click #similar-events-btn": "display"

  render: ->
    if @model.get('Events').length > 0
      @$el.html @templates.some
        num: @model.get('Events').length
    @

  display: ->
    content = @templates.list
      events: @model.get('Events')
    bootbox.dialog content
    ,
      label: "OK",
      callback: null
    ,
      onEscape: true
      animate: false