# Global settings
window.Profiling =
  Models: {}
  Routers: {}
  Views: {}
  DATE_FORMAT: 'LLL'
  NUMERIC_DATE_FORMAT: 'YYYY-MM-DD HH:mm:ss'

  incompleteDateFormat: (y, m, d) ->
    formats = []
    formats.push("YYYY") if y > 0
    formats.push("MM") if m > 0
    formats.push("DD") if d > 0
    formatStr = formats.join "/" 
    moment([y, _.max([0, m-1]), _.max([1, d])]).format $.trim(formatStr)

  # Takes in incomplete dates where the day, month or year where unknown is represented by a '-'.  Used in human rights record.
  prettyPrintIncompleteDate: (date) ->
    format = if /-/.test date
      "#{if /-$/.test(date) then "" else "dddd, Do"} #{if /\/-\//.test(date) then "" else "MMMM"} #{if /^-/.test(date) then "" else "YYYY"}"
    else
      "dddd, Do MMMM YYYY"
    moment(date.replace(/-/g, "1")).format format

  # Used in consolidate and conditionality input screens
  setInputColor: (el) ->
    switch $(el).val()
      when "3"
        $(el).css 'backgroundColor', 'red'
        $(el).css 'color', 'black'
      when "2"
        $(el).css 'backgroundColor', 'yellow'
        $(el).css 'color', 'black'
      when "1"
        $(el).css 'backgroundColor', 'green'
        $(el).css 'color', 'white'
      else
        $(el).css 'backgroundColor', ''
        $(el).css 'color', 'black'

  # Used in final decision screen
  setSupportStatusInputColor: (el) ->
    switch $(el).val()
      when "1"
        $(el).css 'backgroundColor', 'green'
        $(el).css 'color', 'white'
      when "2"
        $(el).css 'backgroundColor', 'red'
        $(el).css 'color', 'black'
      else
        $(el).css 'backgroundColor', ''
        $(el).css 'color', 'black'
        
  # Returns true iff s is an empty string.
  # (This returns false for non-strings as well.)
  isEmptyString: (s) ->
    return true if s instanceof String and s.length == 0
    s == ''

  # Event aggregator
  eventAggregator: {}

  # Use WebStorage to store user's list of recently viewed persons as a LIFO stack. Note possible privacy concern as we don't clear this anywhere yet.
  MOST_RECENT_PERSONS: "most-recent-persons"
  MOST_RECENT_EVENTS: "most-recent-events"

  getFromStorage: (key) ->
    return if !Profiling.checkLocalStorage()
    if not localStorage.getItem key
      localStorage.setItem key, JSON.stringify([])
    JSON.parse(localStorage.getItem key)

  getMostRecentFromStorage: (key) ->
    _(Profiling.getFromStorage(key)).last()

  setInStorage: (opts) ->
    list = Profiling.getFromStorage(opts.key)

    # remove existing stack value if exists
    if _(list).findWhere({ id: opts.id })
      list = _(list).reject (obj) ->
        obj.id == opts.id

    # add this item to list
    list.push opts

    # limit size of stack
    list = _(list).last 5

    localStorage.setItem opts.key, JSON.stringify(list)

  checkLocalStorage: ->
    if !Modernizr.localstorage
      bootbox.alert "Your browser does not support <a href='http://dev.w3.org/html5/webstorage/'>Web Storage</a>! Some features may not function properly."
    Modernizr.localstorage

  setupUITooltips: (selector) ->
    $(selector).uitooltip
      show: false
      hide: false
      position:
        my: "left+15 center"
        at: "right center"
      items: "[title], [data-selector]"
      content: ->
        content = if $(@).is("[data-selector]")
          selector = $(@).data "selector"
          $(selector).html()
        else
          $(@).attr "title"

  setupUnitsMultiSelect: (selector) ->
    new Profiling.MultiSelect
      el: selector
      placeholder: 'Search for unit by ID or name...'
      nameUrl: "#{Profiling.applicationUrl}Profiling/Units/Name/"
      getUrl: "#{Profiling.applicationUrl}Profiling/Units/Get"

  # http://codedisplay.com/jquery-to-merge-html-table-row-for-same-group-of-data-using-rowsapn-attribute/
  # used in unit details combined locations table
  mergeTableCellsWithRowspan: ->
    Column_number_to_Merge = 1
 
    # Previous_TD holds the first instance of same td. Initially first TD=null.
    Previous_TD = null
    i = 1
    $("tbody", this).find('tr').each ->
      # find the correct td of the correct column
      # we are considering the table column 1, You can apply on any table column
      Current_td = $(this).find('td:nth-child(' + Column_number_to_Merge + ')')
                 
      if Previous_TD == null
        # for first row
        Previous_TD = Current_td
        i = 1
      else if (Current_td.text() == Previous_TD.text())
        # the current td is identical to the previous row td
        # remove the current td
        Current_td.remove()
        # increment the rowspan attribute of the first row td instance
        Previous_TD.attr('rowspan', i + 1)
        i = i + 1 
      else
        # means new value found in current td. So initialize counter variable i
        Previous_TD = Current_td
        i = 1

# Handle naming conflict between Bootstrap and jQuery UI tooltip
$.widget.bridge('uitooltip', $.ui.tooltip);

# Provide case insensitive 'icontains' selector
$.expr[":"].icontains = $.expr.createPseudo (arg) ->
  (elem) ->
    return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0

$(document).ready () ->
  # Fix issue when using affix on sidebar
  $("#sidebar").height($("#primary-sidebar-well").height() + 40)
  $("#primary-sidebar-well,#secondary-sidebar").width($("#sidebar").width() - 30)

  # Sidebar sections collapsing
  $(".profiling-items,.maintenance-items,.manage-items,.screening-items,.admin-items").on "hidden", (el) ->
    $(el.target).css "padding", "0px"
  $(".profiling-items,.maintenance-items,.manage-items,.screening-items,.admin-items").on "show", (el) ->
    $(el.target).css "padding", "3px 15px"
    
  # Report unexpected ajax errors
  # When saving a proposed person in initiate screen, MiniProfiler will trigger this.
  $.ajaxSetup
    error: (xhr) ->
      if xhr and xhr.statusText isnt "abort"  # check for abort status since select2 aborts an xhr call if user types more
        alert "An error occurred.\n\n#{xhr.responseText}"

  $("form").submit () ->
    bootbox.alert "Please wait..."

  # Clear WebStorage (not perfect, e.g. if user doesn't logout)
  $("#logout-link").click () ->
    localStorage.removeItem "persons"
