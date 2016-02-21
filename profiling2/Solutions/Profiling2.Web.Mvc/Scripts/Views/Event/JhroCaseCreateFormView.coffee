Profiling.Views.JhroCaseCreateFormView = Profiling.Views.BaseModalForm.extend

  templates:
    icon: _.template """
      <i class="accordion-toggle icon-plus" style="margin-right: 5px;" title="Add Case Code"></i>
    """

  events:
    "click i.icon-plus": "displayModalForm"

  render: ->
    @$el.css 'display', 'inline'
    @$el.html @templates.icon
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    
    @

  formSubmittedSuccessCallback: (data) ->
    if data
      value = $("#{@options.parentDivId} #JhroCaseIds").select2 "data"
      value.push 
        id: data.Id
        text: data.CaseNumber
      $("#{@options.parentDivId} #JhroCaseIds").select2 "data", value
