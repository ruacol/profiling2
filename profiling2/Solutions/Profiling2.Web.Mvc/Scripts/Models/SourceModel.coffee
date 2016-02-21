Profiling.Models.SourceModel = Backbone.Model.extend

  initialize: (opts) ->
    @adminSourceSearchId = opts.adminSourceSearchId

  url: ->
    if @adminSourceSearchId
      "#{Profiling.applicationUrl}Profiling/Sources/Details/#{@get('id')}/#{@adminSourceSearchId}"
    else
      "#{Profiling.applicationUrl}Profiling/Sources/Details/#{@get('id')}"

  parse: (response, opts) ->
    response.Notes = response.Notes.replace(/\r\n/g, '<br />').replace(/\n/g, '<br />') if response.Notes
    response

  getMostRecentReview: ->
    ars = _(@get 'AdminReviewedSources').filter (item) =>
      item.AdminSourceSearchId is parseInt(@get 'adminSourceSearchId')
    mostRecentReview = _.reduce ars, (memo, item) ->
      if item.ReviewedDateTime > memo.ReviewedDateTime
        item
      else
        memo
    , _.first ars

  getImportDate: ->
    list = @get 'AdminSourceImports'
    importDate = if list and _.first list
      _.first(list).ImportDate
    else
      @get 'SourceDate'

  getCreateTime: ->
    createTime = @get('DocumentProperties').CreateTime if @get('DocumentProperties')