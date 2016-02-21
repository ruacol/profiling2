Profiling.Views.CareerCreateFormView = Profiling.Views.BaseCareerFormView.extend

  id: "person-career-create-button"
  tagName: "li"

  events:
    "click": "displayModalForm"

  render: ->
    @$el.append "<a><small>Add Career</small></a>"
    Profiling.Views.BaseModalForm.prototype.render.call @, arguments
    @