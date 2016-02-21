Profiling.Views.SourceSearchFormView = Backbone.View.extend

  templates:
    searchForm: _.template """
      <div id="search-form" class="collapse in">
        <form>
          <input type="hidden" id="search-adminSourceSearchId">
          <table class="table table-bordered" style="width: auto;" id="source-search-table">
            <tr>
              <th>Source ID</th>
              <td><input type="text" id="search-id" placeholder="Source ID" title="Search by source ID number." class="input-small"></td>
            </tr>
            <tr>
              <th>File name</th>
              <td><input type="text" id="search-name" placeholder="File name" title="Search by file name or file path, e.g. '\\\\kindata01\\jmac'" class="input-xxlarge"></td>
            </tr>
            <tr>
              <th>File extension</th>
              <td><input type="text" id="search-extension" placeholder="File extension" maxlength="4" title="Search by file extension, e.g. 'doc', 'jpg', 'pdf'." class="input-small"></td>
            </tr>
            <tr>
              <th class="span3">Search text inside files...</th>
              <td class="span9">
                <input type="text" 
                  id="search-text" 
                  placeholder="Search text inside files..." 
                  class="input-xxlarge"
                  title="Search text surrounded by quotations, e.g. 'This is my phrase'. Exclude terms by prefixing with a minus sign, e.g. '-excluded'. Add a trailing wildcard star (*) for flexible term searching, e.g. 'tree*' will match 'tree', 'trees', and 'treehouse'."
                >
              </td>
            </tr>
            <tr>
              <th>File modified date</th>
              <td>
                <div class="input-daterange" 
                    data-provide="datepicker" 
                    data-date-format="yyyy/mm/dd" 
                    data-date-autoclose="true" 
                    data-date-clear-btn="true"
                    title="Filter results based on the last modified date (if available) of each file.  Files with no dates are not returned.">
                  <input type="text" class="input-small" name="start-date" id="start-date" placeholder="Start date..." />
                  <span class="add-on" style="margin-bottom: 10px;">to</span>
                  <input type="text" class="input-small" name="end-date" id="end-date" placeholder="End date..." />
                </div>
              </td>
            </tr>
            <tr>
              <th class="span3">Authors</th>
              <td class="span9">
                <input type="text" 
                  id="author-search-text" 
                  placeholder="Authors" 
                  class="input-xxlarge"
                  title="Filter results based on the author name/s attached to the source."
                >
              </td>
            </tr>
            <tr>
              <td colspan="2" style="text-align: right;"><button class="btn" type="button" id="btn-search-text">Search</button></td>
            </tr>
          </table>
        </form>
      </div>
    """
    resultsDiv: _.template """
      <div id="results-div"></div>
    """

  events:
    "click #btn-search-text": "submitSearch"
    "keyup #search-id,#search-name,#search-extension,#search-text,#author-search-text": "submitSearchOnEnter"
    "change #search-text": "resetAdminSourceSearchId"

  render: ->
    @$el.append @templates.searchForm
    @$el.append @templates.resultsDiv

    _.defer =>
      # add 'selected person' dropdown to search form table
      @selectAttachTargetView = new Profiling.Views.SourceMostRecentAttachTargetView()
      $("#source-search-table").before @selectAttachTargetView.render().el
      
      # set tooltip help
      _.delay ->
        $("#source-search-table input,select").uitooltip
          show: false
          hide: false
          position:
            my: "left+15 center"
            at: "right center"
        , 1000

    @

  submitSearch: ->
    if @resultsView
      @resultsView.redrawTable()
    else
      @resultsView = new Profiling.Views.SourceResultsView
        sourceSearchFormView: @
      $("#results-div").html @resultsView.render().el
    $("#search-form").collapse "hide"

  submitSearchOnEnter: (e) ->
    @submitSearch() if e.keyCode == 13

  resetAdminSourceSearchId: ->
    $("#search-adminSourceSearchId").val("")